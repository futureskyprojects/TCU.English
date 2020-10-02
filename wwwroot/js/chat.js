"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (discussionId, message) {
    var crrID = document.getElementById("senderId").value;

    var disId = document.getElementById("discussId").value;
    if (disId != discussionId)
        return;

    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "<div class=\"d-flex justify-content-between w-75 mb-4\">" +
        `<div class=\"card bg-${senderId == crrID ? 'blue' : 'gray-500'} p-3 text-dark\">` +
        `<p class=\"mb-0\" style=\"font-family: Tahoma\">${message}</p>` +
        "<div class=\"pt-2\">" +
        `<small class=\"float-${senderId == crrID ? 'left' : 'right'} bg-white p-1 rounded\">${1}</small>` +
        "</div>" +
        "</div>" +
        "</div>";
    var li = document.createElement("div");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);

    scroll();
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var discussionId = document.getElementById("discussId").value;

    var senderId = document.getElementById("senderId").value;

    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", discussionId, message, senderId).catch(function (err) {
        return console.error(err.toString());
    });
    scroll();
    event.preventDefault();
});

function scroll() {
    $("#messages").animate({ scrollTop: $('#messages').prop("scrollHeight") }, 500);
}