using System.Security.Cryptography;
using System.Text;

namespace Sortec.Infrastructure.Helpers
{
    public class HashHelper
    {
        //Return a 256bit (32 bytes) hash of string
        public static byte[] ToHash(string clear) 
        {
            using (var sha256 = SHA256.Create())
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(clear));
        }
    }
}