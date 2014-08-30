var SliderController = function (sliderSelector) {
    var self = this;

    this.drag = false;
    this.relativePosition = 0;
    this.$sliderBar = $(sliderSelector);
    $(sliderSelector).append('<div class="slider"></div>');
    this.$slider = $($(sliderSelector).find('.slider'));

    self.$sliderBar.addClass('volumeBar');
    self.$slider.addClass('volume');
    self.bindListeners();
}

SliderController.prototype.getPosition = function () {
    var self = this;
    return self.relativePosition;
};

SliderController.prototype.setPosition = function (position) {
    var self = this;
    self.relativePosition = position;
    self.$slider.css('height', 100 * self.relativePosition + '%');
};

SliderController.prototype.bindListeners = function () {
    var self = this;

    self.$sliderBar.on('mousedown', function (e) {
        self.drag = true;
        var position = self.$sliderBar.height() - e.pageY + self.$sliderBar.offset().top;
        var percentage = 100 * position / self.$sliderBar.height();
        self.$slider.css('height', percentage + '%');
        self.relativePosition = position / self.$sliderBar.height();

        self.$sliderBar.trigger('OnChange', [self.relativePosition]);
    });

    $(document).on('mousemove', function (e) {
        if (self.drag == false) {
            return;
        }
        if (e.pageY < self.$sliderBar.offset().top) {
            position = self.$sliderBar.height();
        } else if (e.pageY > self.$sliderBar.offset().top + self.$sliderBar.height()) {
            position = 0;
        } else {
            var position = self.$sliderBar.height() - e.pageY + self.$sliderBar.offset().top;
        }
        var percentage = 100 * position / self.$sliderBar.height();
        self.$slider.css('height', percentage + '%');
        self.relativePosition = position / self.$sliderBar.height();

        self.$sliderBar.trigger('OnChange', [self.relativePosition]);
    });

    $(document).mouseup(function () {
        self.drag = false;
    });
};