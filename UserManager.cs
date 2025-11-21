using BCrypt.Net;
using master.core.Entities;
using master.core.Enums;
using master.core.Interfaces;

namespace Master.BAL.Services
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _users;
        public UserManager(IUserRepository users) { _users = users; }

        public async Task<(bool ok, string error)> RegisterAsync(string fullName, string email, string password, UserRole role)
        {
            if (await _users.EmailExistsAsync(email)) return (false, "Email already exists");

            // Hash with BCrypt (work factor 11 default; can increase to 12-13 if needed)
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = hash,
                Role = role
            };
            await _users.AddAsync(user);
            return (true, "");
        }

        public async Task<(bool ok, User? user, string error)> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _users.GetByEmailAsync(email.Trim().ToLowerInvariant());
            if (user == null) return (false, null, "Invalid credentials");

            var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return valid ? (true, user, "") : (false, null, "Invalid credentials");
        }
    }
}
