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
        public Role Role { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес электронной почты.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Хэшсумма пароля пользователя.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Идентификатор пользователя в Telegram.
        /// </summary>
        public long TelegramId { get; set; }

        /// <summary>
        /// Идентификатор часового пояса пользователя.
        /// </summary>
        public Guid DefaultTimeZoneId { get; set; } 

        /// <summary>
        /// Часовой пояс пользователя по-умолчанию.
        /// </summary>
        public TimeZone DefaultTimeZone { get; set; }
    }
}
