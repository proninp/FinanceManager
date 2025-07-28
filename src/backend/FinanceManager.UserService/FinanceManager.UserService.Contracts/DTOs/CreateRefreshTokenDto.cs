using FinanceManager.UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.UserService.Contracts.DTOs
{
    /// <summary>
    /// DTO для создания токена обновления
    /// </summary>
    public record CreateRefreshTokenDto
    {
        /// <summary>
        /// Идентификатор пользователя - владельца токена
        /// </summary>
        public Guid UserId { get; init; }

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
