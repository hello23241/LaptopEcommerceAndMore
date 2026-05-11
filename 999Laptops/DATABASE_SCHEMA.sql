-- SQL Database Schema for E-Commerce System

-- Account Table
CREATE TABLE [dbo].[Account](
	[id] [int] PRIMARY KEY IDENTITY(1,1),
	[User_name] [varchar](20) NOT NULL UNIQUE,
	[full_name] [nvarchar](100) NOT NULL,
	[password] [varchar](150) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[phone] [varchar](50) NOT NULL,
	[birthday] [datetime] NULL,
	[status] [int] NOT NULL DEFAULT 1,
	[notes] [nvarchar](150) NULL,
	[created_date] [datetime] NOT NULL DEFAULT GETDATE()
)

-- Category Table
CREATE TABLE [dbo].[Category](
	[id] [int] PRIMARY KEY IDENTITY(1,1),
	[name] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NULL
)

-- Supplier Table
CREATE TABLE [dbo].[Supplier](
	[id] [int] PRIMARY KEY IDENTITY(1,1),
	[name] [nvarchar](100) NOT NULL,
	[contact_info] [varchar](100) NULL,
	[address] [nvarchar](200) NULL
)

-- Product Table
CREATE TABLE [dbo].[Product](
	[id] [int] PRIMARY KEY IDENTITY(1,1),
	[name] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NULL,
	[price] [decimal](10,2) NOT NULL,
	[quantity] [int] NOT NULL DEFAULT 0,
	[category_id] [int] NOT NULL,
	[supplier_id] [int] NOT NULL,
	[created_date] [datetime] NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY ([category_id]) REFERENCES [Category]([id]),
	FOREIGN KEY ([supplier_id]) REFERENCES [Supplier]([id])
)

-- Order Table
CREATE TABLE [dbo].[Order](
	[id] [int] PRIMARY KEY IDENTITY(1,1),
	[account_id] [int] NOT NULL,
	[order_date] [datetime] NOT NULL DEFAULT GETDATE(),
	[total_amount] [decimal](10,2) NOT NULL,
	[status] [nvarchar](50) NOT NULL DEFAULT 'Pending',
	FOREIGN KEY ([account_id]) REFERENCES [Account]([id])
)

-- OrderDetail Table
CREATE TABLE [dbo].[OrderDetail](
	[id] [int] PRIMARY KEY IDENTITY(1,1),
	[order_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[unit_price] [decimal](10,2) NOT NULL,
	[subtotal] [decimal](10,2) NOT NULL,
	FOREIGN KEY ([order_id]) REFERENCES [Order]([id]),
	FOREIGN KEY ([product_id]) REFERENCES [Product]([id])
)

-- Sample Data
INSERT INTO [Category] VALUES ('Electronics', 'Electronic devices');
INSERT INTO [Category] VALUES ('Books', 'Books and publications');
INSERT INTO [Category] VALUES ('Clothing', 'Clothing items');

INSERT INTO [Supplier] VALUES ('Supplier A', 'contact@suppliera.com', '123 Street A');
INSERT INTO [Supplier] VALUES ('Supplier B', 'contact@supplierb.com', '456 Street B');
INSERT INTO [Supplier] VALUES ('Supplier C', 'contact@supplierc.com', '789 Street C');

INSERT INTO [Product] VALUES ('Laptop', 'High-performance laptop', 1200.00, 10, 1, 1, GETDATE());
INSERT INTO [Product] VALUES ('C# Programming', 'Learn C# from basics', 35.00, 50, 2, 2, GETDATE());
INSERT INTO [Product] VALUES ('T-Shirt', 'Comfortable cotton t-shirt', 25.00, 100, 3, 3, GETDATE());

INSERT INTO [Account] VALUES ('admin', 'Administrator', 'admin123', 'admin@example.com', '0123456789', NULL, 1, NULL, GETDATE());
INSERT INTO [Account] VALUES ('user1', 'John Doe', 'user123', 'user1@example.com', '0987654321', '1990-05-15', 1, NULL, GETDATE());
INSERT INTO [Account] VALUES ('user2', 'Jane Smith', 'user456', 'user2@example.com', '0918273645', '1992-03-22', 1, NULL, GETDATE());

-- Create Indexes for better performance
CREATE INDEX IX_Product_CategoryId ON [Product]([category_id]);
CREATE INDEX IX_Product_SupplierId ON [Product]([supplier_id]);
CREATE INDEX IX_Order_AccountId ON [Order]([account_id]);
CREATE INDEX IX_OrderDetail_OrderId ON [OrderDetail]([order_id]);
CREATE INDEX IX_OrderDetail_ProductId ON [OrderDetail]([product_id]);
CREATE INDEX IX_Account_UserName ON [Account]([User_name]);
