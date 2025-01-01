using System;
using System.Security.Cryptography;
using System.Text;
namespace GiftStore.Services
{
    public class InfoSec
    {
        public string GenerateKey()
        {
            string keyBase64 = "";
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                keyBase64 = Convert.ToBase64String(aes.Key);


            }
            return keyBase64;
        }

        public string Encrypt(string PlainText, string Key,out string IVKey)
        {
           

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(Key);
                aes.GenerateIV();
                IVKey = Convert.ToBase64String(aes.IV);
                ICryptoTransform encryptor = aes.CreateEncryptor();


                byte[] encryptedData;


                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(PlainText);
                        }
                        encryptedData = ms.ToArray();

                    }

                }
                return Convert.ToBase64String(encryptedData);
            }



        }





        public string Decrypt(string cipherText, string key, string ivKey)
        {
            using (Aes aes = Aes.Create())
            {
                // Set padding mode to Zeros
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(ivKey);

                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Convert the cipher text to a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the data
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    // Read the decrypted data
                    string plainText = sr.ReadToEnd();

                    // Remove padding (null characters)
                    plainText = plainText.TrimEnd('\0');

                    return plainText;
                }
            }
        

        }
        public string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            string randomNumber = string.Empty;

            for (int i = 0; i < 10; i++)
            {
                randomNumber += random.Next(0, 10).ToString();
            }

            return randomNumber;
        }
    }



}
