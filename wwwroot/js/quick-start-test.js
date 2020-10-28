function getCookieValue(name) {
    const nameString = name + "="

    const value = document.cookie.split(";").filter(item => {
        return item.includes(nameString)
    })

    if (value.length) {
        return value[0].substring(nameString.length, value[0].length)
    } else {
        return ""
    }
}

function updateCookie(name, value) {
    document.cookie = `${name}=${value}`;
}

if (getCookieValue("quick-test") == undefined || getCookieValue("quick-test") == null || getCookieValue("quick-test").length <= 0) {
    // Thực hiện bài test
    $("#quick-test").modal("show");
} else {
    alert("Đã có");
}