CREATE DATABASE Project2;

GO
CREATE PROC createAllTables
AS 
create table Super_User(
    username varchar(20) primary key,
    password varchar(20)
);

create table System_Admin(
	id int PRIMARY KEY,
	name varchar(20),
    username varchar(20) Foreign KEY references Super_User
);

create table Association_Manager(
    id int primary key identity,
	name varchar(20),
    username varchar(20) references Super_User,
);


CREATE TABLE Club(
id INT PRIMARY KEY IDENTITY,
name VARCHAR(20),
location VARCHAR(20)
);

create table Representative(
    id int primary key identity,
	username varchar(20) Foreign KEY references Super_User,
	name varchar(20),
	club_id int Foreign KEY references Club
);

CREATE TABLE Stadium(
id INT PRIMARY KEY,
name VARCHAR(20),
capacity INT ,
location VARCHAR(20),
status bit,
);

create table Manager(
    id int primary key identity,
    name varchar(20),
    username varchar(20) Foreign KEY references Super_User ,
	stadium_id int Foreign KEY references Stadium
);

CREATE TABLE Match(
id INT PRIMARY KEY IDENTITY,
starting_time date,
ending_time date,
host_club INT references Club,
guest_club INT references Club,
stadium_id INT references Stadium
);

Create Table Stadium_Manager(
id int primary key identity,
name varchar(20),
username varchar(20) Foreign KEY references Super_User,
stadium_id int Foreign KEY references Stadium
);

CREATE TABLE Fan(
national_id VARCHAR(20) PRIMARY KEY,
name varchar(20),
birth_date datetime,
address varchar(20),
phone_number varchar(20),
status bit,
username varchar(20) Foreign KEY references Super_User
);



CREATE TABLE Ticket(
id int PRIMARY KEY IDENTITY,
status varchar(20),
match_id int references Match,
);

CREATE TABLE Ticket_Buying_Transactions(
fan_id VARCHAR(20) Foreign KEY references Fan,
ticket_id int Foreign KEY references Ticket
);

CREATE TABLE Host_Request(
id int PRIMARY KEY IDENTITY,
representative_id int Foreign KEY references Representative,
manager_id int Foreign KEY references Manager,
match_id int Foreign KEY references Match,
status bit default NULL
);
GO

DROP PROC createAllTables;
EXEC createAllTables;
GO


CREATE PROCEDURE dropAllTables 
AS
DROP TABLE Host_Request;
DROP TABLE Ticket_Buying_Transactions;
DROP TABLE Ticket;
DROP TABLE Fan;
DROP TABLE Match;
DROP TABLE Stadium_Manager;
DROP TABLE Representative;
DROP TABLE Club;
DROP TABLE Association_Manager;
DROP TABLE System_Admin;
DROP TABLE Manager;
DROP TABLE Super_User;
DROP TABLE Stadium;
GO

DROP PROC dropAllTables;
EXEC dropAllTables;

GO

CREATE PROC clearAllTables
AS
EXEC dropAllTables;
EXEC createAllTables;
GO

DROP PROC clearAllTables

EXEC clearAllTables;

Go
CREATE VIEW allAssocManagers AS
Select a.username, a.name , s.password
From Association_Manager a INNER JOIN Super_User s ON s.username = a.username;
Go


CREATE VIEW allClubRepresentatives AS
Select r.username, r.name as Representative_Name , c.name as Club_Name , s.password
From Representative r , Club c , Super_User s
where c.id = r.id AND s.username = r.username;
Go


CREATE VIEW allStadiumManagers AS
Select m.username , su.password , m.name, s.name as Stadium_Name
From Manager m , Stadium s , Super_User su
where s.id = m.stadium_id AND m.username = su.username;
GO

CREATE VIEW allFans AS
Select f.username , s.password , f.name, f.id, f.birth_date, f.status
From Fan f , Super_User s 
WHERE f.username = s.username;
GO

CREATE VIEW allMatches AS 
SELECT c1.name as Host_Club , c2.name as Guest_Club , m.starting_time
FROM Club c1 , Club c2 , Match m 
where c1.id = m.host_club AND c2.id = m.guest_club;
GO

CREATE VIEW allTickets AS
SELECT c1.name as Host_Club , c2.name as Guest_Club , s.name as Stadium_Name , m.starting_time
FROM Club c1 , Club c2 , Stadium s , Match m , Ticket t
where m.id = t.match_id AND c1.id = m.host_club AND c2.id = m.guest_club AND m.stadium_id = s.id;

GO

