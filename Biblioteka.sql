CREATE DATABASE Skolska_biblioteka
USE Skolska_biblioteka

CREATE TABLE Knjizevna_vrsta(
id INT PRIMARY KEY IDENTITY(1,1),
naziv VARCHAR(30)
)

CREATE TABLE Autor(
id INT PRIMARY KEY IDENTITY(1,1),
ime VARCHAR(15),
prezime VARCHAR(15),
godina_rodjenja VARCHAR(4),
drzava_porekla VARCHAR(30)
)

CREATE TABLE Izdavac(
id INT PRIMARY KEY IDENTITY(1,1),
naziv VARCHAR(50),
adresa VARCHAR(30),
veb_sajt VARCHAR(50)
)

CREATE TABLE Knjiga(
id INT PRIMARY KEY IDENTITY(1,1),
naziv VARCHAR(30),
id_autora INT REFERENCES Autor(id),
id_izdavaca INT REFERENCES Izdavac(id),
id_vrste INT REFERENCES Knjizevna_vrsta(id)
)

CREATE TABLE Autor_knjiga(
id INT PRIMARY KEY IDENTITY(1,1),
id_knjige INT REFERENCES Knjiga(id),
id_autora INT REFERENCES Autor(id)
)

CREATE TABLE Primerak(
id INT PRIMARY KEY IDENTITY(1,1),
id_knjige INT REFERENCES Knjiga(id),
polica INT,
broj INT,
slobodna BIT
)

CREATE TABLE Ucenik(
id INT PRIMARY KEY IDENTITY(1,1),
ime VARCHAR(20),
prezime VARCHAR(20),
JMBG VARCHAR(13),
email VARCHAR(50),
godina_pocetka VARCHAR(7),
odeljenje VARCHAR(6)
)

CREATE TABLE Zaposleni(
id INT PRIMARY KEY IDENTITY(1,1),
ime VARCHAR(20),
prezime VARCHAR(20),
datum_zaposlenja DATE,
JMBG VARCHAR(13),
email VARCHAR(50),
plata INT,
lozinka INT
)

CREATE TABLE Pozajmica(
id INT PRIMARY KEY IDENTITY(1,1),
datum_uzimanja DATE,
datum_vracanja DATE,
id_zaposlenog INT REFERENCES Zaposleni(id),
id_primerka INT REFERENCES Primerak(id),
id_ucenika INT REFERENCES Ucenik(id)
)

-- Insert za tabelu Autor
INSERT INTO Autor (ime, prezime, godina_rodjenja, drzava_porekla)
VALUES ('Ivo', 'Andric', '1892', 'Bosna i Hercegovina'),
('Mesa Selimovic', 'Selimovic', '1910', 'Bosna i Hercegovina'),
('J.R.R.', 'Tolkien', '1892', 'Velika Britanija'),
('Herman', 'Hesse', '1877', 'Nemačka'),
('Dan', 'Brown', '1964', 'SAD');

-- Insert za tabelu Knjizevna_vrsta
INSERT INTO Knjizevna_vrsta (naziv)
VALUES ('Drama'), ('Roman'), ('Poezija'), ('Fantazija'), ('Triler');

-- Insert za tabelu Izdavac
INSERT INTO Izdavac (naziv, adresa, veb_sajt)
VALUES ('Laguna', 'Kneza Milosa 24, Beograd', 'www.laguna.rs'),
('Vulkan', 'Terazije 27, Beograd', 'www.vulkani.rs'),
('Prometej', 'Kralja Petra 23, Novi Sad', 'www.prometej.rs');

-- Insert za tabelu Knjiga
INSERT INTO Knjiga (naziv, id_autora, id_izdavaca, id_vrste)
VALUES ('Prokleta avlija', 1, 1, 2),
('Na Drini cuprija', 1, 2, 2),
('Dervis i smrt', 2, 3, 2),
('Gospodar prstenova', 3, 1, 4),
('Siddharta', 4, 1, 2),
('Da Vincijev kod', 5, 2, 5);

-- Insert za tabelu Autor_knjiga
INSERT INTO Autor_knjiga (id_knjige, id_autora)
VALUES (1, 1), (2, 1), (3, 2), (4, 3), (5, 4), (6, 5);

-- Insert za tabelu Primerak
INSERT INTO Primerak (id_knjige, polica, broj, slobodna)
VALUES (2, 2, 1, 1);
INSERT INTO Primerak (id_knjige, polica, broj, slobodna)
VALUES (2, 1, 1, 1);

-- Insert za tabelu Ucenik
INSERT INTO Ucenik (ime, prezime, JMBG, email, godina_pocetka, odeljenje)
VALUES ('Marko', 'Markovic', '0101001000101', 'marko.markovic@skola.edu.rs', '2019/20', '10')
INSERT INTO Ucenik
VALUES ('Jovana', 'Jovanovic', '0202002000202', 'jovana.jovanovic@skola.edu.rs', '2018/19', '9')
INSERT INTO Ucenik
VALUES('Nikola', 'Nikolic', '0303003000303', 'nikola.nikolic@skola.edu.rs', '2022/23', '2');

