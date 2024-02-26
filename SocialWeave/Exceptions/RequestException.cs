namespace SocialWeave.Exceptions
{
    /// <summary>
    /// Exception thrown when an error occurs related to requests.
    /// </summary>
    public class RequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RequestException(string message) : base(message)
        {
        }
    }
}
