$(document).ready(function() {

    $('.search-box').off('keyup').keyup(function () {
        $.ajax({
            url: 'api/usertracks/globalsearch?searchText=' + $('.search-box').val() + "&criteria=" + $('select option:selected').val(),
            type: 'GET',
            dataType: 'json',
            success: function (tracks) {
                $('#trackList').find('.track').remove();
                for (var i = 0; i < tracks.length; i++) {
                    var playlistTrackTemplate = $("#trackTemplate");
                    var tmpl = playlistTrackTemplate.tmpl(tracks[i]);
                    tmpl.appendTo('#trackList');
                }
            }
        });
    });
});