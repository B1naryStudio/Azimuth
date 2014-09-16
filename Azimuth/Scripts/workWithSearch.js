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

    var searchManager = new SearchManager(audioManager);
    searchManager.bindListeners();
    audioManager.bindListeners();
});