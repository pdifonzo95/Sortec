using System.Globalization;
using System.Security.Cryptography;

namespace Sortec.Infrastructure.Helpers
{
    public class CryptoHelper
    {
        private Aes aes;
        private string EncryptionKey { get; set; } = "f9Fx@2?+s*H-ws&A";

        private const int SaltBytes = 8;
        private const int IVBytes = 16;
        private readonly int MetadataSize = SaltBytes + IVBytes;
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private Rfc2898DeriveBytes pdb;

        private byte[] Salt { get; set; }

        // AES is a variant of Rijndael which has a fixed block size of 128 bits, and a key size of 128, 192, or 256 bits.
        public CryptoHelper()
        {
            aes = Aes.Create();
            aes.KeySize = aes.LegalKeySizes[0].MinSize; // 128 bits is enough, more bits -> slower performance (256 bits is aprox. 40% slower and it is a default value)
        }

        public CryptoHelper(string key)
        {
            aes = Aes.Create();
            aes.KeySize = aes.LegalKeySizes[0].MinSize; // 128 bits is enough, more bits -> slower performance (256 bits is aprox. 40% slower and it is a default value)
            EncryptionKey = key;
        }

        private void Reset()
        {
            Salt = new byte[SaltBytes];
            rngCsp.GetNonZeroBytes(Salt);
            pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.GenerateIV();
        }

        private void Set(byte[] salt, byte[] iv)
        {
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, salt))
                aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = iv;
        }

        #region String
        /*
        * [......][..............][..............
        * salt iv encrypted file
        */

        public string DecryptStringFromABAP(byte[] encryptedData)
        {
            // Obtener el salt y el IV del flujo de datos
            byte[] salt = encryptedData.Take(SaltBytes).ToArray();
            byte[] iv = encryptedData.Skip(SaltBytes).Take(IVBytes).ToArray();

            // Crear el helper para la desencriptación
            var cryptoHelper = new CryptoHelper(EncryptionKey);
            byte[] encryptedBytes = encryptedData.Skip(MetadataSize).ToArray();

            // Desencriptar los datos
            return cryptoHelper.DecryptString(encryptedBytes);
        }

        public byte[] EncryptString(string clear)
        {
            try
            {
                if (string.IsNullOrEmpty(clear))
                    return null;

                Reset();
                byte[] encryptedBytes;

                aes.Padding = PaddingMode.PKCS7; //default
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(clear);
                        }
                    }

                    encryptedBytes = msEncrypt.ToArray();
                }

                return Salt.Concat(aes.IV).Concat(encryptedBytes).ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encrypt string failed.", ex);
            }
        }

        public string DecryptString(byte[] encrypted)
        {
            try
            {
                if (encrypted == null || encrypted.Length == 0)
                    return string.Empty;

                // Other options are to use Buffer.BlockCopy or Array.Copy
                Set(encrypted.Take(SaltBytes).ToArray(), encrypted.Skip(SaltBytes).Take(IVBytes).ToArray());
                var encryptedBytes = encrypted.Skip(MetadataSize).ToArray();

                aes.Padding = PaddingMode.PKCS7; //default
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Decryption string failed.", ex);
            }
        }
        #endregion

        #region Bytes
        /*
        * PaddingMode.Zeros is necessary for avoiding exception "Padding is invalid and cannot be removed" even when salt and iv are correct.
        * Adds 0s at the end, which should be removed manually. So it is necessary to know the original length in bytes.
        * [......][..............][..............
        * salt iv encrypted file
        */
        public byte[] EncryptBytes(byte[] clear)
        {
            if (clear == null || clear.Length == 0)
                return null;

            Reset();
            byte[] encryptedBytes;

            aes.Padding = PaddingMode.Zeros; //bytes
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(clear, 0, clear.Length);
                    csEncrypt.FlushFinalBlock();
                    encryptedBytes = msEncrypt.ToArray();
                }
            }

            return Salt.Concat(aes.IV).Concat(encryptedBytes).ToArray();
        }

        public byte[] DecryptBytes(byte[] encrypted, int length)
        {
            if (encrypted == null || encrypted.Length == 0)
                return (new byte[0]);

            // Other options are to use Buffer.BlockCopy or Array.Copy
            Set(encrypted.Take(SaltBytes).ToArray(), encrypted.Skip(SaltBytes).Take(IVBytes).ToArray());
            var encryptedBytes = encrypted.Skip(MetadataSize).ToArray();

            aes.Padding = PaddingMode.Zeros; //bytes
            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(encryptedBytes, 0, encryptedBytes.Length);
                    csDecrypt.FlushFinalBlock();
                    var decrypted = msDecrypt.ToArray();

                    Array.Resize(ref decrypted, length);
                    return decrypted;
                }
            }
        }
        #endregion

        #region :: Additional methods ::
        public byte[] EncryptInt(int clear)
        {
            return EncryptString(clear.ToString());
        }

        public int DecryptInt(byte[] encrypted)
        {
            return (encrypted?.Length > 0)
            ? int.Parse(DecryptString(encrypted))
            : new int();
        }

        public byte[] EncryptDecimal(decimal clear)
        {
            return EncryptString(clear.ToString());
        }

        public decimal DecryptDecimal(byte[] encrypted)
        {
            return (encrypted?.Length > 0)
            ? decimal.Parse(DecryptString(encrypted))
            : new decimal();
        }

        public byte[] EncryptDateTimeUTC(DateTime clear)
        {
            if (clear.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTimeKind should be UTC");
            else
                return EncryptString(clear.ToString("o"));
        }

        public DateTime DecryptDateTimeUTC(byte[] encrypted)
        {
            return (encrypted?.Length > 0)
            ? DateTime.Parse(DecryptString(encrypted), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
            : new DateTime();
        }
        #endregion
    }
}