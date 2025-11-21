using master.core.Entities;

namespace master.core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetByRoleAsync(Enums.UserRole role);
        Task AddAsync(User user);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task SaveAsync();
    }
}
