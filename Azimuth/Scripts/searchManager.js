var SearchManager = function(manager) {
    var self = this;
    this.audioManager = manager;

    this._search = function() {
        var searchParam = $('#search').val().toLocaleLowerCase();
        $.ajax({
            url: 'api/usertracks/globalsearch?searchText=' + searchParam + "&criteria=" + $('.btn-primary').data('search').toLocaleLowerCase(),
            type: 'GET',
            dataType: 'json',
            success: function (tracks) {
                $('.vkMusicList').find('.track').remove();
                self._showTracks(tracks, $('#searchTrackTemplate'));
                self.audioManager.refreshTracks();
                self.audioManager.bindPlayBtnListeners();
            }
        });
    };

    this._showTracks = function(tracks, template) {
        $('.vkMusicList').find('.track').detach();
        for (var i = 0; i < tracks.length; i++) {
            var tmpl = template.tmpl(tracks[i]);
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
    };

    this._search();
};

SearchManager.prototype.bindListeners = function () {
    var self = this;

    $('#search').keyup(function () {
        self._search();
    });

    $('.searchBtn').click(function () {
        $('.searchBtn:not(.btn-default)').removeClass('btn-primary').addClass('btn-default');
        $(this).removeClass('btn-default').addClass('btn-primary');
        self._search();
    });
};