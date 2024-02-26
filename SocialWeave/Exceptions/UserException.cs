namespace SocialWeave.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an error occurs related to user operations.
    /// </summary>
    public class UserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserException(string message)
            : base(message)
        {
        }
    }
}
