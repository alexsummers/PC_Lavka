﻿CREATE TABLE [products]
(
	[id] INT IDENTITY(1,1) PRIMARY KEY,
	[name] VARCHAR(50) NOT NULL,
	[specification] VARCHAR(MAX) NOT NULL,
	[photo] IMAGE,
	[price] INT NOT NULL,
	[vendor_id] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[vendors]([id]),
	[category_id] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[categories]([id])
)