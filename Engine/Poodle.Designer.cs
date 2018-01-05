namespace Engine
{
    partial class Poodle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Poodle));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.browseCorpus = new System.Windows.Forms.Button();
            this.browsePosting = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.isStem = new System.Windows.Forms.CheckBox();
            this.cacheButton = new System.Windows.Forms.Button();
            this.dictionaryButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.extend = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBox1.Location = new System.Drawing.Point(68, 298);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(66, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "corpus path";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // browseCorpus
            // 
            this.browseCorpus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.browseCorpus.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.browseCorpus.Location = new System.Drawing.Point(4, 297);
            this.browseCorpus.Name = "browseCorpus";
            this.browseCorpus.Size = new System.Drawing.Size(51, 20);
            this.browseCorpus.TabIndex = 1;
            this.browseCorpus.Text = "browse";
            this.browseCorpus.UseVisualStyleBackColor = false;
            this.browseCorpus.Click += new System.EventHandler(this.button1_Click);
            // 
            // browsePosting
            // 
            this.browsePosting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.browsePosting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.browsePosting.Location = new System.Drawing.Point(5, 319);
            this.browsePosting.Name = "browsePosting";
            this.browsePosting.Size = new System.Drawing.Size(50, 21);
            this.browsePosting.TabIndex = 2;
            this.browsePosting.Text = "browse";
            this.browsePosting.UseVisualStyleBackColor = false;
            this.browsePosting.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBox2.Location = new System.Drawing.Point(68, 321);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(66, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "posting path";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // goButton
            // 
            this.goButton.Enabled = false;
            this.goButton.Font = new System.Drawing.Font("Ravie", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goButton.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.goButton.Image = ((System.Drawing.Image)(resources.GetObject("goButton.Image")));
            this.goButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.goButton.Location = new System.Drawing.Point(378, 152);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(91, 89);
            this.goButton.TabIndex = 4;
            this.goButton.Text = "Go!";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // isStem
            // 
            this.isStem.AutoSize = true;
            this.isStem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.isStem.Location = new System.Drawing.Point(217, 136);
            this.isStem.Name = "isStem";
            this.isStem.Size = new System.Drawing.Size(67, 17);
            this.isStem.TabIndex = 5;
            this.isStem.Text = "Stemmer";
            this.isStem.UseVisualStyleBackColor = false;
            this.isStem.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cacheButton
            // 
            this.cacheButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.cacheButton.Enabled = false;
            this.cacheButton.Location = new System.Drawing.Point(378, 268);
            this.cacheButton.Name = "cacheButton";
            this.cacheButton.Size = new System.Drawing.Size(91, 34);
            this.cacheButton.TabIndex = 7;
            this.cacheButton.Text = "Display Cache";
            this.cacheButton.UseVisualStyleBackColor = false;
            this.cacheButton.Click += new System.EventHandler(this.cacheButton_Click_1);
            // 
            // dictionaryButton
            // 
            this.dictionaryButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dictionaryButton.Enabled = false;
            this.dictionaryButton.Location = new System.Drawing.Point(371, 308);
            this.dictionaryButton.Name = "dictionaryButton";
            this.dictionaryButton.Size = new System.Drawing.Size(105, 33);
            this.dictionaryButton.TabIndex = 8;
            this.dictionaryButton.Text = "Display Dictionary";
            this.dictionaryButton.UseVisualStyleBackColor = false;
            this.dictionaryButton.Click += new System.EventHandler(this.dictionaryButton_Click_1);
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.clearButton.Enabled = false;
            this.clearButton.Location = new System.Drawing.Point(4, 260);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(64, 31);
            this.clearButton.TabIndex = 9;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(290, 43);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(185, 23);
            this.saveButton.TabIndex = 10;
            this.saveButton.Text = "Save Dictionary and Cache to Files";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // LoadButton
            // 
            this.LoadButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.LoadButton.Location = new System.Drawing.Point(299, 72);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(165, 25);
            this.LoadButton.TabIndex = 11;
            this.LoadButton.Text = "Load Dictionary and Cache";
            this.LoadButton.UseVisualStyleBackColor = false;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBox3.Location = new System.Drawing.Point(3, 107);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(193, 20);
            this.textBox3.TabIndex = 12;
            this.textBox3.Text = "Enter query";
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // runButton
            // 
            this.runButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.runButton.Enabled = false;
            this.runButton.Font = new System.Drawing.Font("Ravie", 9F, System.Drawing.FontStyle.Bold);
            this.runButton.Image = ((System.Drawing.Image)(resources.GetObject("runButton.Image")));
            this.runButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.runButton.Location = new System.Drawing.Point(22, 152);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(97, 89);
            this.runButton.TabIndex = 13;
            this.runButton.Text = "Run!";
            this.runButton.UseVisualStyleBackColor = false;
            this.runButton.Click += new System.EventHandler(this.button1_Click_3);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button2.Location = new System.Drawing.Point(202, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Load queries file";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // extend
            // 
            this.extend.AutoSize = true;
            this.extend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.extend.Enabled = false;
            this.extend.Location = new System.Drawing.Point(307, 109);
            this.extend.Name = "extend";
            this.extend.Size = new System.Drawing.Size(88, 17);
            this.extend.TabIndex = 15;
            this.extend.Text = "Extend query";
            this.extend.UseVisualStyleBackColor = false;
            this.extend.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_1);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.checkBox1.Location = new System.Drawing.Point(398, 109);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(77, 17);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "Summarize";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_2);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button1.Location = new System.Drawing.Point(299, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_4);
            // 
            // Poodle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(479, 353);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.extend);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.dictionaryButton);
            this.Controls.Add(this.cacheButton);
            this.Controls.Add(this.isStem);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.browsePosting);
            this.Controls.Add(this.browseCorpus);
            this.Controls.Add(this.textBox1);
            this.Name = "Poodle";
            this.Text = "Poodle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button browseCorpus;
        private System.Windows.Forms.Button browsePosting;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
    }
}

