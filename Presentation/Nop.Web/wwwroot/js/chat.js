"use strict";
var masterRowChat = "masterRowChat";
function CheckUnreadMessagesConversation() {
  $.ajax({
    type: "Get",
    url: "/Chat/GetUnreadMessagesConversation",
    success: function (response) {
      console.log(response);
      for (var i = 0; i < response.length; i++) {
        if (response[i].IsConnected) {
          if (response[i].HasUnreadMessages === true && response[i].PartnerId > 0) {
            StartChat(response[i].PartnerId);
          }
        }
      }

    }
  });
}

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.serverTimeoutInMilliseconds = 1000000; // 1 second

var senderId;
var containerMessages;
//Open bull discussion
function StartChat(destId, autoopen = false) {
  $.ajax({
    type: "Get",
    url: "/Chat/GetConversation/",
    data: { receiverId: destId },
    success: function (response) {
      ShowDivConversation(response.PartnerId, response.ConversationId, autoopen);

    }
  });



}
function ShowDivConversation(destId, conversationId, autoopen = false) {
  $.ajax({
    type: "Get",
    url: "/Chat/_Chat/",
    data: { partnerId: destId },
    success: function (response) {
      if ($('#divConversation_' + conversationId) === undefined || $('#divConversation_' + conversationId).html() === undefined) {
        $('#' + masterRowChat).append(response);
        $('#divConversation_' + conversationId).find('.chat-messages').empty();
        DisplayDivConversation(conversationId);
        LoadOldMessages(conversationId, destId);
        AddConverstaionToCookies(destId);
      }
      if (autoopen) {
        $('#divConversation_' + conversationId).find('.chatbox').removeClass('chatbox-min');
        scrollChatToBottom($('#divConversation_' + conversationId));
      }
      else {
        if ($('#divConversation_' + conversationId).find('.unreadInputMessages').val() === "true") {
          $('#divConversation_' + conversationId).find('.donot-disturb').removeClass('hidden');
        }
        else {
          $('#divConversation_' + conversationId).find('.donot-disturb').addClass('hidden');
        }

      }
    }
  });
}
function AddConverstaionToCookies(destId) {
  var currentConversations = getCookie("currentConversations");
  if (currentConversations === "") {
    currentConversations = [];
  }
  else {
    currentConversations = currentConversations.split(',');
  }
  if (jQuery.inArray(destId.toString(), currentConversations) === -1) {
    currentConversations.push(destId);
  }
  createCookie("currentConversations", currentConversations, 1);
  console.log(currentConversations);
}
function RemoveConversationFromCookies(destId) {
  var currentConversations = getCookie("currentConversations");
  if (currentConversations !== "") {
    currentConversations = currentConversations.split(',');
    if (jQuery.inArray(destId.toString(), currentConversations) !== -1) {
      currentConversations.splice(jQuery.inArray(destId.toString(), currentConversations), 1);
      createCookie("currentConversations", currentConversations, 1);
    }
  }
}
function DisplayLastConversations() {
  var currentConversations = getCookie("currentConversations");
  if (currentConversations !== "") {
    currentConversations = currentConversations.split(',');
  }
  if (currentConversations !== undefined) {
    for (var i = 0; i < currentConversations.length; i++) {
      StartChat(currentConversations[i]);
    }
  }
}
function DisplayDivConversation(conversationId) {
  $('#divConversation_' + conversationId).removeClass('hidden');
  ReorganizeDivConversation();
}
function ReorganizeDivConversation() {
  var marginRight = 0;
  $('#' + masterRowChat).find('.chatbox-holder').each(function () {
    $(this).css('margin-right', marginRight + '%');
    marginRight = marginRight + 30;
  });
}
//Start SignalR Connexion and load connectionId
connection.start().then(function () {
  //document.getElementById("sendButton").disabled = false;
  if (connection.connectionState !=='undefined' && connection.connectionState == "Connected") {
    GetConnectionId();
  }
  //CheckUnreadMessagesConversation();
  //DisplayLastConversations();

}).catch(function (err) {
  return console.error(err.toString());
});


function GetConnectionId() {
  connection.invoke('getconnectionid').then(
    (data) => {
      console.log(data);
      senderId = data;
    }
  );
}

//Send message to chatHub
function SendMessage(source) {

  var message = $(source).parent().find('.chat-input').val();
  connection.invoke("SendMessage", senderId, message, $(source).parent().find('.hiddenPartnerId').val().toString(), connection.connectionId).catch(function (err) {
    return console.error(err.toString());
  });
  $(source).parent().find('.chat-input').val('');
}


