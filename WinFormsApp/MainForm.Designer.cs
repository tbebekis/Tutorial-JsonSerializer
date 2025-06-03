namespace WinFormsApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            btnToDynamic = new Button();
            btnFormat = new Button();
            btnPopulateObject = new Button();
            btnCamelCase = new Button();
            btnPascalCase = new Button();
            btnClear = new Button();
            edtLog = new RichTextBox();
            btnExcludeProperties = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnExcludeProperties);
            panel1.Controls.Add(btnToDynamic);
            panel1.Controls.Add(btnFormat);
            panel1.Controls.Add(btnPopulateObject);
            panel1.Controls.Add(btnCamelCase);
            panel1.Controls.Add(btnPascalCase);
            panel1.Controls.Add(btnClear);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1004, 63);
            panel1.TabIndex = 0;
            // 
            // btnToDynamic
            // 
            btnToDynamic.Location = new Point(607, 17);
            btnToDynamic.Name = "btnToDynamic";
            btnToDynamic.Size = new Size(113, 32);
            btnToDynamic.TabIndex = 5;
            btnToDynamic.Text = "To Dynamic";
            btnToDynamic.UseVisualStyleBackColor = true;
            // 
            // btnFormat
            // 
            btnFormat.Location = new Point(488, 17);
            btnFormat.Name = "btnFormat";
            btnFormat.Size = new Size(113, 32);
            btnFormat.TabIndex = 4;
            btnFormat.Text = "Format Json";
            btnFormat.UseVisualStyleBackColor = true;
            // 
            // btnPopulateObject
            // 
            btnPopulateObject.Location = new Point(369, 17);
            btnPopulateObject.Name = "btnPopulateObject";
            btnPopulateObject.Size = new Size(113, 32);
            btnPopulateObject.TabIndex = 3;
            btnPopulateObject.Text = "Populate Object";
            btnPopulateObject.UseVisualStyleBackColor = true;
            // 
            // btnCamelCase
            // 
            btnCamelCase.Location = new Point(250, 17);
            btnCamelCase.Name = "btnCamelCase";
            btnCamelCase.Size = new Size(113, 32);
            btnCamelCase.TabIndex = 2;
            btnCamelCase.Text = "Camel Case";
            btnCamelCase.UseVisualStyleBackColor = true;
            // 
            // btnPascalCase
            // 
            btnPascalCase.Location = new Point(131, 17);
            btnPascalCase.Name = "btnPascalCase";
            btnPascalCase.Size = new Size(113, 32);
            btnPascalCase.TabIndex = 1;
            btnPascalCase.Text = "Pascal Case";
            btnPascalCase.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(12, 17);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(113, 32);
            btnClear.TabIndex = 0;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // edtLog
            // 
            edtLog.BackColor = Color.Gainsboro;
            edtLog.Dock = DockStyle.Fill;
            edtLog.Font = new Font("Courier New", 9F);
            edtLog.Location = new Point(0, 63);
            edtLog.Name = "edtLog";
            edtLog.Size = new Size(1004, 387);
            edtLog.TabIndex = 1;
            edtLog.Text = "";
            // 
            // btnExcludeProperties
            // 
            btnExcludeProperties.Location = new Point(726, 17);
            btnExcludeProperties.Name = "btnExcludeProperties";
            btnExcludeProperties.Size = new Size(113, 32);
            btnExcludeProperties.TabIndex = 6;
            btnExcludeProperties.Text = "Exclude Properties";
            btnExcludeProperties.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1004, 450);
            Controls.Add(edtLog);
            Controls.Add(panel1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "System.Text.Json test";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private RichTextBox edtLog;
        private Button btnPascalCase;
        private Button btnClear;
        private Button btnCamelCase;
        private Button btnPopulateObject;
        private Button btnFormat;
        private Button btnToDynamic;
        private Button btnExcludeProperties;
    }
}
