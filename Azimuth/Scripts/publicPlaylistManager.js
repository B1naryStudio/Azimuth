var PublicPlaylistManager = function(manager) {
    var self = this;
    this.audioManager = manager;
    this.$playlistSpinner = $('#playlist-spinner');
    this.$trackInfoSpinner = $('#trackInfo-spinner');
    this.$playlistArea = $('#playlistsArea');
    this.$trackArea = $('#tracksArea');
    this.$backToPlaylistsBtn = $('button#backToPlaylistsBtn');

    this._setNewImage = function ($playlist, id) {
        $.ajax({
            url: '/api/playlists/image/' + id,
            success: function (image) {
                var $logo = $playlist;
                $logo.fadeOut(200, function () {
                    if (image != "") {
                        $logo.css({
                            "background-image": 'url(' + image + ')'
                        });
                    } else {
                        $logo.css({
                            "background-image": 'url(http://cdns2.freepik.com/free-photo/music-album_318-1832.jpg)'
                        });
                    }
                });
                $logo.fadeIn(200);
            }
        });
    };
    $('.playlist-plated').each(function () {
        self._setNewImage($(this), $(this).find('.playlistId').val());
    });
};

PublicPlaylistManager.prototype.showPlaylists = function() {
    var self = this;
    self.$trackArea.hide();

    //$('.playlist-plated').click(function () {
    //    $('#playlistsArea').hide();
    //    $('#playlist-spinner').show();
    //    var $currentPlaylist = $(this);
    //    var playlistId = $currentPlaylist.find('.playlistId').val();
    //    $.ajax({
    //        url: '@Url.Action("_PlaylistTracks", "PublicPlaylists")?playlistId=' + playlistId +
    //            "&playlistName=" + $currentPlaylist.find('.playlistName').text() +
    //            "&isLiked=" + $currentPlaylist.find('.isLiked').val() +
    //            "&isFavourited=" + $currentPlaylist.find('.isFavourited').val(),
    //        type: 'GET',
    //        success: function (data) {
    //            $(data).appendTo($('#tracksArea'));
    //            $.ajax({
    //                url: 'api/playlists/raiselistened?id=' + playlistId,
    //                type: 'POST'
    //            });
    //        }
    //    });
    //});

};

