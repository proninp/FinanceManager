using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FinanceManager.UserService.Contracts.DTOs
{
    /// <summary>
    /// DTO модели часового пояса.
    /// </summary>
    public record TimeZoneDto: BaseDto
    {
        /// <summary>
        /// Название часового пояса
        /// </summary>
        public string Name { get; init; }
    }
}
