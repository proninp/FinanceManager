using FinanceManager.UserService.Domain.Entities;

namespace FinanceManager.UserService.Contracts.DTOs
{
    // <summary>
    /// DTO модели токена обновления, используемого для продления срока действия access токена.
    /// </summary>
    public record RefreshTokenDto: BaseDto
    {
        /// <summary>
        /// Идентификатор пользователя - владельца токена
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// Пользователь - владелец токена
        /// </summary>
        public UserDto User { get; init; } 

        /// <summary>
        /// Токен
        /// </summary>
        public string Token { get; init; }

        /// <summary>
        /// Дата истечения срока годности токена
        /// </summary>
        public DateTime ExpiresAt { get; init; }

        /// <summary>
        /// Флаг отозванности токенв
        /// </summary>
        public bool IsRevoked { get; init; }
    }
}
