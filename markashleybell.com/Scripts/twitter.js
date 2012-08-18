function activeUrls(text) {
    text = text.replace(/\s(https?:\/\/[^\s\<]+)(\s)?/gi, ' <a href="$1">$1</a>$2');
    return text.replace(/(^|\s:?)@(\w+)/gi, ' <a href="http://twitter.com/$2">@$2</a>');
}

$(function () {

    var tweetContainer = $('#tweets > div')

    $.ajax({
        url: '/default/recenttwitterstatuses',
        dataType: 'json',
        type: 'post',
        data: {
            count: 5
        },
        success: function (data, status, request) {

            var output = new Array();

            $.each(data, function (i, item) {

                output.push('<p>' + item.date + ': ' + activeUrls(item.status) + '</p>');

            });

            tweetContainer.html(output.join(''));

        },
        error: function (request, status, error) {

            tweetContainer.html('<p>Error loading tweets.</p>');

        }
    });
});