var PublicPlaylistManager = function (manager) {
    var self = this;
    self.audioManager = manager;
    self.playlists_global = null;
    self.$playlistTemplate = $("#playlistTemplate");
    self.$trackTemplate = $("#trackTemplate");
    self.$tracks = $('#tracks');
    self.$playlists = $('#playlists-plated');
    self.$playlistsArea = $('#playlistsArea');
    self.$tracksArea = $('#tracksArea');
    self.$playlistName = $('#playlistName');
    self.$listeners = $('#listeners');
    self.$backToPlaylistsBtn = $('#backToPlaylistsBtn');
    self.currentPlaylist = null;
    self.$likeBtn = $('#likeBtn');
    self.$likesCounter = $('.likesCounter');

    self.addCurrentUserAsListener = function (playlist) {
        $.ajax({
            cache: false,
            type: "POST",
            url: "/api/listeners/" + playlist.Id,
            dataType: "json",
            async: false,
            success: function (data) {
                
            }
        });
    };

    self.removeCurrentUserAsListener = function () {
        console.log('remove');
        console.log(self.currentPlaylist);
        var playlist = self.currentPlaylist;
        $.ajax({
            cache: false,
            type: "delete",
            url: "/api/listeners/" + playlist.Id,
            dataType: "json",
            success: function (data) {
                //var playlist = self.currentPlaylist;
                console.log('remove');
                console.log(playlist);
                self.$listeners.text(self.getPlaylistListeners(playlist));
            }
        });
    };

    self.getPlaylistListeners = function (playlist) {
        if (playlist == null)
            playlist = self.currentPlaylist;
        self.$listeners.empty();
        var res = -1;
        $.ajax({
            cache: false,
            type: "GET",
            url: "/api/listeners/" + playlist.Id,
            dataType: "json",
            async: false,
            success: function (data) {
                res = data;
            }
        });
        return res;
    };

    self.showLikes = function() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/api/likes/" + self.currentPlaylist.Id,
            dataType: "json",
            success: function (data) {
                console.log('Likes shown');
                self.$likesCounter.text(data);
            }
        });
    };

    self.showTracks = function (playlist) {
        var self = this;
        self.$playlistName.text(playlist.Name);
        self.$tracks.empty();
        $.ajax({
            url: "/api/usertracks?playlistId=" + playlist.Id, // TODO replace with class playlistID
            type: 'GET',            
            success: function(tracksData) {
                var tracks = tracksData;
                for (var i = 0; i < tracks.length; ++i) {
                    var track = tracks[i];
                    var mins = Math.floor(track.Duration / 60);
                    var secs = ('0' + (track.Duration % 60)).slice(-2);
                    track.Duration = mins + ':' + secs;
                    self.$tracks.append($('#playlistTrackTemplate').tmpl(track));
                }
            }
        });
        self.audioManager.bindPlayBtnListeners();
        $(this).toggleClass("active");
    }

};


PublicPlaylistManager.prototype.bindListeners = function () {
    var self = this;
    self.$likeBtn.find('.icon').toggleClass("fa-thumbs-o-up");
    console.log('likBTN');
    console.log(self.$likeBtn);
    self.$backToPlaylistsBtn.click(function () {
        self.removeCurrentUserAsListener();
        
        self.$playlistsArea.show();
        self.$tracksArea.hide();
        self.$backToPlaylistsBtn.hide();
        self.currentPlaylist = null;
    });
    self.$playlists.click(function (event) {
        
        self.$playlistsArea.hide();
        self.$tracksArea.show();
        self.$backToPlaylistsBtn.show();
        
        var $playlist = $(event.target).closest('.playlist-plated');

        var playlistName = $playlist.find('.playlist-plated-info-name').text();
        self.currentPlaylist = self.playlists_global.filter(function (index) {
            return index.Name.valueOf() == playlistName;
        })[0];
        
        self.showTracks(self.currentPlaylist);
        self.showLikes();
        $(document).trigger('newListenerAdded', [self.currentPlaylist]);
    });
    
    self.$likeBtn.click(function () {
        console.log('I\' here');
        $.ajax({
            cache: false,
            type: "POST",
            url: "/api/likes/" + self.currentPlaylist.Id,
            dataType: "json",
            success: function (data) {
                $(document).trigger('newLikeAdded');
            }
        });
    });
    $(document).on('newLikeAdded',function() {
        self.showLikes();
    });
    $(document).on('newListenerAdded', function (event, playlist) {
        self.addCurrentUserAsListener(playlist);
        self.$listeners.text('This playlist listening ' + self.getPlaylistListeners(playlist) + ' people');
    });
};

PublicPlaylistManager.prototype.showPlaylists = function () {
    var self = this;
    self.$playlists.empty();
    $.ajax({
        cache: false,
        type: "GET",
        url: "/api/playlists/public",
        dataType: "json",
        success: function(data) {
            var playlists = data;
            self.playlists_global = playlists;
            for (var i = 0; i < playlists.length; i++) {
                var playlist = playlists[i];
                var mins = Math.floor(playlist.Duration / 60);
                var secs = ('0' + (playlist.Duration % 60)).slice(-2);
                playlist.Duration = mins + ':' + secs;
                playlist.Creator = playlist.Creator.Name;
                var $playlist = $('#playlistTemplate').tmpl(playlist);
                self.$playlists.append($playlist);
                var $listener = $playlist.find('.listeners');
                var returnVal = self.getPlaylistListeners(playlist);
                $listener.text('Listening ' + returnVal + ' people');
            }
        }
    });
};