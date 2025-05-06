using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // Required for Path, File, FileStream etc.
using System.Linq;
using System.Security.Cryptography; // Required for RandomNumberGenerator (alternative) or just Random
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Required for Form, Button, MessageBox etc.

// Namespace unchanged
namespace FileEncryptorApp
{
    // Class name unchanged, must match designer file
    public partial class Form1 : Form
    {
        #region Member Variables

        // Variables
        private string _selectedBackupDestinationPath; 
        private byte[] _currentSessionKeyMaterial;    
        private byte[] _currentSessionIVMaterial;     

        // Constants for cryptographic parameters
        private const int AES_KEY_SIZE_BYTES = 32; // 256 bits
        private const int AES_IV_SIZE_BYTES = 16;  // 128 bits

        // Constants for file extensions
        private const string ENCRYPTED_FILE_EXTENSION = ".enc"; 
        private const string HASH_FILE_EXTENSION = ".hash";     
        private const string DECRYPTED_FILE_EXTENSION = ".dec"; 

        #endregion

        #region Constructor

        public Form1()
        {
            InitializeComponent();
            InitializeApplicationState();
        }

        #endregion

        #region Initialization

        private void InitializeApplicationState()
        {
            // Set initial state, e.g., disable buttons until file/folder selected
            processEncryptButton.Enabled = false;
            processDecryptButton.Enabled = false; 
            UpdateOperationStatus("Ожидание выбора файла и папки резервирования.", Color.DimGray); 
        }

        #endregion

        #region Event Handlers (Names must match Designer)

        /// <summary>
        /// Handles the click event for the file selection button.
        /// Prompts the user to select a file via OpenFileDialog.
        /// </summary>
        private void btnSelectFile_Click(object sender, EventArgs e) // Original name kept for designer link
        {
            using (OpenFileDialog fileBrowserDialog = new OpenFileDialog())
            {
                fileBrowserDialog.Title = "Выберите файл для операции";
                fileBrowserDialog.Filter = "Все файлы (*.*)|*.*"; // Keep original filter
                fileBrowserDialog.CheckFileExists = true;
                fileBrowserDialog.CheckPathExists = true;

                if (fileBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    // Display selected path in the TextBox (using new control name)
                    filePathDisplayTextBox.Text = fileBrowserDialog.FileName;
                    UpdateOperationStatus("Файл выбран. Укажите папку резервирования и выберите действие.", Color.Black);
                    ValidateInputsAndUpdateUI(); // Check if other inputs are ready
                }
            }
        }

