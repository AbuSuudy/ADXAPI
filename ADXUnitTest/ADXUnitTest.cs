using ADXAPI;
using ADXAPI.Controllers;
using ADXService;
using ADXService.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace ADXUnitTest
{
    public class ADXUnitTest
    {

        [Fact]
        public void ValidJWT_Issuer_Audience_ValidFrom()
        {
            //Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(x => x["JwtSettings:Key"]).Returns("fhkjdsfhdslfhdsfkldhfaklhkjsdhfd");

            IJWTGenerator jwtGenerator = new JWTGenerator(configurationMock.Object);

            //Act
            var token = jwtGenerator.GenerateToken("fdsfds@gmail.com", false);
            var handler = new JwtSecurityTokenHandler();
            var tokenParsed = handler.ReadToken(token);

            //Assert
            Assert.True(!String.IsNullOrEmpty(token));
            Assert.True(handler.CanReadToken(token));
            Assert.True(tokenParsed.ValidTo < DateTime.Now.AddHours(1));
            Assert.Equal(tokenParsed.Issuer, "https://localhost:44397/");

        }

        [Fact]
        public async Task GetStormData()
        {
            //Arrange 

            var stormData = new List<StormData>
            {
                new StormData
                {
                    DateTime = DateTime.Now,
                    State = "Boston",
                    DamageCost = 10000000000
                }
            };

            var adx = new Mock<IADXAccess>();
            adx.Setup(x => x.StormEventsData()).Returns(stormData);
            adx.Setup(x => x.CheckIfTableExist()).Returns(true);
            ADXController sut = new ADXController(adx.Object);

            //Act
            var response = sut.GetStormData();
            var okResult = response as OkObjectResult;

            //Response
            Assert.IsType<OkObjectResult>(response);
            Assert.Equivalent(okResult.Value, stormData);

        }
    }
}
