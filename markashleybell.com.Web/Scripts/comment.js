function mvcId(id) {
    return id.replace(/\./gi, '_');
}

// Show validation errors for all fields which failed server-side validation,
// plus any model-level validation errors
function showValidationMessages(form, jsonValidator) {

    $('.field-validation-error, .model-validation-error').hide();
    form.find('.input-validation-error').removeClass('input-validation-error');
    $('.model-validation-error').empty();

    $.each(jsonValidator, function (i, item) {

        if (item.field == '') {

            // If the field is blank, this is a model-level error
            // form.find('.model-validation-error').append('<p>' + item.Message + '</p>').show();

        }
        else {

            var id = '#' + mvcId(item.field);
            var input = form.find(id);
            var p = input.closest('p');

            if (!$('.field-validation-error', p).length) {
                p.append('<span class="field-validation-error"' + ((item.field == 'Comment.z7sfd602nlwi') ? ' id="z7sfd602nlwi-validationmsg"' : '') + '></span>');
            }

            input.addClass('input-validation-error');
            $('.field-validation-error', p).html(item.error[0].message).show();

        }

    });

}

$(function () {

    $('#commentform').bind('submit', function() {

        var f = $(this);

        $.ajax({
            url: '/article/validatecomment',
            type: 'POST',
            data: f.serialize(),
            error: function (request, status, error) { },
            success: function (data, status, request) {
                if (data.length == 0) // If nothing is returned by the validator
                {
                    // We have to unbind first otherwise we have an infinite loop of validation!
                    f.unbind('submit');
                    f.submit();
                }
                else {
                    // console.log(f, data);

                    showValidationMessages(f, data);
                }
            }
        });

        return false;

    });

});