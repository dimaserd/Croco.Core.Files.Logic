using Croco.Core.Contract.Enumerations;
using Croco.Core.Logic.Files.Services.PublicFiles;

namespace Croco.Core.Logic.Files.Extensions;

/// <summary>
/// Расширения для сервиса занимающегося локальными копиями файлов
/// </summary>
public static class LocalFileCopyServiceExtensions
{
    /// <summary>
    /// Возвращает путь к локальному расположению изображения из базы данных по идентификатору и типу размера изображения.
    /// (Использовать только при полной уверенности, что данный файл является изображением)
    /// </summary>
    /// <param name="service"></param>
    /// <param name="fileId">Идентификатор изображения</param>
    /// <param name="type">Тип размера изображения</param>
    /// <returns></returns>
    public static string GetResizedImageLocalPath(this PublicFileLocalCopyService service, int fileId, ImageSizeType type)
    {
        return service.GetResizedImageLocalPath(fileId, type.ToString());
    }
}