namespace SocialWeave.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an error occurs during search operations.
    /// </summary>
    public class SearchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SearchException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchException"/> class.
        /// </summary>
        public SearchException()
        {
        }
    }
}
