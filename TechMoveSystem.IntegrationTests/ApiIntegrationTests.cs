using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace TechMoveSystem.IntegrationTests
{
    [TestClass]
    public class ApiIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                });

            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetContracts_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/contracts");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        }

        [TestMethod]
        public async Task GetClients_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/clients");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        }

        [TestMethod]
        public async Task GetServiceRequests_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/ServiceRequests");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        }
    }
}