namespace SocialWeave.Models.Interfaces
{
    public interface IProfilePictureService
    {
        Task<string> SaveProfilePictureAsync(byte[] pictureData, string webRootPath);
    }
}
