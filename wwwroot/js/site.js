// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('#back-to-top').fadeIn();
        } else {
            $('#back-to-top').fadeOut();
        }
    });
    // scroll body to 0px on click
    $('#back-to-top').click(function () {
        $('body,html').animate({
            scrollTop: 0
        }, 400);
        return false;
    });
});

function createSpinnerFor(element) {
    let spinner = document.createElement("div");
    spinner.classList.add("spinner-grow", "text-center", "text-primary", ...element.classList);
    spinner.setAttribute("role", "status");
    let spinnerSize = Math.min(element.clientWidth, element.clientHeight);
    spinner.style.width = `${spinnerSize}px`;
    spinner.style.height = `${spinnerSize}px`;
    spinner.innerHTML = '<span class="sr-only">Loading...</span>'
    return spinner;
}