        /// <summary>
        /// Handles the click event for the backup folder selection button.
        /// Prompts the user to select a directory via FolderBrowserDialog.
        /// </summary>
        private void btnSelectBackupFolder_Click(object sender, EventArgs e) // Original name kept for designer link
        {
            // Use the FolderBrowserDialog placed on the form (using new control name)
            using (var folderSelector = backupLocationSelectorDialog) // Use instance directly
            {
                folderSelector.Description = "Укажите каталог для сохранения резервных копий";
                folderSelector.ShowNewFolderButton = true; // Allow creating new folders

                if (folderSelector.ShowDialog(this) == DialogResult.OK)
                {
                    _selectedBackupDestinationPath = folderSelector.SelectedPath;
                    // Provide feedback - using MessageBox as in original
                    MessageBox.Show($"Каталог для резервных копий установлен: {_selectedBackupDestinationPath}",
                                    "Каталог выбран", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateOperationStatus("Папка резервирования выбрана. Выберите файл и действие.", Color.Black);
                    ValidateInputsAndUpdateUI(); // Check if other inputs are ready
                }
            }
        }

        /// <summary>
        /// Handles the click event for the encryption button.
        /// Performs backup, hashing, and AES encryption.
        /// </summary>
        private void btnEncrypt_Click(object sender, EventArgs e) // Original name kept for designer link
        {
            string fileToProcess = filePathDisplayTextBox.Text; // Use new control name

            // --- Input Validation ---
            if (!ValidatePreOperation(fileToProcess, true)) // True checks for backup folder
            {
                return; // Validation failed, messages shown within the method
            }

            // --- Processing Steps ---
            try
            {
                UpdateOperationStatus("Выполняется резервное копирование...", Color.Blue);
                if (!PerformBackup(fileToProcess)) return; // Backup failed

                UpdateOperationStatus("Вычисляется хэш...", Color.Blue);
                string sourceFileHash = HashUtility.ComputeSHA512(fileToProcess);
                if (sourceFileHash == null)
                {
                    UpdateOperationStatus("Ошибка: Не удалось вычислить хэш исходного файла.", Color.Red);
                    // Consider if backup should be deleted here
                    return;
                }

                UpdateOperationStatus("Сохраняется хэш...", Color.Blue);
                if (!SaveHashToFile(fileToProcess, sourceFileHash)) return; // Hash saving failed

                UpdateOperationStatus("Генерация ключей...", Color.Blue);
                GenerateCryptoMaterials(); // Generate Key and IV

                UpdateOperationStatus("Выполняется шифрование...", Color.DarkCyan);
                string encryptedOutputPath = Path.ChangeExtension(fileToProcess, ENCRYPTED_FILE_EXTENSION);
                AesEncryption.EncryptFile(fileToProcess, encryptedOutputPath, _currentSessionKeyMaterial, _currentSessionIVMaterial);

                // --- Completion ---
                UpdateOperationStatus($"Шифрование успешно завершено. Результат: {encryptedOutputPath}", Color.Green);
                // Optionally update textbox to show the encrypted file path
                // filePathDisplayTextBox.Text = encryptedOutputPath; 
            }
            catch (Exception ex)
            {
                HandleOperationError("шифрования", ex);
            }
        }

        /// <summary>
        /// Handles the click event for the decryption button.
        /// Performs AES decryption and hash verification.
        /// </summary>
        private void btnDecrypt_Click(object sender, EventArgs e) // Original name kept for designer link
        {
            string fileToProcess = filePathDisplayTextBox.Text; // Use new control name

            // --- Input Validation ---
            if (!ValidatePreOperation(fileToProcess, false)) // False: don't check backup folder for decryption
            {
                return;
            }
            if (_currentSessionKeyMaterial == null || _currentSessionIVMaterial == null)
            {
                MessageBox.Show("Ошибка: Материалы для дешифрования (ключ/IV) отсутствуют.\nДешифрование возможно только в той же сессии, где было выполнено шифрование.",
                                "Ошибка сессии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateOperationStatus("Ошибка: Отсутствует ключ/IV сессии.", Color.Red);
                return;
            }

            // --- Processing Steps ---
            try
            {
                UpdateOperationStatus("Выполняется дешифрование...", Color.DarkCyan);
                string decryptedOutputPath = Path.ChangeExtension(fileToProcess, DECRYPTED_FILE_EXTENSION);
                AesEncryption.DecryptFile(fileToProcess, decryptedOutputPath, _currentSessionKeyMaterial, _currentSessionIVMaterial);

                UpdateOperationStatus("Дешифрование завершено. Проверка целостности...", Color.Blue);
                VerifyDecryptedFileIntegrity(fileToProcess, decryptedOutputPath);

                // Optionally update textbox to show the decrypted file path
                // filePathDisplayTextBox.Text = decryptedOutputPath;
            }
            catch (CryptographicException cryptoEx) // Catch specific crypto errors (bad key, padding, etc.)
            {
                HandleOperationError("дешифрования (криптография)", cryptoEx);
                UpdateOperationStatus("Критическая ошибка дешифрования! Возможно, неверный ключ или файл поврежден.", Color.Red);
            }
            catch (Exception ex)
            {
                HandleOperationError("дешифрования", ex);
            }
        }


        /// <summary>
        /// Placeholder for the label click event handler, as present in the original code.
        /// Can be removed if the label is not intended to be interactive.
        /// </summary>
        private void label1_Click(object sender, EventArgs e) // Original name kept for designer link
        {
            // This handler was empty in the original code.
            // Add functionality here if the status label should be clickable.
            // For instance, copy status text to clipboard:
            // if (!string.IsNullOrEmpty(operationStatusLabel.Text))
            // {
            //     Clipboard.SetText(operationStatusLabel.Text);
            //     MessageBox.Show("Статус скопирован в буфер обмена.");
            // }
        }


        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Updates the status label text and color.
        /// Ensures thread safety if called from a different thread (though unlikely in this simple app).
        /// </summary>
        private void UpdateOperationStatus(string message, Color statusColor)
        {
            // Check InvokeRequired on the PARENT control (StatusStrip), not the label itself
            if (mainStatusStrip.InvokeRequired)
            {
                // Invoke on the PARENT control (StatusStrip)
                mainStatusStrip.Invoke(new Action(() =>
                {
                    // Inside the invoked action, we can safely update the label properties
                    operationStatusLabel.Text = $"Статус: {message}";
                    operationStatusLabel.ForeColor = statusColor; // Setting ForeColor on ToolStripStatusLabel might be affected by themes/renderers
                }));
            }
            else
            {
                // Already on the UI thread, update directly
                operationStatusLabel.Text = $"Статус: {message}";
                operationStatusLabel.ForeColor = statusColor;
            }
        }

        /// <summary>
        /// Validates required inputs (file path, backup folder if needed) before starting an operation.
        /// </summary>
        /// <returns>True if inputs are valid, False otherwise.</returns>
        private bool ValidatePreOperation(string filePath, bool checkBackupFolder)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("Ошибка: Укажите существующий файл для операции.", "Неверный файл", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateOperationStatus("Ошибка: Файл не выбран или не существует.", Color.Red);
                return false;
            }

            if (checkBackupFolder)
            {
                if (string.IsNullOrWhiteSpace(_selectedBackupDestinationPath) || !Directory.Exists(_selectedBackupDestinationPath))
                {
                    MessageBox.Show("Ошибка: Укажите существующий каталог для резервного копирования.", "Неверный каталог", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UpdateOperationStatus("Ошибка: Папка резервирования не выбрана или не существует.", Color.Red);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Enables or disables operation buttons based on whether required inputs are present.
        /// </summary>
        private void ValidateInputsAndUpdateUI()
        {
            bool fileSelected = !string.IsNullOrWhiteSpace(filePathDisplayTextBox.Text) && File.Exists(filePathDisplayTextBox.Text);
            bool backupFolderSelected = !string.IsNullOrWhiteSpace(_selectedBackupDestinationPath) && Directory.Exists(_selectedBackupDestinationPath);

            // Enable Encrypt only if both file and backup folder are selected
            processEncryptButton.Enabled = fileSelected && backupFolderSelected;

            // Enable Decrypt only if a file is selected (key/IV check happens later)
            processDecryptButton.Enabled = fileSelected;
        }


        /// <summary>
        /// Creates a backup copy of the source file in the designated backup location.
        /// </summary>
        /// <returns>True if backup was successful, False otherwise.</returns>
        private bool PerformBackup(string sourceFilePath)
        {
            try
            {
                string backupFileName = Path.GetFileName(sourceFilePath);
                string backupFullPath = Path.Combine(_selectedBackupDestinationPath, backupFileName);
                File.Copy(sourceFilePath, backupFullPath, true); // Allow overwrite as in original
                UpdateOperationStatus($"Резервная копия создана: {backupFullPath}", Color.DarkBlue);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось создать резервную копию файла:\n{ex.Message}",
                                "Ошибка резервирования", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateOperationStatus($"Ошибка создания резервной копии: {ex.Message}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Saves the calculated hash value to a file with the .hash extension.
        /// </summary>
        /// <returns>True if saving was successful, False otherwise.</returns>
        private bool SaveHashToFile(string originalFilePath, string hashValue)
        {
            try
            {
                string hashStoragePath = Path.ChangeExtension(originalFilePath, HASH_FILE_EXTENSION);
                File.WriteAllText(hashStoragePath, hashValue);
                UpdateOperationStatus($"Хэш сохранен: {hashStoragePath}", Color.DarkBlue);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить хэш-файл:\n{ex.Message}",
                                 "Ошибка сохранения хэша", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateOperationStatus($"Ошибка сохранения хэша: {ex.Message}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Generates random Key and IV using the (insecure) Random class, matching original behavior.
        /// Stores them in the member variables _currentSessionKeyMaterial and _currentSessionIVMaterial.
        /// </summary>
        private void GenerateCryptoMaterials()
        {
            _currentSessionKeyMaterial = new byte[AES_KEY_SIZE_BYTES];
            _currentSessionIVMaterial = new byte[AES_IV_SIZE_BYTES];

            // *** WARNING: Using System.Random is NOT cryptographically secure! ***
            // Kept here to match the original code's functionality exactly.
            // For real applications, use System.Security.Cryptography.RandomNumberGenerator:
            // using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            // {
            //     rng.GetBytes(_currentSessionKeyMaterial);
            //     rng.GetBytes(_currentSessionIVMaterial);
            // }
            Random nonCryptoRandom = new Random(); // Matches original
            nonCryptoRandom.NextBytes(_currentSessionKeyMaterial);
            nonCryptoRandom.NextBytes(_currentSessionIVMaterial);

            UpdateOperationStatus("Ключ и IV сессии сгенерированы.", Color.Blue);
        }

        /// <summary>
        /// Verifies the integrity of the decrypted file by comparing its hash
        /// with the hash stored in the corresponding .hash file.
        /// </summary>
        private void VerifyDecryptedFileIntegrity(string originalEncryptedPath, string decryptedFilePath)
        {
            string expectedHashFilePath = Path.ChangeExtension(originalEncryptedPath, HASH_FILE_EXTENSION);

            if (!File.Exists(expectedHashFilePath))
            {
                UpdateOperationStatus("Дешифровано, но проверка целостности невозможна (файл хэша отсутствует).", Color.Orange);
                return;
            }

            try
            {
                string expectedHash = File.ReadAllText(expectedHashFilePath);
                string actualHash = HashUtility.ComputeSHA512(decryptedFilePath);

                if (actualHash == null)
                {
                    UpdateOperationStatus("Дешифровано, но не удалось вычислить хэш для проверки целостности.", Color.OrangeRed);
                }
                // Perform case-insensitive comparison as hash is stored lowercase
                else if (string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase))
                {
                    UpdateOperationStatus("Файл успешно дешифрован. Целостность данных подтверждена.", Color.Green);
                }
                else
                {
                    UpdateOperationStatus("ВНИМАНИЕ! Файл дешифрован, но целостность данных НАРУШЕНА (хэши не совпадают).", Color.Red);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке целостности файла:\n{ex.Message}",
                                "Ошибка проверки хэша", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateOperationStatus($"Ошибка проверки целостности: {ex.Message}", Color.OrangeRed);
            }
        }


        /// <summary>
        /// Centralized error handler for major operations.
        /// </summary>
        private void HandleOperationError(string operationName, Exception exceptionInfo)
        {
            string errorMessage = $"Критическая ошибка во время операции '{operationName}':\n{exceptionInfo.Message}\n\nПодробности: {exceptionInfo.StackTrace}";
            MessageBox.Show(errorMessage, $"Ошибка {operationName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateOperationStatus($"Ошибка во время операции '{operationName}'. См. детали.", Color.Red);
            // Consider logging the full exceptionInfo here
        }


        #endregion

    } // End of Form1 class
} // End of namespace