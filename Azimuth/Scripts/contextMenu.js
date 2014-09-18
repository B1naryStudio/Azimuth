var ContextMenu = function() {
    var self = this;
    // Context menu container creation
    this.$contextMenuContainer = $('<div>');
    this.$contextMenuContainer.addClass('contextMenu');
    this.$contextMenuContainer.attr('id', this.contextMenuId);
    // Subcontext menu container creation
    this.$subContextMenuContainer = $('<div>');
    this.$subContextMenuContainer.addClass('contextMenu');
    this.$subContextMenuContainer.addClass('subMenu');
    this.contextMenuSelected = false;
    this.$container = $('#itemsContainer');
    this.$contextMenuTemplate = $('#contextmenuTemplate');
    this.$subContextMenuTemplate = $('#subContextmenuTemplate');
    this.manager = null;
    this.musicList = null;
    this.timerId = null;
    this.provider = 'Vkontakte';
    this.host = window.location.protocol + "//" + window.location.host + "/";

    this._saveTrackFromVkToPlaylist = function($currentItem, index, playlistId) {
        self.manager._saveTrackFromVkToPlaylist($currentItem, index, playlistId);
    };

    this._selectAllTracksAction = function(list) {
        list.toggleClass('draggable-item-selected', true);
    };

    this._removeSelectedTracksAction = function($currentItem, playlistId) {
        var tracksIds = [];
        $currentItem.children('.draggable-item-selected').each(function() {
            tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        $.ajax({
            url: '/api/usertracks/delete?playlistId=' + playlistId,
            type: 'DELETE',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8',
            success: function() {
                var playlistId = $('#playlistTracks').find('.playlistId').text();
                $('#playlistTracks').children().remove();
                self.manager._getTracks(playlistId);

                $('.tableRow.playlist').remove();
                self.manager.playlistsGlobal.length = 0;
                $('.playlist-divider').remove();
                self.manager.showPlaylists();
                self.manager.setDefaultPlaylist();
            }
        });
    };

    this._hideSelectedTracksAction = function(list) {
        list.detach();
        list.toggleClass('draggable-item-selected', false);
        self.audioManager.refreshTracks();
    };

    this._moveTracksBetweenPlaylistsAction = function($currentItem, newPlaylist, oldPlaylist) {
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
                    data: JSON.stringify(tracksIds),
                    contentType: 'application/json; charset=utf-8',
                    success: function() {
                        $('#playlistTracks').children().remove();
                        self.manager._getTracks(oldPlaylist);

                        $('.tableRow.playlist').remove();
                        self.manager.playlistsGlobal.length = 0;
                        $('.playlist-divider').remove();
                        self.manager.showPlaylists();
                        self.manager.setDefaultPlaylist();
                    }
                });
            }
        });
    };

    this._copyTrackToPlaylistAction = function($currentItem, playlistId) {
        var tracksIds = [];
        $currentItem.children('.draggable-item-selected').each(function() {
            tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        $.ajax({
            url: '/api/usertracks/copy?playlistId=' + playlistId,
            type: 'POST',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8',
            success: function() {
                if ($('#playlistTracks').children().length > 0) {
                    var playlistId = $('#playlistTracks').find('.playlistId').text();
                    $('#playlistTracks').children().remove();
                    self.manager._getTracks(playlistId);
                }

                $('.tableRow.playlist').remove();
                self.manager.playlistsGlobal.length = 0;
                $('.playlist-divider').remove();
                self.manager.showPlaylists();
                self.manager.setDefaultPlaylist();
            }
        });
    };

    this._changePlaylistAccessibility = function(playlistId, accessibility) {
        $.ajax({
            url: 'api/playlists?id=' + playlistId + '&accessibilty=' + accessibility,
            type: 'PUT',
            contentType: 'application/json; charset=utf-8',
            success: function () {
                self.manager.playlistsGlobal.length = 0;
                $('.playlist-divider').remove();
                self.manager.showPlaylists();
                self.manager.setDefaultPlaylist();
            }
        });
    };

    this._removePlaylist = function(playlistId) {
        $.ajax({
            url: 'api/playlists/delete/' + playlistId,
            type: 'DELETE',
            success: function() {
                self.manager.playlistsGlobal.length = 0;
                $('.playlist-divider').remove();
                self.manager.showPlaylists();
                self.manager.setDefaultPlaylist();
                $('#playlistsTable').trigger('OnChange');
            }
        });
    };

    this._sharePlaylist = function (playlistId) {
        var $sharingLink = $('#sharingLink');
        var $sharingPlaylist = $('#sharing-playlist');

        $('#sharingLinkModal').modal({
            show: true
        });
        $sharingLink.val("");
        $sharingPlaylist.val("");

        $.ajax({
            url: '/api/playlists/share/' + playlistId,
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $sharingLink.val(self.host + "Share/Index?azimuth_playlist=" + data);
                $sharingPlaylist.val("Share_" + data);
                $('#playlist-guid').html(data);
                $sharingPlaylist.focusout(self._changePlaylistName);
                self._initClipboard($sharingLink);
                $sharingPlaylist.focus();
                $sharingPlaylist.select();
            }
        });
        
        
    };

    this._shareTracks = function() {
        var tracksIds = [];
        $('.draggable-item-selected').each(function() {
            tracksIds.push($(this).closest('.tableRow').find('.trackId').text());
        }).get();
        $('#sharingLinkModal').modal({
            show: true
        });

        var $sharingLink = $('#sharingLink');
        var $sharingPlaylist = $('#sharing-playlist');
        $sharingLink.val("");
        $sharingPlaylist.val("");
        $.ajax({
            url: '/api/playlists/share',
            type: 'PUT',
            dataType: 'json',
            data: JSON.stringify(tracksIds),
            contentType: 'application/json; charset=utf-8',
            success: function(guid) {
                $sharingLink.val(self.host + "Share/Index?azimuth_playlist=" + guid);
                $sharingPlaylist.val("Share_" + guid);
                $('#playlist-guid').html(guid);
                $sharingPlaylist.focusout(self._changePlaylistName);
                self._initClipboard($sharingLink);
                $sharingPlaylist.focus();
                $sharingPlaylist.select();
            }
        });
    };

    this._initClipboard = function($sharingLink) {
        $('#copy-to-clipboard').clipboard({
            path: '/Scripts/jquery.clipboard.swf',
            copy: function () {
                //alert($sharingLink.val());
                return $sharingLink.val();
            }
        });
    };

    this._changePlaylistName = function() {
        var playlistName = $('#sharing-playlist').val();
        var azimuthPlaylist = $('#playlist-guid').html();
        $.ajax({
            url: '/api/playlists/share?azimuthPlaylist=' + azimuthPlaylist + '&playlistName=' + playlistName,
            async: true,
            type: 'GET',
            success: function (newName) {
                $('#sharing-playlist').val(newName);
            }
        });
    };
};

