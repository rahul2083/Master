using master.core.Entities;
using master.core.Enums;
using master.core.Interfaces;
using Master.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) { _db = db; }

        public Task<User?> GetByEmailAsync(string email)
            => _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public Task<User?> GetByIdAsync(int id)
            => _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        public Task<List<User>> GetAllAsync()
            => _db.Users.OrderBy(u => u.FullName).ToListAsync();

        public Task<List<User>> GetByRoleAsync(UserRole role)
            => _db.Users.Where(u => u.Role == role).OrderBy(u => u.FullName).ToListAsync();

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
            => _db.Users.AnyAsync(u => u.Email == email && (!excludeId.HasValue || u.Id != excludeId.Value));

        public Task SaveAsync() => _db.SaveChangesAsync();
    }
}
