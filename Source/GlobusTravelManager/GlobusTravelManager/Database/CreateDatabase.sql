-- Создание базы данных GlobusTravelDB
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'GlobeTravelDB')
    DROP DATABASE GlobusTravelDB;
GO

CREATE DATABASE GlobusTravelDB;
GO

USE GlobusTravelDB;
GO

-- 1. Страны
CREATE TABLE Countries (
    CountryID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName NVARCHAR(100) NOT NULL
);

-- 2. Типы автобусов
CREATE TABLE BusTypes (
    BusTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Capacity INT NOT NULL
);

-- 3. Пользователи
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Role NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL
);

-- 4. Туры
CREATE TABLE Tours (
    TourID INT IDENTITY(1,1) PRIMARY KEY,
    TourName NVARCHAR(200) NOT NULL,
    CountryID INT FOREIGN KEY REFERENCES Countries(CountryID),
    DurationDays INT NOT NULL,
    StartDate DATE NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    BusTypeID INT FOREIGN KEY REFERENCES BusTypes(BusTypeID),
    Capacity INT NOT NULL,
    AvailableSeats INT NOT NULL,
    PhotoFileName NVARCHAR(200),
    CHECK (AvailableSeats >= 0 AND AvailableSeats <= Capacity)
);

-- 5. Заявки
CREATE TABLE Bookings (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    TourID INT FOREIGN KEY REFERENCES Tours(TourID),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    BookingDate DATE DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,
    PeopleCount INT NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    Comment NVARCHAR(500),
    CHECK (PeopleCount > 0)
);

-- Индексы
CREATE INDEX IX_Tours_StartDate ON Tours(StartDate);
CREATE INDEX IX_Bookings_Status ON Bookings(Status);
GO

PRINT '✅ База данных создана успешно!';
GO
