using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AppUser> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            byte[] computedHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                /*                 
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                } 
                */
            }

            return computedHash.SequenceEqual(passwordHash);
        }

        public async Task<AppUser> Register(AppUser user, string password)
        {
            byte[] passwordHash, passworSalt;
            CreatePasswordHash(password, out passwordHash, out passworSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passworSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passworSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passworSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public Task<bool> UserExists(string username)
        {
            return _context.Users.AnyAsync(u => u.UserName == username);
        }
    }
}