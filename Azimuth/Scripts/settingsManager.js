var SettingsManager = function (manager) {
    var self = this;
    this.audioManager = manager;
    this.tracksGlobal = [];
    this.playlistsGlobal = [];
    this.friendsOffset = 0;
    this.playlistTracksGlobal = [];
    this.stringForCreateBtn = "Create new playlist ";
    this.playlistTrackTemplate = $("#playlistTrackTemplate");
    this.playlistTemplate = $("#playlistTemplate");
    this.trackTemplate = $("#trackTemplate");
    this.$friendsTemplate = $("#friendsTemplate");
    this.$friendList = $('#friends-container');
    this.$reloginForm = $("#relogin");
    this.$vkMusic = $("#vkontakteMusic");
    this.$playlistsTable = $('#playlistsTable');
    this.$searchPlaylistInput = $('#searchPlaylistName');
    this.$searchTrackInput = $('#searchTrackName');
    this.$vkMusicTable = $('#vkMusicTable').parent();
    this.$createNewPlaylistLbl = $('#create-playlist-lbl');
    this.$getFriendInfoBtn = $('#get-friends-info-btn');

    this._getTracks = function () {
        $.ajax({
            url: "/api/usertracks?playlistId=" + $(this).find('.playlistId').text(), // TODO replace with class playlistID
            type: 'GET',
            async: false,
            success: function (tracksData) {
                for (var i = 0; i < tracksData.length; i++) {
                    tracksData[i].Duration = Math.floor(tracksData[i].Duration / 60) + ":" + (tracksData[i].Duration % 60 < 10 ? "0" + tracksData[i].Duration % 60 : tracksData[i].Duration % 60);
                }
                self.playlistTracksGlobal = tracksData;
                self.showPlaylistTracks(tracksData);
                self.$playlistsTable.parent().parent().parent().makeDraggable({
                    contextMenu: [
						{ 'id': 'selectall', 'name': 'Select All', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false, "callback": selectAllTracksAction },
						{ 'id': 'copytoplaylist', 'name': 'Copy to another playlist', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true, "callback": copyTrackToPlaylistAction },
                        { 'id': 'movetoplaylist', 'name': 'Move to another plylist', "isNewSection": false, "hasSubMenu": true, "needSelectedItems": true, "callback": moveTracksBetweenPlaylistsAction },
						{ 'id': 'removeselected', 'name': 'Remove selected', "isNewSection": true, "hasSubMenu": false, "needSelectedItems": true, "callback": removeSelectedTracksAction }
                    ],
                    onMoveTrackToNewPosition: moveTrackToNewPosition,
                    showSubContextMenu: showSubContextMenuAction,
                });
                self.$searchPlaylistInput.val('');
            }
        });
        //}
        //$(this).next("#playlistTracksTable").slideToggle(100); // TODO replace with class name
        $('#playlistsTable').hide();
        $('#backToPlaylistsBtn').show();
        $('#playlistTracks').show();
        self.audioManager.bindPlayBtnListeners();
        $(this).toggleClass("active");
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


};

SettingsManager.prototype.showTracks = function (tracks) {
    var self = this;

    $('.vkMusicList').find('.track').remove();
    for (var i = 0; i < tracks.length; i++) {
        self.trackTemplate.tmpl(tracks[i]).appendTo('.vkMusicList');
    }
};

SettingsManager.prototype.showFriends = function(friends) {
    var self = this;

    for (var i = 0; i < friends.length; i++) {
        var friend = friends[i];
        if (friend.city != null) {
            friend.city = friend.city.title;
        }
        if (friend.country != null) {
            friend.country = friend.country.title;
        }
        self.$friendList.append(self.$friendsTemplate.tmpl(friends[i]));
    }
};

SettingsManager.prototype.showPlaylistTracks = function (tracks) {
    var self = this;

    $('#playlistTracks').find('.track').remove();
    for (var i = 0; i < tracks.length; i++) {
        self.playlistTrackTemplate.tmpl(tracks[i]).appendTo('#playlistTracks');
    }
};

SettingsManager.prototype.showPlaylists = function (playlists) {
    var self = this;
    self.$playlistsTable.find(".tableHeader").remove();
    if (typeof playlists === 'undefined') { //Initial run to get playlists from db
        $.ajax({
            url: '/api/playlists',
            success: function (playlistsData) {
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
                        self.$playlistsTable.append(self.playlistTemplate.tmpl(playlist));
                    }
                } else {
                    self.$reloginForm.show();
                    self.$reloginForm.find('a').attr('href', reloginUrl);
                    self.$vkMusicTable.hide();
                }
                $('.accordion .tableRow').on("click", self._getTracks);
            }
        });
    } else { //using to print playlists after using filter
        if (self.playlists.length !== 0) {
            for (var i = 0; i < playlists.length; i++) {
                self.$playlistsTable.append(this.playlistTemplate.tmpl(playlists[i]));
            }
        } else {
            self.$createNewPlaylistBtn.show();
            self.$createNewPlaylistBtn.text(this.stringForCreateBtn + this.$searchPlaylistInput.val());
        }
    }
};

