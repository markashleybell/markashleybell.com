$(function () {
    
    var l = document.getElementById('m'), p = document.getElementById('e'), t = '', a = null;
    
    var v = '109#101#64#109#97#114#107#97#115#104#108#101#121#98#101#108#108#46#99#111#109'.split('#');

    for (var x = 0; x < v.length; x++)
        t += String.fromCharCode(parseInt(v[x], 10));

    a = document.createElement('a');

    a.href = String.fromCharCode(109, 97, 105, 108, 116, 111, 58) + t;

    var n = document.createTextNode(t);

    a.appendChild(n);

    p.replaceChild(a, p.firstChild);

});