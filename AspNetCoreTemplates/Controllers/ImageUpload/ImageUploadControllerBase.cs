using AspNetCoreTemplates.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreTemplates.Controllers
{
    [ApiController]
    [Route("api/ImageUpload")]
    public abstract class ImageUploadControllerBase<T> : ControllerBase where T : System.Enum
    {
        /// <summary>
        /// Получает и публикует изображение
        /// </summary>
        /// <param name="imageData">Объект файла</param>
        /// <param name="imageType">Тип изображения</param>
        /// <returns>Имя опубликованного изображения</returns>
        [HttpPost]
        public async Task<ActionResult<string>> PostImage(IFormFile imageData, T imageType)
        {
            if (imageData == null)
            {
                return BadRequest(new ErrorMessage("Image is null"));
            }
            if (imageData.Length <= 0)
            {
                return BadRequest(new ErrorMessage("Invalid image length")); 
            }
            if (!imageData.ContentType.Contains("image"))
            {
                return BadRequest(new ErrorMessage("Content type is not image")); 
            }

            // путь к папке Files, ЗАМЕНИТЬ Path.GetTempFileName на более надежный генератор
            string newFileName = GenerateFileName(imageType);
            string folderPath = GetPathForType(imageType);

            //Буфер
            using (var resultStream = new MemoryStream())
            {
                //Читаем из полученного файла
                using (var readStream = imageData.OpenReadStream())
                {
                    ProcessImage(readStream, resultStream, imageType);
                }

                //Публикуем буфер
                await UploadResultImage(resultStream, newFileName, folderPath);
            }

            return newFileName;
        }

        /// <summary>
        /// Публикует обработанное изображение
        /// </summary>
        /// <param name="imageStream">Поток обработанного изображения</param>
        /// <param name="imageName">Имя файла</param>
        protected abstract Task UploadResultImage(Stream imageStream, string imageName, string folderPath);

        /// <summary>
        /// Обрабатывает изображение в соответствии с типом
        /// </summary>
        /// <param name="sourceStream">Поток необработанного изображения</param>
        /// <param name="resultStream">Поток, в который будет записано обработанное изображение</param>
        /// <param name="imageType">Тип изображения</param>
        protected abstract void ProcessImage(Stream sourceStream, Stream resultStream, T imageType);

        /// <summary>
        /// Создает имя файла
        /// </summary>
        /// <param name="imageType">Тип изображения</param>
        /// <returns>Имя файла</returns>
        protected abstract string GenerateFileName(T imageType);

        /// <summary>
        /// Создает путь к каталогу файла
        /// </summary>
        /// <param name="imageType">Тип изображения</param>
        /// <returns>Путь к каталогу файла</returns>
        protected abstract string GetPathForType(T imageType);
    }
}
