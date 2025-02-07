using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    ///  Интерфейс для сервиса управления токенами
    /// </summary>
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        public bool VerifyAccessToken(string token);
    }
}
