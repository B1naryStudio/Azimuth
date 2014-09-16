var AudioManager = function(volumeSlider, progressSlider) {
    var self = this;
    this.audio = new Audio();
    this.tracksGlobal = [];
    this.$currentTrack = null;
    this.audio.volume = 0.5;
    this.beforeMuteVolume = null;
    this.volumeSlider = volumeSlider;
    this.progressSlider = progressSlider;
    this.paused = true;

    this.onProgressBarClick = false;

    this._getPlaylistsForPopover = function() {
        $.ajax({
            url: 'api/playlists/',
            type: 'GET',
            dataType: 'json',
            success: function(playlists) {
                var addMenu = $('<div>');

                if (playlists.length != 0) {
                    $(playlists).each(function() {
                        addMenu.append($('#popoverPlaylistTemplate').tmpl(this));
                    });
                    
                    $('#plus-btn .fa').popover({
                        placement: 'bottom',
                        container: 'body',
                        html: true,
                        content: function () {
                            return addMenu.html();
                        }
                    });
                    var popover = $('#plus-btn .fa').data('bs.popover');
                    popover.setContent();
                    popover.$tip.addClass(popover.options.placement);

                    $('#plus-btn .fa').on('click', function() {
                        $('.popoverPlaylistBtn').on('mousedown', function() {
                            var playlistId = $(this).parent().children('.playlistId').text();
                            self._copyTrackToPlaylist(self.$currentTrack, playlistId);
                            $('#plus-btn .fa').popover('hide');

                            $(self).trigger('OnAddToPlaylist');
                        });
                    });
                } else {
                    addMenu.append('<div class="btn btn-default popoverPlaylistBtn">Add new playlist</div>');
                    
                    $('#plus-btn .fa').popover({
                        placement: 'bottom',
                        container: 'body',
                        html: true,
                        content: function () {
                            return addMenu.html();
                        }
                    });
                    var popover = $('#plus-btn .fa').data('bs.popover');
                    popover.setContent();
                    popover.$tip.addClass(popover.options.placement);

                    $('#plus-btn .fa').on('click', function() {
                        $('.popoverPlaylistBtn').on('mousedown', function() {
                            $('#createPlaylistModal').modal({
                                show: true
                            });
                            $('#createPlaylistModal').off('shown.bx.modal').on('shown.bs.modal', function() {
                                $("#playlistNameToCreate").focus();
                            });
                            $('#itemsContainer').append(self.$currentTrack.clone());
                            $('#plus-btn .fa').popover('hide');
                        });
                    });
                }

                //$('#plus-btn .fa').popover({
                //    placement: 'bottom',
                //    container: 'body',
                //    html: true,
                //    content: function() {
                //        return addMenu.html();
                //    }
                //});
                //var popover = $('#plus-btn .fa').data('bs.popover');
                //popover.setContent();
                //popover.$tip.addClass(popover.options.placement);
            }
        });
    };

    self.audio.onended = function () {
        if (self.audio.loop == false) {
            self._nextTrack();
        }
    };

    self.audio.onvolumechange = function() {
        if (self.audio.volume == 0) {
            $('#volumeImg').css('background-position', '0 -120px');
        } else if (self.audio.volume <= 0.33) {
            $('#volumeImg').css('background-position', '0 -80px');
        } else if (self.audio.volume <= 0.66) {
            $('#volumeImg').css('background-position', '0 -40px');
        } else {
            $('#volumeImg').css('background-position', '0 0');
        }
    };

    self.audio.ontimeupdate = function() {
        self.progressSlider.setPosition(self.audio.currentTime / self.audio.duration);
        self.progressSlider.setBackgroundPosition(self.audio.buffered.end(0) / self.audio.duration);
        var remaining = Math.floor(self.audio.duration - self.audio.currentTime);
        self.$currentTrack.find('.track-remaining').text('-' + Math.floor(remaining / 60) + ":" + (remaining % 60 < 10 ? "0" + remaining % 60 : remaining % 60));
    };

    this._copyTrackToPlaylist = function (currentTrack, playlistId) {
        var tracks = [];
        tracks.push({
            ThirdPartId: currentTrack.find('.thirdPartId').text(),
            OwnerId: currentTrack.find('.ownerId').text()
        });
        $.ajax({
            url: '/api/usertracks?provider=Vkontakte&index=-1',
            type: 'POST',
            data: JSON.stringify({
                "PlaylistId": playlistId,
                "TrackInfos": tracks
            }),
            contentType: 'application/json'
        });
    };

    this._nextTrack = function () {
        self.progressSlider.setPosition(0);
        self.progressSlider.setBackgroundPosition(0);
        //self.$currentTrack.find('.track-duration').show();
        //self.$currentTrack.find('.track-remaining').hide();

        var trackItem = self.$currentTrack.next();
        if (trackItem.hasClass('draggable-stub')) {
            trackItem = trackItem.next('.tableRow');
        }
        if (trackItem.length == 0) {
            trackItem = self.$currentTrack.parent().children('.tableRow').first();
        }
        self.$currentTrack = trackItem;

        var url = self.$currentTrack.find('.track-url').text();
        if (!url) {
            self._getCurrentTrackUrl(self.$currentTrack, url);
        } else {
            self._loadNextTracks(self.$currentTrack);
        }

        if (self.audio.paused && self.audio.duration != self.audio.currentTime) {
            self._setPlayImgButton(trackItem);
        } else {
            self._setPauseImgButton(trackItem);
        }

        self._setTrackTo('next');

        $('#audio-player .track-description .track-artist').text(self.$currentTrack.find('.track-artist').text());
        $('#audio-player .track-description .track-title').text(self.$currentTrack.find('.track-title').text());
    };

    this._prevTrack = function () {
        self.progressSlider.setPosition(0);
        self.progressSlider.setBackgroundPosition(0);
        //self.$currentTrack.find('.track-duration').show();
        //self.$currentTrack.find('.track-remaining').hide();

        var trackItem = self.$currentTrack.prev();
        if (trackItem.hasClass('draggable-stub')) {
            trackItem = trackItem.prev('.tableRow');
        }
        if (trackItem.length == 0) {
            trackItem = self.$currentTrack.parent().children('.tableRow').last();
        }
        self.$currentTrack = trackItem;
        var url = self.$currentTrack.find('.track-url').text();
        if (!url) {
            self._getCurrentTrackUrl(self.$currentTrack, url);
        }

        if (self.audio.paused && self.audio.duration != self.audio.currentTime) {
            self._setPlayImgButton(trackItem);
        } else {
            self._setPauseImgButton(trackItem);
        }

        self._setTrackTo('prev');

        $('#audio-player .track-description .track-artist').text(self.$currentTrack.find('.track-artist').text());
        $('#audio-player .track-description .track-title').text(self.$currentTrack.find('.track-title').text());
    };

    this._setPlayImgButton = function(t) {
        t.children('.track-play-btn').removeClass('glyphicon-pause');
        t.children('.track-play-btn').addClass('glyphicon-play');
    };

    this._setPauseImgButton = function(t) {
        $('.track-play-btn').removeClass('glyphicon-pause');
        $('.track-play-btn:not(.glyphicon-play)').addClass('glyphicon-play');
        t.children('.track-play-btn').removeClass('glyphicon-play');
        t.children('.track-play-btn').addClass('glyphicon-pause');
    };

    this._setAttribute = function(src) {
        self.audio.setAttribute('src', src);
    };

    this._setTrackTo = function (dirrection) { //dirrection = 'next' or 'prev'
        if (dirrection === 'next' || dirrection === 'prev') {
            var url = null;

            $(self.tracksGlobal).each(function(index) {
                if ($(this).text() === self.audio.src) {
                    if (dirrection === 'next') {
                        url = $($(self.tracksGlobal).get(index + 1)).text();
                        if (url === '') {
                            url = $($(self.tracksGlobal).get(0)).text();
                        }
                    } else {
                        url = $($(self.tracksGlobal).get(index - 1)).text();
                    }
                    if (!self.audio.paused || self.audio.currentTime === self.audio.duration) {
                        self._setAttribute(url);
                        self.play();
                    } else {
                        self._setAttribute(url);
                    }
                    return false;
                }
                return true;
            });
        }
    };

    self._getPlaylistsForPopover();
};

