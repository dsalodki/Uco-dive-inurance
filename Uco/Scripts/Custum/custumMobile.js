/// <reference path="../jquery-1.4.4-vsdoc.js" />
/// <reference path="../jquery.validate-vsdoc.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />
/// <reference path="../jquery.defaultvalue/jquery.defaultvalue.js" />

$.validator.unobtrusive.adapters.addBool("mandatory", "required");
jQuery.validator.methods["date"] = function (value, element) { return true; } 

$(document).ready(function () {
    $("input.submit_ajax_button").removeAttr("disabled");


});

function shareFacebook(title, url) {
    var url = "http://www.facebook.com/sharer/sharer.php?s=100&p[url]=http://" + window.location.hostname + url + "&p[title]=" + title
    window.open(url, 'Share', 'toolbar=0,status=yes,resizable=yes,status=0,width=626,height=436');
    return false;
}

function shareTwitter(title, url) {
    var url = "http://twitter.com/home?status=" + title + "%20http://" + window.location.hostname + url;
    window.open(url, 'Share', 'toolbar=0,status=yes,resizable=yes,width=626,height=436');
    return false;
}

