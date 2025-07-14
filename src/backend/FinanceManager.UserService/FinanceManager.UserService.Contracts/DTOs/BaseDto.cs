using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.UserService.Contracts.DTOs
{
    /// <summary>
    /// Базовый DTO сущностей.
    /// </summary>
    public record BaseDto
    {
        /// <summary>
        /// Уникальный идентификатор сущности.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Дата и время создания сущности.
        /// </summary>
        public DateTime CreatedAt { get; init; }
    }
}