AudioManager.prototype.refreshTracks = function() {
    var self = this;
    self.tracksGlobal = $('.vkMusicList').children('.track').children('.track-url');
};

AudioManager.prototype.refreshPlaylistTracks = function() {
    var self = this;
    self.tracksGlobal = $('#playlistTracks').children('.track').children('.track-url');
};

AudioManager.prototype.updateProgressbar = function (musicSelector) {
    var self = this;
    $(musicSelector).children('.track').each(function() {
        if (self.$currentTrack != null && $(this).children('.thirdPartId').text() == self.$currentTrack.children('.thirdPartId').text() &&
            $(this).children('.ownerId').text() == self.$currentTrack.children('.ownerId').text()) {

            //$(this).append(self.progressSlider.getSlider());
            self.$currentTrack = $(this);
            if (self.audio.paused == false) {
                $(this).children('.track-play-btn').removeClass('glyphicon-play').addClass('glyphicon-pause');
            }
        }
    });
}

AudioManager.prototype.loop = function (value) {
    var self = this;
    self.audio.loop = value;
};

AudioManager.prototype.play = function() {
    var self = this;

    $('#audio-player .track-description .track-artist').text(self.$currentTrack.find('.track-artist').text());
    $('#audio-player .track-description .track-title').text(self.$currentTrack.find('.track-title').text());
    //self.$currentTrack.find('.track-duration').hide();
    //self.$currentTrack.find('.track-remaining').show();
    //self.$currentTrack.append(self.progressSlider.getSlider());
    //self.progressSlider.getSlider().show();
    self.progressSlider.bindListeners();
    self.audio.play();
    self.refreshProgressBar();
};

