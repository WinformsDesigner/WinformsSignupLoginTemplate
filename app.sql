-- ============================================================================
-- CREATE TABLES
-- ============================================================================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(50) NOT NULL
);

CREATE TABLE UserSettings (
    SettingID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    SettingName NVARCHAR(50) NOT NULL,
    SettingValue BIT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- ============================================================================
-- SIGN UP STORED PROCEDURE
-- ============================================================================
CREATE PROCEDURE SignUpUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    INSERT INTO Users (Username, Password)
    VALUES (@Username, @Password);
END;

-- ============================================================================
-- LOG IN STORED PROCEDURE
-- ============================================================================
CREATE PROCEDURE LogInUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(50),
    @Result INT OUTPUT
AS
BEGIN
    SELECT @Result = UserID
    FROM Users
    WHERE Username = @Username AND Password = @Password;
    
    IF @Result IS NULL
        SET @Result = 0;
END;

-- ============================================================================
-- GET USER SETTINGS STORED PROCEDURE
-- ============================================================================
CREATE PROCEDURE GetUserSettings
    @UserID INT
AS
BEGIN
    SELECT SettingName, SettingValue
    FROM UserSettings
    WHERE UserID = @UserID;
END;

-- ============================================================================
-- SAVE USER SETTING STORED PROCEDURE
-- ============================================================================
CREATE PROCEDURE SaveUserSetting
    @UserID INT,
    @SettingName NVARCHAR(50),
    @SettingValue BIT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM UserSettings WHERE UserID = @UserID AND SettingName = @SettingName)
    BEGIN
        UPDATE UserSettings
        SET SettingValue = @SettingValue
        WHERE UserID = @UserID AND SettingName = @SettingName;
    END
    ELSE
    BEGIN
        INSERT INTO UserSettings (UserID, SettingName, SettingValue)
        VALUES (@UserID, @SettingName, @SettingValue);
    END
END;
