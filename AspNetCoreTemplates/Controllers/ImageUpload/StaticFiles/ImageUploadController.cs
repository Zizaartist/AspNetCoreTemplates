using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreTemplates.Controllers.ImageUpload.StaticFiles
{
    public class ImageUploadController<T> : ImageUploadControllerBase<T> where T : Enum
    {
        private readonly ImageProcessor<T> _imageProcessor;

        public ImageUploadController(ImageProcessor<T> imageProcessor)
        {
            _imageProcessor = imageProcessor;
        }

        protected override void ProcessImage(Stream sourceStream, Stream resultStream, T imageType) => _imageProcessor.ProcessImage(sourceStream, resultStream, imageType);

        protected override async Task UploadResultImage(Stream imageStream, string imageName, string folderPath)
        {
            var newFilePath = Path.Combine(folderPath, imageName);

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                await imageStream.CopyToAsync(fileStream);
            }
        }

        protected override string GenerateFileName(T _) => Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";

        protected override string GetPathForType(T _) => "Images";
    }
}