ContextMenu.prototype.initializeContextMenu = function (contextId, contextmenuoptions, currentList, manager) {
    var self = this;
    self.manager = manager;
    self.contextMenuId = 'container-id' + contextId;
    self.musicList = currentList;

    for (var i = 0; i < contextmenuoptions.length; i++) {
        var object = self.$contextMenuTemplate.tmpl(contextmenuoptions[i]);
        if (contextmenuoptions[i].needSelectedItems == true) {
            object.toggleClass('needSelectedItems', true);
            object.toggleClass('unactiveContextMenuAction', true);
        }
        if (contextmenuoptions[i].hasSubMenu == true) {
            // WRONG!!!!
            object.children('.contextMenuActionName').text(object.children('.contextMenuActionName').text() + ' >');
            object.toggleClass('hasSubMenu', true);
        }
        if (contextmenuoptions[i].isNewSection == true) {

            self.$contextMenuContainer.append("<hr/>");
        }

        object.appendTo(self.$contextMenuContainer);
    }
    self.$contextMenuContainer.children().css('white-space', 'nowrap').css('float', 'left');
    self.$contextMenuContainer.css('width', 'auto');
    self.$subContextMenuContainer.css('width', 'auto');
};

ContextMenu.prototype.selectAction = function ($currentItem, $musicList) {
    var self = this;
    var $target = $(event.target);
    if (self.contextMenuSelected == true && event.which != 3 && !$target.parent().hasClass('hasSubMenu')) {

        self.$contextMenuContainer.detach();
        self.$subContextMenuContainer.detach();
    }
    if ($target.hasClass('contextMenuActionName') && !$target.parent().hasClass('hasSubMenu')) {
        var action = "";
        if ($target.parents().hasClass('subMenu')) {
            action = self.$subContextMenuContainer.attr('action');
        } else {
            action = $target.attr('id');
        }

        if (!$target.parent().hasClass('unactiveContextMenuAction')) {
            switch (action) {
                case 'selectall':
                    $currentItem = null;
                    var $itemsToSelect = self.musicList.children().find('.track');
                    self._selectAllTracksAction($itemsToSelect);
                    $currentItem = null;
                    break;
                case 'copytoplaylist':
                    $currentItem = self.$container;
                    $currentItem.hide();
                    $currentItem.append($('.draggable-item-selected').clone());
                    if ($currentItem.children().length > 0) {
                        var playlistId = $target.parent().children('.playlistId').text();
                        self._copyTrackToPlaylistAction($currentItem, playlistId);
                        self.$container.empty();
                        $currentItem = null;
                    }
                    break;
                case 'movetoplaylist':
                    var newPlaylist = $target.parent().children('.playlistId').text();
                    //var oldPlaylist = $currentItem.parent().children('.playlistId').text();
                    var oldPlaylist = $('.playlist-active').children('.playlistId').text();
                    $currentItem = self.$container;
                    $currentItem.hide();
                    $currentItem.append($('.draggable-item-selected').clone());
                    if ($currentItem.children().length > 0) {
                        self._moveTracksBetweenPlaylistsAction($currentItem, newPlaylist, oldPlaylist);
                        self.$container.empty();
                        $currentItem = null;
                    }
                    break;
                case 'savevktrack':
                    var index = -1;
                    $currentItem = self.$container;
                    $currentItem.hide();
                    $currentItem.append($('.draggable-item-selected').clone());
                    if ($currentItem.children().length > 0) {
                        self._saveTrackFromVkToPlaylist($currentItem, index, $target.parent().children('.playlistId').text());
                    }
                    self.$container.empty();
                    break;
                case 'removeselected':
                    //var playlistId = $currentItem.parent().children('.playlistId').text();
                    var playlistId = $('.playlist-active').children('.playlistId').text();
                    $currentItem = self.$container;
                    $currentItem.hide();
                    $currentItem.append($('.draggable-item-selected').clone());
                    if ($currentItem.children().length > 0) {
                        self._removeSelectedTracksAction($currentItem, playlistId);
                        self._removeSelectedTracksAction($currentItem, playlistId);
                        self.$container.empty();
                        $currentItem = null;
                    }
                    break;
                case 'hideselected':
                    $currentItem = null;
                    self._hideSelectedTracksAction($('.track.draggable-item-selected'));
                    break;
                case 'createplaylist':
                    $currentItem = self.$container;
                    $currentItem.hide();
                    $currentItem.append($('.draggable-item-selected').clone());
                    if ($currentItem.children().length > 0) {
                        $('#createPlaylistModal').modal({
                            show: true
                        });
                        $('#createPlaylistModal').off('shown.bx.modal').on('shown.bs.modal', function () {
                            $("#playlistNameToCreate").focus();
                        });
                    }
                    break;
                case 'renameplaylist':
                    $('#renamePlaylistModal').modal({
                        show: true
                    });
                    $('#renamePlaylistModal').off('shown.bx.modal').on('shown.bs.modal', function () {
                        $("#playlistNameToRename").focus();
                    });
                    break;
                //case 'trackshuffle':
                //    self._shuffleTracksAction(self.musicList.find('.draggable-list'));
                //    break;
                case 'makepublic':
                    var playlistId = $('.playlist.selected').find('.playlistId').text();
                    self._changePlaylistAccessibility(playlistId, 'Public');
                    break;
                case 'makeprivate':
                    var playlistId = $('.playlist.selected').find('.playlistId').text();
                    self._changePlaylistAccessibility(playlistId, 'Private');
                    break;
                case 'shareplaylist':
                    var playlistId = $('.playlist.selected').find('.playlistId').text();
                    self._sharePlaylist(playlistId);
                    break;
                case 'sharetracks':
                    self._shareTracks();
                    break;
                case 'removeplaylist':
                    var $playlist = $('.playlist.selected');
                    var plId = $playlist.find('.playlistId').text();
                    self._removePlaylist(plId);
                    break;
            }
        }
    }
};

