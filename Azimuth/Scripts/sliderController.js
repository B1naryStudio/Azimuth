var SliderController = function (sliderSelector, sliderBarClass, sliderClass, dirrection) {
    var self = this;

    this.drag = false;
    this.relativePosition = 0;
    this.dirrection = dirrection;
    this.$sliderBar = $(sliderSelector);
    $(sliderSelector).append('<div class="slider"></div>');
    this.$slider = $($(sliderSelector).find('.slider'));

    this.size = (dirrection == 'vertical' ? 'height' : 'width');

    self.$sliderBar.addClass(sliderBarClass);
    self.$slider.addClass(sliderClass);
    self.bindListeners();

    this._getSize = function() {
        return (dirrection == 'vertical' ? self.$sliderBar.height() : self.$sliderBar.width());
    };

    this._getOffset = function () {
        return (dirrection == 'vertical' ? self.$sliderBar.offset().top : self.$sliderBar.offset().left);
    };

    this._getPagePos = function(e) {
        return (dirrection == 'vertical' ? e.pageY : e.pageX);
    };

    this._getCurrentPosition = function(e) {
        if (dirrection == 'vertical') {
            return self._getSize() - self._getPagePos(e) + self._getOffset();
        } else {
            return self._getPagePos(e) - self._getOffset();
        }
    };
}

SliderController.prototype.getSlider = function () {
    var self = this;
    return self.$sliderBar;
};

SliderController.prototype.getPosition = function () {
    var self = this;
    return self.relativePosition;
};

SliderController.prototype.setPosition = function (position) {
    var self = this;
    self.relativePosition = position;
    self.$slider.css(self.size, 100 * self.relativePosition + '%');
};

SliderController.prototype.bindListeners = function () {
    var self = this;

    self.$sliderBar.on('mousedown', function (e) {
        self.drag = true;
        var position = self._getCurrentPosition(e);
        var percentage = 100 * position / self._getSize();
        self.$slider.css(self.size, percentage + '%');
        self.relativePosition = position / self._getSize();

        self.$sliderBar.trigger('OnChange', [self.relativePosition]);
    });

    $(document).on('mousemove', function (e) {
        if (self.drag == false) {
            return;
        }
        if (self._getPagePos(e) < self._getOffset()) {
            position = self._getSize();
        } else if (self._getPagePos(e) > self._getOffset() + self._getSize()) {
            position = 0;
        } else {
            var position = self._getCurrentPosition(e);
        }
        var percentage = 100 * position / self._getSize();
        self.$slider.css(self.size, percentage + '%');
        self.relativePosition = position / self._getSize();

        self.$sliderBar.trigger('OnChange', [self.relativePosition]);
    });

    $(document).mouseup(function () {
        self.drag = false;
    });
};