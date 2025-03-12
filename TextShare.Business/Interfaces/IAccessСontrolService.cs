using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса управления доступом к файлам  и полка.
    /// </summary>
    public interface IAccessСontrolService
    {
        /// <summary>
        /// Копирует и возвращает копию правила доступа
        /// </summary>
        /// <param name="accessRule">Правило доступа</param>
        /// <returns>Копия правила доступа не сохраненная в базе данных</returns>
        public Task<AccessRule> GetCopyAccessRule(AccessRule accessRule);
    }
}
