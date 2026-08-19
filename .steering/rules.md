# SYSTEM RULES: RAGA BOLO API

## Core Philosophy
1. INCREMENTAL DEVELOPMENT: Build exactly ONE endpoint at a time. Never create schemas, entities, or infrastructure code for future features.
2. STRICT TDD & OBSERVABLE BEHAVIOR: Test-Driven Development is mandatory using the AAA (Arrange, Act, Assert) pattern. Tests MUST target public, observable behaviors of aggregates/services rather than internal mechanics.
3. SIMPLE CLEAN ARCHITECTURE: Domain models encapsulate business logic. Use standard domain abstractions without CQRS/MediatR overhead.

## Architecture & Domain Guidelines
- Folder Hierarchy:
   - src/Domain (Entities, Enums, Interfaces, Value Objects, Domain Rules)
   - src/Application (DTOs, Service Interfaces, Service Implementations)
   - src/Infrastructure (EF Core DbContext, Configurations, Migrations, Repositories)
   - src/Api (Controllers/Endpoints, Dependency Injection)
   - tests/UnitTests
   - tests/IntegrationTests (Testcontainers for Postgres)

## Domain Rules Strategy
- Structural Input Validation:
   - Basic field constraints (e.g., non-null, max string length, correct formatting) MUST be validated directly inside Value Objects or Aggregate factory/action methods (e.g., `User.Create(...)`) using simple guard clauses or `Result` containers.
   - DO NOT create isolated `IBusinessRule` classes for simple field checks.
- Business Policies (Domain Rules):
   - Reserve `IBusinessRule` implementations in `src/Domain/Rules/` strictly for complex, stateful, or cross-entity domain policies that domain experts articulate using business language (e.g., "A User cannot join a UserGroup if suspended").
- Error Handling:
   - Return a result container (e.g., `Result`) containing error metadata instead of throwing HTTP exceptions from the domain layer.

## Testing Guidelines & Testcontainer Optimization
- Unit Testing Observable Behavior:
   - Target Aggregate Roots, Value Objects, and Application Services via their public APIs.
   - NEVER write standalone unit test suites for internal helper rule classes (e.g., do NOT create `UserNameMaxLengthRuleTests`). Test the validation outcome via the entry point (e.g., `UserCreateTests`).
   - Refactoring internal validation logic must not break tests as long as the observable business behavior remains identical.
- Test Naming Conventions:
   - Test names must describe domain behaviors and observable outcomes, not technical class or method names.
   - Good: `Create_ShouldFail_WhenUsernameExceedsMaximumLength`
   - Bad: `UserNameMaxLengthRule_IsBroken_ReturnsTrueIfExceeded`
- Integration Tests:
   - Must share a SINGLE PostgreSQL Testcontainer instance per test suite run using xUnit `ICollectionFixture`.
   - Isolation: Use `Respawn` or transaction rollback between integration test cases to reset state. NEVER spin up more than 1 container instance total.

## Phase Execution Flow (STRICT LOOP FOR EVERY ENDPOINT)
1. STEP 1: SPECIFICATION & PLAN
   - Generate a single endpoint specification document.
   - STOP AND WAIT FOR USER APPROVAL. Do NOT write code yet.
2. STEP 2: TEST DRIVEN DEVELOPMENT (TDD)
   - Once spec is approved, write Unit Tests (focusing on observable behavior) and Integration Tests for this single endpoint.
   - STOP AND WAIT FOR USER REVIEW.
3. STEP 3: IMPLEMENTATION
   - Implement Domain -> Application -> Infrastructure -> API.
   - Generate feature-specific EF Core Migration (including configuration seed data if applicable).
   - Ensure all tests pass.
   - STOP FOR FINAL FEATURE REVIEW.

## Database Schema Reference Rule
- Refer to `.steering/schema-reference.md` for entity names, column definitions, and foreign key relationships.
- DO NOT generate full DbContext or migrations for all referenced tables upfront.
- Only introduce the specific entities and EF Core mappings required for the current endpoint task.

## Base Entity & Audit Standard
- ALL domain entities MUST inherit from `BaseEntity` (`src/Domain/Common/BaseEntity.cs`).
- Common Audit Fields:
   - `IsActive` (`bool`, default = `true`)
   - `CreatedAt` (`DateTimeOffset`, set automatically on creation)
   - `UpdatedAt` (`DateTimeOffset?`, updated automatically via `DbContext.SaveChangesAsync`)
- Domain logic must use explicit domain methods (e.g., `Deactivate()`, `Activate()`) to toggle status rather than directly modifying properties.