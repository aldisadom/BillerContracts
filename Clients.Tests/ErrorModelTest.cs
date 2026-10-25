using BillerContracts;
using FluentAssertions;
using System.Net;

namespace Clients.Tests;

public class ErrorModelTest
{
    [Theory]
    [InlineData("", "", HttpStatusCode.NotFound, "Error status code: NotFound")]
    [InlineData("exists", "", HttpStatusCode.Conflict, "Error status code: Conflict, message: exists")]
    [InlineData("Validation failure", "Please specify name", HttpStatusCode.BadRequest,
        "Error status code: BadRequest, message: Validation failure, extended message: Please specify name")]
    [InlineData("", "Seller in use", HttpStatusCode.BadRequest, "Error status code: BadRequest, extended message: Seller in use")]
    public void ToString_GivenError_ShowsStatusCodeAndOnlyTheMessagesThatAreSet(string message, string extendedMessage,
        HttpStatusCode statusCode, string expected)
    {
        //Arrange
        ErrorModel error = new(message, extendedMessage, statusCode);

        //Act
        string result = error.ToString();

        //Assert
        result.Should().Be(expected);
    }
}
