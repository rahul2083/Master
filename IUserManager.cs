using master.core.Entities;
using master.core.Enums;

namespace master.core.Interfaces
{
    public interface IUserManager
    {
        Task<(bool ok, string error)> RegisterAsync(string fullName, string email, string password, UserRole role);
        Task<(bool ok, User? user, string error)> ValidateCredentialsAsync(string email, string password);
    }

}
