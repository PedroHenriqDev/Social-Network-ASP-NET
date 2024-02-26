using SocialWeave.Models.Interfaces;
using System.Globalization;
using System.IO;

namespace SocialWeave.Models.Services
{
    public class ProfilePictureService : IProfilePictureService
    {
        public async Task<string> SaveProfilePictureAsync(byte[] pictureData, string webRootPath)
        {
            string profilePictureUrl = string.Empty;
            string profilePicturesDirectory = Path.Combine(webRootPath, "profile-pictures");
            if (!Directory.Exists(profilePicturesDirectory))
            {
                Directory.CreateDirectory(profilePicturesDirectory);
            }

            string fileExtension = ".jpg";

            if (pictureData != null)
            {

                // Detect the image format
                if (IsWebPImage(pictureData))
                {
                    fileExtension = ".webp";
                }
                else if (IsJpegImage(pictureData))
                {
                    fileExtension = ".jpeg";
                }
                else if (IsPngImage(pictureData))
                {
                    fileExtension = ".png";
                }

                string fileName = $"{Guid.NewGuid()}{fileExtension}";

                string filePath = Path.Combine(profilePicturesDirectory, fileName);

                await File.WriteAllBytesAsync(filePath, pictureData);

                profilePictureUrl = $"/profile-pictures/{fileName}";
            }

            return profilePictureUrl;
        }

        private bool IsWebPImage(byte[] imageData)
        {
            return imageData.Length > 12 &&
                   imageData[0] == 0x52 &&
                   imageData[1] == 0x49 &&
                   imageData[2] == 0x46 &&
                   imageData[3] == 0x46 &&
                   imageData[8] == 0x57 &&
                   imageData[9] == 0x45 &&
                   imageData[10] == 0x42 &&
                   imageData[11] == 0x50;
        }

        private bool IsJpegImage(byte[] imageData)
        {
            return imageData.Length > 3 &&
                   imageData[0] == 0xFF &&
                   imageData[1] == 0xD8 &&
                   imageData[2] == 0xFF;
        }

        private bool IsPngImage(byte[] imageData)
        {
            return imageData.Length > 4 &&
                   imageData[0] == 0x89 &&
                   imageData[1] == 0x50 &&
                   imageData[2] == 0x4E &&
                   imageData[3] == 0x47;
        }
    }
}
