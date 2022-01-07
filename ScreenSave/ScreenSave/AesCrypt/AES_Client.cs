using CoreApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class AES_Client
    {

        private static string AesIV128 = "ObcUsyrzyNQTJHdD";
        private static string AesKey256 = "dYWyeOgFlXseoMQDZUmsPrDbkasiHCDI";   //32 chars = 256
       
        public static string Encrypt256(string text)
        {
            // AesCryptoServiceProvider
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV128);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Convert string to byte array
            byte[] src = Encoding.Unicode.GetBytes(text);

            // encryption
            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                // Convert byte array to Base64 strings
                return Convert.ToBase64String(dest);
            }
        }

        public static string Decrypt256(string text)
        {
            // AesCryptoServiceProvider
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV128);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Convert Base64 strings to byte array
            byte[] src = System.Convert.FromBase64String(text);

            // decryption
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                return Encoding.Unicode.GetString(dest);
            }
        }
    }
}



#region testing
//private static string AesIV128 = "SJqZjVlLHtHeHEoW";
//private static string AesKey256 = "CyItzzViAcUyAJhjAOFxKlZDdJtJNSys";   //32 chars = 256

//public static string Encrypt(string plainText)
//{
//    // Check arguments.
//    if (plainText == null || plainText.Length <= 0)
//        throw new ArgumentNullException("plainText");
//    if (AesKey256 == null || AesKey256.Length <= 0)
//        throw new ArgumentNullException("Key");
//    if (AesIV128 == null || AesIV128.Length <= 0)
//        throw new ArgumentNullException("IV");
//    byte[] encrypted;

//    // Create an Aes object
//    // with the specified key and IV.
//    using (Aes aesAlg = Aes.Create())
//    {
//        aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(AesKey256);
//        aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(AesIV128);

//        // Create an encryptor to perform the stream transform.
//        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

//        // Create the streams used for encryption.
//        using (MemoryStream msEncrypt = new MemoryStream())
//        {
//            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
//            {
//                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
//                {
//                    //Write all data to the stream.
//                    swEncrypt.Write(plainText);
//                }
//                encrypted = msEncrypt.ToArray();
//            }
//        }
//    }


//    return Convert.ToBase64String(encrypted);
//}

//public static string Decrypt(string encrypted)
//{
//    // Check arguments.
//    if (encrypted == null || encrypted.Length <= 0)
//        throw new ArgumentNullException("cipherText");
//    if (AesKey256 == null || AesKey256.Length <= 0)
//        throw new ArgumentNullException("Key");
//    if (AesIV128 == null || AesIV128.Length <= 0)
//        throw new ArgumentNullException("IV");

//    // Declare the string used to hold
//    // the decrypted text.
//    string plaintext = null;


//    // Create an Aes object
//    // with the specified key and IV.
//    using (Aes aesAlg = Aes.Create())
//    {
//        aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(AesKey256);
//        aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(AesIV128);

//        // Create a decryptor to perform the stream transform.
//        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

//        // Create the streams used for decryption.
//        using (MemoryStream msDecrypt = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(encrypted)))
//        {
//            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//            {
//                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
//                {

//                    // Read the decrypted bytes from the decrypting stream
//                    // and place them in a string.
//                    plaintext = srDecrypt.ReadToEnd();
//                }
//            }
//        }
//    }
//    return plaintext;
//}



//    public static string Encrypt256(string text)
//    {
//        // AesCryptoServiceProvider
//        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
//        aes.BlockSize = 128;
//        aes.KeySize = 256;
//        aes.IV = Encoding.UTF8.GetBytes(AesIV256);
//        aes.Key = Encoding.UTF8.GetBytes(AesKey256);
//        aes.Mode = CipherMode.CBC;
//        aes.Padding = PaddingMode.PKCS7;

//        // Convert string to byte array
//        byte[] src = Encoding.Unicode.GetBytes(text);

//        // encryption
//        using (ICryptoTransform encrypt = aes.CreateEncryptor())
//        {
//            byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

//            // Convert byte array to Base64 strings
//            return Convert.ToBase64String(dest);
//        }
//    }

//    /// <summary>
//    /// AES decryption
//    /// </summary>
//    public static string Decrypt256(string text)
//    {
//        // AesCryptoServiceProvider
//        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
//        aes.BlockSize = 128;
//        aes.KeySize = 256;
//        aes.IV = Encoding.UTF8.GetBytes(AesIV256);
//        aes.Key = Encoding.UTF8.GetBytes(AesKey256);
//        aes.Mode = CipherMode.CBC;
//        aes.Padding = PaddingMode.PKCS7;

//        // Convert Base64 strings to byte array
//        byte[] src = System.Convert.FromBase64String(text);

//        // decryption
//        using (ICryptoTransform decrypt = aes.CreateDecryptor())
//        {
//            byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
//            return Encoding.Unicode.GetString(dest);
//        }
//    }
#endregion