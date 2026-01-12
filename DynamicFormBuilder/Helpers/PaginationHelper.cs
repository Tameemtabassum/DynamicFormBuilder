using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using X.PagedList;

namespace DynamicFormBuilder.ViewModels
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Generates complete pagination HTML with page numbers, navigation, and page size selector
        /// </summary>
        public static IHtmlContent PaginationFor<T>(
            this IHtmlHelper html,
            IPagedList<T> pagedList,
            Func<int, string> generateUrl,
            int[] pageSizeOptions = null)

        {
            if (pagedList == null)
                return new HtmlString(string.Empty);

            pageSizeOptions = pageSizeOptions ?? new[] { 30, 50, 100, 200 };

            var builder = new StringBuilder();

            // Container div for pagination controls
            builder.Append("<div class='pagination-container d-flex flex-wrap justify-content-center align-items-center mt-2 gap-2'>");

            // Left side: Page size selector
            builder.Append("<div class='d-flex flex-column flex-md-row align-items-center gap-2'>");
            builder.Append("<div class='page-size-selector d-flex align-items-center gap-2'>");
            builder.Append("<label class='mb-0' style='display: inline-flex; align-items: center; white-space: nowrap;'>Page Size:</label>");

            // Get base URL without page parameter
            var baseUrl = generateUrl(1);
            var urlWithoutPage = baseUrl.Contains("&page=")
                ? baseUrl.Substring(0, baseUrl.IndexOf("&page="))
                : (baseUrl.Contains("?page=") ? baseUrl.Substring(0, baseUrl.IndexOf("?page=")) : baseUrl);

            builder.Append($"<select class='form-select form-select-sm' style='width: 70px;' onchange=\"changePageSize('{urlWithoutPage}', this.value)\">");

            foreach (var size in pageSizeOptions)
            {
                var selected = pagedList.PageSize == size ? "selected" : "";
                builder.Append($"<option value='{size}' {selected}>{size}</option>");
            }

            builder.Append("</select>");
            builder.Append("</div>");
            builder.Append("</div>");

            // Center: Page navigation
            builder.Append("<div class='pagination-nav'>");
            builder.Append("<ul class='pagination pagination-sm mb-0'>");

            // First page button
            if (pagedList.HasPreviousPage)
            {
                builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(1)}'>«</a></li>");
                builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(pagedList.PageNumber - 1)}'>‹</a></li>");
            }
            else
            {
                builder.Append("<li class='page-item disabled'><span class='page-link'>«</span></li>");
                builder.Append("<li class='page-item disabled'><span class='page-link'>‹</span></li>");
            }

            // Page numbers with smart range
            int startPage = Math.Max(1, pagedList.PageNumber - 2);
            int endPage = Math.Min(pagedList.PageCount, pagedList.PageNumber + 2);

            // Show first page if not in range
            if (startPage > 1)
            {
                builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(1)}'>1</a></li>");
                if (startPage > 2)
                {
                    builder.Append("<li class='page-item disabled'><span class='page-link'>...</span></li>");
                }
            }

            // Page number buttons
            for (int i = startPage; i <= endPage; i++)
            {
                if (i == pagedList.PageNumber)
                {
                    builder.Append($"<li class='page-item active'><span class='page-link'>{i}</span></li>");
                }
                else
                {
                    builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(i)}'>{i}</a></li>");
                }
            }

            // Show last page if not in range
            if (endPage < pagedList.PageCount)
            {
                if (endPage < pagedList.PageCount - 1)
                {
                    builder.Append("<li class='page-item disabled'><span class='page-link'>...</span></li>");
                }
                builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(pagedList.PageCount)}'>{pagedList.PageCount}</a></li>");
            }

            // Next and Last page buttons
            if (pagedList.HasNextPage)
            {
                builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(pagedList.PageNumber + 1)}'>›</a></li>");
                builder.Append($"<li class='page-item'><a class='page-link' href='{generateUrl(pagedList.PageCount)}'>»</a></li>");
            }
            else
            {
                builder.Append("<li class='page-item disabled'><span class='page-link'>›</span></li>");
                builder.Append("<li class='page-item disabled'><span class='page-link'>»</span></li>");
            }

            builder.Append("</ul>");
            builder.Append("</div>");

            // Right side: Go to page
            builder.Append("<div class='go-to-page d-flex align-items-center gap-2'>");
            builder.Append($"<input type='number' id='goToPageInput' class='form-control form-control-sm' style='width: 70px;' min='1' max='{pagedList.PageCount}' placeholder='Page' />");
            builder.Append($"<button class='btn btn-sm btn-primary' style='background:#E1136E;border:#E1136E;' onclick=\"goToPageDirect('{urlWithoutPage}', {pagedList.PageCount})\">Go</button>");
            builder.Append("</div>");

            builder.Append("</div>");

            // Add inline JavaScript
            builder.Append(@"
<script>
function changePageSize(baseUrl, pageSize) {
    var separator = baseUrl.includes('?') ? '&' : '?';
    window.location.href = baseUrl + separator + 'pageSize=' + pageSize + '&page=1';
}

function goToPageDirect(baseUrl, maxPages) {
    var pageNumber = parseInt(document.getElementById('goToPageInput').value);
    
    if (isNaN(pageNumber) || pageNumber < 1 || pageNumber > maxPages) {
        alert('Please enter a page number between 1 and ' + maxPages);
        document.getElementById('goToPageInput').value = '';
        return;
    }
    
    var separator = baseUrl.includes('?') ? '&' : '?';
    window.location.href = baseUrl + separator + 'page=' + pageNumber;
}

document.addEventListener('DOMContentLoaded', function() {
    var goToInput = document.getElementById('goToPageInput');
    if (goToInput) {
        goToInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                this.nextElementSibling.click();
            }
        });
    }
});
</script>");

            return new HtmlString(builder.ToString());
        }
    }
}