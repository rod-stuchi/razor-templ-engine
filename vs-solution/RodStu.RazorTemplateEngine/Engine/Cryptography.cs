using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RodStu.RazorTemplateEngine.Engine
{
    public class Cryptography
    {
        private Random random = new Random((int)DateTime.Now.Ticks);
        private RijndaelManaged aes = new RijndaelManaged();
        private byte[] Salt;
        private int iterations;

        public string SaltStr { get; set; }
        public string InitializeVector { get; set; }

        public Cryptography(string strPassword)
        {
            SaltStr = Guid.NewGuid().ToString().ToUpper().Replace("-", String.Empty);
            Salt = System.Text.Encoding.UTF8.GetBytes(SaltStr);
            InitializeVector = GetRandomHexNumber(32);
            iterations = 1000;

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.IV = HexToByteUsingByteManipulation(InitializeVector);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = GenerateKey(CreateSHA512Hash(strPassword));
        }

        private byte[] GenerateKey(string strPassword)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), Salt, iterations);

            return rfc2898.GetBytes(aes.KeySize / 8);
        }

        public string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        public string Encrypt(string strPlainText)
        {
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
            ICryptoTransform transform = aes.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

            return Convert.ToBase64String(cipherText);
        }

        public string Decrypt(string encryptedText)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            ICryptoTransform transform = aes.CreateDecryptor();
            byte[] cipherText = transform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return System.Text.Encoding.UTF8.GetString(cipherText);
        }

        public static byte[] HexToByteUsingByteManipulation(string s)
        {
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int hi = s[i * 2] - 65;
                hi = hi + 10 + ((hi >> 31) & 7);

                int lo = s[i * 2 + 1] - 65;
                lo = lo + 10 + ((lo >> 31) & 7) & 0x0f;

                bytes[i] = (byte)(lo | hi << 4);
            }
            return bytes;
        }

        public static byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        public static string CreateSHA512Hash(string Phrase)
        {
            using (SHA512Managed HashTool = new SHA512Managed())
            {
                Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(Phrase));
                Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
                return Convert.ToBase64String(EncryptedBytes);
            }
        }

        public static string FormatCipherHtml(string cipherHtml)
        {
            StringBuilder sb = new StringBuilder();

            char[] array = cipherHtml.ToCharArray();
            int lineLength = 200;

            for (int i = 0, l = cipherHtml.Length; i < l; i = i + lineLength)
            {
                if ((i + lineLength) < l)
                {
                    sb.Append(String.Format("\"{0}\",\r\n", String.Join("", array.Skip(i).Take(lineLength))));
                }
                else
                {
                    sb.Append(String.Format("\"{0}\"", String.Join("", array.Skip(i).Take(lineLength))));
                }
            }
            return sb.ToString();
        }
    }
}
