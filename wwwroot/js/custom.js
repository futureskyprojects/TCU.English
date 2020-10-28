// Cấu hình checkbox group
$(document).ready(function () {
    $('input[type=checkbox]').click(function () {
        var groupName = $(this).attr('groupname');

        if (!groupName)
            return;

        var checked = $(this).is(':checked');

        $("input[groupname='" + groupName + "']:checked").each(function () {
            $(this).prop('checked', '');
            $(this).val(false);
        });

        if (checked) {
            $(this).prop('checked', 'checked');
            $(this).val(true);
        } else {
            $(this).val(false);
        }
    });
});