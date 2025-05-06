#region Usings
using System;
using System.IO;
using System.Security.Cryptography;
#endregion

public static class AesEncryption
{
    private const int BufferSize = 4096; 

    #region Public API

    public static void EncryptFile(string sourcePath, string destinationPath, byte[] secretKey, byte[] initializationVector)
    {
        if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
        if (string.IsNullOrEmpty(destinationPath)) throw new ArgumentNullException(nameof(destinationPath));
        if (secretKey == null || secretKey.Length == 0) throw new ArgumentNullException(nameof(secretKey));
        if (initializationVector == null || initializationVector.Length == 0) throw new ArgumentNullException(nameof(initializationVector));

        PerformCryptoOperation(sourcePath, destinationPath, secretKey, initializationVector, CryptoDirection.Encrypt);
    }

    public static void DecryptFile(string encryptedSourcePath, string decryptedDestinationPath, byte[] secretKey, byte[] initializationVector)
    {
        if (string.IsNullOrEmpty(encryptedSourcePath)) throw new ArgumentNullException(nameof(encryptedSourcePath));
        if (string.IsNullOrEmpty(decryptedDestinationPath)) throw new ArgumentNullException(nameof(decryptedDestinationPath));
        if (secretKey == null || secretKey.Length == 0) throw new ArgumentNullException(nameof(secretKey));
        if (initializationVector == null || initializationVector.Length == 0) throw new ArgumentNullException(nameof(initializationVector));

        PerformCryptoOperation(encryptedSourcePath, decryptedDestinationPath, secretKey, initializationVector, CryptoDirection.Decrypt);
    }

    #endregion

    #region Private Helpers

    private enum CryptoDirection { Encrypt, Decrypt }

    private static void PerformCryptoOperation(string inputFilePath, string outputFilePath, byte[] keyMaterial, byte[] ivMaterial, CryptoDirection direction)
    {
        using (Aes aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.Key = keyMaterial;
            aesAlgorithm.IV = ivMaterial;
            // Явно указываем режим шифрования и дополнения для ясности и безопасности.
            // CBC является распространенным и безопасным режимом при правильном использовании IV.
            // PKCS7 является стандартным режимом дополнения.
            aesAlgorithm.Mode = CipherMode.CBC; 
            aesAlgorithm.Padding = PaddingMode.PKCS7;

            ICryptoTransform cryptoTransformer = (direction == CryptoDirection.Encrypt)
                ? aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV)
                : aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);

            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (cryptoTransformer) 
            {
                CryptoStreamMode streamMode = (direction == CryptoDirection.Encrypt) ? CryptoStreamMode.Write : CryptoStreamMode.Read;

                if (direction == CryptoDirection.Encrypt)
                {
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, cryptoTransformer, streamMode))
                    {
                        inputStream.CopyTo(cryptoStream, BufferSize);
                    }
                }
                else 
                {
                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, cryptoTransformer, streamMode))
                    {
                        cryptoStream.CopyTo(outputStream, BufferSize);
                    }
                }
            } 
        } 
    }

    #endregion
}