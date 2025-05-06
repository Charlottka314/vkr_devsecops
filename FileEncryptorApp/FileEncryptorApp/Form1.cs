using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; 
using System.Linq;
using System.Security.Cryptography; 
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

// Namespace unchanged
namespace FileEncryptorApp
{
    // Class name unchanged, must match designer file
    public partial class Form1 : Form
    {
        #region Member Variables

        // Variables - теперь nullable, чтобы удовлетворить SonarCloud (CS8618)
        private string? _selectedBackupDestinationPath; 
        private byte[]? _currentSessionKeyMaterial;    
        private byte[]? _currentSessionIVMaterial;     

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
            // Инициализируем поля значением по умолчанию, если это возможно, или оставляем nullable
            // В данном случае, они корректно инициализируются по ходу работы или остаются null,
            // что проверяется перед использованием. Nullable reference types (?) помогают это отследить.
        }

        #endregion

        #region Initialization

        private void InitializeApplicationState()
        {
            processEncryptButton.Enabled = false;
            processDecryptButton.Enabled = false; 
            UpdateOperationStatus("Ожидание выбора файла и папки резервирования.", Color.DimGray); 
        }

        #endregion

        #region Event Handlers

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileBrowserDialog = new OpenFileDialog())
            {
                fileBrowserDialog.Title = "Выберите файл для операции";
                fileBrowserDialog.Filter = "Все файлы (*.*)|*.*";
                fileBrowserDialog.CheckFileExists = true;
                fileBrowserDialog.CheckPathExists = true;

                if (fileBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    filePathDisplayTextBox.Text = fileBrowserDialog.FileName;
                    UpdateOperationStatus("Файл выбран. Укажите папку резервирования и выберите действие.", Color.Black);
                    ValidateInputsAndUpdateUI();
                }
            }
        }

        private void btnSelectBackupFolder_Click(object sender, EventArgs e)
        {
            using (var folderSelector = backupLocationSelectorDialog) 
            {
                folderSelector.Description = "Укажите каталог для сохранения резервных копий";
                folderSelector.ShowNewFolderButton = true;

                if (folderSelector.ShowDialog(this) == DialogResult.OK)
                {
                    _selectedBackupDestinationPath = folderSelector.SelectedPath;
                    MessageBox.Show($"Каталог для резервных копий установлен: {_selectedBackupDestinationPath}",
                                    "Каталог выбран", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateOperationStatus("Папка резервирования выбрана. Выберите файл и действие.", Color.Black);
                    ValidateInputsAndUpdateUI();
                }
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string fileToProcess = filePathDisplayTextBox.Text;

            if (!ValidatePreOperation(fileToProcess, true))
            {
                return; 
            }

            if (_currentSessionKeyMaterial == null || _currentSessionIVMaterial == null)
            {
                 // Эта ветка не должна выполниться, если GenerateCryptoMaterials работает корректно
                 UpdateOperationStatus("Ошибка: Не удалось сгенерировать криптографические материалы.", Color.Red);
                 return;
            }


            try
            {
                UpdateOperationStatus("Выполняется резервное копирование...", Color.Blue);
                if (!PerformBackup(fileToProcess)) return;

                UpdateOperationStatus("Вычисляется хэш...", Color.Blue);
                string? sourceFileHash = HashUtility.ComputeSHA512(fileToProcess); // string? так как метод может вернуть null
                if (sourceFileHash == null)
                {
                    UpdateOperationStatus("Ошибка: Не удалось вычислить хэш исходного файла.", Color.Red);
                    return;
                }

                UpdateOperationStatus("Сохраняется хэш...", Color.Blue);
                if (!SaveHashToFile(fileToProcess, sourceFileHash)) return;

                UpdateOperationStatus("Генерация ключей...", Color.Blue);
                GenerateCryptoMaterials(); // Гарантированно инициализирует _currentSessionKeyMaterial и _currentSessionIVMaterial

                // Повторная проверка после GenerateCryptoMaterials (для полноты, хотя и избыточно)
                if (_currentSessionKeyMaterial == null || _currentSessionIVMaterial == null) {
                    UpdateOperationStatus("Критическая ошибка: Ключи не были сгенерированы.", Color.Red);
                    return;
                }


                UpdateOperationStatus("Выполняется шифрование...", Color.DarkCyan);
                string encryptedOutputPath = Path.ChangeExtension(fileToProcess, ENCRYPTED_FILE_EXTENSION);
                AesEncryption.EncryptFile(fileToProcess, encryptedOutputPath, _currentSessionKeyMaterial, _currentSessionIVMaterial);

                UpdateOperationStatus($"Шифрование успешно завершено. Результат: {encryptedOutputPath}", Color.Green);
            }
            catch (Exception ex)
            {
                HandleOperationError("шифрования", ex);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string fileToProcess = filePathDisplayTextBox.Text;

            if (!ValidatePreOperation(fileToProcess, false)) 
            {
                return;
            }

            // Явная проверка на null для _currentSessionKeyMaterial и _currentSessionIVMaterial
            if (_currentSessionKeyMaterial == null || _currentSessionIVMaterial == null)
            {
                MessageBox.Show("Ошибка: Материалы для дешифрования (ключ/IV) отсутствуют.\nДешифрование возможно только в той же сессии, где было выполнено шифрование.",
                                "Ошибка сессии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateOperationStatus("Ошибка: Отсутствует ключ/IV сессии.", Color.Red);
                return;
            }

            try
            {
                UpdateOperationStatus("Выполняется дешифрование...", Color.DarkCyan);
                string decryptedOutputPath = Path.ChangeExtension(fileToProcess, DECRYPTED_FILE_EXTENSION);
                AesEncryption.DecryptFile(fileToProcess, decryptedOutputPath, _currentSessionKeyMaterial, _currentSessionIVMaterial);

                UpdateOperationStatus("Дешифрование завершено. Проверка целостности...", Color.Blue);
                VerifyDecryptedFileIntegrity(fileToProcess, decryptedOutputPath);
            }
            catch (CryptographicException cryptoEx)
            {
                HandleOperationError("дешифрования (криптография)", cryptoEx);
                UpdateOperationStatus("Критическая ошибка дешифрования! Возможно, неверный ключ или файл поврежден.", Color.Red);
            }
            catch (Exception ex)
            {
                HandleOperationError("дешифрования", ex);
            }
        }

        #endregion

        #region Private Helper Methods

        private void UpdateOperationStatus(string message, Color statusColor)
        {
            if (mainStatusStrip.InvokeRequired)
            {
                mainStatusStrip.Invoke(new Action(() =>
                {
                    operationStatusLabel.Text = $"Статус: {message}";
                    operationStatusLabel.ForeColor = statusColor;
                }));
            }
            else
            {
                operationStatusLabel.Text = $"Статус: {message}";
                operationStatusLabel.ForeColor = statusColor;
            }
        }

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
                 // Проверка на null для _selectedBackupDestinationPath
                if (string.IsNullOrWhiteSpace(_selectedBackupDestinationPath) || !Directory.Exists(_selectedBackupDestinationPath))
                {
                    MessageBox.Show("Ошибка: Укажите существующий каталог для резервного копирования.", "Неверный каталог", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UpdateOperationStatus("Ошибка: Папка резервирования не выбрана или не существует.", Color.Red);
                    return false;
                }
            }
            return true;
        }

        private void ValidateInputsAndUpdateUI()
        {
            bool fileSelected = !string.IsNullOrWhiteSpace(filePathDisplayTextBox.Text) && File.Exists(filePathDisplayTextBox.Text);
            // Добавлена проверка на null для _selectedBackupDestinationPath
            bool backupFolderSelected = !string.IsNullOrWhiteSpace(_selectedBackupDestinationPath) && Directory.Exists(_selectedBackupDestinationPath);

            processEncryptButton.Enabled = fileSelected && backupFolderSelected;
            processDecryptButton.Enabled = fileSelected;
        }

        private bool PerformBackup(string sourceFilePath)
        {
            if (_selectedBackupDestinationPath == null) {
                 UpdateOperationStatus("Критическая ошибка: Путь для резервного копирования не установлен.", Color.Red);
                 return false;
            }
            try
            {
                string backupFileName = Path.GetFileName(sourceFilePath);
                string backupFullPath = Path.Combine(_selectedBackupDestinationPath, backupFileName); // _selectedBackupDestinationPath! если уверены, что не null
                File.Copy(sourceFilePath, backupFullPath, true); 
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

        private void GenerateCryptoMaterials()
        {
            _currentSessionKeyMaterial = new byte[AES_KEY_SIZE_BYTES];
            _currentSessionIVMaterial = new byte[AES_IV_SIZE_BYTES];

            // Используем криптографически стойкий генератор случайных чисел
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_currentSessionKeyMaterial);
                rng.GetBytes(_currentSessionIVMaterial);
            }

            UpdateOperationStatus("Ключ и IV сессии сгенерированы (безопасно).", Color.Blue);
        }

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
                string? actualHash = HashUtility.ComputeSHA512(decryptedFilePath); // string?

                if (actualHash == null)
                {
                    UpdateOperationStatus("Дешифровано, но не удалось вычислить хэш для проверки целостности.", Color.OrangeRed);
                }
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

        private void HandleOperationError(string operationName, Exception exceptionInfo)
        {
            string errorMessage = $"Критическая ошибка во время операции '{operationName}':\n{exceptionInfo.Message}\n\nПодробности: {exceptionInfo.StackTrace}";
            MessageBox.Show(errorMessage, $"Ошибка {operationName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateOperationStatus($"Ошибка во время операции '{operationName}'. См. детали.", Color.Red);
        }
        #endregion
    }
}