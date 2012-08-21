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

                output.push('<p><b>' + item.date + ':</b> ' + activeUrls(item.status) + '</p>');

            });

            tweetContainer.html(output.join(''));

        },
        error: function (request, status, error) {

            tweetContainer.html('<p>Error loading tweets.</p>');

        }
    });
});﻿$(function () {

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
});﻿$(function () {

    var e = $('#e');

    if (e.length) {

        var l = document.getElementById('m'), p = document.getElementById('e'), t = '', a = null;

        var v = '109#101#64#109#97#114#107#97#115#104#108#101#121#98#101#108#108#46#99#111#109'.split('#');

        for (var x = 0; x < v.length; x++)
            t += String.fromCharCode(parseInt(v[x], 10));

        e.empty().append('<a href="' + String.fromCharCode(109, 97, 105, 108, 116, 111, 58) + t + '">' + t + '</a>');
    }

});