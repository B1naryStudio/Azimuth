$(document).ready(function () {
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
    var manager = new SettingsManager(audioManager);
    
    manager.showPlaylists();
    manager.bindListeners();
    audioManager.bindListeners();
});