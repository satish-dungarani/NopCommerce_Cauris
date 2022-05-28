CREATE TABLE `cauristwtdemo`.`conversation` (
ID INT  AUTO_INCREMENT PRIMARY KEY,
FirtartChattSenderId INT,
SecondSenderId INT,
CreationDate Datetime,
LastMessageDate Datetime
);
CREATE TABLE `cauristwtdemo`.`conversationmessage` (
ID INT  AUTO_INCREMENT PRIMARY KEY,
ConversationId INT,
SenderId INT,
ReceiverId INT,
IsRead BIT,
CreationDate Datetime,
Text LONGTEXT,
 FOREIGN KEY (ConversationId)
        REFERENCES Conversation (ConversationId)
        ON UPDATE RESTRICT ON DELETE CASCADE,
 FOREIGN KEY (SenderId)
        REFERENCES Customer (Id)
        ON UPDATE RESTRICT ON DELETE CASCADE,
 FOREIGN KEY (ReceiverId)
        REFERENCES Customer (Id)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `cauristwtdemo`.`CustomerConnection` (
Id INT  AUTO_INCREMENT PRIMARY KEY,
ConnectionId NVARCHAR(400),
CustomerId INT,
FOREIGN KEY (CustomerId)
        REFERENCES Customer (Id)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

