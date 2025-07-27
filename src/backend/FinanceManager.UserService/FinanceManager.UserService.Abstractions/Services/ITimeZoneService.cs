using FinanceManager.UserService.Contracts.DTOs;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.UserService.Abstractions.Services
{
    public interface ITimeZoneService
    {
        /// <summary>
        /// Получает запись модели часового пояса.
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO или null, если не найден</returns>
        Task<Result<TimeZoneDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