AudioManager.prototype.pause = function() {
    var self = this;
    self.audio.pause();
};

AudioManager.prototype.stop = function() {
    var self = this;
    self.audio.pause();
    self.audio.currentTime = 0;
};

AudioManager.prototype.setVolume = function(value) {
    var self = this;
    self.audio.volume = value;
};

AudioManager.prototype.setCurrentTime = function (value) {
    var self = this;
    self.audio.currentTime = value;
};

AudioManager.prototype.getDuration = function () {
    var self = this;
    return self.audio.duration;
};

AudioManager.prototype.bindPlayBtnListeners = function() {
    var self = this;

    function onPlayBtnClick() {
        //if (self.$currentTrack != null) {
        //    self.$currentTrack.find('.track-duration').show();
        //    self.$currentTrack.find('.track-remaining').hide();
        //}
        var $currentTrack = $(this).parents('.track');
        var url = $(this).parent().children('.track-url').text();
        if (!url) {
            self._getCurrentTrackUrl($currentTrack, url);
        } else {
            self.tracksGlobal = $(this).parent().parent().children('.track').children('.track-url');

            if (self.audio.paused || self.audio.src != url) {

                if (!$(this).parent().hasClass('vk-item') && !self.audio.paused) {
                    self._loadNextTracks($currentTrack);
                }

                if (self.audio.src != url) {
                    self._setAttribute(url);
                    self.progressSlider.setPosition(0);
                    self.progressSlider.setBackgroundPosition(0);
                }
                self.$currentTrack = $(this).parent();
                self.play();
                self._setPauseImgButton($(this).parent());
                $('#play-button').children('.fa-play').addClass('hide');
                $('#play-button').children('.fa-pause').removeClass('hide');
            } else {
                self.pause();
                self._setPlayImgButton($(this).parent());
                $('#play-button').children('.fa-play').removeClass('hide');
                $('#play-button').children('.fa-pause').addClass('hide');
            }
        }
    };

    this._loadNextTracks = function ($currentTrack) {
        var trackInfo = [];
        var curIndex = $currentTrack.index('.track');
        var howMuchTracksLeft = ($currentTrack.parent().children('.track').length - curIndex - 1);
        //var howMuchTracksLeft = ($currentTrack.parent().children('.track').length - curIndex - 2);
            if (howMuchTracksLeft == 1) {
                trackInfo = {
                    trackData: [
                        { ownerId: $($currentTrack.parent().children('.track')[curIndex + 1]).find('.ownerId').text(), thirdPartId: $($currentTrack.parent().children('.track')[curIndex + 1]).find('.thirdPartId').text() },
                        { ownerId: $($currentTrack.parent().children('.track')[0]).find('.ownerId').text(), thirdPartId: $($currentTrack.parent().children('.track')[0]).find('.thirdPartId').text() }
                    ]
                };
            } else if(howMuchTracksLeft >= 2) {
                trackInfo = {
                    trackData: [
                        { ownerId: $($currentTrack.parent().children('.track')[curIndex + 1]).find('.ownerId').text(), thirdPartId: $($currentTrack.parent().children('.track')[curIndex + 1]).find('.thirdPartId').text() },
                        { ownerId: $($currentTrack.parent().children('.track')[curIndex + 2]).find('.ownerId').text(), thirdPartId: $($currentTrack.parent().children('.track')[curIndex + 2]).find('.thirdPartId').text() }
                    ]
                };
            } else {
                trackInfo = {
                    trackData: [
                        { ownerId: $($currentTrack.parent().children('.track')[0]).find('.ownerId').text(), thirdPartId: $($currentTrack.parent().children('.track')[0]).find('.thirdPartId').text() },
                        { ownerId: $($currentTrack.parent().children('.track')[1]).find('.ownerId').text(), thirdPartId: $($currentTrack.parent().children('.track')[1]).find('.thirdPartId').text() }
                    ]
                };
            }
            $.ajax({
                url: '/api/usertracks/trackurl?provider=Vkontakte',
                type: 'GET',
                dataType: 'json',
                data: { trackData: JSON.stringify(trackInfo) },
                success: function (tracks) {
                    for (var i = 0; i < tracks.length; i++) {
                        var tempUrl = tracks[i];
                        var index = curIndex + i + 1;
                        var listTracksCount = $currentTrack.parent().children('.track').length;
                        var diff = index - listTracksCount;
                        if (diff >= 0) {
                            index = diff;
                        }
                        $($currentTrack.parent().children('.track')[index]).find('.track-url').text(tempUrl);
                    }
                }
            });
    };

    this._getCurrentTrackUrl = function ($currentTrack, url) {
        var trackInfo = {
            trackData: [
                { ownerId: $currentTrack.find('.ownerId').text(), thirdPartId: $currentTrack.find('.thirdPartId').text() }
            ]
        };
        $.ajax({
            url: '/api/usertracks/trackurl?provider=Vkontakte',
            type: 'GET',
            dataType: 'json',
            data: { trackData: JSON.stringify(trackInfo) },
            success: function (track) {
                url = track[0];
                $currentTrack.find('.track-url').text(url);
                self.tracksGlobal = $currentTrack.parent().find('.track-url');
                if (self.audio.paused || self.audio.src != url) {
                    if (self.audio.src != url) {
                        self._setAttribute(url);
                        self.progressSlider.setPosition(0);
                        self.progressSlider.setBackgroundPosition(0);
                    }
                    self.$currentTrack = $currentTrack;
                    self.play();
                    self._setPauseImgButton($($currentTrack));
                    $('#play-button').children('.fa-play').addClass('hide');
                    $('#play-button').children('.fa-pause').removeClass('hide');
                } else {
                    self.pause();
                    self._setPlayImgButton($($currentTrack));
                    $('#play-button').children('.fa-play').removeClass('hide');
                    $('#play-button').children('.fa-pause').addClass('hide');
                }
                self._loadNextTracks($currentTrack);

            }
        });
    }

    $('.track-play-btn:not(.bind-play-btn)').addClass('bind-play-btn').on('click', onPlayBtnClick);
};

