using FinanceManager.UserService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.UserService.Contracts.DTOs
{
    /// <summary>
    /// DTO для создания пользователя
    /// </summary>
    public record CreateUserDto
    {
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
    }
}
