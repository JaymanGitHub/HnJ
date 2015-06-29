var offset = 220;
var duration = 500;

$(document).ready(function () {

    $(window).scroll(function () {
        if (jQuery(this).scrollTop() > offset) {
            jQuery('.back-to-top').fadeIn(duration);
        } else {
            jQuery('.back-to-top').fadeOut(duration);
        }
    });
});

$(document).keyup(function (e) {
    if (e.keyCode == 27) {
        HideOverlay();
    }
});

function backToTop() {
    jQuery('html, body').animate({ scrollTop: 0 }, duration);
    return false;
}

function ShowOverlay(obj) {
    if ($('#Overlay').length) {
        backToTop();

        currentMovie = obj;

        var movieBox = $(obj).parent().parent();

        //var title = $(movieBox).find(".panel-heading h3").text()
        //var img = $(obj).find("img").attr("src");

        var poster = $(obj).find("#poster").val()
        var title = $(obj).find("#title").val()
        var actors = $(obj).find("#actors").val()
        var directors = $(obj).find("#directors").val()
        var releaseDate = $(obj).find("#releaseDate").val()
        var id = $(obj).find("#id").val()

        var genres = "";

        $(obj).find(".genres").each(function (index) {
            genres += $(this).val();
        });


        $(".OverlayPanel .panel-title").text(title)

        $("#overlayTitle").text(title);
        $("#overlayPoster").attr("src", poster)
        $("#overlayActors").text(actors);
        $("#overlayDirectors").text(directors);
        //$("#overlayReleaseDate").text(releaseDate);

        $("#overlayMovieLink").attr("href", "/Movie/Detail/" + id)

        $("#overlayGenres").html("<label>" +  genres + "</label>")

         

        $('#Overlay').fadeIn();
        $("#Overlay .OverlayPanel").show();
        $("body").css("overflow", "hidden");
    }
}

function HideOverlay() {
    $('#Overlay').fadeOut();
    $("#Overlay .OverlayPanel").hide();
    $("body").css("overflow", "auto");
}

+function ($) {
    "use strict";

    // CAROUSEL CLASS DEFINITION
    // =========================

    var Carousel = function (element, options) {
        this.$element = $(element)
        this.$indicators = this.$element.find('.carousel-indicators')
        this.options = options
        this.paused =
        this.sliding =
        this.interval =
        this.$active =
        this.$items = null

        this.options.pause == 'hover' && this.$element
          .on('mouseenter', $.proxy(this.pause, this))
          .on('mouseleave', $.proxy(this.cycle, this))
    }

    Carousel.DEFAULTS = {
        interval: 5000
    , pause: 'hover'
    , wrap: true
    }

    Carousel.prototype.cycle = function (e) {
        e || (this.paused = false)

        this.interval && clearInterval(this.interval)

        this.options.interval
          && !this.paused
          && (this.interval = setInterval($.proxy(this.next, this), this.options.interval))

        return this
    }

    Carousel.prototype.getActiveIndex = function () {
        this.$active = this.$element.find('.item.active')
        this.$items = this.$active.parent().children()

        return this.$items.index(this.$active)
    }

    Carousel.prototype.to = function (pos) {
        var that = this
        var activeIndex = this.getActiveIndex()

        if (pos > (this.$items.length - 1) || pos < 0) return

        if (this.sliding) return this.$element.one('slid', function () { that.to(pos) })
        if (activeIndex == pos) return this.pause().cycle()

        return this.slide(pos > activeIndex ? 'next' : 'prev', $(this.$items[pos]))
    }

    Carousel.prototype.pause = function (e) {
        e || (this.paused = true)

        if (this.$element.find('.next, .prev').length && $.support.transition.end) {
            this.$element.trigger($.support.transition.end)
            this.cycle(true)
        }

        this.interval = clearInterval(this.interval)

        return this
    }

    Carousel.prototype.next = function () {
        if (this.sliding) return
        return this.slide('next')
    }

    Carousel.prototype.prev = function () {
        if (this.sliding) return
        return this.slide('prev')
    }

    Carousel.prototype.slide = function (type, next) {
        var $active = this.$element.find('.item.active')
        var $next = next || $active[type]()
        var isCycling = this.interval
        var direction = type == 'next' ? 'left' : 'right'
        var fallback = type == 'next' ? 'first' : 'last'
        var that = this

        if (!$next.length) {
            if (!this.options.wrap) return
            $next = this.$element.find('.item')[fallback]()
        }

        this.sliding = true

        isCycling && this.pause()

        var e = $.Event('slide.bs.carousel', { relatedTarget: $next[0], direction: direction })

        if ($next.hasClass('active')) return

        if (this.$indicators.length) {
            this.$indicators.find('.active').removeClass('active')
            this.$element.one('slid', function () {
                var $nextIndicator = $(that.$indicators.children()[that.getActiveIndex()])
                $nextIndicator && $nextIndicator.addClass('active')
            })
        }

        if ($.support.transition && this.$element.hasClass('slide')) {
            this.$element.trigger(e)
            if (e.isDefaultPrevented()) return
            $next.addClass(type)
            $next[0].offsetWidth // force reflow
            $active.addClass(direction)
            $next.addClass(direction)
            $active
              .one($.support.transition.end, function () {
                  $next.removeClass([type, direction].join(' ')).addClass('active')
                  $active.removeClass(['active', direction].join(' '))
                  that.sliding = false
                  setTimeout(function () { that.$element.trigger('slid') }, 0)
              })
              .emulateTransitionEnd(600)
        } else {
            this.$element.trigger(e)
            if (e.isDefaultPrevented()) return
            $active.removeClass('active')
            $next.addClass('active')
            this.sliding = false
            this.$element.trigger('slid')
        }

        isCycling && this.cycle()

        return this
    }


    // CAROUSEL PLUGIN DEFINITION
    // ==========================

    var old = $.fn.carousel

    $.fn.carousel = function (option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.carousel')
            var options = $.extend({}, Carousel.DEFAULTS, $this.data(), typeof option == 'object' && option)
            var action = typeof option == 'string' ? option : options.slide

            if (!data) $this.data('bs.carousel', (data = new Carousel(this, options)))
            if (typeof option == 'number') data.to(option)
            else if (action) data[action]()
            else if (options.interval) data.pause().cycle()
        })
    }

    $.fn.carousel.Constructor = Carousel


    // CAROUSEL NO CONFLICT
    // ====================

    $.fn.carousel.noConflict = function () {
        $.fn.carousel = old
        return this
    }


    // CAROUSEL DATA-API
    // =================

    $(document).on('click.bs.carousel.data-api', '[data-slide], [data-slide-to]', function (e) {
        var $this = $(this), href
        var $target = $($this.attr('data-target') || (href = $this.attr('href')) && href.replace(/.*(?=#[^\s]+$)/, '')) //strip for ie7
        var options = $.extend({}, $target.data(), $this.data())
        var slideIndex = $this.attr('data-slide-to')
        if (slideIndex) options.interval = false

        $target.carousel(options)

        if (slideIndex = $this.attr('data-slide-to')) {
            $target.data('bs.carousel').to(slideIndex)
        }

        e.preventDefault()
    })

    $(window).on('load', function () {
        $('[data-ride="carousel"]').each(function () {
            var $carousel = $(this)
            $carousel.carousel($carousel.data())
        })
    })

}(window.jQuery);