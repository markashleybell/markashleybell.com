$(document).ready(function () {
    $('#commentform').submit(function () {
        var requiredFields = ['NewComment_AuthorName', 'NewComment_Email', 'NewComment_Body', 'z7sfd602nlwi'];
        var valid = true;
        $(this).find('span.validationmsg').remove();
        $(this).find('input').removeClass();
        for (var x = 0; x < requiredFields.length; x++) {
            var msg = '';
            var field = $(this).find('[id=' + requiredFields[x] + ']');
            if (field.val() == '') { valid = false; field.addClass('invalid'); msg = 'Please fill in this field' }
            else if (field.attr('id') == 'NewComment_Email' && !field.val().match(/^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$/gi)) {
                valid = false;
                field.addClass('invalid');
                msg = 'Invalid email address';
            }
            else if (field.attr('name') == 'z7sfd602nlwi' && isNaN(parseInt(field.val()))) {
                valid = false;
                field.addClass('invalid');
                msg = 'Hint: the answer is 16...';
            }
            if (msg != '') field.after('<span id="' + field.attr('name') + '-validationmsg" class="validationmsg">' + msg + '</span>')
        }
        return valid;
    });
});