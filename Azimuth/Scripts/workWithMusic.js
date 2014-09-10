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
});