-- Insert za tabelu Zaposleni
INSERT INTO Zaposleni (ime, prezime, datum_zaposlenja, JMBG, email, plata, Lozinka)
VALUES
('Maja', 'Markovic', '2010-01-01', '0404004004000', 'maja.markovic@example.com', 50000, 123),
('Nikola', 'Nikolic', '2015-01-01', '0505005005000', 'nikola.nikolic@example.com', 60000, 123),
('Marija', 'Maric', '2020-01-01', '0606006006000', 'marija.maric@example.com', 70000, 123)

SELECT * FROM Knjizevna_vrsta
SELECT * FROM Autor
SELECT * FROM Izdavac
SELECT * FROM Knjiga
SELECT * FROM Autor_knjiga
SELECT * FROM Primerak
SELECT * FROM Ucenik
SELECT * FROM Zaposleni
SELECT * FROM Pozajmica


-- Brisanje iz tabele Pozajmica
GO
CREATE TRIGGER trg_pozajmica_delete
ON Pozajmica
AFTER DELETE
AS
BEGIN
	DECLARE @datum_vracanja DATE
	IF (@datum_vracanja = NULL)
	BEGIN
		UPDATE Primerak
		SET slobodna = 1
		FROM Primerak
		JOIN deleted ON Primerak.id = deleted.id_primerka
	END

  DELETE FROM Pozajmica
  WHERE id IN (SELECT id FROM deleted WHERE datum_vracanja IS NULL);
END


-- Unos u tabelu Primerak
GO
CREATE TRIGGER trg_primerak_insert
ON Primerak
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @polica INT, @broj INT
    SELECT @polica = [polica], @broj = [broj] FROM inserted

    IF EXISTS (SELECT 1 FROM Primerak WHERE polica = @polica AND broj = @broj)
    BEGIN
        RAISERROR('Polica je zauzeta!', 16, 1)
        ROLLBACK TRANSACTION
        RETURN
    END

	IF EXISTS (SELECT * FROM inserted WHERE inserted.slobodna = 0)
    BEGIN
        RAISERROR('Knjiga prilikom unosa mora biti slobodna!', 16, 1)
        ROLLBACK TRANSACTION
        RETURN
    END

    INSERT INTO Primerak (id_knjige, polica, broj, slobodna)
    SELECT id_knjige, @polica, @broj, slobodna FROM inserted
END


-- Unos u tabelu Ucenik
GO
CREATE TRIGGER trg_ucenik_insert
ON Ucenik
FOR INSERT
AS
DECLARE @godina_pocetka VARCHAR(7), @odeljenje varchar(6)
SET @godina_pocetka = (SELECT godina_pocetka FROM inserted)
SET @odeljenje = (SELECT odeljenje FROM inserted)
IF (LEFT(@godina_pocetka,4) > YEAR(GETDATE()) OR (LEFT(@godina_pocetka,4) = YEAR(GETDATE()) AND MONTH(GETDATE()) < '09'))
BEGIN
  RAISERROR('Ne mozete uneti ucenika jer skolska godina nije pocela!', 16, 1)
  ROLLBACK TRANSACTION
  RETURN
END
IF (CAST(SUBSTRING(@godina_pocetka, 3, 2) AS INT) != (CAST(RIGHT(@godina_pocetka, 2) AS INT) - 1))
BEGIN
	RAISERROR('Uneta skolska godina nije validna!', 16, 1)
  ROLLBACK TRANSACTION
  RETURN
END


-- Update tabele Ucenik
GO
CREATE TRIGGER trg_ucenik_update
ON Ucenik
FOR UPDATE
AS
DECLARE @godina_pocetka VARCHAR(7), @odeljenje varchar(6)
SET @odeljenje = (SELECT odeljenje FROM inserted)
SET @godina_pocetka = (SELECT godina_pocetka FROM inserted)
IF (LEFT(@godina_pocetka,4) > YEAR(GETDATE()) OR (LEFT(@godina_pocetka,4) = YEAR(GETDATE()) AND MONTH(GETDATE()) < '09') OR (CAST(SUBSTRING(@godina_pocetka, 3, 2) AS INT) != (CAST(RIGHT(@godina_pocetka, 2) AS INT) - 1)))
BEGIN
  RAISERROR('Skolska godina nije pocela!', 16, 1)
  ROLLBACK TRANSACTION
END


 -- Unos u tabelu pozajmica Pozajmicu
GO
CREATE TRIGGER trg_pozajmica_insert
ON Pozajmica
INSTEAD OF INSERT
AS
BEGIN
    --IF EXISTS (SELECT * FROM inserted WHERE inserted.datum_vracanja IS NOT NULL)
    --BEGIN
        --RAISERROR ('Datum vraćanja se ne može uneti prilikom pozajmice!', 16, 1)
        --ROLLBACK TRANSACTION
      --RETURN
    --END

	IF EXISTS (SELECT * FROM inserted WHERE inserted.datum_uzimanja > GETDATE())
	BEGIN
		RAISERROR ('Datum uzimanja ne može biti veći od trenutnog datuma!', 16, 1)
		ROLLBACK TRANSACTION
		RETURN
	END

    IF EXISTS (SELECT * FROM inserted i JOIN Primerak p ON i.id_primerka = p.id
        WHERE p.slobodna = 0
    )
    BEGIN
        RAISERROR ('Primerak nije slobodan i ne može se pozajmiti!', 16, 1)
        ROLLBACK TRANSACTION
        RETURN
    END

    INSERT INTO Pozajmica (datum_uzimanja, id_zaposlenog, id_primerka, id_ucenika)
    SELECT datum_uzimanja, id_zaposlenog, id_primerka, id_ucenika
    FROM inserted

	UPDATE Primerak
    SET slobodna = 0
    WHERE id IN (SELECT id_primerka FROM inserted)
