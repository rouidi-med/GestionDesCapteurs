using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Capteurs.Entities;
using Capteurs.Data;

namespace Capteurs.Tests.Functional
{
    public class CapteurControllerFunctionalTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public CapteurControllerFunctionalTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Add the API key to all requests
            _client.DefaultRequestHeaders.Add("x-api-key", "4D62887E-9690-4476-8285-521C6A88DE8A");
        }

        // Call this method to seed the capteurs before running tests
        private async Task SeedDatabaseAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Clear any existing data (optional)
            context.Capteurs.RemoveRange(context.Capteurs);
            await context.SaveChangesAsync();

            // Seed initial capteurs
            context.Capteurs.AddRange(
                new Capteur { Id = 1, Nom = "Temperature Sensor" },
                new Capteur { Id = 2, Nom = "Pressure Sensor" }
            );
            await context.SaveChangesAsync();
        }

        // Ensure the database is seeded before the tests run
        public async Task InitializeAsync()
        {
            await SeedDatabaseAsync();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenCapteurExists()
        {
            // Arrange: Seed the database before the test
            await InitializeAsync();

            int existingId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/Capteur/{existingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenCapteurDoesNotExist()
        {
            // Act
            var response = await _client.DeleteAsync($"/api/Capteur/{9999999999}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenCapteurIdIsZeroOrLessThanZero()
        {
            // Act
            var response = await _client.DeleteAsync($"/api/Capteur/{0}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
