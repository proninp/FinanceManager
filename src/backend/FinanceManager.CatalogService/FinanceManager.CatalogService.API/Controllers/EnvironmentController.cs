using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FinanceManager.CatalogService.API.Controllers;

/// <summary>
/// Контроллер информации о сервисе
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Контроллер информации о сервисе")]
public class EnvironmentController(ILogger logger) : ControllerBase
{
    /// <summary>
    /// Возвращает системную информацию о приложении и окружении.
    /// </summary>
    /// <returns>
    /// Строка с данными:
    /// - Имя и версия сборки,
    /// - Версия .NET,
    /// - Локальное время (ISO 8601),
    /// - Имя машины,
    /// - Архитектура ОС и процесса,
    /// - Платформа и версия ОС.
    /// </returns>
    [HttpGet(Name = "GetEnvInfo")]
    [ProducesResponseType(typeof(string), 200)]
    public ActionResult<string> Get()
    {
        // Получение информации о сборке
        var assembly = Assembly.GetEntryAssembly();
        var assemblyName = assembly?.GetName().Name ?? "Unknown";
        var assemblyVersion = assembly?.GetName().Version?.ToString() ?? "Unknown";

        // Получение информации о платформе и окружении
        var frameworkDescription = RuntimeInformation.FrameworkDescription;
        var localTime = DateTime.Now.ToString("o"); // ISO 8601 формат
        var machineName = Environment.MachineName;
        var osArchitecture = RuntimeInformation.OSArchitecture.ToString();
        var osPlatform = RuntimeInformation.OSDescription;
        var osVersion = Environment.OSVersion.VersionString;
        var processArchitecture = RuntimeInformation.ProcessArchitecture.ToString();

        // Используем StringBuilder для формирования строки
        var sb = new StringBuilder();
        sb.AppendLine($"          Assembly Name = {assemblyName}");
        sb.AppendLine($"       Assembly Version = {assemblyVersion}");
        sb.AppendLine($"  Framework Description = {frameworkDescription}");
        sb.AppendLine($"             Local Time = {localTime}");
        sb.AppendLine($"           Machine Name = {machineName}");
        sb.AppendLine($"        OS Architecture = {osArchitecture}");
        sb.AppendLine($"            OS Platform = {osPlatform}");
        sb.AppendLine($"             OS Version = {osVersion}");
        sb.AppendLine($"   Process Architecture = {processArchitecture}");

        return sb.ToString();
    }
}