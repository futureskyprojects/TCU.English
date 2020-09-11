// Cấu hình checkbox group
$(document).ready(function () {
    $('input[type=checkbox]').click(function () {
        var groupName = $(this).attr('groupname');

        if (!groupName)
            return;

        console.log(`${groupName} đã được chọn!`);

        var checked = $(this).is(':checked');

        console.log(checked);

        $("input[groupname='" + groupName + "']:checked").each(function () {
            console.log(`${$(this).attr('id')} >> Đang chuyển sang fail`);
            $(this).prop('checked', '');
            $(this).val(false);
        });

        if (checked) {
            console.log(`${$(checked).attr('id')} Đang chuyển sang true`);
            $(this).prop('checked', 'checked');
            $(this).val(true);
        } else {
            console.log(`${$(checked).attr('id')} Đang chuyển sang fail`);
            $(this).val(false);
        }
    });
});