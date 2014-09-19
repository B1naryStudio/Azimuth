var MusicManager = function (manager) {
    var self = this;
    this.audioManager = manager;
    this.tracksGlobal = [];
    this.playlistsGlobal = [];
    this.friendsOffset = 0;
    this.notificationsOffset = 0;
    this.provider = "Vkontakte";
    this.reloginUrl = window.location.pathname;
    this.playlistTracksGlobal = [];
    this.topTracks = null;
    this.extraContainerShown = false;
    this.topTracksVk = [];
    this.playlistsTimeouts = [];
    this.currentFriend = null;
    this.gettingFriends = false;
    this.gettingNotification = false;
    this.friendsListOpen = false;
    this.notificationListOpen = false;
    this.stringForCreateBtn = "Create new playlist ";
    this.playlistTrackTemplate = $("#playlistTrackTemplate");
    this.playlistTemplate = $("#playlistTemplate");
    this.trackTemplate = $("#trackTemplate");
    this.$trackContainer = $("#tracks-container");
    this.$extraContainer = $("#extra-container");
    this.$friendsTemplate = $("#friendsTemplate");
    this.$friendsBody = $('#friends-body');
    this.$friendList = $('#friends-container');
    this.$notificationsList = $('#notifications-container');
    this.$friendButton = $('#friends-button');
    this.$notificationButton = $('#notifications-button');
    this.$getNotificationsButton = $('#notifications-button');
    this.$reloginForm = $("#relogin");
    this.$vkMusic = $("#vkontakteMusic");
    this.$getTrackInfoBtn = $('.track-info-btn');
    this.$getUserTracksBtn = $("#get-user-tracks");
    this.$playlistsTable = $('#playlistsTable');
    this.$searchPlaylistInput = $('#searchPlaylistName');
    this.$searchTrackInput = $('#searchTrackName');
    this.$vkMusicTable = $('#vkMusicTable').parent();
    this.$createNewPlaylistLbl = $('#create-playlist-lbl');
    this.$friendsLoadingSpinner = $('#friends-header-spinner');
    this.$infoLoadingSpinner = $('#info-header-spinner');
    this.$playlistsLoadingSpinner = $('#playlist-header-spinner');
    this.$vkMusicLoadingSpinner = $('#vkMusic-header-spinner');
    this.$vkMusicTitle = $('#vkMusic-header-title');
    this.$playlistsTitle = $('#playlist-header-title');
    this.$errorModal = $('#errorModal');
    this.$messageContainer = $('#message-container');
    this.activePlaylistId = null;
    this.isSearch = false;

    this._getTracks = function (plId) {
        var $this = $(this);
        self.$vkMusicLoadingSpinner.show();
        $('.vkMusicList').find('.track').detach();
        var $activePlaylist = $('.playlist-active');
        var playlistId;
        if ($this.hasClass('playlist')) {
            playlistId = $this.children('.playlistId').text();
        } else {
            playlistId = $activePlaylist.find('.playlistId').text();
            this.activePlaylistId = playlistId;
        }

        if (playlistId == null) {
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
                    //self.$playlistsTable.parents('.draggable').makeDraggable({
                   $('.draggable').makeDraggable({
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
        //var temp = self.playlistsGlobal.filter(function (index) {
        //    return index.Id == playlistId;
        //})[0].Name;
        //self.$vkMusicTitle.text(temp);


        self.audioManager.bindPlayBtnListeners();

        $('.playlist-active').toggleClass('playlist-active', false);
        $this.toggleClass("playlist-active", true);
    };

    this._saveTrackFromVkToPlaylist = function ($currentItem, index, playlistId) {
        var tracks = [];

        var currentPlaylist = $('.playlist .playlistId:contains(' + playlistId + ')').parent('.playlist');
        if (currentPlaylist.find('.readonly').text() == 'true') {
            return;
        }
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
                $('.playlist-divider').remove();
                self.showPlaylists();
                self.setDefaultPlaylist();

                //$('#playlistsTable').trigger('OnChange');
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

        var $defaultPlaylist = $('.default-playlist');
        $('.playlist-active').toggleClass('playlist-active', false);
        $defaultPlaylist.toggleClass("playlist-active", true);

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
                    $defaultPlaylist.children('.playlist-duration').html('<i class="fa fa-clock-o"></i> &nbsp;' + allDuration);
                    $defaultPlaylist.children('.playlist-size').html('<i class="fa fa-music"></i> &nbsp;' + tracks.length);

                    self.tracksGlobal = tracks;
                    self.showTracks(tracks);
                } else {
                    self.$reloginForm.show();
                    self.$vkMusicTable.hide();
                }

                self.audioManager.bindPlayBtnListeners();
                $('#vkMusicTable > .tableTitle').text("User Tracks");
                self.$vkMusicLoadingSpinner.hide();
                var $activePlaylist = $('.playlist-active');
                if ($activePlaylist.hasClass('default-playlist')) {
                    $('.vkMusicList').show();
                }
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

    this._setFriendDefaultPlaylist = function () {
        var $currentItem = $(this);
        $('.message-sent').hide();
        $('.friend-active').toggleClass('friend-active', false);
        $currentItem.toggleClass('friend-active', true);
        $('.default-playlist').remove();
        self.currentFriend = $currentItem.children('.friend-id').html();
        $.ajax({
            url: '/api/playlists/friend?id=' + self.currentFriend,
            success: function (playlists) {
                if (playlists !== null) {
                    self.playlistsGlobal.length = 0;
                    for (var i = 0; i < playlists.length; i++) {
                        var playlist = playlists[i];
                        if (playlist.Accessibilty === 1) {
                            playlist.Accessibilty = "public";
                        } else {
                            playlist.Accessibilty = "private";
                        }
                        playlist.Duration = self._toFormattedTime(playlist.Duration, true);
                        playlist.readonly = true;
                        self.playlistsGlobal.push(playlist);
                        self.showPlaylists(playlists);
                    }
                } else {
                    playlists = [];
                    self.showPlaylists(playlists);
                    $('.message-send').show();
                    self.$messageContainer.show();
                }
                $('.playlist-divider').remove();
                self.setDefaultPlaylist(self.currentFriend, true);
            }
        });
    };

    this._getFriendTracks = function () {
        var $currentItem = $(this);
        var $defaultPlaylist = $('.default-playlist');
        $('.playlist-active').toggleClass('playlist-active', false);
        var currentId = $currentItem.children('.friend-id').html();
        $currentItem.toggleClass('playlist-active', true);
        self.$vkMusicTitle.text('Loading...');
        self.$vkMusicLoadingSpinner.fadeIn('normal');
        $('.vkMusicList').hide();
        $.ajax({
            url: '/api/user/friends/audio?provider=' + self.provider + '&friendId=' + self.currentFriend,
            success: function (tracks) {
                if (typeof tracks.Message === 'undefined') {
                    var currentUser = $currentItem.children('.friend-initials').html();
                    
                    self.$reloginForm.hide();
                    self.$vkMusicTable.show();
                    var list = $('.vkMusicList');

                    var allDuration = 0;
                    for (var i = 0; i < tracks.length; i++) {
                        allDuration += tracks[i].duration;
                        tracks[i].duration = self._toFormattedTime(tracks[i].duration, true);
                    }

                    allDuration = self._toFormattedTime(allDuration, true);
                    $defaultPlaylist.children('.playlist-duration').html('<i class="fa fa-clock-o"></i> &nbsp;' + allDuration);
                    $defaultPlaylist.children('.playlist-size').html('<i class="fa fa-music"></i> &nbsp;' + tracks.length);

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

        var timeoutId = setTimeout(timer, time);
        self.playlistsTimeouts.push(timeoutId);
        function timer() {
            time = Math.floor((Math.random() * 40000) + 25000);
            self._setNewImage(playlist);
            timeoutId = setTimeout(timer, time);
            self.playlistsTimeouts.push(timeoutId);
        };
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
        var friendId = $('.friend-active').children('.friend-id').html();
        var text = $this.siblings('textarea').text();
        

        VK.Auth.getLoginStatus(function (response) {
            if (response.session) {
                /* Авторизованный в Open API пользователь */
            } else {
                VK.Auth.login(
                    null,
                    VK.access.FRIENDS | VK.access.WALL
                );
            }
        });

        VK.api('wall.post', {
                owner_id: friendId,
                message: text,
                attachments: "photo268940215_338980514,https://" + window.location.host
        }, function (data) {
        });

        $('.message-send').hide();
        $('.message-sent').show();

        e.stopPropagation();
    };
};

MusicManager.prototype.setDefaultPlaylist = function (friendId, isClicked) {
    var self = this;

    var playlist = {
        Name: "VK Playlist",
        Accessibilty: "Private",
        Genres : []
    };

    self.$playlistsTable.prepend('<div class="playlist-divider" />');

    var tmpl = self.playlistTemplate.tmpl(playlist);

    var img = tmpl.children('.playlist-logo').empty();
    img.append('<i class="fa fa-vk fa-5x" style="padding-top: 10px; padding-left: 10px;"></i>');

    tmpl.addClass('default-playlist');
    self.$playlistsTable.prepend(tmpl);

    if (friendId === undefined) {
        tmpl.click(self._getUserTracks);
        if (!self.isSearch) {
            self._getUserTracks();
        }
        else {
            $('.default-playlist').toggleClass("playlist-active", true);
        }
    } else {
        tmpl.click(self._getFriendTracks);
        if (self.isSearch || isClicked) {
            self._getFriendTracks();
        }
        else {
            $('.default-playlist').toggleClass("playlist-active", true);
        }
    }
    self.$playlistsLoadingSpinner.hide();
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
            if (self.audioManager.audio.paused) {
                self.audioManager._setPlayImgButton(tmpl);
            } else {
                self.audioManager._setPauseImgButton(tmpl);
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
    $('.track-info > a').off('click').click(self._getTrackInfo);
};

MusicManager.prototype.showFriends = function (friends, scrollbarInitialized) {
    var self = this;

    self.$notificationsList.hide();
    self.$friendList.show();

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
            whileScrolling: function () {
                if (this.mcs.topPct >= 75 && !self.gettingFriends) {
                    self.gettingFriends = true;
                    self.$friendsLoadingSpinner.fadeIn('normal');
                    $.ajax({
                        url: '/api/user/friends/' + self.provider + "?offset=" + self.friendsOffset + "&count=10",
                        success: function(friendsData) {
                            self.showFriends(friendsData, true);
                            self.friendsOffset += friendsData.length;
                            self.$friendsLoadingSpinner.fadeOut('normal');
                            self.gettingFriends = false;
                        }
                    });
                }
            }
        }
    });

    $('.friend').click(self._setFriendDefaultPlaylist);
};

MusicManager.prototype.showPlaylistTracks = function (tracks) {
    var self = this;
    self.showTracks(tracks, self.playlistTrackTemplate);
    self.audioManager.bindPlayBtnListeners();
};

MusicManager.prototype.showPlaylists = function (playlists) {
    var self = this;
    for (var i = 0; i < self.playlistsTimeouts.length; i++) {
        clearTimeout(self.playlistsTimeouts[i]);
    }
    self.playlistsTimeouts.length = 0;
    self.$messageContainer.hide();
    $('.playlist').remove();
    self.playlists = playlists;
    self.$playlistsLoadingSpinner.show();
    if (typeof playlists === 'undefined') { //Initial run to get playlists from db
        $.ajax({
            url: '/api/playlists',
            success: function (playlistsData) {
                console.log(playlistsData.Message);
                if (typeof playlistsData.Message === 'undefined') {
                    self.playlistsGlobal.length = 0;
                    self.$reloginForm.hide();
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
                    if (self.activePlaylistId != null && self.activePlaylistId.length > 0) {
                        var $active = $('.playlist span:contains(' + self.activePlaylistId +')').parents('.playlist');
                        $('.playlist-active').toggleClass('playlist-active', false);
                        $active.toggleClass('playlist-active', true);
                        self._getTracks();
                        $('.playlist-active').toggleClass('playlist-active', false);
                        $active.toggleClass('playlist-active', true);
                        $('.vkMusicList').show();
                    } else {
                        self.$vkMusicTable.show();
                    }
                } else {
                    self.$reloginForm.show();
                    self.$vkMusicTable.hide();
                }
                //$('.accordion .tableRow').on("click", self._getTracks);

                $.ajax({
                    url: '/api/playlists/favorite/notOwned',
                    success: function (playlistsData) {
                        if (typeof playlistsData.Message === 'undefined') {
                            self.$reloginForm.hide();
                            //self.$vkMusicTable.show();
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
                                tmpl.children('.playlist-title').append($('<i class="fa fa-star"></i>'));
                                self._setNewImage(tmpl);
                                self.$playlistsTable.append(tmpl);
                                self._setChangingPlaylistImage(tmpl);
                            }
                        } else {
                            self.$reloginForm.show();
                            self.$vkMusicTable.hide();
                        }
                        $('.accordion .tableRow:not(.default-playlist)').on("click", self._getTracks);
                        self._createContextMenuForPlaylists();
                    }
                });
            }
        });
    } else { //using to print playlists after using filter
        self.$playlistsTable.empty();
        if (self.playlists.length !== 0) {
            for (var i = 0; i < self.playlists.length; i++) {
                var tmpl = this.playlistTemplate.tmpl(playlists[i]);
                self.$playlistsTable.append(tmpl);

                if (playlists[i].readonly === true) {
                    tmpl.children('.playlist-title').append($('<i class="fa fa-star"></i>'));
                }

                self._setNewImage(tmpl);
                self._setChangingPlaylistImage(tmpl);
                self._createContextMenuForPlaylists();
            }
        }
        $('.accordion .tableRow:not(.default-playlist)').on("click", self._getTracks);
        //self._createContextMenuForPlaylists();
    }

    this._createContextMenuForPlaylists = function() {
        var ctxMenu = new ContextMenu();
        var contextMenuActions = [
            { id: 'makepublic', name: 'Make public', isNewSection: false, hasSubMenu: false, needSelectedItems: false },
            { id: 'makeprivate', name: 'Make private', isNewSection: false, hasSubMenu: false, needSelectedItems: false },
            { id: 'shareplaylist', name: 'Share it', isNewSection: true, hasSubMenu: false, needSelectedItems: false },
            { id: 'renameplaylist', name: 'Rename', isNewSection: true, hasSubMenu: false, neeSelectedItems: false },
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

MusicManager.prototype.showNotifications = function (notifications, scrollbarInitialized) {
    var self = this;
    self.$notificationsList.show();
    self.$friendList.hide();

    for (var i = 0; i < notifications.length; i++) {
        var template;
        if (notifications[i].NotificationType == 0 ||
            notifications[i].NotificationType == 1 ||
            notifications[i].NotificationType == 8 ||
            notifications[i].NotificationType == 9 ||
            notifications[i].NotificationType == 10 ||
            notifications[i].NotificationType == 11) {
            template = $('#playlistNotificationTemplate');
        } else {
            template = $('#userNotificationTemplate');
        }
        var object = template.tmpl(notifications[i]);
        var container = scrollbarInitialized ? self.$notificationsList.find('.mCSB_container') : self.$notificationsList;

        container.append(object);
        //self.$notificationsList.append(object);
    }

    $('#notifications-container').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        advanced: { updateOnSelectorChange: "true" },
        callbacks: {
            whileScrolling: function () {
                if (this.mcs.topPct >= 75 && !self.gettingNotifications) {
                    self.gettingNotifications = true;
                    self.$friendsLoadingSpinner.fadeIn('normal');
                    var userId = $('#userId').val();
                    $.ajax({
                        //url: '/api/notifications/' + userId + '/' + self.notificationsOffset,
                        url: '/api/notifications/followings' + userId + '/' + $('.list-notification-content').length,
                        type: 'GET',
                        success: function (notificationData) {
                            self.showNotifications(notificationData, true);
                            //self.notificationsOffset += notificationData.length;
                            self.$friendsLoadingSpinner.fadeOut('normal');
                            self.gettingNotifications = false;
                        }
                    });
                }
            }
        }
        //callbacks: {
        //    onTotalScroll: function () {
        //        self.$friendsLoadingSpinner.fadeIn('normal');
        //$.ajax({
        //    url: '/api/user/friends/' + self.provider + "?offset=" + self.friendsOffset + "&count=10",
        //    success: function (friendsData) {
        //        self.showFriends(friendsData, true);
        //        self.friendsOffset += friendsData.length;
        //        self.$friendsLoadingSpinner.fadeOut('normal');
        //    }
        //});
        //    }
        //}
    });
    $('.list-notification-close').click(function () {
        var $notificationToDelete = $(this).parents('.list-notification-item');
        $notificationToDelete.remove();
    });
};

MusicManager.prototype.bindListeners = function() {
    var self = this;

    $(document).on('PlaylistAdded', function(playlist) { // TODO Remove event triggering on document object
        self.playlistsGlobal.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
        self.$searchPlaylistInput.trigger('input');
    });

    $('.send-message-btn').click(self._sendMessage);

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
                if ($('#itemsContainer').children().length != 0) {
                    self._saveTrackFromVkToPlaylist($('#itemsContainer'), -1, playlistId);
                } else {
                    self.showPlaylists();
                    self.setDefaultPlaylist();
                }
            }
        });

        //$('#createPlaylistModal').trigger('OnPlaylistCreate');
    });

    $('#okPlaylistRenameModalBtn').click(function() {
        $('#renamePlaylistModal').modal('hide');
        $('#renamePlaylistModal').on('hidden.bs.modal', function () {
            $('#renamePlaylistModal .modal-body #playlistNameToRename').val("");
            $('#renamePlaylistModal .modal-body select :first').attr("selected", "selected");
        });

        var $playlist = $('.playlist.selected');
        var playlistId = $playlist.find('.playlistId').text();
        var playlistName = $('#playlistNameToRename').val();
        $.ajax({
            url: '/api/playlists/' + playlistId + '?playlistName=' + playlistName,
            type: 'POST',
            contentType: 'application/json',
            success: function(playlistName) {
                $playlist.children('.playlist-title').text(playlistName);
                $('#playlistsTable').trigger('OnChange');
            }
        });
    });

    self.$getUserTracksBtn.click(function(e) {
        self._getUserTracks(self.provider);
        self.$searchTrackInput.val('');
    });

    this.$searchTrackInput.keyup(function(e) {
        var searchParam = $(this).val().toLocaleLowerCase();
        if ($('.playlist-active').hasClass('default-playlist')) {
            self.showTracks(self.tracksGlobal.filter(function (index) {
            self.$searchTrackInput.next().next().children().remove();
            return ((index.title.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.artist.toLocaleLowerCase().indexOf(searchParam) != -1));
        }));
        } else {

            self.showTracks(self.playlistTracksGlobal.filter(function (index) {
                self.$searchTrackInput.next().next().children().remove();
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.Artist.toLocaleLowerCase().indexOf(searchParam) != -1));
            }), self.playlistTrackTemplate);
        }
        self.audioManager.refreshTracks();
        self.audioManager.updateProgressbar('.vkMusicList');
    });

    this._collapseFriendsPanel = function() {
        self.$friendButton.css('color', '');
        self.$extraContainer.toggleClass('col-md-0', true).toggleClass('col-md-3', false);
        self.$trackContainer.toggleClass('col-md-5', false).toggleClass('col-md-8', true);
        $('.playlist-divider').remove();
        self.showPlaylists();
        self.setDefaultPlaylist();
        $('#friends-container').hide();
        self.friendsListOpen = false;
    };

    this._expandFriendsPanel = function() {
        self.$friendButton.css('color', '#F9851F');
        self._getFriendInfo();
        $('#friends-container').show();
        $('.friend-active').toggleClass('friend-active', false);
        self.$extraContainer.toggleClass('col-md-0', false).toggleClass('col-md-3', true);
        self.$trackContainer.toggleClass('col-md-8', false).toggleClass('col-md-5', true);
        self.friendsListOpen = true;
    };

    this.$friendButton.click(function () {
        var delay = 0;
        self.extraContainerShown = !self.extraContainerShown;

        if (!self.extraContainerShown && !self.notificationListOpen) {
            self._collapseFriendsPanel();
        } else {
            if (self.notificationListOpen) {
                self._collapseNotificationPanel();
                delay = 800;
            }
            setTimeout(function() { self._expandFriendsPanel(); }, delay);
        }
    });

    this._collapseNotificationPanel = function() {
        self.$notificationButton.css('color', '');
        self.$extraContainer.toggleClass('col-md-0', true).toggleClass('col-md-3', false);
        self.$trackContainer.toggleClass('col-md-5', false).toggleClass('col-md-8', true);
        //self.$notificationsList.remove('.list-notification-header');
        $('.list-notification-item').remove();
        $('#notification-container').hide();
        self.notificationListOpen = false;
        self.notificationsOffset = 0;
    };

    this._expandNotificationPanel = function() {
        self.$notificationButton.css('color', '#F9851F');
        $('#notification-container').show();
        self.$extraContainer.toggleClass('col-md-0', false).toggleClass('col-md-3', true);
        self.$trackContainer.toggleClass('col-md-8', false).toggleClass('col-md-5', true);
        self.notificationListOpen = true;
    };

    this.$notificationButton.click(function () {
        var delay = 0;
        self.extraContainerShown = !self.extraContainerShown;
        if (!self.extraContainerShown && !self.friendsListOpen) {
            self._collapseNotificationPanel();
        } else {
            if (self.friendsListOpen) {
                self._collapseFriendsPanel();
                delay = 800;
            }
            setTimeout(function() { self._expandNotificationPanel(); }, delay);
        }

    });

    this.$getNotificationsButton.click(function() {
        var userId = $('#userId').val();
        if (self.extraContainerShown) {
            $.ajax({
                //url: '/api/notifications/followings/' + userId,
                url: '/api/notifications/followings/' + userId,
                type: 'GET',
                success: function (notifications) {
                    var $scroll = $('#notifications-container').find('.mCSB_container');
                    if ($scroll.length == 0) {
                        self.showNotifications(notifications, false);
                    } else {
                        self.showNotifications(notifications, true);
                    }
                }
            });
        }


//if (self.$friendsBody.is(':visible')) {
        //    self.$friendsBody.hide('slow');
        //} else if (self.$friendList.children().length == 0) {
        //    $.ajax({
        //        url: '/api/user/friends/' + self.provider + "?offset=" + self.friendsOffset + "&count=10",
        //        success: function (friends) {
        //            self.showFriends(friends);
        //            self.$friendsBody.show('slow');
        //            self.friendsOffset += friends.length;
        //        }
        //    });
        //} else {
        //    self.$friendsBody.show('slow');
        //}
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
                    $('.playlist-divider').remove();
                    self.setDefaultPlaylist();
                }
            } else {
                self.$createNewPlaylistLbl.hide();
            }
            $('.playlist-divider').remove();
            self.showPlaylists(foundedPlaylist);
            self.isSearch = true;
            self.setDefaultPlaylist();
            $('.accordion .tableRow:not(.default-playlist)').on("click", self._getTracks);
        } else {
            self.showPlaylistTracks(self.playlistTracksGlobal.filter(function(index) {
                self.$searchPlaylistInput.next().children().remove();
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.Artist.toLocaleLowerCase().indexOf(searchParam) != -1));
            }));
            self.audioManager.refreshPlaylistTracks();
            self.audioManager.updateProgressbar('#playlistTracks');
        }
        self.$playlistsLoadingSpinner.hide();
    });

    $('#searchPlaylistName').focusout(function () {
        self.isSearch = false;
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
            success: function () {
                $('.playlist-divider').remove();
                self.showPlaylists();
                self.setDefaultPlaylist();
                self.$playlistsLoadingSpinner.hide();
            },
            error: function () {
                $('.playlist-divider').remove();
                self.showPlaylists();
                self.setDefaultPlaylist();
                self.$playlistsLoadingSpinner.hide();
            }
        });
        self.$createNewPlaylistLbl.hide();
        $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
    };

    this._getFriendInfo = function() {
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
    };

    $('#infoModal').on('hidden.bs.modal', function() {
        $('#infoModal .modal-body').text('');
        $('#listenTopBtn').attr('disabled', true);
    });

    $(self.audioManager).on('OnAddToPlaylist', function () {
        if (self.$extraContainer.hasClass('col-md-0')) {
            self.showPlaylists();
            self.setDefaultPlaylist();
        }
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
                    url: '/api/usertracks/tracksearch?provider=Vkontakte',
                    type: 'POST',
                    dataType: 'json',
                    data: tracks,
                    contentType: 'application/json; charset=utf-8',
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
                                { 'id': 'createplaylist', 'name': 'Create new playlist', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true }
                            ]
                        });
                        self.$vkMusicLoadingSpinner.hide();
                    },
            }).fail(function() {
                self.$vkMusicLoadingSpinner.hide();
                    self.$errorModal.find('.error').text('Error occured during search ...');
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
