namespace SocialWeave.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an integrity violation or inconsistency in the data.
    /// </summary>
    public class IntegrityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrityException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IntegrityException(string message)
            : base(message)
        {
        }
    }
}
