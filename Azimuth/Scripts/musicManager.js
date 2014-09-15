var MusicManager = function (manager) {
    var self = this;
    this.audioManager = manager;
    this.tracksGlobal = [];
    this.playlistsGlobal = [];
    this.friendsOffset = 0;
    this.provider = "Vkontakte";
    this.reloginUrl = window.location.pathname;
    this.playlistTracksGlobal = [];
    this.topTracks = null;
    this.topTracksVk = [];
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
    this.$infoLoadingSpinner = $('#info-header-spinner');
    this.$playlistsLoadingSpinner = $('#playlist-header-spinner');
    this.$vkMusicLoadingSpinner = $('#vkMusic-header-spinner');
    this.$vkMusicTitle = $('#vkMusic-header-title');
    this.$playlistsTitle = $('#playlist-header-title');
    this.$errorModal = $('#errorModal');

    this._getTracks = function (plId) {
        var $this = $(this);
        self.$vkMusicLoadingSpinner.show();
        $('.vkMusicList').find('.track').detach();
        var playlistId = $this.children('.playlistId').text();
        if (playlistId.length == 0) {
            playlistId = plId;
        }
        $.ajax({
            url: "/api/usertracks?playlistId=" + playlistId,
            type: 'GET',
            success: function (tracksData) {
                self.$vkMusicLoadingSpinner.hide();
                for (var i = 0; i < tracksData.length; i++) {
                    tracksData[i].Duration = self._toFormattedTime(tracksData[i].Duration, true);
                }
                self.playlistTracksGlobal = tracksData;
                self.showPlaylistTracks(tracksData, playlistId);

                var currentPlaylist = $('.playlist .playlistId:contains(' + playlistId + ')').parent('.playlist');
                if (currentPlaylist.find('.readonly').text() == 'true') {
                    var ctxMenu = new ContextMenu();
                    var contextMenuActions = [
                        { id: 'selectall', name: 'Select All', isNewSection: false, hasSubMenu: false, needSelectedItems: false },
                        { id: 'copytoplaylist', name: 'Copy to another playlist', isNewSection: true, hasSubMenu: true, needSelectedItems: true },
                        { id: 'movetoplaylist', name: 'Move to another plylist', isNewSection: false, hasSubMenu: true, needSelectedItems: true },
                        { id: 'removeselected', name: 'Remove selected', isNewSection: true, hasSubMenu: false, needSelectedItems: true },
                        { id: 'sharetracks', name: 'Share tracks', isNewSection: false, hasSubMenu: false, needSelectedItems: true }
                    ];

                    ctxMenu.initializeContextMenu(-1, contextMenuActions, this, self);
                } else {
                    self.$playlistsTable.parents('.draggable').makeDraggable({
                        contextMenu: [
                            { 'id': 'selectall', 'name': 'Select All', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false },
                            { 'id': 'copytoplaylist', 'name': 'Copy to another playlist', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true },
                            { 'id': 'movetoplaylist', 'name': 'Move to another plylist', "isNewSection": false, "hasSubMenu": true, "needSelectedItems": true },
                            { 'id': 'removeselected', 'name': 'Remove selected', "isNewSection": true, "hasSubMenu": false, "needSelectedItems": true },
                            { 'id': 'sharetracks', 'name': 'Share tracks', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true }

                        ],
                        onMoveTrackToNewPosition: self._moveTrackToNewPosition,
                        manager: self
                    });
                }
            }
        });
        //$(this).next("#playlistTracksTable").slideToggle(100); // TODO replace with class name
        var temp = self.playlistsGlobal.filter(function (index) {
            return index.Id == playlistId;
        })[0].Name;
        self.$vkMusicTitle.text(temp);
        self.audioManager.bindPlayBtnListeners();

        $this.siblings('.playlist-active').removeClass('playlist-active');
        $this.toggleClass("playlist-active");
    };

    this._saveTrackFromVkToPlaylist = function ($currentItem, index, playlistId) {
        var tracks = [];

        var currentPlaylist = $('.playlist .playlistId:contains(' + playlistId + ')').parent('.playlist');
        if (currentPlaylist.find('.readonly').text() == 'true')
            return;
        $currentItem.children().toggleClass('vk-item', false);
        $currentItem.children('.tableRow').each(function () {
            tracks.push({
                ThirdPartId: $(this).closest('.tableRow').find('.thirdPartId').text(),
                OwnerId: $(this).closest('.tableRow').find('.ownerId').text()
            });
        }).get();
        $currentItem.empty();
        $.ajax({
            url: '/api/usertracks?provider=' + self.provider + "&index=" + index,
            type: 'POST',
            data: JSON.stringify({
                "PlaylistId": playlistId,
                "TrackInfos": tracks
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

    this._getUserTracks = function () {
        
        self.$vkMusicTitle.text('Loading...');

        $defaultPlaylist = $('.default-playlist');
        $defaultPlaylist.siblings('.playlist-active').removeClass('playlist-active');
        $defaultPlaylist.toggleClass("playlist-active");

        self.$vkMusicLoadingSpinner.fadeIn('normal');
        $('.vkMusicList').hide();
        $.ajax({
            url: '/api/usertracks?provider=' + self.provider,
            success: function (tracks) {
                self.userTracks = tracks;
                self.$vkMusicTitle.text('User tracks');
                if (typeof tracks.Message === 'undefined') {
                    self.$reloginForm.hide();
                    self.$vkMusicTable.show();
                    var allDuration = 0;
                    for (var i = 0; i < tracks.length; i++) {
                        allDuration += tracks[i].duration;
                        tracks[i].duration = self._toFormattedTime(tracks[i].duration, true);
                    }

                    allDuration = self._toFormattedTime(allDuration, true);
                    $defaultPlaylist.children('.playlist-duration').html("Duration: " + allDuration);
                    $defaultPlaylist.children('.playlist-size').html("Songs: " + tracks.length);

                    self.tracksGlobal = tracks;
                    self.showTracks(tracks);
                } else {
                    self.$reloginForm.show();
                    self.$vkMusicTable.hide();
                }

                self.audioManager.bindPlayBtnListeners();
                $('#vkMusicTable > .tableTitle').text("User Tracks");
                self.$vkMusicLoadingSpinner.hide();
                $('.vkMusicList').show();
            },
            error: function () {
                $('#vkMusicTable > .tableTitle').text("Error has occurred");
                self.$vkMusicLoadingSpinner.hide();
                $('.vkMusicList').show();
            }
        });
    };

    this._getTrackInfo = function (e) {
        var $self = $(this);
        var author = $self.children('.track-artist').html();
        var trackName = $self.children('.track-title').html();
        var $trackInfoContainer = $('#infoModal .modal-body');
        $trackInfoContainer.text('');
        self.$infoLoadingSpinner.show();
        $.ajax({
            url: '/api/usertracks/trackinfo?artist=' + author + '&trackName=' + trackName,
            success: function (trackInfo) {
                self.$infoLoadingSpinner.hide();
                var $trackInfoTemplate = $('#trackInfoTemplate');
                var object = $trackInfoTemplate.tmpl(trackInfo);
                
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
                self.topTracks = trackInfo.ArtistTopTrackList;
                $('#listenTopBtn').attr('disabled', false);
            },
        }).fail(function() {
            var $trackInfoContainer = $('#infoModal .modal-body');
            self.$infoLoadingSpinner.hide();
            $trackInfoContainer.append('<div id="forbidden" class="alert alert-danger" style="margin-top: 10px">' +
                                            '<p class="error">Error occured during searching ...</p>' +
                                        '</div>');
            $('#listenTopBtn').attr('disable', true);
        });
    };

    this._getFriendTracks = function () {
        var $currentItem = $(this);
        $currentItem.parent().children('.friend').toggleClass('active', false);
        var currentId = $currentItem.children('.friend-id').html();
        $currentItem.toggleClass('active', true);

        //var provider = "Vkontakte"; // TODO: Fix for all providers
        self.$vkMusicTitle.text('Loading...');
        self.$vkMusicLoadingSpinner.fadeIn('normal');
        $('.vkMusicList').hide();
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
                } else {
                    self.$reloginForm.show();
                    self.$vkMusicTable.hide();
                }
                self.audioManager.bindPlayBtnListeners();
                self.$vkMusicLoadingSpinner.hide();
                $('.vkMusicList').show();
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
                $('.vkMusicList').show();
                var timerId = null;
                clearTimeout(timerId);
                timerId = setTimeout(function () {
                    $accessDenied.fadeOut(1000);
                }, 4000);
            }
        });
    };

    this._setChangingPlaylistImage = function (playlist) {
        var time = Math.floor((Math.random() * 40000) + 25000);

        function timer() {
            time = Math.floor((Math.random() * 40000) + 25000);
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
                $logo.fadeOut(500, function() {
                    if (image != "") {
                        $logo.attr("src", image);
                    } else {
                        $logo.attr("src", "http://cdns2.freepik.com/free-photo/music-album_318-1832.jpg");
                    }
                });
                $logo.fadeIn(500);
            }
        });
    };

    this._moveTrackToNewPosition = function (playlist) {
        var playlistId = $('#playlistTracks').children('.playlistId').text();
        var currentPlaylist = $('.playlist .playlistId:contains(' + playlistId + ')').parent('.playlist');
        if (currentPlaylist.find('.readonly').text() == 'true')
            return;
        var $tracks = $(playlist).children('.tableRow');
        var wholePlaylist = [];
        for (var i = 0; i < $tracks.length; i++) {
            if (!$($tracks[i]).hasClass('draggable-stub')) {
                wholePlaylist.push({
                    'trackId': $($tracks[i]).find('.trackId').text(),
                    'trackPosition': i
                });
            }
        }

        $.ajax({
            url: '/api/usertracks/put?playlistId=' + playlistId,
            type: 'PUT',
            data: JSON.stringify(wholePlaylist),
            contentType: 'application/json; charset=utf-8',
            success: function() {
                self.audioManager.refreshPlaylistTracks();
            }
        });
    };

    this._sendMessage = function(e) {
        var $this = $(this);
        var friendId = $this.siblings('.friend-id').html();

        VK.api('wall.post', {
                owner_id: friendId,
                message: "new message",
                attachments: "photo261844377_336279002,http://localhost:44300/"
        }, function (data) {
        });

        e.stopPropagation();
    };
};

