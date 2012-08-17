$(function () {

    var githubContainer = $('#github > div')

    $.ajax({
        url: '/default/recentgithubactivity',
        dataType: 'json',
        type: 'post',
        data: {
            count: 10
        },
        success: function (data, status, request) {

            var output = new Array();

            $.each(data, function (i, item) {

                output.push('<p>' + item.date + ': ' + item.status + '</p>');

            });

            githubContainer.html(output.join(''));

        },
        error: function (request, status, error) {

            githubContainer.html('<p>Error loading activity.</p>');

        }
    });
});