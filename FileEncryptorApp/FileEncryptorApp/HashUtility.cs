#region Usings
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text; // Keep StringBuilder for alternative formatting
#endregion

/// <summary>
/// Provides utility functions for cryptographic hashing.
/// </summary>
public static class HashUtility // Class name unchanged
{
    #region Public Hashing Methods

    /// <summary>
    /// Computes the SHA512 hash of a specified file.
    /// </summary>
    /// <param name="targetFilePath">The full path to the file to be hashed.</param>
    /// <returns>A lowercase hexadecimal string representation of the SHA512 hash,
    /// or null if the file cannot be read or accessed.</returns>
    /// <remarks>
    /// Handles potential IOExceptions and UnauthorizedAccessExceptions during file access.
    /// </remarks>
    public static string ComputeSHA512(string targetFilePath)
    {
        // Pre-condition check
        if (string.IsNullOrWhiteSpace(targetFilePath) || !File.Exists(targetFilePath))
        {
            // Log or handle appropriately if file doesn't exist or path is invalid
            System.Diagnostics.Debug.WriteLine($"[HashUtility] File not found or path invalid: {targetFilePath}");
            return null;
        }

        byte[] computedHashBytes; // To store the resulting hash bytes

        try
        {
            // Use SHA512 algorithm implementation
            // 'using' ensures the cryptographic resource is disposed correctly
            using (SHA512 sha512Provider = SHA512.Create())
            {
                // Open the file for reading
                // 'using' ensures the file stream is closed correctly
                using (FileStream fileContentStream = File.OpenRead(targetFilePath))
                {
                    // Compute the hash from the stream content
                    computedHashBytes = sha512Provider.ComputeHash(fileContentStream);
                } // FileStream disposed here
            } // SHA512 provider disposed here
        }
        catch (IOException ioEx)
        {
            // Handle file access errors (e.g., file locked)
            System.Diagnostics.Debug.WriteLine($"[HashUtility] IO Error computing hash for {targetFilePath}: {ioEx.Message}");
            return null; // Indicate failure
        }
        catch (UnauthorizedAccessException authEx)
        {
            // Handle permission errors
            System.Diagnostics.Debug.WriteLine($"[HashUtility] Access Denied computing hash for {targetFilePath}: {authEx.Message}");
            return null; // Indicate failure
        }
        catch (Exception ex) // Catch unexpected errors
        {
            System.Diagnostics.Debug.WriteLine($"[HashUtility] Unexpected error computing hash for {targetFilePath}: {ex.Message}");
            return null; // Indicate failure
        }


        // --- Convert hash bytes to hexadecimal string ---
        // Original method: return BitConverter.ToString(computedHashBytes).Replace("-", "").ToLowerInvariant();

        // Alternative formatting using StringBuilder for potentially better performance with large hashes (though negligible here)
        StringBuilder hexBuilder = new StringBuilder(computedHashBytes.Length * 2);
        foreach (byte b in computedHashBytes)
        {
            hexBuilder.AppendFormat("{0:x2}", b); // "x2" formats the byte as a two-digit lowercase hexadecimal number
        }

        return hexBuilder.ToString(); // Return the final hex string

    } // End of ComputeSHA512

    #endregion
}