SettingsManager.prototype.bindListeners = function() {
    var self = this;

    $(document).on('PlaylistAdded', function(playlist) { // TODO Remove event triggering on document object
        self.playlistsGlobal.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
        self.$searchPlaylistInput.trigger('input');
    });

    $('.providerBtn').click(function(e) {
        var provider = $(e.target).data('provider');
        var reloginUrl = $(e.target).data('reloginurl');
        $.ajax({
            url: '/api/usertracks?provider=' + provider,
            success: function(tracks) {
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
                            { 'id': 'selectall', 'name': 'Select all', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": false, "callback": selectAllTracksAction },
                            { 'id': 'hideselected', 'name': 'Hide selected', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true, "callback": hideSelectedTracksAction },
                            { 'id': 'savevktrack', 'name': 'Move to', "isNewSection": true, "hasSubMenu": true, "needSelectedItems": true, "callback": saveTrackFromVkToPlaylist },
                            { 'id': 'createplaylist', 'name': 'Create new playlist', "isNewSection": false, "hasSubMenu": false, "needSelectedItems": true, "callback": createPlaylistAction }
                        ],
                        showSubContextMenu: showSubContextMenuAction    
                    });
                } else {
                    self.$reloginForm.show();
                    self.$reloginForm.find('a').attr('href', reloginUrl);
                    self.$vkMusicTable.hide();
                }
                self.audioManager.bindPlayBtnListeners();
            }
        });
    });

    this.$searchTrackInput.keyup(function(e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        self.showTracks(self.tracksGlobal.filter(function(index) {
            self.$searchTrackInput.next().next().children().remove();
            return ((index.title.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.artist.toLocaleLowerCase().indexOf(searchParam) != -1));
        }));

        self.audioManager.bindPlayBtnListeners();
        self.progressSlider.bindListeners();
    });

    this.$searchPlaylistInput.keyup(function(e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        if (self.$playlistsTable.css('visibility') != 'hidden' && self.$playlistsTable.css('display') != 'none') {

            var foundedPlaylist = self.playlistsGlobal.filter(function(index) {
                self.$searchPlaylistInput.next().children().remove();
                return (index.Name.toLocaleLowerCase().indexOf(searchParam) != -1);
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
            self.showPlaylistTracks(self.playlistTracksGlobal.filter(function(index) {
                self.$searchPlaylistInput.next().children().remove();
                return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.Artist.toLocaleLowerCase().indexOf(searchParam) != -1));
            }));
            self.audioManager.bindPlayBtnListeners();
            self.progressSlider.bindListeners();
        }
    });


    $('#backToPlaylistsBtn').click(function() {
        $('#backToPlaylistsBtn').hide();
        $('#playlistTracks').empty();
        $('#playlistTracks').hide();
        self.$playlistsTable.empty();
        self.showPlaylists(self.playlistsGlobal);
        self.$playlistsTable.show();
        self.$searchPlaylistInput.val('');
        $('.accordion .tableRow').on("click", self._getTracks);
    });

    $('#playlists').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        advanced: { updateOnSelectorChange: "true"}
    });

    $('#vk-track-list').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        advanced: { updateOnSelectorChange: "true" }
    });
 
    this._createPlaylist = function() {
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
        if (self.$friendList.is(':visible')) {
            self.$friendList.hide('slow');
        } else if (self.$friendList.children().length == 0) {
            var provider = $('.tab-pane.active').attr('id');
            $.ajax({
                url: '/api/user/friends/' + provider + "?offset="+ self.friendsOffset + "&count=10",
                async: true,
                success: function(friends) {
                    self.showFriends(friends);
                    self.$friendList.show('slow');
                    self.friendsOffset += friends.length;
                }
            });
        } else {
            self.$friendList.show('slow');
        }
    });

    self.$friendList.click(function(e) {;
        var $currentItem = $(e.target);
        var currentId;
        if ($currentItem.hasClass('friend')) {
            currentId = $currentItem.children('.friend-id').html();
        } else {
            currentId = $currentItem.parents('.friend').children('.friend-id').html();
        }
        var provider = "Vkontakte"; // TODO: Fix for all providers
        var reloginUrl = $(e.target).data('reloginurl');
        $.ajax({
            url: '/api/user/friends/audio?provider=' + provider + '&friendId=' + currentId,
            success: function (tracks) {
                if (typeof tracks.Message === 'undefined') {
                    self.$reloginForm.hide();
                    self.$vkMusicTable.show();
                    var list = $('.vkMusicList');
                    for (var i = 0; i < tracks.length; i++) {
                        tracks[i].duration = self._toFormattedTime(tracks[i].duration, true);
                    }
                    self.tracksGlobal = tracks;
                    self.showTracks(tracks);
                    list.parent().parent().makeDraggable({
                        contextMenu: [
                            { 'id': '1', 'name': 'Select All', "isNewSection": "false" },
                            { 'id': '2', 'name': 'second action', "isNewSection": "false" },
                            { 'id': '3', 'name': 'third action', "isNewSection": "true" },
                            { 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
                        ],
                        saveVkTrack: saveTrackFromVkToPlaylist
                    });
                } else {
                    self.$reloginForm.show();
                    self.$reloginForm.find('a').attr('href', reloginUrl);
                    self.$vkMusicTable.hide();
                }
                self.audioManager.bindPlayBtnListeners();
            }
        });
    });

    $('#get-other').click(function () {
        var $this = $(this);
        $this.hide();
        var provider = $('.tab-pane.active').attr('id');
        $.ajax({
            url: '/api/user/friends/' + provider + "?offset=" + self.friendsOffset + "&count=10",
            success: function (friends) {
                self.showFriends(friends);
                self.$friendList.show('slow');
                self.friendsOffset += friends.length;
                $this.show();
            }
        });
    });
};