MusicManager.prototype.setDefaultPlaylist = function () {
    var self = this;
    self.$playlistsTable.prepend('<div class="playlist-divider" />');

    var playlist = {
        Name: "VK Playlist",
        Accessibilty: "Private",
        Genres : []
    };

    var tmpl = self.playlistTemplate.tmpl(playlist);

    var img = tmpl.children('.playlist-logo').children('img');
    img.attr("src", "http://cdn.marketplaceimages.windowsphone.com/v8/images/ab99fcba-4240-45a9-a025-80a0edba0c0a?imageType=ws_icon_large");

    tmpl.addClass('default-playlist');

    tmpl.click(self._getUserTracks);
    self.$playlistsTable.prepend(tmpl);

    self._getUserTracks();
};

MusicManager.prototype.showTracks = function (tracks, template) {
    var self = this;

    $('.vkMusicList').find('.track').detach();
    for (var i = 0; i < tracks.length; i++) {
        if (template === undefined) {
            var tmpl = self.trackTemplate.tmpl(tracks[i]);
            tmpl.appendTo('.vkMusicList');
        } else {
            var tmpl = template.tmpl(tracks[i]);
            tmpl.appendTo('.vkMusicList');
        }
        if (self.audioManager.$currentTrack !== null
            && self.audioManager.$currentTrack.children('.trackId').html() == tracks[i].id) {
            tmpl.toggleClass('.draggable-item-selected');
            self.audioManager.$currentTrack = tmpl;
            tmpl.append(self.audioManager.progressSlider.getSlider());
            if (self.audioManager.audio.paused) {
                self.audioManager._setPlayImgButton(tmpl);
                //self.audioManager.$currentTrack.find('.track-duration').show();
                //self.audioManager.$currentTrack.find('.track-remaining').hide();
            } else {
                self.audioManager._setPauseImgButton(tmpl);
                //self.audioManager.$currentTrack.find('.track-duration').hide();
                //self.audioManager.$currentTrack.find('.track-remaining').show();
            }
        }
    }

    if ($('#audio-player .track-artist').text() == '' || $('#audio-player .track-title').text() == '') {
        $('#audio-player .track-artist').text($($('.vkMusicList .track-artist')[0]).text());
        $('#audio-player .track-title').text($($('.vkMusicList .track-title')[0]).text());
    }

    self.$vkMusicTable.makeDraggable({
                contextMenu: [
                    { 'id': 'selectall', 'name': 'Select all', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false },
                    { 'id': 'hideselected', 'name': 'Hide selected', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true },
                    { 'id': 'savevktrack', 'name': 'Move to', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true },
                    { 'id': 'createplaylist', 'name': 'Create new playlist', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true },
                ],
                manager: self
            });

    self.audioManager.bindPlayBtnListeners();
    $('.track-info > a').click(self._getTrackInfo);
};

