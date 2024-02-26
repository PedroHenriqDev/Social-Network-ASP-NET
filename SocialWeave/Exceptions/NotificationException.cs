namespace SocialWeave.Exceptions
{
    /// <summary>
    /// Exception thrown when an error occurs related to notifications.
    /// </summary>
    public class NotificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotificationException(string message)
            : base(message)
        {
        }
    }
}
