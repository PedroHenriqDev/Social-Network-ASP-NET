"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
$("#send").disabled = true;

connection.on("ReceiveMessage", function (userSender, message, userReceiver) {
    var userLogged = $("#user").val();

    if (userLogged == userReceiver || userReceiver == "all") {
        var msg = message.replace(/&/g, "&amo;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var li = $("<li><li>").text(userSender + ": " + msg);
        li.addClass("list-group-item");
        $("#messageList").append(li);
    }

    if (userLogged == userSender || userReceiver != "all") {
        var msg = message.replace(/&/g, "&amo;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var li = $("<li><li>").text(userSender + ": " + msg);
        li.addClass("list-group-item");
        li.addClass("whenSend");
        $("#messagesList").append(li);

    }
});

$("#send").on("click", function (event) {
    var message = $("#message").val();
    var userDestiny = $("#userDestiny").val();
    var connectionId = sessionStorage.getItem('conectionId');
    var userLogged = $("#user").val();

    connection.invoke("SendMessage", userLogged, message, userDestiny, connectionId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}

connection.start().then(function () {
    console.log("connected-SignalR");

    connection.invoke('getConnectionId').then(function ('connectionId');
}).catch(err => console.error(err.toString()));;
});

$(document).ready(function ()
{
    var quotes = new Array()
})


