var SettingsManager = function (manager) {
    var self = this;
    this.audioManager = manager;
    this.tracksGlobal = [];
    this.playlistsGlobal = [];
    this.friendsOffset = 0;
    this.provider = "";
    this.playlistTracksGlobal = [];
    this.stringForCreateBtn = "Create new playlist ";
    this.playlistTrackTemplate = $("#playlistTrackTemplate");
    this.playlistTemplate = $("#playlistTemplate");
    this.trackTemplate = $("#trackTemplate");
    this.$friendsTemplate = $("#friendsTemplate");
    this.$friendsBody = $('#friends-body');
    this.$friendList = $('#friends-container');
    this.$reloginForm = $("#relogin");
    this.$vkMusic = $("#vkontakteMusic");
    this.$getTrackInfoBtn = $('.track-info-btn');
    this.$getUserTracksBtn = $("#get-user-tracks");
    this.$playlistsTable = $('#playlistsTable');
    this.$searchPlaylistInput = $('#searchPlaylistName');
    this.$searchTrackInput = $('#searchTrackName');
    this.$vkMusicTable = $('#vkMusicTable').parent();
    this.$createNewPlaylistLbl = $('#create-playlist-lbl');
    this.$getFriendInfoBtn = $('#get-friends-info-btn');
    this.$friendsLoadingSpinner = $('#friends-header-spinner');
    this.$playlistsLoadingSpinner = $('#playlist-header-spinner');
    this.$vkMusicLoadingSpinner = $('#vkMusic-header-spinner');
    this.$vkMusicTitle = $('#vkMusic-header-title');
    this.$playlistsTitle = $('#playlist-header-title');
    this._getTracks = function (plId) {
        self.$playlistsTitle.text('platlist\'re loading');
        self.$playlistsLoadingSpinner.fadeIn('normal');
        console.log(self.playlistsGlobal);
        var playlistId = $(this).find('.playlistId').text();
        if (playlistId.length == 0) {
            playlistId = plId;
        }
        $.ajax({
            url: "/api/usertracks?playlistId=" + playlistId, // TODO replace with class playlistID
            type: 'GET',
            async: false,
            success: function (tracksData) {
                for (var i = 0; i < tracksData.length; i++) {
                    tracksData[i].Duration = Math.floor(tracksData[i].Duration / 60) + ":" + (tracksData[i].Duration % 60 < 10 ? "0" + tracksData[i].Duration % 60 : tracksData[i].Duration % 60);
                }
                self.playlistTracksGlobal = tracksData;
                self.showPlaylistTracks(tracksData, playlistId);
                self.$playlistsTable.parents('.draggable').makeDraggable({
                    contextMenu: [
						{ 'id': 'selectall', 'name': 'Select All', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false, "callback": self._selectAllTracksAction },
						{ 'id': 'copytoplaylist', 'name': 'Copy to another playlist', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true, "callback": self._copyTrackToPlaylistAction },
                        { 'id': 'movetoplaylist', 'name': 'Move to another plylist', "isNewSection": false, "hasSubMenu": true, "needSelectedItems": true, "callback": self._moveTracksBetweenPlaylistsAction },
						{ 'id': 'removeselected', 'name': 'Remove selected', "isNewSection": true, "hasSubMenu": false, "needSelectedItems": true, "callback": self._removeSelectedTracksAction }
                    ],
                    onMoveTrackToNewPosition: self._moveTrackToNewPosition
                });
                self.$searchPlaylistInput.val('');
                console.log(playlistId);
                var temp = self.playlistsGlobal.filter(function(index) {
                    console.log(index);

                    return index.Id == playlistId;
                })[0].Name;
                console.log(temp);
                self.$playlistsTitle.text(temp);
                self.$playlistsLoadingSpinner.fadeOut('normal');
            }
        });
        //}
        //$(this).next("#playlistTracksTable").slideToggle(100); // TODO replace with class name
        $('#playlistsTable').hide();
        $('#backToPlaylistsBtn').show();
        $('#playlistTracks').show();
        self.audioManager.bindPlayBtnListeners();
        $(this).toggleClass("active");

        $('#playlistTracks > .tableRow > .track-info-btn').click(self._getTrackInfo);
    };

    this._toFormattedTime = function (input, roundSeconds) {
        if (roundSeconds) {
            input = Math.ceil(input);
        }

        var withHours = false;

        if (input >= 60 * 60) {
            withHours = true;
        }

        var hoursString = '00';
        var minutesString = '00';
        var secondsString = '00';
        var hours = 0;
        var minutes = 0;
        var seconds = 0;

        hours = Math.floor(input / (60 * 60));
        input = input % (60 * 60);

        minutes = Math.floor(input / 60);
        input = input % 60;

        seconds = input;

        hoursString = (hours >= 10) ? hours.toString() : '0' + hours.toString();
        minutesString = (minutes >= 10) ? minutes.toString() : '0' + minutes.toString();
        secondsString = (seconds >= 10) ? seconds.toString() : '0' + seconds.toString();

        return ((withHours) ? hoursString + ':' : '') + minutesString + ':' + secondsString;
    };

    this._moveTrackToNewPosition = function ($currentItem, $draggableStub) {
        var playlistId = $('.playlist.active').children('.playlistId').text();
        var tracksIds = [];

        $currentItem.children().each(function () {
            tracksIds.push($(this).find('.trackId').text());
        }).get();

        var index = $draggableStub.index();
        $.ajax({
            url: '/api/usertracks/put?playlistId=' + playlistId + "&newIndex=" + index,
            type: 'PUT',
            dataType: 'json',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8'
        });
    };

    this._saveTrackFromVkToPlaylist = function ($currentItem, index, playlistId) {
        var provider = $('.tab-pane.active').attr('id');
        var tracks = [];
        var friendId = $('.friend.active').children('.friend-id').text();
        $currentItem.children().toggleClass('vk-item', false);
        $currentItem.children('.draggable-item-selected').each(function () {
            tracks.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        //if ($('.friend.active').length > 0) {
        //    friendId = $('.friend.active').text();
        //}

        $.ajax({
            url: '/api/usertracks?provider=' + provider + "&index=" + index + "&friendId=" + friendId,
            type: 'POST',
            data: JSON.stringify({
                "Id": playlistId,
                "TrackIds": tracks
            }),
            contentType: 'application/json',
            success: function () {
                if ($('#playlistTracks').children().length > 0) {
                    var playlistId = $('#playlistTracks').find('.playlistId').text();
                    $('#playlistTracks').children().remove();
                    self._getTracks(playlistId);
                }

                $('.tableRow.playlist').remove();
                self.playlistsGlobal.length = 0;
                self.showPlaylists();
            }
        });
    };

    this._selectAllTracksAction = function (list) {
        list.toggleClass('draggable-item-selected', true);
    };

    this._removeSelectedTracksAction = function ($currentItem, playlistId) {
        var tracksIds = [];
        $currentItem.children('.draggable-item-selected').each(function () {
            tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        $.ajax({
            url: '/api/usertracks/delete?playlistId=' + playlistId,
            type: 'DELETE',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8',
            success: function () {
                var playlistId = $('#playlistTracks').find('.playlistId').text();
                $('#playlistTracks').children().remove();
                self._getTracks(playlistId);

                $('.tableRow.playlist').remove();
                self.playlistsGlobal.length = 0;
                self.showPlaylists();
            }
        });
    };

    this._hideSelectedTracksAction = function (list) {
        list.detach();
        list.toggleClass('draggable-item-selected', false);
        self.audioManager.refreshTracks();
    };

    this._createPlaylistAction = function ($currentItem) {
        var playlist = prompt('Enter playlist name', "");
        if (playlist != null) {
            $.ajax({
                url: '/api/playlists?name=' + playlist + '&accessibilty=Public',
                type: 'POST',
                contentType: 'application/json',
                async: false,
                success: function (playlistId) {
                    self._saveTrackFromVkToPlaylist($currentItem, -1, playlistId);
                }
            });
        }
    };

    this._moveTracksBetweenPlaylistsAction = function ($currentItem, newPlaylist, oldPlaylist) {
        var tracksIds = [];
        $currentItem.children('.draggable-item-selected').each(function () {
            tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        $.ajax({
            url: '/api/usertracks/copy?playlistId=' + newPlaylist,
            type: 'POST',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8',
            success: function () {
                $.ajax({
                    url: '/api/usertracks/delete?playlistId=' + oldPlaylist,
                    type: 'DELETE',
                    data: JSON.stringify(tracksIds),
                    contentType: 'application/json; charset=utf-8',
                    success: function () {
                        //var playlistId = $('#playlistTracks').find('.playlistId').text();
                        $('#playlistTracks').children().remove();
                        self._getTracks(oldPlaylist);

                        $('.tableRow.playlist').remove();
                        self.playlistsGlobal.length = 0;
                        self.showPlaylists();
                    }
                });
            }
        });
    };

    this._copyTrackToPlaylistAction = function ($currentItem, playlistId) {
        var tracksIds = [];
        $currentItem.children('.draggable-item-selected').each(function () {
            tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        $.ajax({
            url: '/api/usertracks/copy?playlistId=' + playlistId,
            type: 'POST',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8',
            success: function () {
                if ($('#playlistTracks').children().length > 0) {
                    var playlistId = $('#playlistTracks').find('.playlistId').text();
                    $('#playlistTracks').children().remove();
                    self._getTracks(playlistId);
                }

                $('.tableRow.playlist').remove();
                self.playlistsGlobal.length = 0;
                self.showPlaylists();
            }
        });
    };

    this._getUserTracks = function (provider, reloginUrl) {
        
        self.$vkMusicTitle.text('Pls wait, tracks\'re loading');
        self.$vkMusicLoadingSpinner.fadeIn('normal');
        $.ajax({
            url: '/api/usertracks?provider=' + provider,
            success: function (tracks) {
                self.$vkMusicTitle.text('User tracks');
                if (typeof tracks.Message === 'undefined') {
                    self.$reloginForm.hide();
                    self.$vkMusicTable.show();
                    var list = $('.vkMusicList');
                    for (var i = 0; i < tracks.length; i++) {
                        tracks[i].duration = self._toFormattedTime(tracks[i].duration, true);
                    }
                    self.tracksGlobal = tracks;
                    self.showTracks(tracks);
                    self.$vkMusicTable.makeDraggable({
                        contextMenu: [
                            { 'id': 'selectall', 'name': 'Select all', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false, "callback": self._selectAllTracksAction },
                            { 'id': 'hideselected', 'name': 'Hide selected', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true, "callback": self._hideSelectedTracksAction },
                            { 'id': 'savevktrack', 'name': 'Move to', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true, "callback": self._saveTrackFromVkToPlaylist },
                            { 'id': 'createplaylist', 'name': 'Create new playlist', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true, "callback": self._createPlaylistAction }
                        ]
                    });
                } else {
                    self.$reloginForm.show();
                    self.$reloginForm.find('a').attr('href', reloginUrl);
                    self.$vkMusicTable.hide();
                }
                $('.vkMusicList > .tableRow > .track-info-btn').click(self._getTrackInfo);

                self.audioManager.bindPlayBtnListeners();
                $('#vkMusicTable > .tableTitle').text("User Tracks");
                self.$vkMusicLoadingSpinner.hide();
            },
            error: function () {
                $('#vkMusicTable > .tableTitle').text("Error has occurred");
                self.$vkMusicLoadingSpinner.hide();
            }
        });
    };

    this._getTrackInfo = function() {
        var $self = $(this);
        var author = $self.parent().children('.track-description').children('.track-info');
        var trackName = $self.parent().children('.track-description').children('.track-title');
        $.ajax({
            url: '/api/usertracks/trackinfo?artist=' + author.html() + '&trackName=' + trackName.html(),
            async: true,
            success: function(trackInfo) {
                var $trackInfoTemplate = $('#trackInfoTemplate');
                var object = $trackInfoTemplate.tmpl(trackInfo);
                var $trackInfoContainer = $('.modal-body');
                $trackInfoContainer.text('');
                object.appendTo($trackInfoContainer);
                if (trackInfo.Lyric != null) {
                    for (var i = 0; i < trackInfo.Lyric.length; i++) {
                        var $p = $('<p>');
                        $p.text(trackInfo.Lyric[i]);
                        $trackInfoContainer.find('.trackLyric').append($p);
                    }
                }
                if (trackInfo.Summary != null) {
                    $trackInfoContainer.find('.trackSummary').append(trackInfo.Summary);
                }
            }
        });
    };

    this._getFriendTracks = function () {
        var $currentItem = $(this);
        $currentItem.parent().children('.friend').toggleClass('active', false);
        var currentId = $currentItem.children('.friend-id').html();
        $currentItem.toggleClass('active', true);

        //var provider = "Vkontakte"; // TODO: Fix for all providers
        self.$vkMusicTitle.text('Pls wait, tracks\'re loading');
        self.$vkMusicLoadingSpinner.fadeIn('normal');
        $.ajax({
            url: '/api/user/friends/audio?provider=' + self.provider + '&friendId=' + currentId,
            success: function (tracks) {
                if (typeof tracks.Message === 'undefined') {
                    var currentUser = $currentItem.children('.friend-initials').html();
                    
                    self.$vkMusicTitle.html("Now playing: " + currentUser + "'s playlist");
                    self.$reloginForm.hide();
                    self.$vkMusicTable.show();
                    var list = $('.vkMusicList');
                    for (var i = 0; i < tracks.length; i++) {
                        tracks[i].duration = self._toFormattedTime(tracks[i].duration, true);
                    }
                    self.tracksGlobal = tracks;
                    self.showTracks(tracks);
                    self.$vkMusicTable.makeDraggable({
                        contextMenu: [
                            { 'id': 'selectall', 'name': 'Select all', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false, "callback": self._selectAllTracksAction },
                            { 'id': 'hideselected', 'name': 'Hide selected', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true, "callback": self._shideSelectedTracksAction },
                            { 'id': 'savevktrack', 'name': 'Move to', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true, "callback": self._saveTrackFromVkToPlaylist },
                            { 'id': 'createplaylist', 'name': 'Create new playlist', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true, "callback": self._createPlaylistAction }
                        ]
                    });
                } else {
                    self.$reloginForm.show();
                    self.$reloginForm.find('a').attr('href', reloginUrl);
                    self.$vkMusicTable.hide();
                }
                self.audioManager.bindPlayBtnListeners();
                self.$vkMusicLoadingSpinner.hide();

                $('.vkMusicList > .tableRow > .track-info-btn').click(self._getTrackInfo);
            },
            error: function (thrownException) {
                
                self.$vkMusicTitle.html("Error");
                var $accessDenied = $('#forbidden');
                var $messageContainer = $('#forbidden .error ');
                $messageContainer.text(thrownException.responseJSON.ExceptionMessage);
                $accessDenied.css({
                    'top': 100 + 'px',
                    'left': 800 + 'px',
                    'position': 'absolute'
                });
                $accessDenied.show();
                self.$vkMusicLoadingSpinner.hide();
                var timerId = null;
                clearTimeout(timerId);
                timerId = setTimeout(function () {
                    $accessDenied.fadeOut(1000);
                }, 4000);
            }
        });
    };

    this._setChangingPlaylistImage = function (playlist) {
        var time = Math.floor((Math.random() * 40000) + 30000);

        function timer() {
            time = Math.floor((Math.random() * 40000) + 30000);
            self._setNewImage(playlist);
            setTimeout(timer, time);
        };

        setTimeout(timer, time);
    };

    this._setNewImage = function (playlist) {
        var id = playlist.children('.playlistId').html();
        $.ajax({
            url: '/api/playlists/image/' + id,
            success: function (image) {
                var $logo = playlist.children('.playlist-logo').children();
                if (image != "") {
                    $logo.attr("src", image)
                } else {
                    $logo.attr("src", "http://cdns2.freepik.com/free-photo/music-album_318-1832.jpg");
                }
            }
        });
    };
};

SettingsManager.prototype.showTracks = function (tracks) {
    var self = this;

    $('.vkMusicList').find('.track').remove();
    for (var i = 0; i < tracks.length; i++) {
        var tmpl = self.trackTemplate.tmpl(tracks[i]);
        tmpl.appendTo('.vkMusicList');
        if (self.audioManager.$currentTrack !== null
            && self.audioManager.$currentTrack.children('.trackId').html() == tracks[i].id) {
            tmpl.toggleClass('.draggable-item-selected');
            self.audioManager.$currentTrack = tmpl;
            tmpl.append(self.audioManager.progressSlider.getSlider());
            if (self.audioManager.audio.paused) {
                self.audioManager._setPlayImgButton(tmpl);
                self.audioManager.$currentTrack.find('.track-duration').show();
                self.audioManager.$currentTrack.find('.track-remaining').hide();
            } else {
                self.audioManager._setPauseImgButton(tmpl);
                self.audioManager.$currentTrack.find('.track-duration').hide();
                self.audioManager.$currentTrack.find('.track-remaining').show();
            }
        }
    }
    self.audioManager.bindPlayBtnListeners();

};

SettingsManager.prototype.showFriends = function (friends, scrollbarInitialized) {
    var self = this;

    for (var i = 0; i < friends.length; i++) {
        var friend = friends[i];
        if (friend.city != null) {
            friend.city = friend.city.title;
        }
        if (friend.country != null) {
            friend.country = friend.country.title;
        }
        var container = scrollbarInitialized ? self.$friendList.find('.mCSB_container') : self.$friendList;
        container.append(self.$friendsTemplate.tmpl(friends[i]));
    }
    $('#friends-container').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        advanced: { updateOnSelectorChange: "true" },
        callbacks: {
            onTotalScroll: function () {
                self.$friendsLoadingSpinner.fadeIn('normal');
                var provider = $('.tab-pane.active').attr('id');
                $.ajax({
                    url: '/api/user/friends/' + provider + "?offset=" + self.friendsOffset + "&count=10",
                    success: function (friendsData) {
                        self.showFriends(friendsData, true);
                        self.friendsOffset += friendsData.length;
                        self.$friendsLoadingSpinner.fadeOut('normal');
                    }
                });
            }
        }
    });

    $('.friend').click(self._getFriendTracks);
};

SettingsManager.prototype.showPlaylistTracks = function (tracks, playlistId) {
    var self = this;

    $('#playlistTracks').find('.track').remove();
    for (var i = 0; i < tracks.length; i++) {
        var tmpl = self.playlistTrackTemplate.tmpl(tracks[i]);
        tmpl.appendTo('#playlistTracks');
        if (self.audioManager.$currentTrack !== null
            && self.audioManager.$currentTrack.children('.track-url').html() == tracks[i].Url) {
            tmpl.toggleClass('.draggable-item-selected');
            self.audioManager.$currentTrack = tmpl;
            tmpl.append(self.audioManager.progressSlider.getSlider());
            if (self.audioManager.audio.paused) {
                self.audioManager._setPlayImgButton(tmpl);
                self.audioManager.$currentTrack.find('.track-duration').show();
                self.audioManager.$currentTrack.find('.track-remaining').hide();
            } else {
                self.audioManager._setPauseImgButton(tmpl);
                self.audioManager.$currentTrack.find('.track-duration').hide();
                self.audioManager.$currentTrack.find('.track-remaining').show();
            }
        }
    }
    self.audioManager.bindPlayBtnListeners();
    $('#playlistTracks').append('<div style="display: none" class="playlistId">' + playlistId + '</div>');
};

SettingsManager.prototype.showPlaylists = function (playlists) {
    var self = this;
    //self.$playlistsTable.find(".tableHeader").remove();
    self.$playlistsTitle.text('My playlists');
    self.$playlistsLoadingSpinner.fadeIn("normal");
    if (typeof playlists === 'undefined') { //Initial run to get playlists from db
        $.ajax({
            url: '/api/playlists',
            success: function (playlistsData) {
                console.log(playlistsData.Message);
                if (typeof playlistsData.Message === 'undefined') {
                    
                    self.$reloginForm.hide();
                    self.$vkMusicTable.show();
                    self.playlists = playlistsData;
                    for (var i = 0; i < self.playlists.length; i++) {
                        var playlist = self.playlists[i];
                        if (playlist.Accessibilty === 1) {
                            playlist.Accessibilty = "public";
                        } else {
                            playlist.Accessibilty = "private";
                        }
                        playlist.Duration = self._toFormattedTime(playlist.Duration, true);
                        self.playlistsGlobal.push(playlist);
                        var tmpl = self.playlistTemplate.tmpl(playlist);
                        self._setNewImage(tmpl);
                        self.$playlistsTable.append(tmpl);
                        self._setChangingPlaylistImage(tmpl);
                    }
                } else {
                    self.$reloginForm.show();
                    self.$reloginForm.find('a').attr('href', reloginUrl);
                    self.$vkMusicTable.hide();
                }
                self.$playlistsLoadingSpinner.fadeOut("normal");
                $('.accordion .tableRow').on("click", self._getTracks);
            }
        });
    } else { //using to print playlists after using filter
        self.$playlistsTable.empty();
        if (self.playlists.length !== 0) {
            for (var i = 0; i < playlists.length; i++) {
                var tmpl = this.playlistTemplate.tmpl(playlists[i]);
                self.$playlistsTable.append(tmpl);
                self._setNewImage(tmpl);
                self._setChangingPlaylistImage(tmpl);
            }
        } else {
            self.$createNewPlaylistBtn.show();
            self.$createNewPlaylistBtn.text(this.stringForCreateBtn + this.$searchPlaylistInput.val());
        }
        self.$playlistsLoadingSpinner.fadeOut("normal");
    }
};

SettingsManager.prototype.bindListeners = function () {
    var self = this;

    $(document).on('PlaylistAdded', function (playlist) { // TODO Remove event triggering on document object
        self.playlistsGlobal.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
        self.$searchPlaylistInput.trigger('input');
    });

    $('.providerBtn').click(function (e) {
        self.provider = $(e.target).data('provider');
        var reloginUrl = $(e.target).data('reloginurl');
        self._getUserTracks(self.provider, reloginUrl);
    });

    self.$getUserTracksBtn.click(function(e) {
        self._getUserTracks(self.provider);
    });

    this.$searchTrackInput.keyup(function (e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        self.showTracks(self.tracksGlobal.filter(function (index) {
            self.$searchTrackInput.next().next().children().remove();
            return ((index.title.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.artist.toLocaleLowerCase().indexOf(searchParam) != -1));
        }));
        self.audioManager.refreshTracks();
    });

    this.$searchPlaylistInput.keyup(function (e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        if (self.$playlistsTable.css('visibility') != 'hidden' && self.$playlistsTable.css('display') != 'none') {

            var foundedPlaylist = self.playlistsGlobal.filter(function (index) {
                self.$searchPlaylistInput.next().children().remove();
                var isGenre = false;
                for (var i = 0; i < index.Genres.length && isGenre == false; i++) {
                    isGenre = index.Genres[i].toLocaleLowerCase().indexOf(searchParam) != -1;
                }
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || isGenre);
            });

            if (foundedPlaylist.length == 0) {
                self.$createNewPlaylistLbl.show();
                if (e.keyCode == 13) {
                    console.log(e);
                    self._createPlaylist();
                    self.playlistsGlobal = [];
                    $(this).val("");
                    self.showPlaylists();
                }
            } else {
                self.$createNewPlaylistLbl.hide();
            }

            self.showPlaylists(foundedPlaylist);
            $('.accordion .tableRow').on("click", self._getTracks);
        } else {
            console.log('nothing to do here');
            self.showPlaylistTracks(self.playlistTracksGlobal.filter(function (index) {
                self.$searchPlaylistInput.next().children().remove();
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.Artist.toLocaleLowerCase().indexOf(searchParam) != -1));
            }));
            self.audioManager.refreshPlaylistTracks();
        }
    });


    $('#backToPlaylistsBtn').click(function () {
        $('#backToPlaylistsBtn').hide();
        $('#playlistTracks').empty();

        $('#playlistTracks').hide();
        self.$playlistsTable.empty();
        self.showPlaylists(self.playlistsGlobal);
        self.$playlistsTable.show();
        self.$searchPlaylistInput.val('');
        $('.accordion .tableRow').on("click", self._getTracks);
    });

    $('#playlists, .vkMusicTable').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        scrollInertia: 0,
        autoHideScrollbar: true,
        advanced: { updateOnSelectorChange: "true" }
    });

    this._createPlaylist = function () {
        var playlistName = self.$searchPlaylistInput.val();
        $.ajax({
            url: '/api/playlists?name=' + playlistName + '&accessibilty=Public',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            async: false
        });
        self.$createNewPlaylistLbl.hide();
        $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
    };

    this.$getFriendInfoBtn.click(function (e) {
        if (self.$friendsBody.is(':visible')) {
            self.$friendsBody.hide('slow');
        } else if (self.$friendList.children().length == 0) {
            var provider = $('.tab-pane.active').attr('id');
            $.ajax({
                url: '/api/user/friends/' + provider + "?offset=" + self.friendsOffset + "&count=10",
                async: true,
                success: function (friends) {
                    self.showFriends(friends);
                    self.$friendsBody.show('slow');
                    self.friendsOffset += friends.length;
                }
            });
        } else {
            self.$friendsBody.show('slow');
        }
    });

    $('#infoModal').on('hidden.bs.modal', function() {
        $('.modal-body').text('');
    });
};