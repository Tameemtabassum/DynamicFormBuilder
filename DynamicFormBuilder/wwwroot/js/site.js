// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//SAGOR

// ==========================================
// CENTRALIZED PAGINATION UTILITY
// Include this in _Layout.cshtml or as a separate JS file
// ==========================================

/**
 * Loads a partial view into the specified container via AJAX
 * @param {string} url - The URL to fetch
 * @param {string} targetContainer - The ID of the container to update (default: 'display')
 */
function loadPartialView(url, targetContainer = 'display') {
    $.ajax({
        url: url,
        type: 'GET',
        beforeSend: function () {
            if (typeof Spinner !== 'undefined') {
                Spinner.show();
            }
        },
        success: function (response) {
            if (typeof Spinner !== 'undefined') {
                Spinner.hide();
            }
            $(`#${targetContainer}`).html(response);

            // Scroll to top of container after loading
            $('html, body').animate({
                scrollTop: $(`#${targetContainer}`).offset().top - 100
            }, 300);
        },
        error: function (xhr, status, error) {
            if (typeof Spinner !== 'undefined') {
                Spinner.hide();
            }
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Error!',
                    text: 'Something went wrong. Please try again later.',
                    icon: 'error',
                    confirmButtonText: 'Close'
                });
            } else {
                alert('Error loading page. Please try again.');
            }
        }
    });
}

/**
 * Handles page size changes for pagination
 * @param {string} baseUrl - The base URL for pagination
 * @param {number} pageSize - The new page size
 * @param {string} targetContainer - The ID of the container to update (default: 'display')
 */
function loadPagination(baseUrl, pageSize, targetContainer = 'display') {
    try {
        // Parse the base URL and update parameters
        var url = new URL(baseUrl, window.location.origin);
        url.searchParams.set('pageSize', pageSize);
        url.searchParams.set('page', '1'); // Reset to page 1 when changing page size

        loadPartialView(url.toString(), targetContainer);
    } catch (e) {
        // Fallback for older browsers
        var separator = baseUrl.includes('?') ? '&' : '?';
        var cleanUrl = baseUrl.replace(/[?&]pageSize=\d+/g, '').replace(/[?&]page=\d+/g, '');
        var newUrl = cleanUrl + separator + 'pageSize=' + pageSize + '&page=1';

        loadPartialView(newUrl, targetContainer);
    }
}

/**
 * Navigates to a specific page number
 * @param {string} baseUrl - The base URL for pagination
 * @param {number} maxPages - The maximum number of pages
 * @param {string} targetContainer - The ID of the container to update (default: 'display')
 */
function goToPage(baseUrl, maxPages, targetContainer = 'display') {
    var pageNumber = parseInt($('#goToPageInput').val());

    if (isNaN(pageNumber) || pageNumber < 1 || pageNumber > maxPages) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Invalid Page',
                text: `Please enter a page number between 1 and ${maxPages}`,
                icon: 'warning',
                confirmButtonText: 'OK'
            });
        } else {
            alert(`Please enter a page number between 1 and ${maxPages}`);
        }
        $('#goToPageInput').val('');
        return;
    }

    try {
        var url = new URL(baseUrl, window.location.origin);
        url.searchParams.set('page', pageNumber.toString());

        loadPartialView(url.toString(), targetContainer);
    } catch (e) {
        // Fallback for older browsers
        var separator = baseUrl.includes('?') ? '&' : '?';
        var cleanUrl = baseUrl.replace(/[?&]page=\d+/g, '');
        var newUrl = cleanUrl + separator + 'page=' + pageNumber;

        loadPartialView(newUrl, targetContainer);
    }
}

/**
 * Handle Enter key in Go to Page input
 */
$(document).on('keypress', '#goToPageInput', function (e) {
    if (e.which === 13) { // Enter key
        e.preventDefault();
        var goButton = $(this).siblings('button').first();
        if (goButton.length) {
            goButton.click();
        }
    }
});
