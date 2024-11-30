using ADXAPI;
using ADXAPI.Model;
using ADXService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ADXIntegrationTest
{
    public class ADXAPIIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
    
        public ADXAPIIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/ADX/GetStormData")]
        public async Task Get_StormDataIncorrectClaims_Forbidden(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            User user = new User
            {
                Email = "Test@gmail.com",
                ADXUser = false
            };

            var tokenService = _factory.Services.GetService<IJWTGenerator>();
            var token = tokenService.GenerateToken(user.Email, user.ADXUser);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync(url);

            // Assert   
            Assert.Equivalent(response.StatusCode, StatusCodes.Status403Forbidden);

        }

        [Theory]
        [InlineData("api/ADX/GetStormData")]
        public async Task Get_EndpointsWithoutToken_Unauthorised(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert   
            Assert.Equivalent(response.StatusCode, 401);

        }

        [Theory]
        [InlineData("api/ADX/GetStormData")]
        public async Task Get_StormDataCorrectClaims_OK(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            User user = new User
            {
                Email = "Test@gmail.com",
                ADXUser = true
            };

            var tokenService = _factory.Services.GetService<IJWTGenerator>();
            var  token = tokenService.GenerateToken(user.Email, user.ADXUser);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync(url);

            // Assert   
            Assert.Equivalent(response.StatusCode, StatusCodes.Status200OK);

        }

        [InlineData("api/ADX/GetStormData")]
        [Theory]
        public async Task Get_StormDataCheckReturnType_OK(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            User user = new User
            {
                Email = "Test@gmail.com",
                ADXUser = true
            };

            var tokenService = _factory.Services.GetService<IJWTGenerator>();
            var token = tokenService.GenerateToken(user.Email, user.ADXUser);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var reponseData = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<List<StormData>>(reponseData);

            // Assert   
            Assert.IsType<List<StormData>>(JsonSerializer.Deserialize<List<StormData>>(reponseData));

        }
    }
}