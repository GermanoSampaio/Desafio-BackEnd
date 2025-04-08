
using MotoService.Domain.Utils.Models;

namespace MotoService.Domain.Utils
{
    public static  class FileHelper
    {
        public static ImageInfo? DetectImageInfo(byte[] imageBytes)
        {
            if (imageBytes.Length >= 4)
            {
                if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
                    imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                    return new ImageInfo { ContentType = "image/png", Extension = ".png" };

                if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                    return new ImageInfo { ContentType = "image/bmp", Extension = ".bmp" };

               
            }

            return null;
        }
    }
}
