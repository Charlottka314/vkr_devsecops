#region Usings
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
#endregion

// Перемещаем класс в неймспейс FileEncryptorApp
namespace FileEncryptorApp
{
    public static class HashUtility
    {
        #region Public Hashing Methods

        public static string? ComputeSHA512(string targetFilePath)
        {
            if (string.IsNullOrWhiteSpace(targetFilePath))
            {
                System.Diagnostics.Debug.WriteLine($"[HashUtility] Target file path is null, empty, or whitespace: {targetFilePath}");
                return null;
            }

            byte[]? computedHashBytes = null;

            try
            {
                if (!File.Exists(targetFilePath))
                {
                    System.Diagnostics.Debug.WriteLine($"[HashUtility] File not found: {targetFilePath}");
                    return null;
                }

                using (SHA512 sha512Provider = SHA512.Create())
                {
                    using (FileStream fileContentStream = File.OpenRead(targetFilePath))
                    {
                        computedHashBytes = sha512Provider.ComputeHash(fileContentStream);
                    }
                }
            }
            catch (IOException ioEx)
            {
                System.Diagnostics.Debug.WriteLine($"[HashUtility] IO Error computing hash for {targetFilePath}: {ioEx.Message}");
                return null;
            }
            catch (UnauthorizedAccessException authEx)
            {
                System.Diagnostics.Debug.WriteLine($"[HashUtility] Access Denied computing hash for {targetFilePath}: {authEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HashUtility] Unexpected error computing hash for {targetFilePath}: {ex.Message}");
                return null;
            }

            if (computedHashBytes == null || computedHashBytes.Length == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[HashUtility] Computed hash is null or empty for {targetFilePath}.");
                return null;
            }

            StringBuilder hexBuilder = new StringBuilder(computedHashBytes.Length * 2);
            foreach (byte b in computedHashBytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b);
            }

            return hexBuilder.ToString();
        }
        #endregion
    }
}