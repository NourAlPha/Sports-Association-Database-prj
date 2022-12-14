CREATE DATABASE Project;

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
Select f.username , s.password , f.name, f.national_id, f.birth_date, f.status
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
INSERT INTO Super_User VALUES(@user_name, @password)
INSERT INTO Association_Manager VALUES(@name, @user_name);
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
INSERT INTO Super_User VALUES(@representative_username, @password)
INSERT INTO Representative VALUES(@representative_username, @name, dbo.getClubID(@club_name))
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
------------------------------>>>>>>>>>>>>>>>>>>>>
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
INSERT INTO Super_User values(@username, @password)
INSERT INTO Manager values(@name, @username, dbo.getStadiumID(@stadium_name))
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
INSERT INTO Super_User VALUES(@username, @password)
INSERT INTO Fan VALUES(@national_id, @name, @birth_date, @address, @phone_num, 0, @username)
GO


CREATE FUNCTION getClubName(@id int)
RETURNS VARCHAR(20)
BEGIN 
DECLARE @res VARCHAR(20)
SELECT @res = name 
From Club
where id = @id
return @res
END
GO

CREATE FUNCTION getStadiumName(@id int)
RETURNS VARCHAR(20)
BEGIN 
DECLARE @res VARCHAR(20)
SELECT @res = name 
From Stadium
where id = @id
return @res
END
GO



CREATE FUNCTION upcomingMatchesOfClub(@clubName VARCHAR(20))
RETURNS @res Table(firstClub VARCHAR(20), secondClub VARCHAR(20), start datetime, place VARCHAR(20))
AS
BEGIN
declare @inputClubID int = dbo.getClubID(@clubName)
declare @temp Table(id int, startTime datetime, endingTime datetime, hostClub int, guestClub int, stadiumId int)

INSERT INTO @temp(id, startTime, endingTime, hostClub, guestClub, stadiumID) 
SELECT * FROM Match 
WHERE starting_time > current_TimeStamp AND (host_club = @inputClubID OR guest_club = @inputClubID)

INSERT INTO @res(firstClub, SecondClub, start, place)
SELECT dbo.getClubName(hostClub), dbo.getClubName(guestClub), startTime, dbo.getStadiumName(stadiumID)
FROM @temp

return
END
GO

Create FUNCTION availableMatchesToAttend(@date_time datetime)
RETURNS TABLE
AS
RETURN
(   
SELECT Distinct c1.name as first_Club, c2.name second_Club, m.starting_time, s.name stadium_Name
FROM Match m, Club c1, Club c2, Stadium s , Ticket t
WHERE t.match_id = m.id And t.status = 1 AND  m.host_club = c1.id AND m.guest_club = c2.id AND m.stadium_id = s.id AND m.starting_time > @date_time
)
GO

CREATE FUNCTION getTicketId(@match_id int)
RETURNS INT
BEGIN
DECLARE @id INT
SELECT TOP 1 @id = id
FROM Ticket 
WHERE match_id = @match_id AND status = 1
RETURN @id
END
Go

CREATE PROC purchaseTicket
@fan_national_id VARCHAR(20),
@host_club VARCHAR(20),
@guest_club VARCHAR(20),
@date_time datetime
AS
Insert into Ticket_Buying_Transactions values(@fan_national_id, dbo.getTicketId(dbo.getMatchID(@host_club, @guest_club, @date_time)))
UPDATE Ticket
SET status = 0
WHERE id = dbo.getTicketId(dbo.getMatchID(@host_club, @guest_club, @date_time))
GO


CREATE PROC updateMatchHost
@host_club VARCHAR(20),
@guest_club VARCHAR(20),
@date_time datetime
AS
UPDATE Match
SET host_club = dbo.getClubID(@guest_club) , guest_club = dbo.getClubID(@host_club)
WHERE id = dbo.getMatchID(@host_club, @guest_club, @date_time)
GO


-- Fetches all club names and the number of matches they have already played
CREATE view matchesPerTeam
AS
SELECT c.name, COUNT(m.id) AS matches
FROM Club c, Match m
WHERE m.starting_time < current_TimeStamp AND c.id = m.host_club OR c.id = m.guest_club 
GROUP BY c.name
GO

