insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('account.fields.usertype','User Type',1);
insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('account.fields.usertype.vendor','vendor',1);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('account.fields.usertype.seller','Seller',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('account.usertype','Are you ?',1);


INSERT INTO setting(name,value,storeId) VALUES('blacklisttext','',0);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Products.Quotation.Request','Request a quotation',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Products.Quotation.Button','Send a request quotation',1);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.LeadTime','Lead time',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.RequestQuotation.RequestQuotationAlreadySended','A request quotation already sended',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.New','New request quotation',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('LeadTime','Lead time',1);


INSERT INTO fees(Percent,Label, description) VALUES(5,'Fees Causris','Fees Cauris');
INSERT INTO fees(Percent,Label, description) VALUES(10,'Fess Banque','Fees banque');

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Success','Quote request sent successfully',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Send','Send',1);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Country','Country',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.PriceHT','Price excl tax',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.UnitPrice','Unit Price',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.PriceTTC','Price incl tax',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Quantity','Quantity',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Received','Quote requests received',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Request.Received','Quote  received',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Sended','Quote requests sent',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Request.Sended','Requests sent',1);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Product','Product',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Customer','Customer',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.Status','Status',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.LeadTime','Lead time',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quotation.Request.NewLeadTime','New lead time',1);

INSERT INTO setting(name,value,storeId) VALUES('blacklisttext','',0);



insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Response.Received','Response received',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Response.Sended','Response sended',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Refused','Refused',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Accepted','Accepted',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.BeingProcessed','Being Processed',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.WaitingVendorResponse','Waiting vendor response',1);


insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Request.ChangePriceTTC','Change total price',1);



insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Accept','Accept',1);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Refuse','Refuse',1);


INSERT INTO MESSAGETEMPLATE(Name,Subject,EmailAccountId, Body,IsActive,DelayPeriodId,AttachedDownloadId,LimitedToStores) 
Values ('Quote.Request.CustomerNotification','%Store.Name%. New request Quote',1,  N'<p><a href="%Store.URL%">%Store.Name%</a><br /><br />Order #%Order.OrderNumber% has been just refunded<br /><br />Amount refunded: %Order.AmountRefunded%<br /><br />Date Ordered: %Order.CreatedOn%</p>',1,0,0,0);
INSERT INTO MESSAGETEMPLATE(Name,Subject,EmailAccountId, Body,IsActive,DelayPeriodId,AttachedDownloadId,LimitedToStores) Values ('Quote.Request.VendorNotification','%Store.Name%. New request Quote',1, '',1,0,0,0);
INSERT INTO MESSAGETEMPLATE(Name,Subject,EmailAccountId, Body,IsActive,DelayPeriodId,AttachedDownloadId,LimitedToStores) Values ('Quote.Response.CustomerNotification','%Store.Name%. Vendor response Quote',1, '',1,0,0,0);
INSERT INTO MESSAGETEMPLATE(Name,Subject,EmailAccountId, Body,IsActive,DelayPeriodId,AttachedDownloadId,LimitedToStores) Values ('Quote.Accept.CustomerNotification','%Store.Name%. Accept Quote',1, '',1,0,0,0);
INSERT INTO MESSAGETEMPLATE(Name,Subject,EmailAccountId, Body,IsActive,DelayPeriodId,AttachedDownloadId,LimitedToStores) Values ('Quote.Accept.VendorNotification','%Store.Name%. Customer Accept Quote',1, '',1,0,0,0);

insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Moderator.Refused','Refused by moderator',1);
insert into localestringresource(ResourceName,ResourceValue,LanguageId) VALUES ('Quote.Moderator.Refused','Refuser par moderateur',2);
