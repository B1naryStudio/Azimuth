$(document).ready(function () {
    var progressSlider = new SliderController({
        sliderSelector: '#progressSlider',
        sliderBarClass: 'progressBar',
        sliderClass: 'progress',
        backgroundSliderClass: 'cache',
        dirrection: 'horizontall'
    });
    var audioManager = new AudioManager(null, progressSlider);

    var searchManager = new SearchManager(audioManager);
    searchManager.bindListeners();
    audioManager.bindListeners();
});