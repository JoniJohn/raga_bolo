# SERVICE CAPABILITIES: RAGA BOLO TOURNAMENT API

## Overview
The Raga Bolo API is a Tournament Management System designed to handle tournament creation, participant management, fixture scheduling, and live match event logging.

---

## 1. Capability Modules & Boundaries

| Capability Module | Boundaries & Responsibilities | Key Entities Involved |
| :--- | :--- | :--- |
| **User Management** | Manages application user profiles and user groups for tournament permissions. *Note: Auth/Identity is handled externally.* | `User`, `UserGroup`, `UserGroupMember` |
| **Tournament Administration** | Manages tournament lifecycle, tournament committee assignments, and configuration parameters. | `Tournament`, `TournamentCommittee`, `TournamentCupType` |
| **Participant Management** | Registers competing teams, head coaches, and players participating in tournaments. | `Team`, `Player`, `HeadCoach`, `TeamPlayer`, `TeamHeadCoach` |
| **Fixture & Schedule Management** | Controls scheduling calendars, venues, and manual fixture lifecycle operations (postpone/cancel). | `Fixture`, `MatchStatus`, `Venue`, `TournamentSchedule`, `TournamentCalendar` |
| **Match Operations & Live Events** | Handles real-time score tracking and match event logs (goals, yellow/red cards, assists). | `FixturePlayerEvent`, `FixtureEventType`, `OfficiatingPersonnel` |

---

## 2. Feature Roadmap & Endpoint Matrix

Agent MUST build these endpoints **strictly one at a time** in the sequence outlined below. Do NOT jump ahead to future phases.

### Phase 1: User Management (Current Scope)
- [ ] `POST /api/users` — Onboard a new user profile.
- [ ] `POST /api/user-groups` — Register a new user group.
- [ ] `POST /api/user-groups/{id}/members` — Add a user to a registered user group.

### Phase 2: Tournament Administration & Participants
- [ ] `POST /api/tournaments` — Register a tournament.
- [ ] `POST /api/tournaments/{id}/committees` — Register a tournament committee member.
- [ ] `POST /api/tournaments/{id}/teams` — Register a team under a tournament.
- [ ] `POST /api/teams/{id}/players` — Register a player under a team.
- [ ] `POST /api/teams/{id}/head-coaches` — Assign a Head Coach to a team.

### Phase 3: Tournament Scheduler (Single Fixture League)
- [ ] `POST /api/tournaments/{id}/schedules/league-single` — Generate single fixture league schedule.

### Phase 4: Fixture Management & Match Operations
- [ ] `PATCH /api/fixtures/{id}/postpone` — Postpone a fixture.
- [ ] `PATCH /api/fixtures/{id}/cancel` — Cancel a fixture.
- [ ] `POST /api/fixtures/{id}/events` — Record a match event (e.g., Goal, Card). *Rule: Match must be Live.*
- [ ] `PATCH /api/fixtures/events/{id}/link-player` — Associate an event with a specific player post-match.

### Phase 5: Advanced Schedulers
- [ ] `POST /api/tournaments/{id}/schedules/league-double` — Generate Home and Away league schedule.
- [ ] `POST /api/tournaments/{id}/schedules/knockout` — Generate Single Fixture Knockout schedule (up to Round of 16).

---

## 3. Seed Configuration Data Dictionary

When generating feature-specific EF Core migrations, Agent must ensure these static lookup values are seeded via migrations:

1. **TournamentCupType**:
   - `SL` — Single Fixture League
   - `HAD` — Home and Away League
   - `SK` — Single Fixture Knockout
   - `HAK` — Home and Away Knockout
2. **MatchStatus**:
   - `SCHEDULED`, `STARTED`, `ENDED`, `POSTPONED`, `CANCELLED`
3. **FixtureEventType**:
   - `GOAL`, `ASSIST`, `YELLOW_CARD`, `SECOND_YELLOW`, `RED_CARD`