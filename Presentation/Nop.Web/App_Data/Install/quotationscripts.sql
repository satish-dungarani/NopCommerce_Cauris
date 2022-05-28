CREATE TABLE Quotation(
Id INT  AUTO_INCREMENT PRIMARY KEY,
CustomerId INT,
VendorId INT,
ProductId INT,
Quantity FLOAT,
CountryId INT,
CreationDate DATETIME,
Status INT,
StatusDate DATETIME,
Specification LONGTEXT,
LeadTime DATETIME,
UnitPrice decimal(18, 4),
Price decimal(18, 4),
FOREIGN KEY (CustomerId)
        REFERENCES Customer (Id)
        ON UPDATE RESTRICT ON DELETE CASCADE,
FOREIGN KEY (VendorId)
        REFERENCES Vendor (Id)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE Fees(
Id INT  AUTO_INCREMENT PRIMARY KEY,
Percent FLOAT,
Label NVARCHAR(255),
Description NVARCHAR(255)
)
