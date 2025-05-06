// Namespace unchanged
namespace FileEncryptorApp
{
    // Class name unchanged
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            // --- Icons would be loaded from project resources ---
            // System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            this.filePathDisplayTextBox = new System.Windows.Forms.TextBox();
            this.chooseDataSourceButton = new System.Windows.Forms.Button();
            this.processEncryptButton = new System.Windows.Forms.Button();
            this.processDecryptButton = new System.Windows.Forms.Button();
            this.backupLocationSelectorDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.configureBackupLocationButton = new System.Windows.Forms.Button();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.operationStatusLabel = new System.Windows.Forms.ToolStripStatusLabel(); // Changed to ToolStripStatusLabel
            this.fileSelectionGroupBox = new System.Windows.Forms.GroupBox();
            this.actionsGroupBox = new System.Windows.Forms.GroupBox();
            this.backupGroupBox = new System.Windows.Forms.GroupBox();
            this.mainStatusStrip.SuspendLayout();
            this.fileSelectionGroupBox.SuspendLayout();
            this.actionsGroupBox.SuspendLayout();
            this.backupGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // filePathDisplayTextBox
            // 
            this.filePathDisplayTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathDisplayTextBox.BackColor = System.Drawing.SystemColors.Window; // Standard white background
            this.filePathDisplayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filePathDisplayTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.filePathDisplayTextBox.Location = new System.Drawing.Point(15, 28);
            this.filePathDisplayTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.filePathDisplayTextBox.Name = "filePathDisplayTextBox";
            this.filePathDisplayTextBox.ReadOnly = true; // Kept read-only
            this.filePathDisplayTextBox.Size = new System.Drawing.Size(440, 27); // Adjusted size
            this.filePathDisplayTextBox.TabIndex = 0;
            this.filePathDisplayTextBox.Text = "Файл не выбран..."; // Placeholder text
            // 
            // chooseDataSourceButton
            // 
            this.chooseDataSourceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseDataSourceButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225))))); // Light gray background
            this.chooseDataSourceButton.FlatAppearance.BorderColor = System.Drawing.Color.Silver; // Subtle border
            this.chooseDataSourceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat; // Flat style
            this.chooseDataSourceButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chooseDataSourceButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))); // Darker text
            this.chooseDataSourceButton.Location = new System.Drawing.Point(463, 25); // Adjusted position
            this.chooseDataSourceButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chooseDataSourceButton.Name = "chooseDataSourceButton";
            this.chooseDataSourceButton.Size = new System.Drawing.Size(120, 34); // Slightly wider
            this.chooseDataSourceButton.TabIndex = 1;
            this.chooseDataSourceButton.Text = "Обзор..."; // Changed text slightly
            this.chooseDataSourceButton.UseVisualStyleBackColor = false; // Important for custom colors
            this.chooseDataSourceButton.Click += new System.EventHandler(this.btnSelectFile_Click); // Event handler name kept
            // 
            // processEncryptButton
            // 
            this.processEncryptButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79))))); // Reddish color for danger/lock
            this.processEncryptButton.FlatAppearance.BorderSize = 0; // No border
            this.processEncryptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.processEncryptButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.processEncryptButton.ForeColor = System.Drawing.Color.White; // White text
            // this.processEncryptButton.Image = global::FileEncryptorApp.Properties.Resources.LockIcon; // Placeholder for icon
            this.processEncryptButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.processEncryptButton.Location = new System.Drawing.Point(15, 30); // Position within GroupBox
            this.processEncryptButton.Margin = new System.Windows.Forms.Padding(4, 5, 10, 5); // Add right margin
            this.processEncryptButton.Name = "processEncryptButton";
            this.processEncryptButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0); // Padding for icon space
            this.processEncryptButton.Size = new System.Drawing.Size(180, 45); // Consistent size
            this.processEncryptButton.TabIndex = 0; // TabIndex within GroupBox
            this.processEncryptButton.Text = "  Шифрование"; // Text unchanged, added space for icon
            this.processEncryptButton.UseVisualStyleBackColor = false;
            this.processEncryptButton.Click += new System.EventHandler(this.btnEncrypt_Click); // Event handler name kept
            // 
            // processDecryptButton
            // 
            this.processDecryptButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(184)))), ((int)(((byte)(92))))); // Greenish color for success/unlock
            this.processDecryptButton.FlatAppearance.BorderSize = 0;
            this.processDecryptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.processDecryptButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.processDecryptButton.ForeColor = System.Drawing.Color.White;
            // this.processDecryptButton.Image = global::FileEncryptorApp.Properties.Resources.UnlockIcon; // Placeholder for icon
            this.processDecryptButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.processDecryptButton.Location = new System.Drawing.Point(209, 30); // Position next to encrypt button
            this.processDecryptButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.processDecryptButton.Name = "processDecryptButton";
            this.processDecryptButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.processDecryptButton.Size = new System.Drawing.Size(180, 45); // Consistent size
            this.processDecryptButton.TabIndex = 1; // TabIndex within GroupBox
            this.processDecryptButton.Text = "  Дешифрование"; // Text unchanged, added space for icon
            this.processDecryptButton.UseVisualStyleBackColor = false;
            this.processDecryptButton.Click += new System.EventHandler(this.btnDecrypt_Click); // Event handler name kept
            // 
            // backupLocationSelectorDialog 
            // 
            this.backupLocationSelectorDialog.Description = "Выберите папку для сохранения резервных копий";
            this.backupLocationSelectorDialog.ShowNewFolderButton = true;
            // 
            // configureBackupLocationButton
            // 
            this.configureBackupLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configureBackupLocationButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(139)))), ((int)(((byte)(202))))); // Blue color for info/config
            this.configureBackupLocationButton.FlatAppearance.BorderSize = 0;
            this.configureBackupLocationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.configureBackupLocationButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.configureBackupLocationButton.ForeColor = System.Drawing.Color.White;
            // this.configureBackupLocationButton.Image = global::FileEncryptorApp.Properties.Resources.FolderIcon; // Placeholder for icon
            this.configureBackupLocationButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.configureBackupLocationButton.Location = new System.Drawing.Point(15, 30); // Position within GroupBox
            this.configureBackupLocationButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.configureBackupLocationButton.Name = "configureBackupLocationButton";
            this.configureBackupLocationButton.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.configureBackupLocationButton.Size = new System.Drawing.Size(568, 45); // Make it wider
            this.configureBackupLocationButton.TabIndex = 0; // TabIndex within GroupBox
            this.configureBackupLocationButton.Text = "  Указать папку резервного копирования..."; // Changed text slightly
            this.configureBackupLocationButton.UseVisualStyleBackColor = false;
            this.configureBackupLocationButton.Click += new System.EventHandler(this.btnSelectBackupFolder_Click); // Event handler name kept
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationStatusLabel});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 309); // Docked at bottom
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.mainStatusStrip.Size = new System.Drawing.Size(622, 26); // Span form width
            this.mainStatusStrip.SizingGrip = false; // Optional: disable resizing grip
            this.mainStatusStrip.TabIndex = 3; // Tab index after group boxes
            this.mainStatusStrip.Text = "statusStrip1";
            // 
            // operationStatusLabel
            // 
            this.operationStatusLabel.Name = "operationStatusLabel";
            this.operationStatusLabel.Size = new System.Drawing.Size(602, 20); // Spring to fill width
            this.operationStatusLabel.Spring = true;
            this.operationStatusLabel.Text = "Статус: Готов"; // Initial text
            this.operationStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // Note: The original Click handler 'label1_Click' is now irrelevant for ToolStripStatusLabel
            // If click is needed, handle mainStatusStrip.ItemClicked event
            // 
            // fileSelectionGroupBox
            // 
            this.fileSelectionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectionGroupBox.Controls.Add(this.filePathDisplayTextBox);
            this.fileSelectionGroupBox.Controls.Add(this.chooseDataSourceButton);
            this.fileSelectionGroupBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileSelectionGroupBox.Location = new System.Drawing.Point(12, 12); // Position near top
            this.fileSelectionGroupBox.Name = "fileSelectionGroupBox";
            this.fileSelectionGroupBox.Size = new System.Drawing.Size(598, 75); // Adjusted size
            this.fileSelectionGroupBox.TabIndex = 0; // First group
            this.fileSelectionGroupBox.TabStop = false;
            this.fileSelectionGroupBox.Text = "1. Выбор файла";
            // 
            // actionsGroupBox
            // 
            this.actionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actionsGroupBox.Controls.Add(this.processEncryptButton);
            this.actionsGroupBox.Controls.Add(this.processDecryptButton);
            this.actionsGroupBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.actionsGroupBox.Location = new System.Drawing.Point(12, 103); // Position below file selection
            this.actionsGroupBox.Name = "actionsGroupBox";
            this.actionsGroupBox.Size = new System.Drawing.Size(598, 90); // Adjusted size
            this.actionsGroupBox.TabIndex = 1; // Second group
            this.actionsGroupBox.TabStop = false;
            this.actionsGroupBox.Text = "2. Действия";
            // 
            // backupGroupBox
            // 
            this.backupGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backupGroupBox.Controls.Add(this.configureBackupLocationButton);
            this.backupGroupBox.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.backupGroupBox.Location = new System.Drawing.Point(12, 209); // Position below actions
            this.backupGroupBox.Name = "backupGroupBox";
            this.backupGroupBox.Size = new System.Drawing.Size(598, 90); // Adjusted size
            this.backupGroupBox.TabIndex = 2; // Third group
            this.backupGroupBox.TabStop = false;
            this.backupGroupBox.Text = "3. Резервное копирование (для шифрования)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F); // Use Segoe UI font size base
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control; // Standard light gray background
            this.ClientSize = new System.Drawing.Size(622, 335); // Adjusted client size for new layout
            this.Controls.Add(this.backupGroupBox);
            this.Controls.Add(this.actionsGroupBox);
            this.Controls.Add(this.fileSelectionGroupBox);
            this.Controls.Add(this.mainStatusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))); // Base font for the form
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // Prevent resizing
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false; // Disable maximize button
            this.Name = "Form1"; // Name unchanged
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Center on screen
            this.Text = "File Protector Pro"; // Changed window title text to sound more professional
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.fileSelectionGroupBox.ResumeLayout(false);
            this.fileSelectionGroupBox.PerformLayout();
            this.actionsGroupBox.ResumeLayout(false);
            this.backupGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Declare controls using their NEW names (unchanged from previous refactor)
        private System.Windows.Forms.TextBox filePathDisplayTextBox;
        private System.Windows.Forms.Button chooseDataSourceButton;
        private System.Windows.Forms.Button processEncryptButton;
        private System.Windows.Forms.Button processDecryptButton;
        // Label removed, replaced by StatusStrip
        private System.Windows.Forms.FolderBrowserDialog backupLocationSelectorDialog;
        private System.Windows.Forms.Button configureBackupLocationButton;
        // New controls added for styling/layout
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel operationStatusLabel; // Now a ToolStripStatusLabel
        private System.Windows.Forms.GroupBox fileSelectionGroupBox;
        private System.Windows.Forms.GroupBox actionsGroupBox;
        private System.Windows.Forms.GroupBox backupGroupBox;
    }
}