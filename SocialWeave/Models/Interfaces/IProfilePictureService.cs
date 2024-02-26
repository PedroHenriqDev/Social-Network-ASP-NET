using System.Threading.Tasks;

namespace SocialWeave.Models.Interfaces
{
    /// <summary>
    /// Interface for managing profile pictures.
    /// </summary>
    public interface IProfilePictureService
    {
        /// <summary>
        /// Saves the profile picture asynchronously.
        /// </summary>
        /// <param name="pictureData">The byte array representing the picture data.</param>
        /// <param name="webRootPath">The root path of the web application.</param>
        /// <returns>The URL of the saved profile picture.</returns>
        Task<string> SaveProfilePictureAsync(byte[] pictureData, string webRootPath);
    }
}