PublicPlaylistManager.prototype.showTracks = function() {
    var self = this;
    self.$trackArea.show();
    self.$backToPlaylistsBtn.off('click').click(function () {
        $('#tracksArea').empty();
        $('#playlistsArea').show();
    });

    $('.track-info-btn').click(function () {
        var $self = $(this);
        var author = $self.parent().children('.track-description').children('.track-info');
        var trackName = $self.parent().children('.track-description').children('.track-title');
        //self.$infoLoadingSpinner.show();
        $.ajax({
            url: '/api/usertracks/trackinfo?artist=' + author.text() + '&trackName=' + trackName.text(),
            async: true,
            success: function (trackInfo) {
                //self.$infoLoadingSpinner.hide();
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
    });
};































//var PublicPlaylistManager = function (manager) {
//    var self = this;
//    self.audioManager = manager;
//    self.playlists_global = null;
//    //self.$playlistTemplate = $("#playlistTemplate");
//    self.$trackTemplate = $("#trackTemplate");
//    self.$tracks = $('#tracks');
//    self.$playlists = $('#playlists-plated');
//    self.$playlistsArea = $('#playlistsArea');
//    self.$tracksArea = $('#tracksArea');
//    self.$playlistName = $('#playlistName');
//    self.$listeners = $('#listeners');
//    self.$backToPlaylistsBtn = $('#backToPlaylistsBtn');
//    self.currentPlaylistId = 0;
//    self.$likeBtn = $('#likeBtn');
//    self.$favoriteBtn = $('#favoriteBtn');
//    self.$likesCounter = $('.likesCounter');
//    self.currentLikeStatus = null;
//    self.currentFavoriteStatus = null;

//    this._setNewImage = function ($playlist, id) {
//        $.ajax({
//            url: '/api/playlists/image/' + id,
//            success: function (image) {
//                var $logo = $playlist;
//                $logo.fadeOut(200, function() {
//                    if (image != "") {
//                        $logo.css({
//                            "background-image":'url('+image+')'});
//                    } else {
//                        $logo.css({
//                            "background-image": 'url(http://cdns2.freepik.com/free-photo/music-album_318-1832.jpg)'
//                        });
//                    }
//                });
//                $logo.fadeIn(200);
//            }
//        });
//    };

//    self.addCurrentUserAsListener = function () {
//        $.ajax({
//            cache: false,
//            type: "POST",
//            url: "/api/listened/" + self.currentPlaylistId,
//            dataType: "json",
//            async: false,
//            success: function (data) {
                
//            }
//        });
//    };

//    //self.removeCurrentUserAsListener = function () {
//    //    console.log('remove');
//    //    console.log(self.currentPlaylist);
//    //    var playlist = self.currentPlaylist;
//    //    $.ajax({
//    //        cache: false,
//    //        type: "delete",
//    //        url: "/api/listeners/" + playlist.Id,
//    //        dataType: "json",
//    //        success: function (data) {
//    //            //var playlist = self.currentPlaylist;
//    //            console.log('remove');
//    //            console.log(playlist);
//    //            self.$listeners.text(self.getPlaylistListeners(playlist));
//    //        }
//    //    });
//    //};

//    self.getPlaylistListeners = function () {
//        self.$listeners.empty();
//        var res = -1;
//        $.ajax({
//            cache: false,
//            type: "GET",
//            url: "/api/listened/" + self.currentPlaylistId,
//            dataType: "json",
//            async: false,
//            success: function (data) {
//                res = data;
//            }
//        });
//        return res;
//    };

//    self.showLikes = function() {
//        $.ajax({
//            cache: false,
//            type: "GET",
//            url: "/api/likes/" + self.currentPlaylistId,
//            dataType: "json",
//            success: function (data) {
//                self.$likesCounter.text(data);
//            }
//        });
//    };

//    self.showTracks = function (playlist) {
//        var self = this;
//        self.$playlistName.text(playlist.find('.playlistName').text());
//        self.$tracks.empty();
//        $.ajax({
//            url: "/api/usertracks?playlistId=" + playlist.find('.playlistId').text(), // TODO replace with class playlistID
//            type: 'GET',            
//            success: function(tracksData) {
//                var tracks = tracksData;
//                for (var i = 0; i < tracks.length; ++i) {
//                    var track = tracks[i];
//                    var mins = Math.floor(track.Duration / 60);
//                    var secs = ('0' + (track.Duration % 60)).slice(-2);
//                    track.Duration = mins + ':' + secs;
//                    self.$tracks.append($('#playlistTrackTemplate').tmpl(track));
//                }
//                $('#playlistId').val(playlist.find('.playlistId').text());
//                //$('#playlist-info').append('<input type="hidden" name="playlistId" value="' + playlist.find('.playlistId').text() + '"/>');
//            }
//        });
//        self.audioManager.bindPlayBtnListeners();
//        $(this).toggleClass("active");
//    }

//    self.setLikeBtnIcon = function () {
//        self.$likeBtn.find('.icon').removeClass('fa-thumbs-o-up').removeClass('fa-thumbs-up');
//        if (!self.currentLikeStatus) {
//            self.$likeBtn.find('.icon').toggleClass('fa-thumbs-up');
//            self.$likeBtn.css({
//                'background-color': 'rgb(100,120,255)'
//            });
//        } else {
//            self.$likeBtn.find('.icon').toggleClass('fa-thumbs-o-up');
//            self.$likeBtn.css({
//                'background-color': 'white'
//            });
//        }
//    }

//    self.setFavoriteBtnIcon = function () {
//        if (!self.currentFavoriteStatus) {
//            self.$favoriteBtn.css({
//                'background-color': 'rgb(100,120,255)',
//                'color' : 'white'
//            });
//        } else {
//            self.$favoriteBtn.css({
//                'background-color': 'white',
//                'color' : 'black'
//            });
//        }
//    }

//    self.isLiked = function () {
//        $.ajax({
//            url: "/api/likes/status/" + self.currentPlaylistId,
//            type: 'GET',
//            async: false,
//            success: function (data) {
//                self.currentLikeStatus = data^true;
//            }
//        });
//        return self.currentLikeStatus;
//    }

//    self.isFavorite = function () {
//        $.ajax({
//            url: "/api/favorite/status/" + self.currentPlaylistId,
//            type: 'GET',
//            async: false,
//            success: function (data) {
//                self.currentFavoriteStatus = data ^ true;
//            }
//        });
//        return self.currentFavoriteStatus;
//    }

//};


//PublicPlaylistManager.prototype.bindListeners = function () {
//    var self = this;
//    //self.currentLikeStatus = 0;
//    //self.$likeBtn.find('.icon').toggleClass("fa-thumbs-o-up");
//    console.log('likBTN');
//    console.log(self.$likeBtn);
//    self.$backToPlaylistsBtn.click(function () {
//        //self.removeCurrentUserAsListener(); was needed when it worked with listeners
        
//        self.$playlistsArea.show();
//        self.showPlaylists();
//        self.$tracksArea.hide();
//        self.$backToPlaylistsBtn.hide();
//    });
//    self.$playlists.click(function (event) {
        
//        self.$playlistsArea.hide();
//        self.$tracksArea.show();
//        self.$backToPlaylistsBtn.show();
//        var $playlist = $(event.target).closest('.playlist-plated');
//        self.showTracks($playlist);
//        self.showLikes();
//        $(document).trigger('newListenerAdded');
//        self.currentPlaylistId = $playlist.find('.playlistId').text();
//        self.isLiked();
//        self.setLikeBtnIcon();
//        self.isFavorite();
//        self.setFavoriteBtnIcon();
//    });
    
//    //self.$likeBtn.click(function () {
//    function likeBtnClick(){
//        var action = null;
//        if (self.isLiked())
//            action = "POST";
//        else 
//            action = "DELETE";
            
//        $.ajax({
//            cache: false,
//            type: action,
//            url: "/api/likes/" + self.currentPlaylistId,
//            dataType: "json",
//            async:false,
//            success: function (data) {
//                $(document).trigger('likeStatusChanged');
//            }
//        });
//        self.isLiked();//TODO donno why, but I doing wasted request to server
//        self.setLikeBtnIcon();
        
//    };
//    //self.$favoriteBtn.click(function () {
//    function favouriteBtnClick(){
//        var action = null;
//        if (self.isFavorite())
//            action = "POST";
//        else
//            action = "DELETE";
//        $.ajax({
//            cache: false,
//            type: action,
//            url: "/api/favorite/" + self.currentPlaylistId,
//            dataType: "json",
//            async: false,
//            success: function (data) {
//            }
//        });
//        self.isFavorite();//TODO donno why, but I doing wasted request to server
//        self.setFavoriteBtnIcon();

//    };
//    $(document).on('likeStatusChanged', function () {
//        self.showLikes();
//    });
//    $(document).on('newListenerAdded', function (event) {
//        self.addCurrentUserAsListener();
//        self.$listeners.text('This playlist listened ' + self.getPlaylistListeners() + ' people');
//    });
//};

//PublicPlaylistManager.prototype.showPlaylists = function () {
//    var self = this;
//    self.$playlists.empty();
//    $.ajax({
//        cache: false,
//        type: "GET",
//        url: "/api/playlists/public",
//        async:false,
//        dataType: "json",
//        success: function (data) {
//            var playlists = data;
//            self.playlists_global = playlists;
//            for (var i = 0; i < playlists.length; i++) {
                
//                var playlist = playlists[i];
//                var mins = Math.floor(playlist.Duration / 60);
//                var secs = ('0' + (playlist.Duration % 60)).slice(-2);
//                playlist.Duration = mins + ':' + secs;
//                playlist.Creator = playlist.Creator.Name;
//                playlist.CreatorId = playlist.Creator.Id;
//                //var $playlist = $('#playlistTemplate').tmpl(playlist);
//                //self.$playlists.append($playlist);
//                console.log(self.$playlists.length);
//            }
//        },
//        error:function() {
//            console.log('wtf is it?!');
//        }
//    });
//    var $playlists = self.$playlists.find('.playlist-plated');
//    for (var i = 0; i < $playlists.length; ++i) {
//        var $playlist = $($playlists[i]);
//        var $listener = $playlist.find('.listeners');
//        self.currentPlaylistId = $playlist.find('.playlistId').text();
//        console.log("id");
//        console.log(self.currentPlaylistId);
//        $listener.text('Listening ' + self.getPlaylistListeners() + ' people');
//    }

//    for (i = 0; i < $playlists.length; ++i) {
//        $playlist = $($playlists[i]);
//        self.currentPlaylistId = $playlist.find('.playlistId').text();
//        self._setNewImage($playlist, self.currentPlaylistId);
//    }
//};