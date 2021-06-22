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
    let spinnerSize = Math.max(Math.min(element.clientWidth, element.clientHeight), 15);
    spinner.style.width = `${spinnerSize}px`;
    spinner.style.height = `${spinnerSize}px`;
    spinner.innerHTML = '<span class="sr-only">Loading...</span>'
    return spinner;
}

function createLoadingButtonFor(button) {
    let btn = document.createElement('button');
    btn.classList.add(...button.classList);
    btn.disabled = true;
    let loadingSpan = document.createElement('span');
    loadingSpan.classList.add('spinner-grow', 'spinner-grow-sm');
    loadingSpan.setAttribute('role', 'status');
    loadingSpan.setAttribute('aria-hidden', 'true');
    btn.appendChild(loadingSpan)
    btn.append('Loading...');

    return btn;
}