MusicManager.prototype.showFriends = function (friends, scrollbarInitialized) {
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
                $.ajax({
                    url: '/api/user/friends/' + self.provider + "?offset=" + self.friendsOffset + "&count=10",
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
    $('.send-message-btn').click(self._sendMessage);
};

MusicManager.prototype.showPlaylistTracks = function (tracks) {
    var self = this;
    self.showTracks(tracks, self.playlistTrackTemplate);
    self.audioManager.bindPlayBtnListeners();
};

MusicManager.prototype.showPlaylists = function (playlists) {
    var self = this;
    $('.playlist').remove();
    self.playlists = playlists;
    self.$playlistsLoadingSpinner.show();
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
                            playlist.Accessibilty = "Public";
                        } else {
                            playlist.Accessibilty = "Private";
                        }
                        
                        playlist.Duration = self._toFormattedTime(playlist.Duration, true);
                        playlist.readonly = false;
                        self.playlistsGlobal.push(playlist);
                        var tmpl = self.playlistTemplate.tmpl(playlist);
                        self._setNewImage(tmpl);
                        self.$playlistsTable.append(tmpl);
                        self._setChangingPlaylistImage(tmpl);
                    }
                } else {
                    self.$reloginForm.show();
                    self.$vkMusicTable.hide();
                }
                //$('.accordion .tableRow').on("click", self._getTracks);

                $.ajax({
                    url: '/api/playlists/favorite/notOwned',
                    success: function (playlistsData) {
                        console.log('Hi');
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
                                playlist.readonly = true;
                                self.playlistsGlobal.push(playlist);
                                var tmpl = self.playlistTemplate.tmpl(playlist);
                                self._setNewImage(tmpl);
                                self.$playlistsTable.append(tmpl);
                                self._setChangingPlaylistImage(tmpl);
                            }
                        } else {
                            self.$reloginForm.show();
                            self.$vkMusicTable.hide();
                        }
                        self.$playlistsLoadingSpinner.hide();
                        $('.accordion .tableRow:not(.default-playlist)').on("click", self._getTracks);
                        self._createContextMenuForPlaylists();
                    }
                });
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
                self._createContextMenuForPlaylists();
            }
        }
        //self._createContextMenuForPlaylists();
        self.$playlistsLoadingSpinner.hide();
    }
    self.setDefaultPlaylist();

    this._createContextMenuForPlaylists = function() {
        var ctxMenu = new ContextMenu();
        var contextMenuActions = [
            { id: 'makepublic', name: 'Make public', isNewSection: false, hasSubMenu: false, needSelectedItems: false },
            { id: 'makeprivate', name: 'Make private', isNewSection: false, hasSubMenu: false, needSelectedItems: false },
            { id: 'shareplaylist', name: 'Share it', isNewSection: true, hasSubMenu: false, needSelectedItems: false },
            { id: 'removeplaylist', name: "Remove", isNewSection: false, hasSubMenu: false, needSelectedItems: false }
        ];

        ctxMenu.initializeContextMenu(-1, contextMenuActions, this, self);

        //$('.playlist').off('mousedown').mousedown(function (event) {
        $('.playlist').mousedown(function(event) {
            if (event.which == 3) {
                $('.playlist.selected').toggleClass('selected', false);
                var $target = $(event.target).parents('.playlist');

                //if ($target.find('.readonly').text() == "true")
                //    return;

                $target.toggleClass('selected', true);
                ctxMenu.drawContextMenu(event);
            }
        });

        ctxMenu.$contextMenuContainer.mousedown(function(event) {
            ctxMenu.$contextMenuContainer.hide();
            ctxMenu.selectAction($('.playlist.selected'));
        });
    };
};

