var AudioManager = function(volumeSlider, progressSlider) {
    var self = this;
    this.audio = new Audio();
    this.tracksGlobal = [];
    this.$currentTrack = null;
    this.audio.volume = 0.5;
    this.beforeMuteVolume = null;
    this.volumeSlider = volumeSlider;
    this.progressSlider = progressSlider;

    this.onProgressBarClick = false;

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

    this._nextTrack = function () {
        self.progressSlider.setPosition(0);
        self.progressSlider.setBackgroundPosition(0);
        self.$currentTrack.find('.track-duration').show();
        self.$currentTrack.find('.track-remaining').hide();

        var trackItem = $('.track-play-btn:not(.glyphicon-play)').parent().next('.tableRow');
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

        self._setPauseImgButton(trackItem);

        self._setTrackTo('next');
    };

    this._prevTrack = function () {
        self.progressSlider.setPosition(0);
        self.progressSlider.setBackgroundPosition(0);
        self.$currentTrack.find('.track-duration').show();
        self.$currentTrack.find('.track-remaining').hide();

        var trackItem = $('.track-play-btn:not(.glyphicon-play)').parent().not('.draggable-stub').prev('.tableRow');
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

        self._setPauseImgButton(trackItem);

        self._setTrackTo('prev');
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
        if ($(this).children('.thirdPartId').text() == self.$currentTrack.children('.thirdPartId').text() &&
            $(this).children('.ownerId').text() == self.$currentTrack.children('.ownerId').text()) {

            $(this).append(self.progressSlider.getSlider());
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

    self.$currentTrack.find('.track-duration').hide();
    self.$currentTrack.find('.track-remaining').show();
    self.$currentTrack.append(self.progressSlider.getSlider());
    self.progressSlider.getSlider().show();
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
        if (self.$currentTrack != null) {
            self.$currentTrack.find('.track-duration').show();
            self.$currentTrack.find('.track-remaining').hide();
        }
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
                $('#playTrackBtn').css('background-position', '8px -50px');
            } else {
                self.pause();
                self._setPlayImgButton($(this).parent());
                $('#playTrackBtn').css('background-position', '8px 0');
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
                    $('#playTrackBtn').css('background-position', '8px -50px');
                } else {
                    self.pause();
                    self._setPlayImgButton($($currentTrack));
                    $('#playTrackBtn').css('background-position', '8px 0');
                }
                self._loadNextTracks($currentTrack);

            }
        });
    }

    $('.track-play-btn:not(.bind-play-btn)').addClass('bind-play-btn').on('click', onPlayBtnClick);
};

AudioManager.prototype.refreshProgressBar = function () {
    var self = this;

    $('#progressSlider').on('mousedown', function () {
        self.audio.pause();
        self.onProgressBarClick = true;
    });
};

AudioManager.prototype.bindListeners = function() {
    var self = this;

    $('#playTrackBtn').click(function() {
        if (self.audio.paused) {
            if (self.audio.src === '') {
                $($('.track:not(.draggable-stub)').children('.btn').get(0)).click();
            } else {
                self.play();
                self._setPauseImgButton(self.$currentTrack);
                $('#playTrackBtn').css('background-position', '8px -50px');
            }
        } else {
            self.pause();
            self._setPlayImgButton(self.$currentTrack);
            $('#playTrackBtn').css('background-position', '8px 0');
        }
    });

    $('#nextTrackBtn').click(self._nextTrack);
    $('#prevTrackBtn').click(self._prevTrack);

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

    $('#progressSlider').on('mousedown', function() {
        self.audio.pause();
        self.onProgressBarClick = true;
    });

    $(document).on('mouseup', function (e) {
        if (self.onProgressBarClick == true) {
            self.setCurrentTime(self.progressSlider.getPosition() * self.getDuration());
            self.audio.play();
            self.onProgressBarClick = false;
        }
    });
};