CREATE VIEW allClubs AS
Select name, location
From Club;
GO

create view allStadiums as
Select name, location, capacity, status 
From Stadium;
GO

CREATE VIEW allRequests AS
SELECT r.username AS Representative_Username , m.username As Manager_Username , h.status
From Representative r , Manager m , Host_Request h
where r.id = h.representative_id AND m.id = h.manager_id;

GO

----------------------ALL_Tickets and All_Matches --------------------------------------------


CREATE PROC addAssociationManager 
@name VARCHAR(20),
@user_name VARCHAR(20),
@password VARCHAR(20)
AS
INSERT INTO Association_Manager VALUES(@name, @user_name);
INSERT INTO Super_User VALUES(@user_name, @password)
GO

CREATE FUNCTION getClubID (@name varchar(20))
RETURNS INT
BEGIN
DECLARE @ret int
SELECT @ret = c.id 
FROM Club c
WHERE c.name = @name
RETURN @ret
END
GO

CREATE PROC addNewMatch 
@host_name VARCHAR(20),
@guest_name VARCHAR(20),
@start_time datetime,
@end_time datetime
AS
INSERT INTO Match (starting_time, ending_time, host_club, guest_club) 
VALUES(@start_time , @end_time, dbo.getClubID(@host_name), dbo.getClubID(@guest_name)); 
GO

CREATE VIEW clubsWithNoMatches AS
SELECT c.name
FROM Club c
WHERE c.id NOT IN(select host_club from match) and c.id not in (select guest_club from match);
GO

CREATE PROC deleteMatch
@host_club VARCHAR(20) , @guest_club VARCHAR(20)
AS
delete from Match
where host_club = dbo.getClubID(@host_club) AND guest_club = dbo.getClubID(@guest_club)
GO


CREATE PROC deleteMatchesOnStadium
@stadium_name VARCHAR(20)
As
delete from Match 
where stadium_id in 
(Select id from Stadium where
name = @stadium_name) and starting_time > CURRENT_TIMESTAMP;
GO

CREATE PROC addClub
@name VARCHAR(20) , @location VARCHAR(20)
AS
INSERT INTO Club(name , location) Values(@name , @location);
GO

CREATE FUNCTION getMatchID(@hname varchar(20) , @gname varchar(20), @date datetime) 
RETURNS INT
BEGIN
DECLARE @res int
SELECT @res = m.id
FROM Match m, Club c1, Club c2
WHERE c1.name = @hname AND C2.name = @gname AND c1.id = m.host_club AND c2.id = m.guest_club AND m.starting_time = @date
RETURN @res
END
GO


CREATE PROC addTicket
@host_club VARCHAR(20) , @guest_club VARCHAR(20) , @date_time datetime
AS
INSERT INTO Ticket (match_id) VALUES (dbo.getMatchID(@host_club, @guest_club, @date_time));
go

CREATE PROC deleteClub
@name VARCHAR(20)
AS
delete from CLub
where club.name = @name;
GO

CREATE PROC addStadium
@name VARCHAR(20) , @location VARCHAR(20) , @capacity INT
AS
INSERT INTO Stadium(name , location , capacity) values(@name , @location , @capacity);
GO

CREATE PROC deleteStadium
@name VARCHAR(20)
AS
delete from Stadium
where Stadium.name = @name;
GO

CREATE PROC blockFan 
@national_id VARCHAR(20)
AS
UPDATE Fan
SET status = 1 WHERE @national_id = national_id;
GO

CREATE PROC unblockFan 
@national_id VARCHAR(20)
AS
UPDATE Fan
SET status = 0 WHERE @national_id = national_id;
GO

CREATE PROC addRepresentative
@name VARCHAR(20),
@club_name VARCHAR(20),
@representative_username VARCHAR(20),
@password VARCHAR(20)
AS
INSERT INTO Representative VALUES(@representative_username, @name, dbo.getClubID(@club_name))
INSERT INTO Super_User VALUES(@representative_username, @password)
GO

CREATE FUNCTION viewAvailableStadiumsOn
(@date datetime)
RETURNS TABLE
AS
RETURN
(
SELECT s.name, s.location, s.capacity
FROM Stadium s
WHERE s.id NOT IN
(SELECT m.stadium_id
FROM Match m
WHERE m.starting_time = @date)
)
GO

CREATE FUNCTION getMatchID1(@name varchar(20), @date datetime) 
RETURNS INT
BEGIN
DECLARE @res int
SELECT @res = m.id
FROM Match m, Club c1
WHERE c1.name = @name AND c1.id = m.host_club AND m.starting_time = @date
RETURN @res
END
GO

