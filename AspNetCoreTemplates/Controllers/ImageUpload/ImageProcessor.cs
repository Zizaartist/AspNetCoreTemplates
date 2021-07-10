using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCoreTemplates.Controllers.ImageUpload
{
    /// <summary>
    /// Обработчик изображений, занимается измнением размера, положением, компрессией и т.д.
    /// </summary>
    /// <typeparam name="T">
    /// Enum тип, перечисляющий все поддерживаемые типы картинок.
    /// Например: avatar, preview, postImage.
    /// </typeparam>
    public abstract class ImageProcessor<T> where T : Enum
    {
        /// <summary>
        /// Перечень функций обработки для определенных видов изображений
        /// Например: avatar - сделать соотношение сторон квадратным, по возможности уменьшить размер, обычное сжатие
        /// </summary>
        private Dictionary<T, Action<MagickImage, Stream>> Processes { get; set; } = new Dictionary<T, Action<MagickImage, Stream>>();

        /// <summary>
        /// Добавляет процесс обработки в коллекцию
        /// </summary>
        /// <param name="key">Тип изображения</param>
        /// <param name="newProcess">Делегат обработки изображения</param>
        protected void AddProcess(T key, Action<MagickImage, Stream> newProcess) => Processes.Add(key, newProcess);

        /// <summary>
        /// Запускает процесс обработки изображения, процесс выбирается из коллекции.
        /// В случае если его нет в наличии применяется процесс по-умолчанию
        /// </summary>
        /// <param name="sourceStream">Входящий файловый поток</param>
        /// <param name="resultStream">Файловый поток обработанного изображения</param>
        /// <param name="imageType">Тип изображения</param>
        public void ProcessImage(Stream sourceStream, Stream resultStream, T imageType)
        {
            using (var image = new MagickImage(sourceStream))
            {
                Action<MagickImage, Stream> process = DefaultProcess;
                if (Processes.ContainsKey(imageType))
                {
                    process = Processes[imageType];
                }

                process.Invoke(image, resultStream);
            }
        }

        /// <summary>
        /// Обработчик изображения по-умолчанию. Просто производит небольшое сжатие.
        /// </summary>
        /// <param name="inputImage">Объект изображения</param>
        /// <param name="resultStream">Файловый поток обработанного изображения</param>
        private void DefaultProcess(MagickImage inputImage, Stream resultStream)
        {
            //Записываем в буфер
            inputImage.Write(resultStream);

            resultStream.Position = 0; //Сбрасываем каретку, автоматически это не делается

            //Компрессируем буфер
            ImageOptimizer optimizer = new ImageOptimizer();
            optimizer.Compress(resultStream);
        }
    }
}
