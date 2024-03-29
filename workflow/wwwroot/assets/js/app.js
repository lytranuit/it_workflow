﻿/**
 * Theme: Metrica - Responsive Bootstrap 4 Admin Dashboard
 * Author: Mannatthemes
 * Module/App: Main Js
 */


(function ($) {
    var dropdownMenu;

    $(document).on('click', '.keepopen .dropdown-menu', function (e) {
        e.stopPropagation();
    });
    // and when you show it, move it to the body                                     
    //$(window).on('show.bs.dropdown', function (e) {
    //    //if (dropdownMenu) {
    //    //    var labelledby = dropdownMenu.attr("aria-labelledby");
    //    //    var parent = $("#" + labelledby);
    //    //    console.log(parent);
    //    //    $(parent).append(dropdownMenu.detach());
    //    //    dropdownMenu.hide();
    //    //}
    //    $(" dropdown-toggle").modal("hide");
    //    // grab the menu        
    //    dropdownMenu = $(e.target).find('.dropdown-menu');

    //    // detach it and append it to the body
    //    $('body').append(dropdownMenu.detach());

    //    // grab the new offset position
    //    var eOffset = $(e.target).offset();

    //    // make sure to place it where it would normally go (this could be improved)
    //    dropdownMenu.css({
    //        'display': 'block',
    //        'top': eOffset.top + $(e.target).outerHeight(),
    //        'left': eOffset.left
    //    });

    //});

    //// and when you hide it, reattach the drop down, and hide it normally                                                   
    //$(window).on('hide.bs.dropdown', function (e) {
    //    //var target = $(e.target);
    //    $(e.target).append(dropdownMenu.detach());
    //    dropdownMenu.hide();

    //});
    'use strict';

    function initSlimscroll() {
        $('.slimscroll').slimscroll({
            height: 'auto',
            position: 'right',
            size: "7px",
            color: '#e0e5f1',
            opacity: 1,
            wheelStep: 5,
            touchScrollStep: 50
        });
    }


    function initMetisMenu() {
        //metis menu
        $(".metismenu").metisMenu();
    }

    function initLeftMenuCollapse() {
        // Left menu collapse
        $('.button-menu-mobile').on('click', function (event) {
            event.preventDefault();
            $("body").toggleClass("enlarge-menu");
            initSlimscroll();
        });
    }

    function initEnlarge() {
        if ($(window).width() < 1025) {
            $('body').addClass('enlarge-menu');
        } else {
            if ($('body').data('keep-enlarged') != true)
                $('body').removeClass('enlarge-menu');
        }
    }



    function initActiveMenu() {
        // === following js will activate the menu in left side bar based on url ====
        $(".left-sidenav a").each(function () {
            var pageUrl = window.location.href.split(/[?#]/)[0];
            if (this.href == pageUrl) {
                $(this).addClass("active");
                $(this).parent().addClass("active"); // add active to li of the current link      
                $(this).parent().addClass("mm-active");
                $(this).parent().parent().addClass("in");
                $(this).parent().parent().addClass("mm-show");
                $(this).parent().parent().parent().addClass("mm-active");
                $(this).parent().parent().prev().addClass("active"); // add active class to an anchor
                $(this).parent().parent().parent().addClass("active");
                $(this).parent().parent().parent().parent().addClass("mm-show"); // add active to li of the current link                
                $(this).parent().parent().parent().parent().parent().addClass("mm-active");

            }
        });
    }



    function init() {
        initSlimscroll();
        initMetisMenu();
        initLeftMenuCollapse();
        initEnlarge();
        initTooltipPlugin();
        initActiveMenu();
        Waves.init();



        //warning Message

        //Swal.fire(
        //	'Thông báo bảo trì!',
        //	'Hệ thống sẽ bảo trì lúc 16h ngày 14/11/2022 và dự kiến sẽ mở lại lúc 7h sáng ngày 15/11/2022',
        //	'warning'
        //)

        $(document).ready(function () {
            ////Confirm
            $(document)
                .off("click", "[data-type='confirm']")
                .on("click", "[data-type='confirm']", function (e) {
                    e.preventDefault();
                    var title = $(this).attr("title");
                    var href = $(this).attr("href");
                    if (confirm(title) == true) {
                        if (href) location.href = href;
                    }
                    return false;
                });
            $("[autocomplete]").each(function () {
                var value = $(this).attr("autocomplete");
                if (value == "off") {
                    //console.log($(this).val());
                    $(this).val('');
                }
            });
            if (!$(".wait_loading").length) {
                $(".preloader").fadeOut();

                if ($(".chosen").length) {
                    $(".chosen").chosen({
                        search_contains: true
                    });
                }
                if ($(".dropify").length) {
                    $('.dropify').dropify();
                }

                if ($(".chosen").length) {
                    $(".chosen").chosen({
                        search_contains: true
                    });
                }
                if ($(".multi_select").length) {
                    $(".multi_select").multiselect({
                        enableFiltering: true,
                        buttonWidth: "100%",
                        includeSelectAllOption: true
                    });
                }
            }
        })

    }

    init();

})(jQuery)

function number_format(number, decimals, decPoint, thousandsSep) {
    // eslint-disable-line camelcase
    //  discuss at: https://locutus.io/php/number_format/
    // original by: Jonas Raoni Soares Silva (https://www.jsfromhell.com)
    // improved by: Kevin van Zonneveld (https://kvz.io)
    // improved by: davook
    // improved by: Brett Zamir (https://brett-zamir.me)
    // improved by: Brett Zamir (https://brett-zamir.me)
    // improved by: Theriault (https://github.com/Theriault)
    // improved by: Kevin van Zonneveld (https://kvz.io)
    // bugfixed by: Michael White (https://getsprink.com)
    // bugfixed by: Benjamin Lupton
    // bugfixed by: Allan Jensen (https://www.winternet.no)
    // bugfixed by: Howard Yeend
    // bugfixed by: Diogo Resende
    // bugfixed by: Rival
    // bugfixed by: Brett Zamir (https://brett-zamir.me)
    //  revised by: Jonas Raoni Soares Silva (https://www.jsfromhell.com)
    //  revised by: Luke Smith (https://lucassmith.name)
    //    input by: Kheang Hok Chin (https://www.distantia.ca/)
    //    input by: Jay Klehr
    //    input by: Amir Habibi (https://www.residence-mixte.com/)
    //    input by: Amirouche
    //   example 1: number_format(1234.56)
    //   returns 1: '1,235'
    //   example 2: number_format(1234.56, 2, ',', ' ')
    //   returns 2: '1 234,56'
    //   example 3: number_format(1234.5678, 2, '.', '')
    //   returns 3: '1234.57'
    //   example 4: number_format(67, 2, ',', '.')
    //   returns 4: '67,00'
    //   example 5: number_format(1000)
    //   returns 5: '1,000'
    //   example 6: number_format(67.311, 2)
    //   returns 6: '67.31'
    //   example 7: number_format(1000.55, 1)
    //   returns 7: '1,000.6'
    //   example 8: number_format(67000, 5, ',', '.')
    //   returns 8: '67.000,00000'
    //   example 9: number_format(0.9, 0)
    //   returns 9: '1'
    //  example 10: number_format('1.20', 2)
    //  returns 10: '1.20'
    //  example 11: number_format('1.20', 4)
    //  returns 11: '1.2000'
    //  example 12: number_format('1.2000', 3)
    //  returns 12: '1.200'
    //  example 13: number_format('1 000,50', 2, '.', ' ')
    //  returns 13: '100 050.00'
    //  example 14: number_format(1e-8, 8, '.', '')
    //  returns 14: '0.00000001'

    number = (number + "").replace(/[^0-9+\-Ee.]/g, "");
    var n = !isFinite(+number) ? 0 : +number;
    var prec = !isFinite(+decimals) ? 0 : Math.abs(decimals);
    var sep = typeof thousandsSep === "undefined" ? "," : thousandsSep;
    var dec = typeof decPoint === "undefined" ? "." : decPoint;
    var s = "";

    var toFixedFix = function (n, prec) {
        if (("" + n).indexOf("e") === -1) {
            return +(Math.round(n + "e+" + prec) + "e-" + prec);
        } else {
            var arr = ("" + n).split("e");
            var sig = "";
            if (+arr[1] + prec > 0) {
                sig = "+";
            }
            return (+(
                Math.round(+arr[0] + "e" + sig + (+arr[1] + prec)) +
                "e-" +
                prec
            )).toFixed(prec);
        }
    };

    // @todo: for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec).toString() : "" + Math.round(n)).split(".");
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || "").length < prec) {
        s[1] = s[1] || "";
        s[1] += new Array(prec - s[1].length + 1).join("0");
    }

    return s.join(dec);
}


