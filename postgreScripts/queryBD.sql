-- Добавление записей в таблицу Города
INSERT INTO City (Id, nameCity, population) VALUES
(1, 'Москва', 12615882),
(2, 'Санкт-Петербург', 5384342),
(3, 'Новосибирск', 1620162);

-- Добавление записей в таблицу Районы
INSERT INTO District (Id, nameDistrict, id_city) VALUES
(1, 'Центральный', 1),
(2, 'Восточный', 1),
(3, 'Калининский', 2),
(4, 'Адмиралтейский', 2),
(5, 'Заельцовский', 3);

-- Добавление записей в таблицу Улицы
INSERT INTO Street (Id, nameStreet, sizeStreet, id_district) VALUES
(1, 'Тверская', 1.5, 1),
(2, 'Арбат', 1.2, 1),
(3, 'Невский проспект', 4.5, 3),
(4, 'Малая Морская', 0.8, 4),
(5, 'Красный проспект', 6.5, 5);

-- Добавление записей в таблицу Дома
INSERT INTO House (Id, numberHouse, floorHouse, id_street) VALUES
(1,1, 10, 1),
(2, 2, 5, 2),
(3, 5, 12, 3),
(4, 7, 6, 4),
(5, 10, 25, 5);

-- Добавление записей в таблицу жители
INSERT INTO Resident (Id, fullName, birthday, passportNumber, passportSeries, house_id) VALUES
(1, 'Иванов Иван Иванович', '1985-06-15', '123456', '1111', 1),
(2, 'Петров Петр Петрович', '1990-08-20', '654321', '2222', 2),
(3, 'Сидорова Мария Павловна', '1995-03-10', '345678', '3333', 3),
(4, 'Кузнецова Елена Викторовна', '1988-12-01', '987654', '4444', 4),
(5, 'Смирнов Алексей Александрович', '2000-11-25', '456789', '5555', 5);

-- Добавление записей в таблицу Прописка
INSERT INTO Registration (Id, dateRegistration, dataDeregistration, id_resident, id_house) VALUES
(1, '2020-01-15', NULL, 1, 1),
(2, '2019-07-10', '2023-01-01', 2, 2),
(3, '2021-04-20', NULL, 3, 3),
(4, '2018-09-05', '2022-12-31', 4, 4),
('2022-05-15', NULL, 5, 5);

CREATE TABLE City (
    Id SERIAL PRIMARY KEY,
    nameCity VARCHAR(40) UNIQUE NOT NULL,
    population INTEGER
);

CREATE TABLE District (
    Id SERIAL PRIMARY KEY,
    nameDistrict VARCHAR(100) UNIQUE NOT NULL,
    id_city INTEGER NOT NULL REFERENCES City(Id) ON DELETE CASCADE
);

CREATE TABLE Street (
    Id SERIAL PRIMARY KEY,
    nameStreet VARCHAR(100) UNIQUE NOT NULL,
    sizeStreet FLOAT NOT NULL,
    id_district INTEGER NOT NULL REFERENCES District(Id) ON DELETE CASCADE
);

CREATE TABLE House (
    Id SERIAL PRIMARY KEY,
    numberHouse INTEGER NOT NULL UNIQUE,
    floorHouse INTEGER NOT NULL CHECK(floorHouse > 0 AND floorHouse <= 100),
    id_street INTEGER NOT NULL REFERENCES Street(Id) ON DELETE CASCADE
);

CREATE TABLE Resident (
    Id SERIAL PRIMARY KEY,
    fullName TEXT UNIQUE NOT NULL,
    birthday DATE NOT NULL,
    passportNumber VARCHAR(6) UNIQUE NOT NULL,
    passportSeries VARCHAR(4) UNIQUE NOT NULL,
    house_id INTEGER REFERENCES House(Id) ON DELETE SET NULL
);

CREATE TABLE Registration (
    Id SERIAL PRIMARY KEY,
    dateRegistration DATE,
    dataDeregistration DATE,
    id_resident INTEGER NOT NULL REFERENCES Resident(Id) ON DELETE CASCADE,
    id_house INTEGER NOT NULL REFERENCES House(Id) ON DELETE CASCADE
);


CREATE TABLE City (
    Id INTEGER PRIMARY KEY,
    nameCity VARCHAR(40) UNIQUE NOT NULL,
    population INTEGER
);

CREATE TABLE District (
    Id INTEGER PRIMARY KEY,
    nameDistrict VARCHAR(100) UNIQUE NOT NULL,
    id_city INTEGER NOT NULL REFERENCES City(Id) ON DELETE CASCADE
);

CREATE TABLE Street (
    Id INTEGER PRIMARY KEY,
    nameStreet VARCHAR(100) UNIQUE NOT NULL,
    sizeStreet FLOAT NOT NULL,
    id_district INTEGER NOT NULL REFERENCES District(Id) ON DELETE CASCADE
);

CREATE TABLE House (
    Id INTEGER PRIMARY KEY,
    numberHouse INTEGER NOT NULL UNIQUE,
    floorHouse INTEGER NOT NULL CHECK(floorHouse > 0 AND floorHouse <= 100),
    id_street INTEGER NOT NULL REFERENCES Street(Id) ON DELETE CASCADE
);

CREATE TABLE Resident (
    Id INTEGER PRIMARY KEY,
    fullName TEXT UNIQUE NOT NULL,
    birthday DATE NOT NULL,
    passportNumber VARCHAR(6) UNIQUE NOT NULL,
    passportSeries VARCHAR(4) UNIQUE NOT NULL,
    house_id INTEGER REFERENCES House(Id) ON DELETE SET NULL
);

CREATE TABLE Registration (
    Id INTEGER PRIMARY KEY,
    dateRegistration DATE,
    dataDeregistration DATE,
    id_resident INTEGER NOT NULL REFERENCES Resident(Id) ON DELETE CASCADE,
    id_house INTEGER NOT NULL REFERENCES House(Id) ON DELETE CASCADE
);

DROP TABLE City;
DROP TABLE District;
DROP TABLE Street;
DROP TABLE House;
DROP TABLE Resident;
DROP TABLE Registration;

