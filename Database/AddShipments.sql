/*
    Role 4 - Shipping database setup
    Run this script against the eStoreJA_DB database AFTER the existing eStoreJA database
    schema has been created. It is safe to run more than once.
*/
USE [eStoreJA_DB];
GO

IF OBJECT_ID(N'dbo.Shipments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Shipments
    (
        ShipmentId          INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Shipments PRIMARY KEY,
        OrderId             INT NOT NULL,
        CustomerName        NVARCHAR(100) NOT NULL,
        ProductName         NVARCHAR(200) NOT NULL,
        Quantity            INT NOT NULL,
        AddressLine1        NVARCHAR(100) NOT NULL,
        AddressLine2        NVARCHAR(100) NULL,
        City                NVARCHAR(50) NOT NULL,
        State               NVARCHAR(50) NOT NULL,
        Country             NVARCHAR(50) NOT NULL,
        ZipCode             NVARCHAR(20) NOT NULL,
        TrackingNumber      VARCHAR(30) NOT NULL,
        Status              NVARCHAR(30) NOT NULL
            CONSTRAINT DF_Shipments_Status DEFAULT ('Processing'),
        EstimatedDelivery   DATETIME2 NOT NULL,
        ActualDelivery      DATETIME2 NULL,
        CreatedAtUtc        DATETIME2 NOT NULL
            CONSTRAINT DF_Shipments_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT UQ_Shipments_OrderId UNIQUE (OrderId),
        CONSTRAINT UQ_Shipments_TrackingNumber UNIQUE (TrackingNumber),
        CONSTRAINT CK_Shipments_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_Shipments_Status CHECK
            (Status IN ('Processing', 'Shipped', 'In Transit', 'Out for Delivery', 'Delivered')),
        CONSTRAINT FK_Shipments_Orders FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders(OrderId)
    );

    CREATE INDEX IX_Shipments_Status ON dbo.Shipments(Status);
END;
GO
