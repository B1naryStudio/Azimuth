var MyMusicManager = function (manager) {
    var self = this;
    this.tracksGlobal = [];
    this.audioManager = manager;
    this.$tracksContainer = $('#tracksTable');
    this.$playlistContainer = $('#playlistsTable');
    this.$searchTrackInput = $('#searchTrackName');

    this._getTracks = function() {
        $.ajax({
            url: "/api/usertracks?playlistId=" + $(this).find('#playlistId').text(),
            type: 'GET',
            async: false,
            success: function(tracksData) {
                var tracks = tracksData;
                self.$tracksContainer.find('.tableRow').remove();
                for (var i = 0; i < tracks.length; i++) {
                    tracks[i].Duration = Math.floor(tracks[i].Duration / 60) + ":" + (tracks[i].Duration % 60 < 10 ? "0" + tracks[i].Duration % 60 : tracks[i].Duration % 60);
                }
                self.tracksGlobal = tracks;
                self.showTracks(tracks);
                self.audioManager.bindPlayBtnListeners();
            }
        });
    };
};

MyMusicManager.prototype.showTracks = function (tracks) {
    var self = this;

    $('#tracksTable').find('.track').remove();
    for (var i = 0; i < tracks.length; i++) {
        self.$tracksContainer.append($("#trackTemplate").tmpl(tracks[i]));
    }
};

MyMusicManager.prototype.showPlaylists = function () {
    var self = this;
    $.ajax({
        cache: false,
        type: "GET",
        url: "/api/playlists/own",
        dataType: "json",
        async: false,
        success: function (playlists) {
            for (var i = 0; i < playlists.length; i++) {
                var playlist = playlists[i];
                if (playlist.Accessibilty == 1)
                    playlist.Accessibilty = "public";
                else
                    playlist.Accessibilty = "private";
                self.$playlistContainer.append($("#playlistTemplate").tmpl(playlist));
            }
        }
    });
};

MyMusicManager.prototype.bindListeners = function () {
    var self = this;
    $('.accordion .tableRow').on("click", self._getTracks);

    this.$searchTrackInput.keyup(function (e) {
        var searchParam = $(this).val().toLocaleLowerCase();

        self.showTracks(self.tracksGlobal.filter(function (index) {
            self.$searchTrackInput.next().children().remove();
            return ((index.Name.toLocaleLowerCase().indexOf(searchParam) != -1) || (index.Artist.toLocaleLowerCase().indexOf(searchParam) != -1));
        }));
    });
};