using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreTemplates.Controllers.ImageUpload.AzureBlob
{
    class ImageUploadController<T> : ImageUploadControllerBase<T> where T : Enum
    {
        private readonly ImageProcessor<T> _imageProcessor;
        private readonly IConfiguration _configuration;

        public ImageUploadController(ImageProcessor<T> imageProcessor, IConfiguration configuration)
        {
            _imageProcessor = imageProcessor;
            _configuration = configuration;
        }
        
        protected override string GenerateFileName(T _) => Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";

        protected override string GetPathForType(T _) => "Images";

        protected override void ProcessImage(Stream sourceStream, Stream resultStream, T imageType) => _imageProcessor.ProcessImage(sourceStream, resultStream, imageType);

        protected override async Task UploadResultImage(Stream imageStream, string imageName, string folderPath)
        {
            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                        _configuration["BlobStorageName"],
                                                        Path.Combine(folderPath, imageName));

            // Get a credential and create a client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                new DefaultAzureCredential());

            await containerClient.UploadBlobAsync(imageName, imageStream);
        }
    }
}