--Fetches pair of club names (first club name and second club name) which have never played against each other.
CREATE view clubsNeverMatched
AS
SELECT c1.name AS firstClub, c2.name AS secondClub
FROM Club c1, Club c2
WHERE c1.id <> c2.id AND NOT EXISTS 
(SELECT * FROM Match m WHERE (m.host_club = c1.id AND m.guest_club = c2.id) OR (m.host_club = c2.id AND m.guest_club = c1.id))
GO

--returns a table containing all club names which the given club has never competed against.
CREATE Function clubsNeverPlayed(@clubName VARCHAR(20))
RETURNS TABLE
AS
RETURN
(
SELECT c.name
FROM Club c
WHERE c.id <> dbo.getClubID(@clubName) AND NOT EXISTS
(SELECT * FROM Match m 
    WHERE (m.host_club = dbo.getClubID(@clubName) AND m.guest_club = c.id) OR (m.host_club = c.id AND m.guest_club = dbo.getClubID(@clubName)))
)
GO

--returns a table containing the name of the host club and the name of the guest club of the match which sold the highest number of tickets so far.
CREATE FUNCTION matchWithHighestAttendance()
RETURNS TABLE
AS
RETURN
(
SELECT c1.name AS hostClub, c2.name AS guestClub
FROM Match m, Club c1, Club c2
WHERE m.host_club = c1.id AND m.guest_club = c2.id AND m.id =
(SELECT TOP 1 t.match_id
FROM Ticket_Buying_Transactions tbt , Ticket t
where t.id = tbt.ticket_id
GROUP BY t.match_id
ORDER BY COUNT(t.match_id) DESC)
)
GO


--returns a table containing the name of the host club and the name of the guest club of all played matches sorted descendingly by the total number of tickets they have sold.
CREATE FUNCTION matchesRankedByAttendance()
RETURNS @res Table(hostClub VARCHAR(20), guestClub VARCHAR(20), ticketsSold int)
AS
BEGIN
INSERT INTO @res(hostClub , guestClub , ticketsSold)
SELECT c1.name AS hostClub, c2.name AS guestClub, COUNT(t.match_id) AS ticketsSold
FROM Match m, Club c1, Club c2, Ticket_Buying_Transactions tbt , Ticket t
WHERE m.host_club = c1.id AND m.guest_club = c2.id AND m.id = t.match_id AND t.id = tbt.ticket_id
GROUP BY c1.name, c2.name
ORDER BY COUNT(t.match_id) DESC
RETURN
END
GO




--returns a table containing the name of the host club and the name of the guest club of all matches that are requested to be hosted on the given stadium sent by the representative ofthe given club.
Create Function requestsFromClub(@clubName VARCHAR(20), @stadiumName VARCHAR(20))
RETURNS TABLE
AS
RETURN
(
SELECT dbo.getClubName(m.host_club) AS hostClub, dbo.getClubName(m.guest_club) AS guestClub
FROM Match m,  Host_Request h
WHERE h.representative_ID = dbo.getRepresentativeID(@clubName) AND h.manager_id = dbo.getManagerID(@stadiumName) AND h.match_id = m.id
)
GO

Create PROC deleteAllProcs
AS
BEGIN
declare @procName varchar(500)
declare cur cursor 

for select [name] from sys.objects where type = 'p'
open cur
fetch next from cur into @procName
while @@fetch_status = 0
begin
    exec('drop procedure [' + @procName + ']')
    fetch next from cur into @procName
end
close cur
deallocate cur
END
GO

Create Proc deleteAllFunctions
AS
BEGIN
Declare @sql NVARCHAR(MAX) = N'';

SELECT @sql = @sql + N' DROP FUNCTION ' 
                   + QUOTENAME(SCHEMA_NAME(schema_id)) 
                   + N'.' + QUOTENAME(name)
FROM sys.objects
WHERE type_desc LIKE '%FUNCTION%';
Exec sp_executesql @sql
GO
END
GO

Create Proc deleteAllViews
AS
BEGIN
DECLARE @sql VARCHAR(MAX) = ''
        , @crlf VARCHAR(2) = CHAR(13) + CHAR(10) ;

SELECT @sql = @sql + 'DROP VIEW ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(v.name) +';' + @crlf
FROM   sys.views v
END
GO

Create Proc dropAllProceduresFunctionsViews
AS
BEGIN
EXEC deleteAllProcs
EXEC deleteAllFunctions
EXEC deleteAllViews
END
GO