using System.Security.Cryptography;
using System.Text;

namespace AspCustomLogin.Services
{
    public class LoginHandler
    {
        // Pass your IUnitOfWork here, if any.
        private readonly IUnitOfWork _uow;
        public byte[] Salt = new byte[16];

        public LoginHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public (int, bool, string) Login(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                return (0, false, "");

            password = ToBase(password);

            var user = _uow.Logins.GetAll(u => u.Username == username).FirstOrDefault();
            if (user == null) return (0, false, "");

            /// This checks if the user is banned. If they are, they cannot log in.
            if (IsBanned(user.UserId))
                return (0, false, "");

            new RNGCryptoServiceProvider().GetBytes(Salt);
            byte[] hash = HashPassword(password, Salt);
            Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");

            /// Compares whether the password hashed is the password the user has.
            byte[] hashToVerify = HashPassword(user.PasswordHash, Salt);
            bool passwordsMatch = hashToVerify.SequenceEqual(hash);
            if (!passwordsMatch)
                return (0, false, "");

            return (user.UserId, true, user.Role);
        }

        /// Encrypts the password.
        public byte[] HashPassword(string password, byte[] salt)
        {
            var sha256 = new SHA256Managed();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordAndSaltBytes = new byte[passwordBytes.Length + salt.Length];

            Buffer.BlockCopy(passwordBytes, 0, passwordAndSaltBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, passwordAndSaltBytes, passwordBytes.Length, salt.Length);

            return sha256.ComputeHash(passwordAndSaltBytes);
        }

        /// Converts the password to Base64.
        public string ToBase(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
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