END


-- Izmena u tabeli Pozajmica sa datumom vracanja
GO
CREATE TRIGGER trg_pozajmica_update
ON Pozajmica
AFTER UPDATE
AS
BEGIN
	DECLARE @datum_vracanja DATE, @datum_uzimanja DATE
	SELECT @datum_vracanja = (SELECT datum_vracanja FROM inserted), @datum_uzimanja = (SELECT datum_uzimanja FROM inserted)

	IF (@datum_vracanja < @datum_uzimanja)
	BEGIN
		RAISERROR ('Datum vracanja ne moze biti manji od datuma uzimanja!', 16, 1)
        ROLLBACK TRANSACTION
        RETURN
	END

	IF (@datum_vracanja > GETDATE())
	BEGIN
		RAISERROR ('Datum vracanja ne moze biti veci od trenutnog datuma!', 16, 1)
        ROLLBACK TRANSACTION
        RETURN
	END

    UPDATE Primerak
    SET slobodna = 1
    FROM Primerak
    JOIN inserted ON Primerak.id = inserted.id_primerka
    WHERE inserted.datum_vracanja IS NOT NULL
END


-- Prikaz svih slobodnih knjiga
GO
CREATE PROCEDURE prikazi_slobodne_knjige
AS
BEGIN
  SELECT Primerak.id AS 'Id primerka', Knjiga.naziv AS 'Naziv knjige', Autor.ime + ' ' + Autor.prezime AS 'Autor', Knjizevna_vrsta.naziv AS 'Knjizevna vrsta', polica, broj FROM Primerak 
  JOIN Knjiga ON Knjiga.id = Primerak.id
  JOIN Knjizevna_vrsta ON Knjiga.id_vrste = Knjizevna_vrsta.id
  JOIN Autor ON Knjiga.id_autora = Autor.id 
  WHERE slobodna = 1
END
	


-- Prikaz svih slobodnih knjiga po trazenoj vrsti
GO
CREATE PROCEDURE prikazi_slobodne_knjige_po_vrsti
    @knjizevna_vrsta VARCHAR(50)
AS
BEGIN
	SELECT Primerak.id AS 'Id primerka', Knjiga.naziv AS 'Naziv knjige', Autor.ime + ' ' + Autor.prezime AS 'Autor', Knjizevna_vrsta.naziv AS 'Knjizevna vrsta', polica, broj FROM Primerak 
	JOIN Knjiga ON Knjiga.id = Primerak.id_knjige
	JOIN Knjizevna_vrsta ON Knjiga.id_vrste = Knjizevna_vrsta.id
	JOIN Autor ON Knjiga.id_autora = Autor.id 
	WHERE slobodna = 1 AND Knjizevna_vrsta.naziv = @knjizevna_vrsta
END

-- Prikaz svih slobodnih knjiga po nazivu
GO
CREATE PROCEDURE fn_slobodne_knjige_po_nazivu
    @naziv_knjige NVARCHAR(50)
AS
BEGIN
	SELECT Primerak.id AS 'Id primerka', Knjiga.naziv AS 'Naziv knjige', Autor.ime + ' ' + Autor.prezime AS 'Autor', Knjizevna_vrsta.naziv AS 'Knjizevna vrsta', polica, broj FROM Primerak 
	JOIN Knjiga ON Knjiga.id = Primerak.id_knjige
	JOIN Knjizevna_vrsta ON Knjiga.id_vrste = Knjizevna_vrsta.id
	JOIN Autor ON Knjiga.id_autora = Autor.id 
	WHERE slobodna = 1 AND Knjiga.naziv = @naziv_knjige
END

-- Prikaz svih slobodnih knjiga po autoru
GO
CREATE PROCEDURE prikazi_slobodne_knjige_po_autoru
    @autor NVARCHAR(50)
AS
BEGIN
	SELECT Primerak.id AS 'Id primerka', Knjiga.naziv AS 'Naziv knjige', Autor.ime + ' ' + Autor.prezime AS 'Autor', Knjizevna_vrsta.naziv AS 'Knjizevna vrsta', polica, broj FROM Primerak 
	JOIN Knjiga ON Knjiga.id = Primerak.id_knjige
	JOIN Knjizevna_vrsta ON Knjiga.id_vrste = Knjizevna_vrsta.id
	JOIN Autor ON Knjiga.id_autora = Autor.id 
	WHERE slobodna = 1 AND (Autor.ime + ' ' + Autor.prezime)= @autor
END