namespace BillerContracts.Clients;

public class ClientAPIException : Exception
{
    /// <summary>
    /// The error the API returned: always the status code, plus the message and extended message when the body is an
    /// <see cref="Responses.ErrorResponse"/>. Null when the exception isn't about an error response.
    /// </summary>
    public ErrorModel? Error { get; }

    public ClientAPIException()
    {
    }

    public ClientAPIException(string message) : base(message)
    {
    }

    public ClientAPIException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ClientAPIException(string message, ErrorModel error) : base(message)
    {
        Error = error;
    }
}