var moveTrackToNewPosition = function($currentItem, $draggableStub) {
    var playlistId = $('.playlist.active').children('.playlistId').text();
    var tracksIds = [];

    $currentItem.children().each(function() {
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

var saveTrackFromVkToPlaylist = function ($currentItem, index, playlistId) {
    var self = this;
    var provider = $('.tab-pane.active').attr('id');
    var tracks = [];
    $currentItem.children().toggleClass('vk-item', false);
    $currentItem.children('.draggable-item-selected').each(function () {
        tracks.push($(this).closest('.tableRow').find('.trackId').text());
    }).get();

    $.ajax({
        url: '/api/usertracks?provider=' + provider + "&index=" + index,
        type: 'POST',
        data: JSON.stringify({
            "Id": playlistId,
            "TrackIds": tracks
        }),
        contentType: 'application/json',
        success: function () {
            self.showPlaylists();
        }
    });
};

var selectAllTracksAction = function(list) {
    list.toggleClass('draggable-item-selected', true);
};

var removeSelectedTracksAction = function ($currentItem, playlistId) {
    var self = this;
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
            self.showPlaylists();
        }
    });
};

var hideSelectedTracksAction = function(list) {
    list.hide();
    list.toggleClass('draggable-item-selected', false);
};

var createPlaylistAction = function() {
    alert('createplaylist');
};

var showSubContextMenuAction = function($subContextMenuContainer, $object, $toElement) {
    if ($object.hasClass('hasSubMenu') && $toElement.parents('.subMenu').length < 1) {
        $subContextMenuContainer.hide();
    }
};

var moveTracksBetweenPlaylistsAction = function($currentItem, newPlaylist, oldPlaylist) {
    var self = this;
    var tracksIds = [];
    $currentItem.children('.draggable-item-selected').each(function() {
        tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
    }).get();
    $.ajax({
        url: '/api/usertracks/copy?playlistId=' + newPlaylist,
        type: 'POST',
        data: JSON.stringify(tracksIds),
        contentType: 'application/json; charset=utf-8',
        success: function() {
            $.ajax({
                url: '/api/usertracks/delete?playlistId=' + oldPlaylist,
                type: 'DELETE',
                dataType: 'json',
                data: JSON.stringify(tracksIds),
                contentType: 'application/json; charset=utf-8',
                success: function() {
                    self.showPlaylists();
                }
            });
        }
    });
};

var copyTrackToPlaylistAction = function($currentItem, playlistId) {
    var tracksIds = [];
    $currentItem.children('.draggable-item-selected').each(function() {
        tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
    }).get();
    $.ajax({
        url: '/api/usertracks/copy?playlistId=' + playlistId,
        type: 'POST',
        data: JSON.stringify(tracksIds),
        contentType: 'application/json; charset=utf-8'
    });
};
