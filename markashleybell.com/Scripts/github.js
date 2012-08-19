$(function () {

    var githubContainer = $('#github > div')

    $.ajax({
        url: '/default/recentgithubactivity',
        dataType: 'json',
        type: 'post',
        data: {
            count: 5
        },
        success: function (data, status, request) {

            var output = new Array();

            $.each(data, function (i, item) {

                output.push('<p><b>' + item.date + ':</b> ' + item.status + '</p>');

            });

            githubContainer.html(output.join(''));

        },
        error: function (request, status, error) {

            githubContainer.html('<p>Error loading activity.</p>');

        }
    });
});