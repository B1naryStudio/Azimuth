var AudioManager = function() {
    var self = this;
    this.audio = new Audio();
    this.tracksGlobal = [];
    this.$currentTrack = null;

    //var volume = new Dragdealer('volume', {
    //    vertical: true,
    //    horizontal: false,
    //    bottom: 4,
    //    animationCallback: function (x, y) {
    //        var t = Math.round(y * 100);
    //        if (y >= 0 && y <= 1) {
    //            self.audio.volume = 1 - y;
    //        }
    //        if (t <= 33) {
    //            $('#volumeImg').css('background-position', '0 0');
    //        } else if (t <= 66) {
    //            $('#volumeImg').css('background-position', '0 -40px');
    //        } else if (t === 100) {
    //            $('#volumeImg').css('background-position', '0 -120px');
    //        } else {
    //            $('#volumeImg').css('background-position', '0 -79px');
    //        }
    //    }
    //});
    //volume.setValue(0, 0.5);

    self.audio.onended = function() {
        self._nextTrack();
    };

    this._nextTrack = function() {
        $('.track-play-btn').removeClass('glyphicon-pause');
        var trackItem = $('.track-play-btn:not(.glyphicon-play)').parent().next();
        if (trackItem.hasClass('draggable-stub')) {
            trackItem = trackItem.next();
        }
        if (trackItem.length == 0) {
            trackItem = self.$currentTrack.parent().children().first();
        }
        self.$currentTrack = trackItem;
        $('.track-play-btn:not(.glyphicon-play)').addClass('glyphicon-play');
        trackItem.children('.glyphicon').removeClass('glyphicon-play');
        trackItem.children('.glyphicon').addClass('glyphicon-pause');

        $(self.tracksGlobal).each(function(index) {
            if ($(this).text() === self.audio.src) {
                var url = $($(self.tracksGlobal).get(index + 1)).text();
                if (url === '') {
                    url = $($(self.tracksGlobal).get(0)).text();
                }
                if (!self.audio.paused) {
                    self._setAttribute(url);
                    self.play();
                } else {
                    self._setAttribute(url);
                }
                return false;
            }
            return true;
        });
    };

    this._prevTrack = function() {
        $('.track-play-btn').removeClass('glyphicon-pause');
        var trackItem = $('.track-play-btn:not(.glyphicon-play)').parent().not('.draggable-stub').prev();
        if (trackItem.hasClass('draggable-stub')) {
            trackItem = trackItem.prev();
        }
        if (trackItem.length == 0) {
            trackItem = self.$currentTrack.parent().children().last();
        }
        self.$currentTrack = trackItem;
        $('.track-play-btn:not(.glyphicon-play)').addClass('glyphicon-play');
        trackItem.children('.glyphicon').removeClass('glyphicon-play');
        trackItem.children('.glyphicon').addClass('glyphicon-pause');

        $(self.tracksGlobal).each(function(index) {
            if ($(this).text() === self.audio.src) {
                var url = $($(self.tracksGlobal).get(index - 1)).text();
                if (!self.audio.paused) {
                    self._setAttribute(url);
                    self.play();
                } else {
                    self._setAttribute(url);
                }
                return false;
            }
            return true;
        });
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

    this._updateGlobalTracks = function() {
        self.tracksGlobal = $('.draggable-stub').parent().children('.track').children('.track-url');
    };
};

AudioManager.prototype.play = function() {
    var self = this;
    self.audio.play();
};

AudioManager.prototype.pause = function() {
    var self = this;
    self.audio.pause();
};

AudioManager.prototype.bindPlayBtnListeners = function() {
    var self = this;

    function onPlayBtnClick() {
        self.tracksGlobal = $(this).parent().parent().children('.track').children('.track-url');
        var url = $(this).parent().children('.track-url').text();
        if (self.audio.paused || self.audio.src != url) {
            if (self.audio.src != url) {
                self._setAttribute(url);
            }
            self.$currentTrack = $(this).parent();
            self.play();
            self._setPauseImgButton($(this).parent());
        } else {
            self.pause();
            self._setPlayImgButton($(this).parent());
        }
    };

    $('.track-play-btn:not(.bind-play-btn)').addClass('bind-play-btn').on('click', onPlayBtnClick);
};

AudioManager.prototype.bindListeners = function() {
    var self = this;

    $('#playTrackBtn').click(function() {
        if (self.audio.paused) {
            self.play();
            self._setPauseImgButton(self.$currentTrack);
        } else {
            self.pause();
            self._setPlayImgButton(self.$currentTrack);
        }
    });

    $('#nextTrackBtn').click(self._nextTrack);
    $('#prevTrackBtn').click(self._prevTrack);

    $('.itemsContainer').on('AfterDropped', self._updateGlobalTracks);
};