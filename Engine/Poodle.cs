using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Engine
{
    public partial class Poodle : Form
    {
        private Button goButton = new Button();
        private Button clearButton = new Button();
        private Button saveButton = new Button();
        private Button LoadButton = new Button();
        private Button cacheButton = new Button();
        private Button dictionaryButton = new Button();
        private Button runButton = new Button();
        private CheckBox extend = new CheckBox();
        private CheckBox isStem = new CheckBox();
        Controller c = new Controller();
        string CorpusPath = "";
        string finalPath = "";
        string savePath = "";
        bool stem = false;
        bool fromFinal = false;
        string query = "";

        public Poodle()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)//corpus path
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                CorpusPath = folderBrowserDialog1.SelectedPath;
            }
            if (CorpusPath != "" && finalPath != "")
            {
                goButton.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)//browse posting
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                finalPath = folderBrowserDialog1.SelectedPath;
            }
            if (CorpusPath != "" && finalPath != "")
            {
                goButton.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)//browse corpus
        {
               FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
               if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
               {
                CorpusPath = folderBrowserDialog1.SelectedPath;
               }
                if (CorpusPath != "" && finalPath != "")
                {
                    goButton.Enabled = true;
                }
        }
       
        private void textBox2_TextChanged(object sender, EventArgs e)//posting path
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                finalPath = folderBrowserDialog1.SelectedPath;
            }
            if(CorpusPath != "" && finalPath != "")
            {
                goButton.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)//go!
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            MessageBox.Show("Indexing...");
            c.Engine(CorpusPath, finalPath, stem);
            sw.Stop();
            MessageBox.Show("Done Indexing!");
            TimeSpan ts = sw.Elapsed;
            MessageBox.Show("Number of indexed document: " + c.GetNumOfDocs() + " size of the index in bytes " + c.SizeOfIndex(finalPath, stem).ToString()
               + " Total running time is " + ts.TotalSeconds + " seconds");
            saveButton.Enabled = true;
            cacheButton.Enabled = true;
            dictionaryButton.Enabled = true;
            clearButton.Enabled = true;
            fromFinal = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (isStem.Checked == true)
                stem = true;
            else
                stem = false;
        }

        private void button1_Click_2(object sender, EventArgs e)//clear
        {
            c.Delete(savePath, finalPath, stem);
        }

        private void button1_Click_1(object sender, EventArgs e)//save
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                savePath = folderBrowserDialog1.SelectedPath;
            }
            c.Save(savePath, stem);
            fromFinal = false;
        }

        private void LoadButton_Click(object sender, EventArgs e)//load dictionary and cache
        {
            MessageBox.Show("Loading...");
            string loadPath = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                loadPath = folderBrowserDialog1.SelectedPath;
            }
            MessageBox.Show("Done Loading!");
            savePath = loadPath;
            c.Load(loadPath, stem);
            saveButton.Enabled = true;
            cacheButton.Enabled = true;
            dictionaryButton.Enabled = true;
            clearButton.Enabled = true;
            fromFinal = false;
        }

        private void cacheButton_Click_1(object sender, EventArgs e)
        {
            Process myProcess = new Process();
            if (fromFinal)
            {
                Process.Start("notepad.exe", finalPath + c.GetCachePath(stem));
            }
            else
            {
                Process.Start("notepad.exe", savePath + c.GetCachePath(stem));
            }
        }

        private void dictionaryButton_Click_1(object sender, EventArgs e)
        {
            Process myProcess = new Process();
            if (fromFinal)
            {
                Process.Start("notepad.exe", finalPath + c.GetDicPath(stem));
            }
            else
            {
                Process.Start("notepad.exe", savePath + c.GetDicPath(stem));
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)//fill query
        {
            TextBox objTextBox = (TextBox)sender;
            string theText = objTextBox.Text;
            query = theText;
            if (!query.Equals(""))
                runButton.Enabled = true;
            else
            {
                runButton.Enabled = false;
            }
            if (!query.Contains(" ") && !query.Equals(""))
                extend.Enabled = true;
            else
                extend.Enabled = false;
        }

        private void button1_Click_3(object sender, EventArgs e)//run
        {
            string queryText = query;
            c.RunQuery(queryText);
        }

        private void button2_Click_1(object sender, EventArgs e)//load file of qureis
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                //finalPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)//extend
        {

        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)//summarize
        {
            string nameOfDoc = query;
        }

        private void button1_Click_4(object sender, EventArgs e)//reset
        {

        }
    }
}
