using FinanceManager.UserService.Contracts.DTOs;
using FluentResults;

namespace FinanceManager.UserService.Abstractions.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Получает запись пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO пользователя или null, если не найден</returns>
        Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        /// <param name="createDto">Данные для создания пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат с созданным пользователем или ошибкой</returns>
        Task<Result<UserDto>> CreateAsync(
            CreateUserDto createDto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="updateDto">Данные для обновления пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат с обновленными данными пользователя или ошибкой</returns>
        Task<Result<UserDto>> UpdateAsync(
            UpdateUserDto updateDto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат операции</returns>
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
