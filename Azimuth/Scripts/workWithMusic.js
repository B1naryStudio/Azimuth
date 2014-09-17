$(document).ready(function () {
    var volumeSlider = new SliderController({
        sliderSelector: '#volumeSlider', 
        sliderBarClass: 'volumeBar', 
        sliderClass: 'volume', 
        dirrection: 'horizontall'
    });
    //var progressSlider = new SliderController({
    //    sliderSelector: '#progressSlider',
    //    sliderBarClass: 'progressBar',
    //    sliderClass: 'progress',
    //    backgroundSliderClass: 'cache',
    //    dirrection: 'horizontall'
    //});
    var progressBar = new ProgressBar();
    progressBar.bindListeners();

    var audioManager = new AudioManager(volumeSlider, progressBar);
    var manager = new MusicManager(audioManager);
    
    manager.showPlaylists();
    manager.setDefaultPlaylist();
    manager.bindListeners();
    audioManager.bindListeners();


    $(this).mousedown(function (event) {
        var $target = $(event.target);
        if ($target.hasClass('progressBar') || $target.hasClass('progress') || $target.hasClass('cache')) {
            mousedownOnProgressBar = true;
        }
        if (!$target.parents().hasClass('draggable-list') && !$target.parents().hasClass('list')) {
            document.oncontextmenu = function() {
                return true;
            };
        }
        if (!$target.hasClass('contextMenuActionName') && event.which != 3) {
            $('.contextMenu').detach();
        }
    });

});