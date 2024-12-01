using ADXAPI;
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
        public void GetStormData()
        {
            var dataReader = new Mock<IDataReader>();
            dataReader.Setup(m => m.FieldCount).Returns(2);
            // Mock return from ADX and check c
        }

        [Fact]
        public void RowCount()
        {
            // Use dataReader and mock resopnse
        }
    }
}