ContextMenu.prototype.drawContextMenu = function (event) {
    var self = this;

    document.oncontextmenu = function () {
        return false;
    }
    var anotherContextMenu = $('.contextMenu');
    if (anotherContextMenu.length > 0) {
        anotherContextMenu.detach();
    }

    self.contextMenuSelected = true;
    var $target = $(event.target).parents('.draggable-list').parent();
    if ($target.hasClass('vkMusicTable')) {
        $target = $target.parent();
    }
    if ($(event.target).parents().hasClass('playlist')) {

        var $playlist = $(event.target).parents('.playlist');

        if ($playlist.find('.readonly').text() == 'true') {
            self.$contextMenuContainer.children('.tableRow').toggleClass('unactiveContextMenuAction', true);
            self.$contextMenuContainer.children('.tableRow').has('#removeplaylist').toggleClass('unactiveContextMenuAction', false);
            self.$contextMenuContainer.children('.tableRow').has('#shareplaylist').toggleClass('unactiveContextMenuAction', false);
        }else if($playlist.hasClass('default-playlist')){
            self.$contextMenuContainer.children('.tableRow').toggleClass('unactiveContextMenuAction', true);
        }else {
            self.$contextMenuContainer.children('.tableRow').toggleClass('unactiveContextMenuAction', false);
        }
    }
    var y = $(event.clientY)[0];
    var x = $(event.clientX)[0];

    if ((self.$contextMenuContainer.height() + y) > $(window).height()) {
        y = y - self.$contextMenuContainer.height();
    }
    if ((self.$contextMenuContainer.width() + x) > $(window).width()) {
        x = x - self.$contextMenuContainer.width();
    }
    self.$contextMenuContainer.css({
        'top': y + 'px',
        'left': x + 'px'
});
    $('body').append(self.$contextMenuContainer);

    if ($('.draggable-item-selected').length == 0) {
        $('.needSelectedItems').toggleClass('unactiveContextMenuAction', true);
    } else {
        $('.needSelectedItems').toggleClass('unactiveContextMenuAction', false);
    }
    self.$contextMenuContainer.show();
    self.$contextMenuContainer.children('.tableRow').hover(function (event) {

        $('.contextMenu .tableRow:hover').off('mouseleave').mouseleave(function (event) {

            var $target = $(event.target);
            if ($('.subMenu').length > 0) {
                if (!$target.hasClass('subMenu')) {
                    clearTimeout(self.timerId);
                    self.timerId = setTimeout(function () {
                        self.$subContextMenuContainer.detach();
                    }, 1000);
                }
            }
        });
        var $elem = $('.tableRow:hover');
        if ($elem.hasClass('hasSubMenu') && !$elem.hasClass('unactiveContextMenuAction')) {
            var contextMenuItemOffset = $('.contextMenu .tableRow:hover').position();
            if (contextMenuItemOffset != undefined) {
                var $playlists = $('#playlistsTable').children('.playlist');
                self.$subContextMenuContainer.children().remove('.tableRow');
                for (var j = 0; j < $playlists.length; j++) {
                    var $playlist = $($playlists[j]);
                    if ($playlist.hasClass('default-playlist') || $playlist.hasClass('playlist-active'))
                        continue;

                    var playlist = {
                        "name": $($playlists[j]).children('.playlist-title').text(),
                        "id": $($playlists[j]).children('.playlistId').text()
                    };
                    self.$subContextMenuContainer.remove('.tableRow');
                    var object = self.$subContextMenuTemplate.tmpl(playlist);
                    object.appendTo(self.$subContextMenuContainer);
                }

                if (self.$subContextMenuContainer.children().length > 0) {
                    var x = self.$contextMenuContainer.width();
                    var y = $('.tableRow.hasSubMenu:hover').position().height;
                    if ((self.$subContextMenuContainer.width() + x + self.$contextMenuContainer.position().left) > $(window).width()) {
                        x = x - 2 * self.$subContextMenuContainer.width();
                    }
                    self.$subContextMenuContainer.css({
                        'top': y + 'px',
                        'left': x + 'px',
                        'position': 'absolute'
                    });

                    self.$subContextMenuContainer.attr('action', $('.tableRow:hover').children().attr('id'));
                    $('.tableRow.hasSubMenu:hover').append(self.$subContextMenuContainer);
                    self.$subContextMenuContainer.show();
                }
            }
        }
    });
};