# SYSTEM RULES: RAGA BOLO API

## Core Philosophy
1. INCREMENTAL DEVELOPMENT: Build exactly ONE endpoint at a time. Never create schemas, entities, or infrastructure code for future features.
2. STRICT TDD: Test-Driven Development is mandatory using the AAA (Arrange, Act, Assert) pattern.
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
- Domain logic must reside in `src/Domain`.
- Simple Entity Validation: Enforce directly within Aggregate/Entity methods.
- Cross-Entity or Complex Business Policies: Implement isolated policy rules using `IBusinessRule` in `src/Domain/Rules/`. Entities/Services validate these rules before executing actions.
- Functional Errors: Return a result container (e.g., `Result<T>`) containing error metadata instead of throwing HTTP exceptions from the domain.

## Testing Guidelines & Testcontainer Optimization
- Unit Tests: Target pure Domain/Application logic with fast execution.
- Integration Tests: Must share a **SINGLE** PostgreSQL Testcontainer instance per test suite run using xUnit `ICollectionFixture`.
- Isolation: Use `Respawn` or transaction rollback between integration test cases to reset state. NEVER spin up more than 1 container instance total for integration tests.

## Phase Execution Flow (STRICT LOOP FOR EVERY ENDPOINT)
1. STEP 1: SPECIFICATION & PLAN
   - Generate a single endpoint specification document.
   - STOP AND WAIT FOR USER APPROVAL. Do NOT write code yet.

2. STEP 2: TEST DRIVEN DEVELOPMENT (TDD)
   - Once spec is approved, write Unit Tests and Integration Tests for this single endpoint.
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