function initTooltipPlugin() {
    $.fn.tooltip && $('[data-toggle="tooltip"]').tooltip()
}

function fillForm(form, data) {
    $("input, select, textarea", form).not("[type=file]").each(function () {
        var type = $(this).attr("type");
        var name = $(this).attr("name");
        if (!name) return;
        name = name.replace("[]", "");
        var value = "";
        if ($(this).hasClass("input-tmp")) return;
        if ($.type(data[name]) !== "undefined" && $.type(data[name]) !== "null") {
            value = data[name];
        } else {
            return;
        }
        //console.log(value);
        if (name == "date_effect" || name == "date_expire" || name == "date_review" || name == "birthday" || name == "date_work" || name == "date") {
            value = moment(value).format("YYYY-MM-DD");
            //console.log(value);
        }
        switch (type) {
            case "checkbox":
                $(this).prop("checked", false);
                var rdvalue = $(this).val();
                if (rdvalue == value || value.indexOf(rdvalue) != -1) {
                    $(this).prop("checked", true);
                }
                break;
            case "radio":
                $(this).removeAttr("checked", "checked");
                var rdvalue = $(this).val();
                if (rdvalue == value) {
                    $(this).prop("checked", true);
                }
                break;
            default:
                $(this).val(value);
                if ($(this).hasClass("chosen")) {
                    $(this).trigger("chosen:updated");
                }
                if ($(this).hasClass("multi_select")) {
                    $(this).multiselect("refresh");
                }
                break;
        }
    });
};
function getOffsetRect(element) {
    let box = element.getBoundingClientRect()

    let scrollTop = window.pageYOffset
    let scrollLeft = window.pageXOffset

    let top = box.top + scrollTop
    let left = box.left + scrollLeft

    return { top: Math.round(top), left: Math.round(left) }
}
function getMousePosition(element, event) {
    let mouseX = event.pageX || event.clientX + document.documentElement.scrollLeft
    let mouseY = event.pageY || event.clientY + document.documentElement.scrollTop

    let offset = getOffsetRect(element)
    let x = mouseX - offset.left
    let y = mouseY - offset.top

    return {
        x: x,
        y: y
    }
}
function rand(length) {
    // https://stackoverflow.com/a/47496558/810360
    //[...Array(length)].map(() => Math.random().toString(36)[2])

    // https://stackoverflow.com/a/46536578/810360
    var char;
    var arr = [];
    var length = length || 32;
    var alphaNumeric = [48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122]
    do {
        char = ~~(Math.random() * 128);
        if (alphaNumeric.indexOf(char) !== -1) {
            arr.push(String.fromCharCode(char))
        }
    } while (arr.length < length);

    return arr.join('')
}