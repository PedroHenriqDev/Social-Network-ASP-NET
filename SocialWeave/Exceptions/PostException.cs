namespace SocialWeave.Exceptions
{
    /// <summary>
    /// Exception thrown when an error occurs related to posts.
    /// </summary>
    public class PostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PostException(string message) : base(message)
        {
        }
    }
}
