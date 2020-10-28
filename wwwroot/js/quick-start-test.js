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
    var current = Math.floor(Date.now() / 1000);
    var old = parseInt(getCookieValue("quick-test").replace("=", ""));

    if (current - old > 24 * 60 * 60 - 1) {
        $("#quick-test").modal("show");
    }
}