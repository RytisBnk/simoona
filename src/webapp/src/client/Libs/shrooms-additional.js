$(document).on('click', '.nc-table-active tbody tr', function(e) {
    var target = $(e.target);

    if (!target.is('a')) {
        $('.nc-table-active tbody tr').removeClass('active');
        $(this).addClass('active');
    }
});

$(document).ready(function() {
    let themeId = localStorage.getItem("settings.theme.id");
    if (themeId === "theme-dark") {
        $('body').addClass("theme-dark");
    }
});