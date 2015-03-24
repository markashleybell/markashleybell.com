$(function () {

    // Encode email address to try and limit spam...    
    var e = $('#e');
    if (e.length) {
        var l = document.getElementById('m'), p = document.getElementById('e'), t = '', a = null;
        var v = '109#101#64#109#97#114#107#98#46#99#111#46#117#107'.split('#');

        for (var x = 0; x < v.length; x++)
            t += String.fromCharCode(parseInt(v[x], 10));

        e.empty().append('<a class="email-link" href="' + String.fromCharCode(109, 97, 105, 108, 116, 111, 58) + t + '">' + t + '</a>');
    }

    // Toggle mobile navigation visibility
    var nav = $('#nav');
    $('#nav-button').on('click', function(e) { 
        nav.toggleClass('show'); 
    });

    var externalLinkQualifier = 'External Link: ';
    $('a').filter('[title^="' + externalLinkQualifier + '"]').on('click', function(e) {
        e.preventDefault();
        var link = $(this);
        var title = link.attr('title').substring(externalLinkQualifier.length);
        ga('send', {
            'hitType': 'event', 
            'eventCategory': 'External Link',
            'eventAction': 'Click', 
            'eventLabel': title,
            'hitCallback': function() {
                window.location = link.attr('href');
            }
        });
    });

});