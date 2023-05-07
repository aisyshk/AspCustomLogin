using System.Security.Cryptography;
using System.Text;

namespace AspCustomLogin.Services
{
    public class LoginHandler
    {
        // Pass your IUnitOfWork here, if any.
        private readonly IUnitOfWork _uow;

        public LoginHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public (int, bool, string) Login(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                return (0, false, "");

            var user = _uow.Logins.GetAll(u => u.Username == username).FirstOrDefault();
            if (user == null) return (0, false, "");

            /// This checks if the user is banned. If they are, they cannot log in.
            if (IsBanned(user.UserId))
                return (0, false, "");

            /// Compares whether the password hashed is the password the user has.
            var hashedPassword = HashPassword(password);
            if (!VerifyPassword(hashedPassword, user.PasswordHash))
                return (0, false, "");

            return (user.UserId, true, user.Role);
        }

        /// Compares whether the password hashed is the password.
        public bool VerifyPassword(string password, string hash)
        {
            return hash == password;
        }

        /// SHA512 encrypts the password.
        public string HashPassword(string password)
        {
            var sha1 = SHA512.Create();

            var hashedBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hash;
        }

        /// This checks if the user is banned. If they are, they cannot log in.
        public bool IsBanned(int? userId)
        {
            bool? userBanned;

            if (!(userId <= 0))
            {
                userBanned = _uow.Bans.GetAll(ban => ban.UserId == userId).FirstOrDefault();
                if (userBanned != null)
                    return true;
            }

            return false;
        }

        public int UniqueId()
        {
            int id1 = new Random().Next(0, 99999999);
            int id2 = new Random().Next(0, 33);
            int id3 = new Random().Next(0, 873171);

            int idTotal = id1 + id2 + id3;

            return int.MaxValue - idTotal;
        }
    }
}