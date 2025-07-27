using FinanceManager.UserService.Contracts.DTOs;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.UserService.Abstractions.Services
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Получает запись токена обновления
        /// </summary>
        /// <param name="id">Идентификатор токена обновления</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO токена обновления или null, если не найден</returns>
        Task<Result<RefreshTokenDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Создает новый токен обновления
        /// </summary>
        /// <param name="createDto">Данные для создания пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат с созданным пользователем или ошибкой</returns>
        Task<Result<RefreshTokenDto>> CreateAsync(
            CreateUserDto createDto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Деактивация токена (мягкое удаление)
        /// </summary>
        /// <param name="id">Идентификатор токена обновления</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат операции</returns>
        Task<Result<RefreshTokenDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет токен
        /// </summary>
        /// <param name="id">Идентификатор токена обновления</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат операции</returns>
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
