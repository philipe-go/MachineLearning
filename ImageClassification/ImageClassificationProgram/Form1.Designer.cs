namespace ImageClassificationProgram
{
    partial class Form1
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.loadImagebtn = new System.Windows.Forms.Button();
            this.clearBtn = new System.Windows.Forms.Button();
            this.classifyBtn = new System.Windows.Forms.Button();
            this.exitBtn = new System.Windows.Forms.Button();
            this.classificationTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(277, 194);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // loadImagebtn
            // 
            this.loadImagebtn.Location = new System.Drawing.Point(295, 12);
            this.loadImagebtn.Name = "loadImagebtn";
            this.loadImagebtn.Size = new System.Drawing.Size(138, 44);
            this.loadImagebtn.TabIndex = 1;
            this.loadImagebtn.Text = "Load Image";
            this.loadImagebtn.UseVisualStyleBackColor = true;
            this.loadImagebtn.Click += new System.EventHandler(this.loadImagebtn_Click);
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(295, 62);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(138, 44);
            this.clearBtn.TabIndex = 2;
            this.clearBtn.Text = "Clear Image";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // classifyBtn
            // 
            this.classifyBtn.Location = new System.Drawing.Point(295, 112);
            this.classifyBtn.Name = "classifyBtn";
            this.classifyBtn.Size = new System.Drawing.Size(138, 44);
            this.classifyBtn.TabIndex = 3;
            this.classifyBtn.Text = "Classify";
            this.classifyBtn.UseVisualStyleBackColor = true;
            this.classifyBtn.Click += new System.EventHandler(this.classifyBtn_Click);
            // 
            // exitBtn
            // 
            this.exitBtn.Location = new System.Drawing.Point(295, 162);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(138, 44);
            this.exitBtn.TabIndex = 4;
            this.exitBtn.Text = "Exit";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // classificationTB
            // 
            this.classificationTB.Location = new System.Drawing.Point(295, 212);
            this.classificationTB.Name = "classificationTB";
            this.classificationTB.Size = new System.Drawing.Size(138, 20);
            this.classificationTB.TabIndex = 5;
            this.classificationTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(191, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "This is an image of:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 243);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.classificationTB);
            this.Controls.Add(this.exitBtn);
            this.Controls.Add(this.classifyBtn);
            this.Controls.Add(this.clearBtn);
            this.Controls.Add(this.loadImagebtn);
            this.Controls.Add(this.pictureBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button loadImagebtn;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Button classifyBtn;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.TextBox classificationTB;
        private System.Windows.Forms.Label label1;
    }
}

