"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

function generateMessage(discussionId, messageJsonObject) {
    // Lấy người dùng hiện tại
    var crrID = document.getElementById("senderId").value;

    // Lấy mã cuộc hội thoại hiện tại
    var disId = document.getElementById("discussId").value;

    // Nếu mã không giống mã tin nhắn, bỏ qua
    if (disId != discussionId)
        return;

    var encodedMsg = `<div class=\"d-flex justify-content-between w-75 mb-4 float-${messageJsonObject.SenderId != crrID ? 'left' : 'right'}\">` +
        `<div class=\"card w-100 bg-${messageJsonObject.SenderId == crrID ? 'blue' : 'gray-500'} p-3 text-${messageJsonObject.SenderId == crrID ? 'white' : 'dark'}\">` +
        `<p class=\"mb-0\" style=\"font-family: Tahoma\">${messageJsonObject.Message}</p>` +
        "<div class=\"pt-2\">" +
        `<small class=\"float-${messageJsonObject.SenderId != crrID ? 'left' : 'right'} bg-white p-1 rounded text-dark\">${messageJsonObject.Time}</small>` +
        "</div>" +
        "</div>" +
        "</div>";
    var li = document.createElement("div");
    li.innerHTML = encodedMsg;
    document.getElementById("messagesList").appendChild(li);

    scroll();
}

connection.on("ReceiveMessage", function (discussionId, message) {
    try {
        message = JSON.parse(message);
    } catch (e) {

    }

    generateMessage(discussionId, message);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var discussionId = $("#discussId").val();

    var senderId = $("#senderId").val();

    var message = $("#messageInput").val();

    if (message.length <= 0)
        return;

    $("#messageInput").val("");

    var message = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

    connection.invoke("SendMessage", discussionId.toString(), JSON.stringify({ SenderId: senderId, Message: message })).catch(function (err) {
        return console.error(err.toString());
    });
    scroll();
    event.preventDefault();
});

function scroll() {
    $("#messages").animate({ scrollTop: $('#messages').prop("scrollHeight") }, 500);
}