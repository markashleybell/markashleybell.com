$(document).ready(function () {
    $('#commentform').submit(function () {

        var requiredFields = ['Comment_AuthorName', 'Comment_Email', 'Comment_Body', 'z7sfd602nlwi'];
        var valid = true;
        $(this).find('.field-validation-error').remove();
        $(this).find('input, textarea').removeClass('input-validation-error');
        for (var x = 0; x < requiredFields.length; x++) {
            var msg = '';
            var field = $(this).find('[id=' + requiredFields[x] + ']');
            if (field.val() == '') { valid = false; field.addClass('invalid'); msg = 'Please fill in this field' }
            else if (field.attr('id') == 'Comment_Email' && !field.val().match(/^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$/gi)) {
                valid = false;
                msg = 'Invalid email address';
            }
            else if (field.attr('name') == 'z7sfd602nlwi' && isNaN(parseInt(field.val()))) {
                valid = false;
                msg = 'Hint: the answer is 16...';
            }
            if (msg != '') field.addClass('input-validation-error').after('<span id="' + field.attr('name') + '-validationmsg" class="field-validation-error">' + msg + '</span>')
        }
        return valid;
    });
});