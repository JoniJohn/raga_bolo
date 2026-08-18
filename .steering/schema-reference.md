# DATABASE SCHEMA REFERENCE: RAGA BOLO TOURNAMENT API

This document serves as the target blueprint for Entity Framework Core entity configurations. 

> **STRICT RULE:** > Do NOT build EF Core migrations or DbContext sets for all tables at once. 
> Only implement and map the specific tables/entities required for the current single-endpoint task.

---

## 1. Schema Namespaces & Modules

- **User Module (`usr`)**: User profiles and permissions.
- **Tournament Module (`tour`)**: Tournament setup, committees, and scheduling boundaries.
- **Participant Module (`part`)**: Teams, coaches, and players.
- **Match Operations Module (`ops`)**: Fixtures, venues, and live match events.
- **Configuration Module (`conf`)**: System lookup tables and enums.

---

## 2. Table Definitions

### User Module (`usr`)
- **`usr.User`**
  - `Id` (BigInt, PK)
  - `Name` (string(255), Required)
  - `AuthId` (Guid, Nullable) — External Identity Provider identifier

- **`usr.UserGroup`**
  - `Id` (BigInt, PK)
  - `Name` (string(200), Required)
  - `OwnerId` (BigInt, FK -> `usr.User.Id`)

- **`usr.UserGroupMember`**
  - `UserGroupId` (BigInt, FK -> `usr.UserGroup.Id`, Composite PK)
  - `UserId` (BigInt, FK -> `usr.User.Id`, Composite PK)

---

### Configuration Module (`conf`)
- **`conf.TournamentCupType`**
  - `Id` (BigInt, PK)
  - `Code` (string(10), Unique) e.g., `SL`, `HAD`, `SK`, `HAK`
  - `Name` (string(255)) e.g., "Single Fixture League", "Home & Away League"
  - `Description` (string(500))

- **`conf.MatchStatus`**
  - `Id` (BigInt, PK)
  - `Code` (string(50), Unique) e.g., `SCHEDULED`, `STARTED`, `ENDED`, `POSTPONED`, `CANCELLED`
  - `Name` (string(200))
  - `Description` (string(500))

- **`conf.FixtureEventType`**
  - `Id` (BigInt, PK)
  - `Code` (string(50), Unique) e.g., `GOAL`, `ASSIST`, `YELLOW_CARD`, `SECOND_YELLOW`, `RED_CARD`
  - `Name` (string(100))
  - `Description` (string(500))
  - `Icon` (string, Nullable)

- **`conf.OfficiatingPersonnelType`**
  - `Id` (BigInt, PK)
  - `Name` (string(250)) e.g., "Referee", "Assistant Referee", "Match Commissioner"
  - `RoleDescription` (string(500))

---

### Tournament Module (`tour`)
- **`tour.Tournament`**
  - `Id` (BigInt, PK)
  - `Name` (string, Required)
  - `RefNumber` (string, Unique, Required)
  - `Description` (string)
  - `LogoUrl` (string, Nullable)
  - `OwnerId` (BigInt, FK -> `usr.User.Id`)
  - `TournamentCupTypeId` (int, FK -> `conf.TournamentCupType.Id`)

- **`tour.TournamentCommittee`**
  - `TournamentId` (BigInt, FK -> `tour.Tournament.Id`, Composite PK)
  - `UserId` (BigInt, FK -> `usr.User.Id`, Composite PK)

- **`tour.TournamentSchedule`**
  - `Id` (BigInt, PK)
  - `TournamentId` (BigInt, FK -> `tour.Tournament.Id`)
  - `StartDate` (DateTimeOffset)
  - `EndDate` (DateTimeOffset)

- **`tour.TournamentTeam`**
  - `TournamentId` (BigInt, FK -> `tour.Tournament.Id`, Composite PK)
  - `TeamId` (BigInt, FK -> `part.Team.Id`, Composite PK)

---

### Participant Module (`part`)
- **`part.Team`**
  - `Id` (BigInt, PK)
  - `Name` (string, Required)
  - `LogoUrl` (string, Nullable)

- **`part.HeadCoach`**
  - `Id` (BigInt, PK)
  - `FullName` (string, Required)
  - `DateOfBirth` (DateOnly)
  - `Gender` (string)
  - `UserId` (BigInt, Nullable, FK -> `usr.User.Id`)

- **`part.Player`**
  - `Id` (BigInt, PK)
  - `FullName` (string, Required)
  - `DateOfBirth` (DateOnly)
  - `Gender` (string)
  - `UserId` (BigInt, Nullable, FK -> `usr.User.Id`)

- **`part.TeamPlayer`**
  - `TeamId` (BigInt, FK -> `part.Team.Id`, Composite PK)
  - `PlayerId` (BigInt, FK -> `part.Player.Id`, Composite PK)

- **`part.TeamHeadCoach`**
  - `Id` (BigInt, PK)
  - `TeamId` (BigInt, FK -> `part.Team.Id`)
  - `CoachId` (BigInt, FK -> `part.HeadCoach.Id`)
  - `StartDate` (DateTimeOffset, Default = Now)
  - `EndDate` (DateTimeOffset, Nullable)

---

### Match Operations Module (`ops`)
- **`ops.Venue`**
  - `Id` (BigInt, PK)
  - `Name` (string, Required)
  - `OwnerId` (BigInt, FK -> `usr.User.Id`)

- **`ops.OfficiatingPersonnel`**
  - `Id` (BigInt, PK)
  - `FullName` (string, Required)
  - `DateOfBirth` (DateOnly)
  - `Gender` (string)
  - `OfficiatingPersonnelTypeId` (int, FK -> `conf.OfficiatingPersonnelType.Id`)

- **`ops.Fixture`**
  - `Id` (BigInt, PK)
  - `Date` (DateOnly)
  - `KickoffTime` (TimeOnly)
  - `ActualStartTime` (DateTimeOffset, Nullable)
  - `FinishTime` (DateTimeOffset, Nullable)
  - `HomeTeamId` (BigInt, FK -> `part.Team.Id`)
  - `AwayTeamId` (BigInt, FK -> `part.Team.Id`)
  - `HomeTeamGoals` (int, Default = 0)
  - `AwayTeamGoals` (int, Default = 0)
  - `MatchStatusId` (int, FK -> `conf.MatchStatus.Id`, Default = Scheduled)
  - `VenueId` (BigInt, FK -> `ops.Venue.Id`)
  - `Summary` (string, Nullable)

- **`ops.FixturePlayerEvent`**
  - `Id` (BigInt, PK)
  - `FixtureId` (BigInt, FK -> `ops.Fixture.Id`)
  - `FixtureEventTypeId` (int, FK -> `conf.FixtureEventType.Id`)
  - `TeamId` (BigInt, FK -> `part.Team.Id`)
  - `PlayerId` (BigInt, Nullable, FK -> `part.Player.Id`) — *Nullable to allow post-match assignment*
  - `IsCancelled` (bool, Default = false)