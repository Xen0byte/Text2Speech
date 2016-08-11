using System;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace Text2Speech
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }

        public Form1()
        {
            InitializeComponent();

            synth.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synth_SpeakCompleted);

            foreach (InstalledVoice voice in synth.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                ComboboxItem item = new ComboboxItem();
                item.Text = info.Name;
                item.Value = info.Id;
                comboBox1.Items.Add(item);
            }

            textBox1.ForeColor = SystemColors.GrayText;
            textBox1.Text = "Type/Paste Text Here:";
            textBox1.Leave += new EventHandler(textBox1_Leave);
            textBox1.Enter += new EventHandler(textBox1_Enter);


            comboBox1.Text = synth.Voice.Name.ToString();
        }

        private void synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Text = "Save to WAV";
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                textBox1.Text = "Type/Paste Text Here:";
                textBox1.ForeColor = SystemColors.GrayText;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Type/Paste Text Here:")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            synth.SelectVoice(comboBox1.Text);

            label6.Text = synth.Voice.Culture.ToString();
            label7.Text = synth.Voice.Age.ToString();
            label8.Text = synth.Voice.Gender.ToString();
            label9.Text = synth.Voice.Description.ToString();
            label10.Text = synth.Voice.Id.ToString();

            if (!label1.Visible)
                foreach (Control x in tabPage2.Controls)
                {
                    if (x is Label)
                    {
                        ((Label)x).Visible = true;
                    }
                }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "" | textBox1.Text == "Type/Paste Text Here:")
            {
                button1.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                button4.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!button2.Enabled)
            {
                button2.Visible = true;
                button2.Enabled = true;
                button3.Visible = true;
                button3.Enabled = true;
                button1.Enabled = false;
            }

            synth.SetOutputToDefaultAudioDevice();
            synth.SpeakAsync(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (synth.State.ToString() == "Speaking")
            {
                synth.Pause();
                button2.Text = "Resume";
                button3.Enabled = false;
            }
            else if (synth.State.ToString() == "Paused")
            {
                synth.Resume();
                button2.Text = "Pause";
                button3.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            synth.SpeakAsyncCancelAll();
            button2.Enabled = false;
            button3.Enabled = false;
            button1.Enabled = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://msdn.microsoft.com/en-us/library/hh378476.aspx");
        }

        private void label13_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            ActiveControl = textBox1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Text = "Processing...";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = string.Format("{0:yyyy-MM-dd_HH-mm-ss}.WMV", DateTime.Now);
                synth.SetOutputToWaveFile(folderBrowserDialog1.SelectedPath + "\\" + fileName);
                synth.SpeakAsync(textBox1.Text);
            }
            else button4.Text = "Save to WAV";
        }

        private void label14_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            ActiveControl = textBox1;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                textBox1.SelectAll();
                ActiveControl = textBox1;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}