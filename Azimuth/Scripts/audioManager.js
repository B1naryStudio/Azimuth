var AudioManager = function(volumeSlider, progressSlider) {
    var self = this;
    this.audio = new Audio();
    this.tracksGlobal = [];
    this.$currentTrack = null;
    this.audio.volume = 0.5;
    this.beforeMuteVolume = null;
    this.volumeSlider = volumeSlider;
    this.progressSlider = progressSlider;

    self.audio.onended = function() {
        self._nextTrack();
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
        var remaining = Math.floor(self.audio.duration - self.audio.currentTime);
        self.$currentTrack.find('.track-remaining').text('-' + Math.floor(remaining / 60) + ":" + (remaining % 60 < 10 ? "0" + remaining % 60 : remaining % 60));
    };

    this._nextTrack = function () {
        self.progressSlider.setPosition(0);
        self.$currentTrack.find('.track-duration').show();
        self.$currentTrack.find('.track-remaining').hide();

        var trackItem = $('.track-play-btn:not(.glyphicon-play)').parent().next();
        if (trackItem.hasClass('draggable-stub')) {
            trackItem = trackItem.next();
        }
        if (trackItem.length == 0) {
            trackItem = self.$currentTrack.parent().children().first();
        }
        self.$currentTrack = trackItem;

        self._setPauseImgButton(trackItem);

        self._setTrackTo('next');
    };

    this._prevTrack = function () {
        self.progressSlider.setPosition(0);
        self.$currentTrack.find('.track-duration').show();
        self.$currentTrack.find('.track-remaining').hide();

        var trackItem = $('.track-play-btn:not(.glyphicon-play)').parent().not('.draggable-stub').prev();
        if (trackItem.hasClass('draggable-stub')) {
            trackItem = trackItem.prev();
        }
        if (trackItem.length == 0) {
            trackItem = self.$currentTrack.parent().children().last();
        }
        self.$currentTrack = trackItem;

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

AudioManager.prototype.play = function() {
    var self = this;

    self.$currentTrack.find('.track-duration').hide();
    self.$currentTrack.find('.track-remaining').show();
    self.$currentTrack.append(self.progressSlider.getSlider());
    self.progressSlider.getSlider().show();
    self.audio.play();
};

AudioManager.prototype.pause = function() {
    var self = this;
    self.audio.pause();
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
        self.progressSlider.setPosition(0);
        if (self.$currentTrack != null) {
            self.$currentTrack.find('.track-duration').show();
            self.$currentTrack.find('.track-remaining').hide();
        }

        self.tracksGlobal = $(this).parent().parent().children('.track').children('.track-url');
        var url = $(this).parent().children('.track-url').text();
        if (self.audio.paused || self.audio.src != url) {
            if (self.audio.src != url) {
                self._setAttribute(url);
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
    };

    $('.track-play-btn:not(.bind-play-btn)').addClass('bind-play-btn').on('click', onPlayBtnClick);
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

    $('#progressSlider').on('OnChange', function (e, value) {
        self.audio.currentTime = value * self.audio.duration;
    });
};