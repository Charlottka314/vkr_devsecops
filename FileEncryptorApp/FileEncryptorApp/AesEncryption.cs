#region Usings
using System;
using System.IO;
using System.Security.Cryptography;
#endregion

/// <summary>
/// Provides static methods for AES file encryption and decryption operations.
/// Relies on caller providing valid key and IV.
/// </summary>
public static class AesEncryption // Class name unchanged
{
    // --- Constants ---
    private const int BufferSize = 4096; // Define a buffer size for copying

    #region Public API

    /// <summary>
    /// Encrypts the contents of a source file and writes the result to a target file using AES.
    /// </summary>
    /// <param name="sourcePath">The path to the file to encrypt.</param>
    /// <param name="destinationPath">The path where the encrypted file will be saved.</param>
    /// <param name="secretKey">The secret key for AES encryption (e.g., 32 bytes for AES-256).</param>
    /// <param name="initializationVector">The initialization vector (IV) for AES (e.g., 16 bytes).</param>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
    /// <exception cref="CryptographicException">Thrown if encryption fails.</exception>
    /// <exception cref="IOException">Thrown if file I/O fails.</exception>
    public static void EncryptFile(string sourcePath, string destinationPath, byte[] secretKey, byte[] initializationVector)
    {
        // Argument validation
        if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
        if (string.IsNullOrEmpty(destinationPath)) throw new ArgumentNullException(nameof(destinationPath));
        if (secretKey == null || secretKey.Length == 0) throw new ArgumentNullException(nameof(secretKey));
        if (initializationVector == null || initializationVector.Length == 0) throw new ArgumentNullException(nameof(initializationVector));

        // Core encryption logic
        PerformCryptoOperation(sourcePath, destinationPath, secretKey, initializationVector, CryptoDirection.Encrypt);
    }

    /// <summary>
    /// Decrypts the contents of an encrypted source file and writes the result to a target file using AES.
    /// </summary>
    /// <param name="encryptedSourcePath">The path to the file to decrypt.</param>
    /// <param name="decryptedDestinationPath">The path where the decrypted file will be saved.</param>
    /// <param name="secretKey">The secret key used for the original encryption.</param>
    /// <param name="initializationVector">The initialization vector (IV) used for the original encryption.</param>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
    /// <exception cref="CryptographicException">Thrown if decryption fails (e.g., bad key, IV, or corrupted data).</exception>
    /// <exception cref="IOException">Thrown if file I/O fails.</exception>
    public static void DecryptFile(string encryptedSourcePath, string decryptedDestinationPath, byte[] secretKey, byte[] initializationVector)
    {
        // Argument validation
        if (string.IsNullOrEmpty(encryptedSourcePath)) throw new ArgumentNullException(nameof(encryptedSourcePath));
        if (string.IsNullOrEmpty(decryptedDestinationPath)) throw new ArgumentNullException(nameof(decryptedDestinationPath));
        if (secretKey == null || secretKey.Length == 0) throw new ArgumentNullException(nameof(secretKey));
        if (initializationVector == null || initializationVector.Length == 0) throw new ArgumentNullException(nameof(initializationVector));

        // Core decryption logic
        PerformCryptoOperation(encryptedSourcePath, decryptedDestinationPath, secretKey, initializationVector, CryptoDirection.Decrypt);
    }

    #endregion

    #region Private Helpers

    // Enum to define the operation type
    private enum CryptoDirection { Encrypt, Decrypt }

    /// <summary>
    /// Internal worker method to handle both encryption and decryption stream setup and processing.
    /// </summary>
    private static void PerformCryptoOperation(string inputFilePath, string outputFilePath, byte[] keyMaterial, byte[] ivMaterial, CryptoDirection direction)
    {
        // Initialize AES provider within a using block for proper disposal
        using (Aes aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.Key = keyMaterial;
            aesAlgorithm.IV = ivMaterial;
            // Consider adding Mode and Padding specification explicitly for clarity, though defaults are usually fine.
            // aesAlgorithm.Mode = CipherMode.CBC;
            // aesAlgorithm.Padding = PaddingMode.PKCS7;

            // Create the appropriate transform (Encryptor or Decryptor)
            ICryptoTransform cryptoTransformer = (direction == CryptoDirection.Encrypt)
                ? aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV)
                : aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);

            // Set up the file streams and the crypto stream
            // Using nested 'using' statements ensures all streams are closed and disposed correctly, even if errors occur.
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (cryptoTransformer) // Dispose the transformer
            {
                // The CryptoStreamMode depends on the direction of the operation relative to the *output* stream
                CryptoStreamMode streamMode = (direction == CryptoDirection.Encrypt) ? CryptoStreamMode.Write : CryptoStreamMode.Read;

                if (direction == CryptoDirection.Encrypt)
                {
                    // Encrypt: Write to CryptoStream which writes to FileStream
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, cryptoTransformer, streamMode))
                    {
                        inputStream.CopyTo(cryptoStream, BufferSize);
                        // FlushFinalBlock is implicitly called when CryptoStream is disposed for Write mode
                    }
                }
                else // Decrypt
                {
                    // Decrypt: Read from CryptoStream which reads from FileStream
                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, cryptoTransformer, streamMode))
                    {
                        cryptoStream.CopyTo(outputStream, BufferSize);
                    }
                }
            } // End of using blocks for streams
        } // End of using block for Aes
    }

    #endregion
}