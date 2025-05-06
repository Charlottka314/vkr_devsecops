#region Usings
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text; 
#endregion

public static class HashUtility
{
    #region Public Hashing Methods

    /// <summary>
    /// Computes the SHA512 hash of a specified file.
    /// </summary>
    /// <param name="targetFilePath">The full path to the file to be hashed.</param>
    /// <returns>A lowercase hexadecimal string representation of the SHA512 hash,
    /// or null if the file cannot be read, accessed, or if an unexpected error occurs.</returns>
    public static string? ComputeSHA512(string targetFilePath) // Возвращаемый тип теперь string?
    {
        if (string.IsNullOrWhiteSpace(targetFilePath)) // Убрана проверка File.Exists, чтобы избежать двойной проверки и потенциальных гонок состояний
        {
            System.Diagnostics.Debug.WriteLine($"[HashUtility] Target file path is null, empty, or whitespace: {targetFilePath}");
            return null;
        }

        byte[]? computedHashBytes = null; // Инициализируем null

        try
        {
            // Проверка существования файла непосредственно перед использованием
            if (!File.Exists(targetFilePath))
            {
                System.Diagnostics.Debug.WriteLine($"[HashUtility] File not found: {targetFilePath}");
                return null; // Файл не найден
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

        // Если computedHashBytes остался null (например, из-за ошибки, не пойманной выше, хотя это маловероятно),
        // или если хэш пустой (тоже маловероятно для SHA512), то вернуть null.
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