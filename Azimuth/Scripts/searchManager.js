﻿var SearchManager = function(manager) {
    var self = this;
    this.offset = 0;
    this.audioManager = manager;
    this.topTracksVk = [];
    this.searchTracks = [];
    this.searching = false;
    this.$infoLoadingSpinner = $('#info-header-spinner');
    this.$vkMusicLoadingSpinner = $('#vkMusic-header-spinner');

    this._uploadNewSongs = function () {
        if ($('.btn-primary.searchBtn').text() == 'Vkontakte' && this.mcs.topPct >= 75 && !self.searching) {
            self.searching = true;
            var searchParam = $('#search').val().toLocaleLowerCase();
            self.offset += 20;
            $.ajax({
                url: 'api/search/vkontakte?searchText=' + searchParam + "&offset=" + self.offset,
                type: 'GET',
                dataType: 'json',
                success: function (tracks) {
                    self.searchTracks = $.merge(self.searchTracks, tracks);
                    self._showTracks(self.searchTracks, $('#searchTrackTemplate'));
                    self.searching = false;
                }
            });
        }
    };

    this._updatePlaylists = function() {
        $.ajax({
            url: 'api/playlists/',
            type: 'GET',
            dataType: 'json',
            success: function (playlists) {
                $('#playlistsTable').empty();
                $(playlists).each(function () {
                    $('#playlistsTable').append('<div class="playlist"><div class="playlist-title">' + this.Name + '</div><div class="playlistId">' + this.Id + '</div></div>');
                });
            }
        });
    };

    this._search = function() {
        var searchParam = $('#search').val().toLocaleLowerCase();
        $('.vkMusicList').find('.track').remove();
        if (searchParam != '') {
            self.$vkMusicLoadingSpinner.fadeIn('normal');
            $.ajax({
                url: 'api/usertracks/globalsearch?searchText=' + searchParam + "&criteria=" + $('.btn-primary').data('search').toLocaleLowerCase(),
                type: 'GET',
                dataType: 'json',
                success: function (tracks) {
                    self.searchTracks = tracks;
                    self._showTracks(self.searchTracks, $('#searchTrackTemplate'));
                }
            });
        }
    };

    this._showTracks = function (tracks, template) {
        var correctedTracks = $.extend(true, [], tracks);
        for (var i = 0; i < tracks.length; i++) {
            correctedTracks[i].Duration = self._toFormattedTime(correctedTracks[i].Duration, true);
        }
        $('.vkMusicList').find('.track').detach();
        for (var i = 0; i < correctedTracks.length; i++) {
            var tmpl = template.tmpl(correctedTracks[i]);
            tmpl.appendTo('.vkMusicList');
            if (self.audioManager.$currentTrack !== null
                && self.audioManager.$currentTrack.children('.trackId').html() == correctedTracks[i].id) {
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

        self.audioManager.refreshTracks();
        self.audioManager.bindPlayBtnListeners();
        $('.vkMusicList > .tableRow > .track-info-btn').click(self._getTrackInfo);
        $('.vkMusicTable').mCustomScrollbar({
            theme: 'dark-3',
            scrollButtons: { enable: true },
            updateOnContentResize: true,
            scrollInertia: 0,
            autoHideScrollbar: true,
            advanced: { updateOnSelectorChange: "true" },
            callbacks: { whileScrolling: self._uploadNewSongs }
        });
        self._createContextMenu();
        self.$vkMusicLoadingSpinner.hide();
    };

    this._getTrackInfo = function () {
        var $self = $(this);
        var author = $self.parent().children('.track-description').children('.track-info');
        var trackName = $self.parent().children('.track-description').children('.track-title');
        self.$infoLoadingSpinner.show();
        $.ajax({
            url: '/api/usertracks/trackinfo?artist=' + author.html() + '&trackName=' + trackName.html(),
            async: true,
            success: function(trackInfo) {
                self.$infoLoadingSpinner.hide();
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
                self.topTracks = trackInfo.ArtistTopTrackList;
                $('#listenTopBtn').attr('disabled', false);
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

    this._delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        }
    })();

    this._createContextMenu = function() {
        var ctxMenu = new ContextMenu();
        var contextMenuActions = [
            { id: 'savevktrack', name: 'Move to', isNewSection: true, hasSubMenu: true, needSelectedItems: false },
            { id: 'createplaylist', name: 'Create new playlist', isNewSection: false, hasSubMenu: false, needSelectedItems: false },
        ];

        ctxMenu.initializeContextMenu(-1, contextMenuActions, this, self);

        $('.track').off('mousedown').mousedown(function (event) {
            if (event.which == 3) {
                $('.track.draggable-item-selected').toggleClass('draggable-item-selected', false);
                var $target = $(event.target).parents('.track');
                $target.toggleClass('draggable-item-selected', true);
                ctxMenu.drawContextMenu(event);
            }
        });

        ctxMenu.$contextMenuContainer.mousedown(function (event) {
            ctxMenu.$contextMenuContainer.hide();
            ctxMenu.selectAction($('.track.selected'));
        });
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
            url: '/api/usertracks?provider=Vkontakte&index=' + index,
            type: 'POST',
            data: JSON.stringify({
                "PlaylistId": playlistId,
                "TrackInfos": tracks
            }),
            contentType: 'application/json'
        });
    };

    this._updatePlaylists();
    this._search();
};

SearchManager.prototype.bindListeners = function () {
    var self = this;

    $('#search').keyup(function () {
        self._delay(function() {
            self._search();
        }, 1000);
    });

    $('.searchBtn').click(function () {
        $('.searchBtn:not(.btn-default)').removeClass('btn-primary').addClass('btn-default');
        $(this).removeClass('btn-default').addClass('btn-primary');
        self._search();
    });

    $('#infoModal').on('hidden.bs.modal', function () {
        $('#infoModal .modal-body').text('');
        $('#listenTopBtn').attr('disabled', true);
    });

    $('#listenTopBtn').click(function () {
        $('.modal-body').text('');
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
                    success: function (data) {
                        self.topTracksVk = [];
                        $(data).each(function () {
                            this.duration = self._toFormattedTime(this.duration, true);
                            if ((typeof (this.artist) != 'undefined') && (typeof (this.title) != 'undefined')) {
                                self.topTracksVk.push(this);
                            }
                        });
                        self._showTracks(self.topTracksVk, $('#trackTemplate'));
                        self.audioManager.bindPlayBtnListeners();
                        $('.vkMusicList > .tableRow > .track-info-btn').click(self._getTrackInfo);
                        self.$vkMusicLoadingSpinner.hide();
                    }
                });
            }
        }
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
            async: false,
            success: function (playlistId) {
                self._saveTrackFromVkToPlaylist($('#itemsContainer'), -1, playlistId);
            }
        });
        self._updatePlaylists();
    });
};