using System.Net;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.CatalogService.API.Extensions;

/// <summary>
/// Extension методы для преобразования Result в ActionResult
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Преобразует Result&lt;T&gt; в ActionResult&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    /// <param name="result">Результат операции</param>
    /// <param name="controller">Контроллер для создания ActionResult</param>
    /// <returns>ActionResult с соответствующим HTTP статусом</returns>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result.IsSuccess 
            ? controller.Ok(result.Value) 
            : HandleErrors(result.Errors, controller);
    }
    
    /// <summary>
    /// Преобразует Result в ActionResult
    /// </summary>
    /// <param name="result">Результат операции</param>
    /// <param name="controller">Контроллер для создания ActionResult</param>
    /// <returns>ActionResult с соответствующим HTTP статусом</returns>
    public static ActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        return result.IsSuccess 
            ? controller.Ok() 
            : HandleErrors(result.Errors, controller);
    }

    /// <summary>
    /// Обрабатывает список ошибок и возвращает соответствующий ActionResult на основе метаданных ошибок.
    /// </summary>
    /// <param name="errors">Список ошибок, содержащих метаданные и сообщения.</param>
    /// <param name="controller">Контроллер для создания ActionResult.</param>
    /// <returns>
    /// ActionResult с соответствующим HTTP статусом и сообщением об ошибке.
    /// Если метаданные ошибки отсутствуют или некорректны, возвращается статус 500 (Internal Server Error).
    /// </returns>
    private static ActionResult HandleErrors(List<IError> errors, ControllerBase controller)
    {
        var error = errors.FirstOrDefault();
        if (error is null || 
            !error.Metadata.TryGetValue(nameof(HttpStatusCode), out var statusCodeObj) ||
            statusCodeObj is not HttpStatusCode statusCode)
        {
            return controller.StatusCode((int)HttpStatusCode.InternalServerError,
                "Произошла внутренняя ошибка сервера");
        }
        var errorMessage = error.Message;

        return statusCode switch
        {
            HttpStatusCode.NotFound => controller.NotFound(errorMessage),
            HttpStatusCode.BadRequest => controller.BadRequest(errorMessage),
            HttpStatusCode.Conflict => controller.Conflict(errorMessage),
            HttpStatusCode.Unauthorized => controller.Unauthorized(errorMessage),
            HttpStatusCode.Forbidden => controller.Forbid(errorMessage),
            HttpStatusCode.UnprocessableEntity => controller.UnprocessableEntity(errorMessage),
            _ => controller.StatusCode((int)statusCode, errorMessage)
        };
    }
}