CREATE FUNCTION getManagerID(@name VARCHAR(20))
RETURNS INT
BEGIN
DECLARE @res int
SELECT @res = m.id
FROM Manager m, Stadium s
WHERE s.name = @name AND m.stadium_id = s.id
RETURN @res
END
GO

CREATE FUNCTION getRepresentativeID(@club_name VARCHAR(20))
RETURNS INT
BEGIN
DECLARE @res int
SELECT @res = r.id
FROM Representative r, Club c
WHERE c.name = @club_name AND r.club_id = c.id
RETURN @res
END
GO

CREATE PROC addHostRequest
@club_name VARCHAR(20),
@stadium_name VARCHAR(20),
@date_time datetime
AS
INSERT INTO Host_Request VALUES(dbo.getRepresentativeID(@club_name), dbo.getManagerID(@stadium_name), dbo.getMatchID1(@club_name, @date_time), NULL)
GO

CREATE FUNCTION allUnassignedMatches(@club_name VARCHAR(20))
RETURNS TABLE
AS
RETURN
(
SELECT c2.name, m.starting_time
FROM Match m, Club c1, Club c2
WHERE m.host_club = c1.id AND m.guest_club = c2.id AND m.stadium_id IS NULL AND c1.name = @club_name
)
GO

CREATE FUNCTION getStadiumID(@stadium_name VARCHAR(20))
RETURNS INT
BEGIN
DECLARE @id INT
SELECT @id = id
FROM Stadium 
WHERE name = @stadium_name
RETURN @id
END
GO


CREATE PROC addStadiumManager
@name VARCHAR(20),
@stadium_name VARCHAR(20),
@username VARCHAR(20),
@password VARCHAR(20)
AS
INSERT INTO Manager values(@name, @username, dbo.getStadiumID(@stadium_name))
INSERT INTO Super_User values(@username, @password)
GO

CREATE FUNCTION getManagerID2(@username VARCHAR(20))
RETURNS INT
BEGIN
DECLARE @id INT
SELECT @id = id
FROM Manager 
WHERE username = @username
RETURN @id
END
GO

CREATE FUNCTION allPendingRequests (@username VARCHAR(20))
RETURNS TABLE 
AS
RETURN
(
SELECT R.name AS Rep_name, C.name AS Club_name, M.starting_time
FROM Representative R , Club C, Match M, Host_Request H
WHERE H.representative_id = R.id AND H.manager_id = dbo.getManagerID2(@username) AND H.match_id = M.id AND C.id = M.guest_club AND R.club_id = M.guest_club
) 
GO

CREATE PROC acceptRequest
@managerUserName VARCHAR(20),
@hostClub VARCHAR(20),
@guestClub VARCHAR(20),
@startTime datetime
AS
update host_request
set status = 1
where manager_id = dbo.getManagerID2(@managerUserName) and representative_ID = dbo.getRepresentativeID(@hostClub) and match_ID = dbo.getMatchID(@hostClub, @guestClub, @startTime)
GO

CREATE PROC rejectRequest
@managerUserName VARCHAR(20),
@hostClub VARCHAR(20),
@guestClub VARCHAR(20),
@startTime datetime
AS
update host_request
set status = 0
where manager_id = dbo.getManagerID2(@managerUserName) and representative_ID = dbo.getRepresentativeID(@hostClub) and match_ID = dbo.getMatchID(@hostClub, @guestClub, @startTime)
GO

CREATE PROC addFan 
@name VARCHAR(20),
@username VARCHAR(20),
@password VARCHAR(20),
@national_id VARCHAR(20),
@birth_date datetime,
@phone_num VARCHAR(20),
@address VARCHAR(20)
AS
INSERT INTO Fan VALUES(@national_id, @name, @birth_date, @address, @phone_num, 0, @username)
INSERT INTO Super_User VALUES(@username, @password)
GO

CREATE FUNCTION upcomingMatchesOfClub(@club_name varchar(20))
RETURNS TABLE 
AS
RETURN
(
SELECT c1.name as Club_Name , c2.name as Competing_Club , m.starting_time , s.name as Stadium_ID
FROM Club C1, Club C2, Match M, Stadium S
WHERE M.starting_time > CURRENT_TIMESTAMP AND C1.id = dbo.getClubID(@club_name) AND M.stadium_id = S.id AND 
)
GO

CREATE FUNCTION availableMatchesToAttend(@datetime date    
