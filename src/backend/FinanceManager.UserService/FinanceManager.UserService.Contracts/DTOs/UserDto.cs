using FinanceManager.UserService.Domain.Enums;
using System.Security.Principal;

namespace FinanceManager.UserService.Contracts.DTOs
{
    /// <summary>
    /// DTO сущности пользователя системы финансового менеджера.
    /// </summary>
    public record UserDto: BaseDto
    {
        /// <summary>
        /// Роль пользователя в системе финансового менеджера.
        /// </summary>
        public Role Role { get; init; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Адрес электронной почты.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Хэшсумма пароля пользователя.
        /// </summary>
        public string? PasswordHash { get; init; }

        /// <summary>
        /// Идентификатор пользователя в Telegram.
        /// </summary>
        public long TelegramId { get; init; }

        /// <summary>
        /// Идентификатор часового пояса пользователя.
        /// </summary>
        public Guid DefaultTimeZoneId { get; init; } 

        /// <summary>
        /// Часовой пояс пользователя по-умолчанию.
        /// </summary>
        public TimeZoneDto DefaultTimeZone { get; init; }
    }
}