//Handle receive message
connection.on("ReceiveMessage", function (messageFromPartner, partnerName, conversationId, partnerId, message) {
  console.log("receive message");
  if ($('#divConversation_' + conversationId) !== undefined && $('#divConversation_' + conversationId).html() !== undefined) {
    ShowMessage(messageFromPartner, partnerName, conversationId, partnerId, message);
  }
  else {
    StartChat(partnerId);
  }
  scrollChatToBottom($('#divConversation_' + conversationId));
  $('#divConversation_' + conversationId).find('.unreadInputMessages').val('true');
  if ($('#divConversation_' + conversationId).find('.chatbox').hasClass('chatbox-min') === true) {
    $('#divConversation_' + conversationId).find('.donot-disturb').removeClass('hidden');
  }
  else {
    $('#divConversation_' + conversationId).find('.donot-disturb').addClass('hidden');
  }
});
function ShowMessage(messageFromPartner, partnerName, conversationId, partnerId, message) {
  if ($('#divConversation_' + conversationId).find('.hiddenPartnerId').val() !== partnerId || $('#divConversation_' + conversationId).hasClass('hidden') === true) {
    $('#divConversation_' + conversationId).find('.chat-messages').empty();
    LoadOldMessages(conversationId, null);
  }
  else {
    AppendContentMessage(messageFromPartner, partnerName, partnerId, message, conversationId);
  }
}
function LoadOldMessages(conversationId, receiverId) {
  $.ajax({
    type: "Get",
    url: "/Chat/GetOldMessages",
    data: { conversationId: conversationId, receiverId: receiverId },
    success: function (response) {
      console.log(response);
      for (var i = 0; i < response.length; i++) {
        AppendContentMessage(response[i].FromPartner, response[i].PartnerName, response[i].PartnerId, response[i].Message, conversationId);
      }
      scrollChatToBottom($('#divConversation_' + conversationId));
    }
  });
}
function AppendContentMessage(messageFromPartner, partnerName, partnerId, message, conversationId) {

  //DisplayDivConversation(conversationId);
  var divMessageBoxHolder = CreateDivMessageBox(messageFromPartner, partnerName, message);
  $('#divConversation_' + conversationId).find('.chat-messages').append(divMessageBoxHolder);
  $('#divConversation_' + conversationId).find('.hiddenPartnerId').val(partnerId);
  $('#divConversation_' + conversationId).find(".chat-partner-name").find('a').html(partnerName);

  if ($('#divConversation_' + conversationId).find('.chatbox').hasClass('chatbox-min')) {
    //$('#divConversation_' + conversationId).find('.chat-partner-name').addClass('backgroundBlueLight');
  }

}

//Create html div message
function CreateDivMessageBox(messageFromPartner, partnerName, message) {
  var encodedMsg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  var divMessageBoxHolder = document.createElement("div");
  divMessageBoxHolder.className = "message-box-holder";
  if (messageFromPartner) {
    var divMessageBoxSender = document.createElement("div");
    divMessageBoxSender.className = "message-sender";
    divMessageBoxSender.textContent = partnerName;
    var divMessageBoxPartner = document.createElement("div");
    divMessageBoxPartner.className = "message-box message-partner";
    divMessageBoxPartner.textContent = encodedMsg;
    divMessageBoxHolder.appendChild(divMessageBoxSender);
    divMessageBoxHolder.appendChild(divMessageBoxPartner);
  }
  else {
    var divMessageBox = document.createElement("div");
    divMessageBox.className = "message-box";
    divMessageBox.textContent = encodedMsg;
    divMessageBoxHolder.appendChild(divMessageBox);
  }
  return divMessageBoxHolder;
}

function scrollChatToBottom(source) {
  $(source).find('.chat-messages').scroll();
  $(source).find('.chat-messages').animate({
    scrollTop: $(source).find('.chat-messages')[0].scrollHeight
  }, "fast");
}

//Set read messages
function SetReadMessagesConversation(conversationId) {
  $.ajax({
    type: "Get",
    url: "/Chat/SetReadMessagesConversation",
    data: { conversationId: conversationId },
    success: function (response) {
      $('#divConversation_' + conversationId).find('.donot-disturb').addClass('hidden');

    }
  });
}