$(document).ready(function() {
    var playlists_global = [];
    var stringForCreateBtn = "Create new playlist ";
    var showPlaylists = function(playlists) {
        $('#playlistsTable').find(".tableHeader").remove();
        if (typeof (playlists) == 'undefined') { //Initial run to get playlists from db
            $.ajax({
                url: '/api/playlists',
                success: function(playlists) {
                    if (typeof playlists.Message === 'undefined') {
                        $("#relogin").hide();
                        playlists = playlists.Result;
                        var list = $('#playlistsTable');
                        for (var i = 0; i < playlists.length; i++) {
                            var playlist = playlists[i];
                            if (playlist.Accessibilty == 1)
                                playlist.Accessibilty = "public";
                            else
                                playlist.Accessibilty = "private";
                            playlists_global.push(playlist);
                            list.append($("#playlistTemplate").tmpl(playlist));
                        }
                    } else {
                        $("#relogin").show();
                        var reloginContainer = $('#relogin');
                        reloginContainer.find('a').attr('href', reloginUrl);
                    }
                    $('.accordion .tableRow').on("click", _getTracks);
                }
            });
        } else { //using to print playlists after using filter
            if (playlists.length != 0) {
                var list = $('#playlistsTable');
                for (var i = 0; i < playlists.length; i++) {
                    var playlist = playlists[i];
                    list.append($("#playlistTemplate").tmpl(playlist));
                }
            } else {
                var $createBtn = $('#createNewPlaylistBtn');
                $createBtn.show();
                $createBtn.text(stringForCreateBtn + $('#searchPlaylistName').val());
            }
        }
    };

    function _getTracks() {
        var $dragList = $(this).parent().children('.draggable-list');

        if (!$(this).hasClass('active') && $dragList.children().length == 0) {
            $.ajax({
                url: "/api/playlists/" + $(this).find('#playlistId').text(),
                type: 'GET',
                async: false,
                success: function(playlistData) {
                    var tracks = playlistData.Result.Tracks;
                    var list = $dragList;
                    for (var i = 0; i < tracks.length; i++) {
                        var track = tracks[i];
                        $("#playlistTrackTemplate").tmpl(track).appendTo(list);
                    }
                    $('.draggable').makeDraggable();

                }
            });
        }

        $(".accordion > .tableRow").click(function () {

            //$(this).next(".draggable-list").slideToggle("slow");
            $(this).next("#playlistTracksTable").slideToggle(100);
            $(this).toggleClass("active");
        });
    };

    $(document).on('PlaylistAdded', function(playlist) {
        console.log(playlist);
        playlists_global.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
        $('#searchPlaylistName').trigger('input');
    });

    $(document).ready(function() {
        showPlaylists();
        $('.providerBtn').click(function(e) {
            var provider = $(e.target).data('provider');
            var reloginUrl = $(e.target).data('reloginurl');
            console.log(provider);
            //$("#tracks > tr").remove();
            $.ajax({
                url: '/api/usertracks?provider=' + provider,
                success: function(tracks) {
                    if (typeof tracks.Message === 'undefined') {
                        console.log(tracks);
                        $("#relogin").hide();
                        //var list = $('#tracksTable');
                        var list = $('.vkMusicList');
                        for (var i = 0; i < tracks.length; i++) {
                            var track = tracks[i];
                            $("#trackTemplate").tmpl(track).appendTo(list);
                        }
                        $('.draggable').makeDraggable();
                    } else {
                        $("#relogin").show();
                        var reloginContainer = $('#relogin');
                        reloginContainer.find('a').attr('href', reloginUrl);
                    }
                }
            });
        });
    });

    $(document).ready(function () {
        $('#createNewPlaylistBtn').hide();
        $('#createNewPlaylistBtn').click(function () {
            var playlistName = $('#searchPlaylistName').val();
            $.ajax({
                url: '/api/playlists?name=' + playlistName + '&accessibilty=Public',
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                async: false
            });
            $('#createNewPlaylistBtn').hide();
            $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
        });
        $('#applyBtn').click(function (e) {
            var provider = $(e.target).data('provider');
            var tableControl = document.getElementById('tracksTable');
            var tracks = [];
            var accessibilty = ($('#setAccessibilty').val() === "Private" ? 0 : 1);
            console.log(accessibilty);
            $('input:checkbox:checked', tableControl).each(function () {
                tracks.push($(this).closest('.tableRow').find('#trackId').text());
            }).get();
            console.log(tracks);
            var playlistName = $('#inputPlaylistName').val();
            console.log(playlistName);

            $.ajax({
                url: '/api/usertracks?provider=' + provider,
                type: 'POST',
                data: JSON.stringify({
                    "Name": playlistName,
                    "Accessibilty": accessibilty,
                    "TrackIds": tracks
                }) + JSON.stringify({ "Provider": provider }),
                dataType: 'json',
                contentType: 'application/json',
                async: false
            });
            $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: accessibilty });
        });
    });

    $(document).ready(function () {
        $('#checkall').click(function () {
            if ($(this).prop('checked'))
                $('input:checkbox').prop('checked', true);
            else
                $('input:checkbox').prop('checked', false);
        });
    });
    $(document).ready(function () {
        $('#searchPlaylistName').on('input', function (e) {
            $('#createNewPlaylistBtn').hide();
            var searchParam = $(this).val().toLocaleLowerCase();
            showPlaylists(playlists_global.filter(function (index) {
                return (index.Name.toLocaleLowerCase().indexOf(searchParam) != -1);
            }));
        });
    });
});