$(document).ready(function() {
    var playlistsGlobal = [];
    var stringForCreateBtn = "Create new playlist ";
    var $reloginForm = $("#relogin");
    var $vkMusic = $("#vkontakteMusic");
    var $playlistsTable = $('#playlistsTable');
    var $searchInput = $('#searchPlaylistName');
    var $createNewPlaylistBtn = $('#createNewPlaylistBtn');
    var playlistTrackTemplate = $("#playlistTrackTemplate");
    var playlistTemplate = $("#playlistTemplate");
    var trackTemplate = $("#trackTemplate");

    var showPlaylists = function(playlists) {
        $playlistsTable.find(".tableHeader").remove();
        if (typeof playlists === 'undefined') { //Initial run to get playlists from db
            $.ajax({
                url: '/api/playlists',
                success: function(playlists) {
                    if (typeof playlists.Message === 'undefined') {
                        $reloginForm.hide();
                        $vkMusic.show();
                        playlists = playlists.Result;
                        for (var i = 0; i < playlists.length; i++) {
                            var playlist = playlists[i];
                            if (playlist.Accessibilty == 1) {
                                playlist.Accessibilty = "public";
                            } else {
                                playlist.Accessibilty = "private";
                                playlistsGlobal.push(playlist);
                                $playlistsTable.append(playlistTemplate.tmpl(playlist));
                            }
                        }
                    } else {
                        $reloginForm.show();
                        $reloginForm.find('a').attr('href', reloginUrl);
                        $vkMusic.hide();
                    }
                    $('.accordion .tableRow').on("click", _getTracks);
                }
            });
        } else { //using to print playlists after using filter
            if (playlists.length != 0) {
                for (var i = 0; i < playlists.length; i++) {
                    $playlistsTable.append(playlistTemplate.tmpl(playlists[i]));
                }
            } else {
                $createNewPlaylistBtn.show();
                $createNewPlaylistBtn.text(stringForCreateBtn + $searchInput.val());
            }
        }
    };

    function _getTracks() {
        var $dragList = $(this).next();
        if (!$(this).hasClass('active') && $dragList.children().length == 0) {
            $.ajax({
                url: "/api/playlists/" + $(this).find('#playlistId').text(), // TODO replace with class playlistID
                type: 'GET',
                async: false,
                success: function(playlistData) {
                    var tracks = playlistData.Result.Tracks;
                    var list = $dragList;
                    for (var i = 0; i < tracks.length; i++) {
                        var track = tracks[i];
                        track.Duration = Math.floor(track.Duration / 60) + ":" + (track.Duration % 60 < 10 ? "0" + track.Duration % 60 : track.Duration % 60);
                        playlistTrackTemplate.tmpl(track).appendTo(list);
                    }
                    $('.draggable').makeDraggable({
                        contextMenu:[
                            { 'id': '1', 'name': 'first action', "isNewSection": "false" },
                            { 'id': '2', 'name': 'second action', "isNewSection": "false" },
                            { 'id': '3', 'name': 'third action', "isNewSection": "true" },
                            { 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
                        ]
                    });
                }
            });
        }
        $(this).next("#playlistTracksTable").slideToggle(100); // TODO replace with class name
        $(this).toggleClass("active");
    };

    $(document).on('PlaylistAdded', function(playlist) {
        playlistsGlobal.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
        $searchInput.trigger('input');
    });

    $(document).ready(function() {
        showPlaylists();
        $('.providerBtn').click(function(e) {
            var provider = $(e.target).data('provider');
            var reloginUrl = $(e.target).data('reloginurl');
            console.log(provider);
            $.ajax({
                url: '/api/usertracks?provider=' + provider,
                success: function(tracks) {
                    if (typeof tracks.Message === 'undefined') {
                        $reloginForm.hide();
                        $vkMusic.show();
                        var list = $('.vkMusicList');
                        for (var i = 0; i < tracks.length; i++) {
                            var track = tracks[i];
                            track.duration = Math.floor(track.duration / 60) + ":" + (track.duration % 60 < 10 ? "0" + track.duration % 60 : track.duration % 60);
                            trackTemplate.tmpl(track).appendTo(list);
                        }
                        $('.draggable').makeDraggable({
                            contextMenu: [
                                { 'id': '1', 'name': 'first action', "isNewSection": "false" },
                                { 'id': '2', 'name': 'second action', "isNewSection": "false" },
                                { 'id': '3', 'name': 'third action', "isNewSection": "true" },
                                { 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
                            ]
                        });
                    } else {
                        $reloginForm.show();
                        $reloginForm.find('a').attr('href', reloginUrl);
                        $vkMusic.hide();
                    }
                    $('.playBtn').on('click', _playTrack);
                }
            });
        });
    });

    $(document).ready(function () {
        $createNewPlaylistBtn.hide();
        $createNewPlaylistBtn.click(function () {
            var playlistName = $searchInput.val();
            $.ajax({
                url: '/api/playlists?name=' + playlistName + '&accessibilty=Public',
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                async: false
            });
            $createNewPlaylistBtn.hide();
            $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
        });
    });

    $(document).ready(function () {
        $('#checkall').click(function () {
            if ($(this).prop('checked')) {
                $('input:checkbox').prop('checked', true);
            } else {
                $('input:checkbox').prop('checked', false);
            }
        });
    });
    $(document).ready(function () {
        $searchInput.on('input', function (e) {
            $createNewPlaylistBtn.hide();
            var searchParam = $(this).val().toLocaleLowerCase();
            showPlaylists(playlistsGlobal.filter(function (index) {
                $searchInput.next().children().remove();
                return (index.Name.toLocaleLowerCase().indexOf(searchParam) != -1);
            }));
            $('.accordion .tableRow').on("click", _getTracks);
        });
    });
});