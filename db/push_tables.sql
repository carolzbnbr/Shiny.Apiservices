// SQL SERVER
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'PushRegistrations' and xtype = 'U')
    CREATE TABLE [dbo].[PushRegistrations](
        [Platform] [varchar](10) NOT NULL PRIMARY KEY,
        [DeviceToken] [varchar](50) NOT NULL PRIMARY KEY,
        [UserId] [varchar](50) NULL,
        [Tags] [varchar](2000) NULL
    )
GO

// POSTGRES
CREATE TABLE IF NOT EXISTS PushRegistrations (
    Platform varchar(10) NOT NULL,
    DeviceToken varchar(50) NOT NULL,
    UserId varchar(50) NULL,
    Tags varchar(2000) NULL,
    PRIMARY KEY(Platform, DeviceToken)
);


// SQLITE - use for testing
CREATE TABLE IF NOT EXISTS PushRegistrations (
    Platform    TEXT NOT NULL,
    DeviceToken TEXT NOT NULL,
    UserId      TEXT,
    Tags        TEXT,
    PRIMARY KEY (Platform, DeviceToken)
);