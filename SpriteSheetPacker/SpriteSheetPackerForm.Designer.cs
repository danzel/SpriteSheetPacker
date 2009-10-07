namespace SpriteSheetPacker
{
	partial class SpriteSheetPackerForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.removeImageBtn = new System.Windows.Forms.Button();
			this.addImageBtn = new System.Windows.Forms.Button();
			this.buildBtn = new System.Windows.Forms.Button();
			this.imageOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.clearBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.paddingTxtBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.maxWidthTxtBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.maxHeightTxtBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.imageFileTxtBox = new System.Windows.Forms.TextBox();
			this.browseImageBtn = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.textFileTxtBox = new System.Windows.Forms.TextBox();
			this.browseTextBtn = new System.Windows.Forms.Button();
			this.imageSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.textSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.powOf2CheckBox = new System.Windows.Forms.CheckBox();
			this.squareCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.AllowDrop = true;
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 16;
			this.listBox1.Location = new System.Drawing.Point(12, 12);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.listBox1.Size = new System.Drawing.Size(697, 196);
			this.listBox1.TabIndex = 0;
			this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
			this.listBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox1_DragEnter);
			// 
			// removeImageBtn
			// 
			this.removeImageBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.removeImageBtn.Location = new System.Drawing.Point(479, 214);
			this.removeImageBtn.Name = "removeImageBtn";
			this.removeImageBtn.Size = new System.Drawing.Size(112, 51);
			this.removeImageBtn.TabIndex = 1;
			this.removeImageBtn.Text = "Remove Selected";
			this.removeImageBtn.UseVisualStyleBackColor = true;
			this.removeImageBtn.Click += new System.EventHandler(this.removeImageBtn_Click);
			// 
			// addImageBtn
			// 
			this.addImageBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.addImageBtn.Location = new System.Drawing.Point(361, 214);
			this.addImageBtn.Name = "addImageBtn";
			this.addImageBtn.Size = new System.Drawing.Size(112, 51);
			this.addImageBtn.TabIndex = 1;
			this.addImageBtn.Text = "Add Images";
			this.addImageBtn.UseVisualStyleBackColor = true;
			this.addImageBtn.Click += new System.EventHandler(this.addImageBtn_Click);
			// 
			// buildBtn
			// 
			this.buildBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buildBtn.Location = new System.Drawing.Point(479, 380);
			this.buildBtn.Name = "buildBtn";
			this.buildBtn.Size = new System.Drawing.Size(230, 51);
			this.buildBtn.TabIndex = 1;
			this.buildBtn.Text = "Build Sprite Sheet";
			this.buildBtn.UseVisualStyleBackColor = true;
			this.buildBtn.Click += new System.EventHandler(this.buildBtn_Click);
			// 
			// imageOpenFileDialog
			// 
			this.imageOpenFileDialog.AddExtension = false;
			this.imageOpenFileDialog.FileName = "openFileDialog1";
			this.imageOpenFileDialog.Filter = "Image Files (png, jpg, bmp)|*.png;*.jpg;*.bmp";
			this.imageOpenFileDialog.Multiselect = true;
			// 
			// clearBtn
			// 
			this.clearBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.clearBtn.Location = new System.Drawing.Point(597, 214);
			this.clearBtn.Name = "clearBtn";
			this.clearBtn.Size = new System.Drawing.Size(112, 51);
			this.clearBtn.TabIndex = 1;
			this.clearBtn.Text = "Remove All";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 231);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Image Padding:";
			// 
			// paddingTxtBox
			// 
			this.paddingTxtBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.paddingTxtBox.Location = new System.Drawing.Point(124, 228);
			this.paddingTxtBox.Name = "paddingTxtBox";
			this.paddingTxtBox.Size = new System.Drawing.Size(100, 22);
			this.paddingTxtBox.TabIndex = 3;
			this.paddingTxtBox.Text = "0";
			this.paddingTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 280);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(142, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Maximum Sheet Size:";
			// 
			// maxWidthTxtBox
			// 
			this.maxWidthTxtBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.maxWidthTxtBox.Location = new System.Drawing.Point(124, 300);
			this.maxWidthTxtBox.Name = "maxWidthTxtBox";
			this.maxWidthTxtBox.Size = new System.Drawing.Size(100, 22);
			this.maxWidthTxtBox.TabIndex = 3;
			this.maxWidthTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(70, 303);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 17);
			this.label3.TabIndex = 2;
			this.label3.Text = "Width:";
			// 
			// maxHeightTxtBox
			// 
			this.maxHeightTxtBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.maxHeightTxtBox.Location = new System.Drawing.Point(124, 328);
			this.maxHeightTxtBox.Name = "maxHeightTxtBox";
			this.maxHeightTxtBox.Size = new System.Drawing.Size(100, 22);
			this.maxHeightTxtBox.TabIndex = 3;
			this.maxHeightTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(65, 331);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 17);
			this.label4.TabIndex = 2;
			this.label4.Text = "Height:";
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(266, 300);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(97, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "Output Image:";
			// 
			// imageFileTxtBox
			// 
			this.imageFileTxtBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.imageFileTxtBox.Location = new System.Drawing.Point(378, 297);
			this.imageFileTxtBox.Name = "imageFileTxtBox";
			this.imageFileTxtBox.Size = new System.Drawing.Size(287, 22);
			this.imageFileTxtBox.TabIndex = 3;
			// 
			// browseImageBtn
			// 
			this.browseImageBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.browseImageBtn.Location = new System.Drawing.Point(671, 297);
			this.browseImageBtn.Name = "browseImageBtn";
			this.browseImageBtn.Size = new System.Drawing.Size(38, 22);
			this.browseImageBtn.TabIndex = 4;
			this.browseImageBtn.Text = "...";
			this.browseImageBtn.UseVisualStyleBackColor = true;
			this.browseImageBtn.Click += new System.EventHandler(this.browseImageBtn_Click);
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(266, 329);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(86, 17);
			this.label6.TabIndex = 2;
			this.label6.Text = "Output Text:";
			// 
			// textFileTxtBox
			// 
			this.textFileTxtBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.textFileTxtBox.Location = new System.Drawing.Point(378, 326);
			this.textFileTxtBox.Name = "textFileTxtBox";
			this.textFileTxtBox.Size = new System.Drawing.Size(287, 22);
			this.textFileTxtBox.TabIndex = 3;
			// 
			// browseTextBtn
			// 
			this.browseTextBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.browseTextBtn.Location = new System.Drawing.Point(671, 326);
			this.browseTextBtn.Name = "browseTextBtn";
			this.browseTextBtn.Size = new System.Drawing.Size(38, 22);
			this.browseTextBtn.TabIndex = 4;
			this.browseTextBtn.Text = "...";
			this.browseTextBtn.UseVisualStyleBackColor = true;
			this.browseTextBtn.Click += new System.EventHandler(this.browseTextBtn_Click);
			// 
			// imageSaveFileDialog
			// 
			this.imageSaveFileDialog.DefaultExt = "png";
			this.imageSaveFileDialog.Filter = "PNG Files|*.png";
			// 
			// textSaveFileDialog
			// 
			this.textSaveFileDialog.DefaultExt = "txt";
			this.textSaveFileDialog.Filter = "TXT Files|*.txt";
			// 
			// powOf2CheckBox
			// 
			this.powOf2CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.powOf2CheckBox.AutoSize = true;
			this.powOf2CheckBox.Location = new System.Drawing.Point(15, 366);
			this.powOf2CheckBox.Name = "powOf2CheckBox";
			this.powOf2CheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.powOf2CheckBox.Size = new System.Drawing.Size(224, 21);
			this.powOf2CheckBox.TabIndex = 5;
			this.powOf2CheckBox.Text = "Require Power of Two Output?";
			this.powOf2CheckBox.UseVisualStyleBackColor = true;
			// 
			// squareCheckBox
			// 
			this.squareCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.squareCheckBox.AutoSize = true;
			this.squareCheckBox.Location = new System.Drawing.Point(15, 393);
			this.squareCheckBox.Name = "squareCheckBox";
			this.squareCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.squareCheckBox.Size = new System.Drawing.Size(185, 21);
			this.squareCheckBox.TabIndex = 5;
			this.squareCheckBox.Text = "Require Square Output?";
			this.squareCheckBox.UseVisualStyleBackColor = true;
			// 
			// SpriteSheetPackerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(721, 443);
			this.Controls.Add(this.squareCheckBox);
			this.Controls.Add(this.powOf2CheckBox);
			this.Controls.Add(this.browseTextBtn);
			this.Controls.Add(this.browseImageBtn);
			this.Controls.Add(this.maxHeightTxtBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.maxWidthTxtBox);
			this.Controls.Add(this.textFileTxtBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.imageFileTxtBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.paddingTxtBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buildBtn);
			this.Controls.Add(this.addImageBtn);
			this.Controls.Add(this.clearBtn);
			this.Controls.Add(this.removeImageBtn);
			this.Controls.Add(this.listBox1);
			this.MinimumSize = new System.Drawing.Size(739, 488);
			this.Name = "SpriteSheetPackerForm";
			this.Text = "Sprite Sheet Packer";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button removeImageBtn;
		private System.Windows.Forms.Button addImageBtn;
		private System.Windows.Forms.Button buildBtn;
		private System.Windows.Forms.OpenFileDialog imageOpenFileDialog;
		private System.Windows.Forms.Button clearBtn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox paddingTxtBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox maxWidthTxtBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox maxHeightTxtBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox imageFileTxtBox;
		private System.Windows.Forms.Button browseImageBtn;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textFileTxtBox;
		private System.Windows.Forms.Button browseTextBtn;
		private System.Windows.Forms.SaveFileDialog imageSaveFileDialog;
		private System.Windows.Forms.SaveFileDialog textSaveFileDialog;
		private System.Windows.Forms.CheckBox powOf2CheckBox;
		private System.Windows.Forms.CheckBox squareCheckBox;
	}
}