AudioManager.prototype.refreshProgressBar = function () {
    var self = this;

    $('#progress-bar').on('mousedown', function () {
        self.audio.pause();
        self.onProgressBarClick = true;
    });
};

AudioManager.prototype.bindListeners = function() {
    var self = this;

    $('#play-button').click(function() {
        if (self.audio.paused) {
            if (self.audio.src === '') {
                $($('.track:not(.draggable-stub)').children('.btn').get(0)).click();
            } else {
                self.play();
                self._setPauseImgButton(self.$currentTrack);
                $('#play-button').children('.fa-play').addClass('hide');
                $('#play-button').children('.fa-pause').removeClass('hide');
            }
        } else {
            self.pause();
            self._setPlayImgButton(self.$currentTrack);
            $('#play-button').children('.fa-play').removeClass('hide');
            $('#play-button').children('.fa-pause').addClass('hide');
        }
    });

    $('#next-button').click(self._nextTrack);
    $('#prev-button').click(self._prevTrack);

    $('#volumeImg').click(function () {
        if (self.audio.volume == 0) {
            self.audio.volume = self.beforeMuteVolume;
            $('#volumeImg').css('background-position', '0 0');
            self.volumeSlider.setPosition(self.beforeMuteVolume);
            //$('#volume').css('height', self.beforeMuteVolume*100 + '%');
        } else {
            self.beforeMuteVolume = self.audio.volume;
            self.audio.volume = 0;
            //$('#volume').css('height', 0);
            self.volumeSlider.setPosition(0);
        }
    });

    $('.itemsContainer').on('AfterDropped', function() {
        self.tracksGlobal = $('.draggable-stub').parent().children('.track').children('.track-url');
    });

    $('#volumeSlider').on('OnChange', function (e, value) {
        self.setVolume(value);
    });

    //$('#progressSlider').on('OnChange', function (e, value) {
    //    self.audio.currentTime = value * self.audio.duration;
    //});

    $('#progress-bar').on('mousedown', function () {
        self.paused = self.audio.paused;
        self.audio.pause();
        self.onProgressBarClick = true;
    });

    $(document).on('mouseup', function () {
        if (self.onProgressBarClick == true) {
            self.setCurrentTime(self.progressSlider.getPosition() * self.getDuration());
            if (!self.paused) {
                self.audio.play();
            }
            self.onProgressBarClick = false;
        }
    });

    $('#createPlaylistModal').on('OnPlaylistCreate', function () {
        //$('#plus-btn .fa').popover('hide');
        //$('.popover').remove();
        self._getPlaylistsForPopover();
    });

    $('#repeat-btn').on('mousedown', function () {
        $('#repeat-btn .fa').css('color', !self.audio.loop? 'orange' : '');
        self.loop(!self.audio.loop);
    });
};