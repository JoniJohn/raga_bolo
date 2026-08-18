using Raga.IntegrationTests.Fixtures;
using Xunit;

namespace Raga.IntegrationTests;

/// <summary>
/// Binds the DatabaseFixture to the "Integration" collection.
/// Decorate every integration test class with [Collection("Integration")]
/// to share the single container instance.
/// </summary>
[CollectionDefinition("Integration")]
public sealed class IntegrationTestCollection : ICollectionFixture<DatabaseFixture>
{
    // No body required — xUnit uses this declaration to wire up the fixture.
}
