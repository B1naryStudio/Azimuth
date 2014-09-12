$(document).ready(function () {
    VK.init({
        apiId: "4469725"
    });
    VK.Auth.login(
        null,
        VK.access.FRIENDS | VK.access.WALL
    );

    var volumeSlider = new SliderController({
        sliderSelector: '#volumeSlider', 
        sliderBarClass: 'volumeBar', 
        sliderClass: 'volume', 
        dirrection: 'vertical'
    });
    var progressSlider = new SliderController({
        sliderSelector: '#progressSlider',
        sliderBarClass: 'progressBar',
        sliderClass: 'progress',
        backgroundSliderClass: 'cache',
        dirrection: 'horizontall'
    });

    var audioManager = new AudioManager(volumeSlider, progressSlider);
    var manager = new MusicManager(audioManager);
    
    manager.showPlaylists();
    manager.bindListeners();
    audioManager.bindListeners();


    $(this).mousedown(function (event) {
        var $target = $(event.target);
        if ($target.hasClass('progressBar') || $target.hasClass('progress') || $target.hasClass('cache')) {
            mousedownOnProgressBar = true;
        }
        if (!$target.parents().hasClass('draggable-list') && !$target.parents().hasClass('list')) {
            document.oncontextmenu = function () {
                return true;
            }
        }
        if (!$target.hasClass('contextMenuActionName') && event.which != 3) {
            $('.contextMenu').detach();
        }
    });

});