MusicManager.prototype.bindListeners = function() {
    var self = this;

    $(document).on('PlaylistAdded', function(playlist) { // TODO Remove event triggering on document object
        self.playlistsGlobal.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
        self.$searchPlaylistInput.trigger('input');
    });

    $('#okPlaylistCreateModalBtn').click(function () {

        $('#createPlaylistModal').modal('hide');
        $('#createPlaylistModal').on('hidden.bs.modal', function () {
            $('#createPlaylistModal .modal-body #playlistNameToCreate').val("");
            $('#createPlaylistModal .modal-body select :first').attr("selected", "selected");
        });

        var playlistName = $('#playlistNameToCreate').val();
        var playlistAccessibility = $('#newPlaylistAccessibility option:selected').val();
        $.ajax({
            url: '/api/playlists?name=' + playlistName + '&accessibilty=' + playlistAccessibility,
            type: 'POST',
            contentType: 'application/json',
            success: function (playlistId) {
                self._saveTrackFromVkToPlaylist($('#itemsContainer'), -1, playlistId);
            }
        });
    });

    self.$getUserTracksBtn.click(function(e) {
        self._getUserTracks(self.provider);
        self.$searchTrackInput.val('');
    });

    this.$searchTrackInput.keyup(function(e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        self.showTracks(self.tracksGlobal.filter(function (index) {
            self.$searchTrackInput.next().next().children().remove();
            return ((index.title.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.artist.toLocaleLowerCase().indexOf(searchParam) != -1));
        }));
        self.audioManager.refreshTracks();
        self.audioManager.updateProgressbar('.vkMusicList');
    });

    this.$searchPlaylistInput.keyup(function(e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        if (self.$playlistsTable.css('visibility') != 'hidden' && self.$playlistsTable.css('display') != 'none') {

            var foundedPlaylist = self.playlistsGlobal.filter(function(index) {
                self.$searchPlaylistInput.next().children().remove();
                var isGenre = false;
                for (var i = 0; i < index.Genres.length && isGenre == false; i++) {
                    isGenre = index.Genres[i].toLocaleLowerCase().indexOf(searchParam) != -1;
                }
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || isGenre);
            });

            if (foundedPlaylist.length == 0 && searchParam != "") {
                self.$createNewPlaylistLbl.show();
                if (e.keyCode == 13) {
                    self._createPlaylist();
                    self.playlistsGlobal = [];
                    $(this).val("");
                    self.showPlaylists();
                }
            } else {
                self.$createNewPlaylistLbl.hide();
            }

            self.showPlaylists(foundedPlaylist);
            $('.accordion .tableRow:not(.default-playlist)').on("click", self._getTracks);
        } else {
            //console.log('nothing to do here');
            self.showPlaylistTracks(self.playlistTracksGlobal.filter(function(index) {
                self.$searchPlaylistInput.next().children().remove();
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.Artist.toLocaleLowerCase().indexOf(searchParam) != -1));
            }));
            self.audioManager.refreshPlaylistTracks();
            self.audioManager.updateProgressbar('#playlistTracks');
        }
    });

    $('#scrollable-playlist, .vkMusicTable').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        scrollInertia: 0,
        autoHideScrollbar: true,
        advanced: { updateOnSelectorChange: "true" }
    });

    this._createPlaylist = function() {
        var playlistName = self.$searchPlaylistInput.val();
        $.ajax({
            url: '/api/playlists?name=' + playlistName + '&accessibilty=Public',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
        });
        self.$createNewPlaylistLbl.hide();
        $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
    };

    this.$getFriendInfoBtn.click(function(e) {
        if (self.$friendsBody.is(':visible')) {
            self.$friendsBody.hide('slow');
        } else if (self.$friendList.children().length == 0) {
            $.ajax({
                url: '/api/user/friends/' + self.provider + "?offset=" + self.friendsOffset + "&count=10",
                success: function(friends) {
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
        $('#infoModal .modal-body').text('');
        $('#listenTopBtn').attr('disabled', true);
    });

    $('#createPlaylistModal').on('hidden.bs.modal', function () {
        $('#createPlaylistModal .modal-body #playlistNameToCreate').val('');
    });

    $('#listenTopBtn').click(function() {
        $('#infoModal .modal-body').text('');
        if (self.topTracks != null) {
            $('#vkMusic-header-title').text('');
            self.$vkMusicLoadingSpinner.show();
            $('.vkMusicList').find('.track').remove();

            if (self.topTracks != null) {
                var tracks = JSON.stringify({ trackdata: self.topTracks });
                $.ajax({
                    url: '/api/usertracks/tracksearch?provider=Vkontakte&infoForSearch=' + tracks,
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json;',
                    success: function(data) {
                        self.topTracksVk = [];
                        $(data).each(function() {
                            this.duration = self._toFormattedTime(this.duration, true);
                            if ((typeof (this.artist) != 'undefined') && (typeof (this.title) != 'undefined')) {
                                self.topTracksVk.push(this);
                            }
                        });
                        self.showTracks(self.topTracksVk);
                        self.$vkMusicTable.makeDraggable({
                            contextMenu: [
                                { 'id': 'selectall', 'name': 'Select all', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false },
                                { 'id': 'hideselected', 'name': 'Hide selected', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true },
                                { 'id': 'savevktrack', 'name': 'Move to', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true },
                                { 'id': 'createplaylist', 'name': 'Create new playlist', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true },
                                { 'id': 'trackshuffle', 'name': 'Shuffle', 'isNewSection': false, 'hasSubMenu': false, 'needSelectedItems': false }
                            ]
                        });
                        self.$vkMusicLoadingSpinner.hide();
                    },
            }).fail(function() {
                self.$vkMusicLoadingSpinner.hide();
                    self.$errorModal.find('.error').text('Error occured during search ...');
                //$('#listenTopBtn').attr('disable', true);
                    self.$errorModal.modal('show');
                    $('#okErrorButton').click(function() {
                        $('#errorModal .error').text("");
                        self.$errorModal.modal('hide');
                    });
                });
            }
        }        
    });
};
