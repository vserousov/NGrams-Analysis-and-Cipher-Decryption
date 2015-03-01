using Cipher;
using ProjectController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    /// <summary>
    /// Делегат шифрования
    /// </summary>
    /// <typeparam name="T">Тип</typeparam>
    /// <param name="str">Строка</param>
    /// <returns>Ключ</returns>
    public delegate T CipherDel<T>(string str, DictDel dict);

    public partial class Form9 : Form
    {
        /// <summary>
        /// Максимальный размер текста для поиска ключа
        /// </summary>
        const int maxNeededSize = 1024;

        /// <summary>
        /// Результат шифрования\дешифрования
        /// </summary>
        bool success;

        /// <summary>
        /// Сслыка на 1-ю форму
        /// </summary>
        Form1 form1;

        /// <summary>
        /// Текущий документ
        /// </summary>
        string document;

        /// <summary>
        /// Метод криптоанализа
        /// </summary>
        int analyzeMethod;

        /// <summary>
        /// Метод шифрования
        /// </summary>
        int cipherMethod;

        /// <summary>
        /// Частотный словарь
        /// </summary>
        string frequencyDictionary;

        /// <summary>
        /// Операция (шифрование/дешифрование)
        /// </summary>
        string operation;

        /// <summary>
        /// Результат
        /// </summary>
        string result;

        /// <summary>
        /// Конструтктор формы
        /// </summary>
        /// <param name="form">Ссылка на 1-ю форму</param>
        /// <param name="document">Активный документ</param>
        public Form9(Form1 form, string document = null)
        {
            InitializeComponent();
            form1 = form;

            List<FileData> files = Controller.Project.GetAllFiles();
            
            if (files.Count > 0)
            {
                foreach (FileData file in files)
                {
                    comboBox1.Items.Add(file.Name);
                }
                if (document == null)
                    comboBox1.SelectedIndex = 0;
                else
                    comboBox1.SelectedItem = document;
            }

            if (Controller.Project.Dicts != null)
            {
                foreach (var item in Controller.Project.Dicts)
                {
                    comboBox4.Items.Add(item.Key);
                }
            }

            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            radioButton1.CheckedChanged += radioButton_checked;
            radioButton2.CheckedChanged += radioButton_checked;
            radioButton1.Checked = true;
            progressBar1.Visible = false;
        }

        /// <summary>
        /// Обновление comboBox1
        /// </summary>
        /// <param name="doc">Документ</param>
        public void comboBox1_update(string doc)
        {
            List<FileData> files = Controller.Project.GetAllFiles();

            if (files.Count > 0)
            {
                comboBox1.Items.Clear();
                foreach (FileData file in files)
                {
                    comboBox1.Items.Add(file.Name);
                }
                if (doc == null)
                    comboBox1.SelectedIndex = 0;
                else
                    comboBox1.SelectedItem = doc;
            }
        }

        /// <summary>
        /// Обновление radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void radioButton_checked(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                label1.Text = "Документ для дешифрования";
                operation = "decode";
            }
            else
            {
                label1.Text = "Документ для шифрования";
                operation = "encode";
            }
        }

        /// <summary>
        /// Обновление comboBox1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            document = (string)comboBox1.SelectedItem;
        }

        /// <summary>
        /// Обновление comboBox2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            analyzeMethod = comboBox2.SelectedIndex;
        }

        /// <summary>
        /// Обновление comboBox3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            cipherMethod = comboBox3.SelectedIndex;
        }

        /// <summary>
        /// Обновление comboBox4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            frequencyDictionary = (string)comboBox3.SelectedItem;
        }

        /// <summary>
        /// Запуск шифрования\дешфирования 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("Выберите файл!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            progressBar1.Visible = true;
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Процесс шифрования\дешифрования (выполняется в фоне)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (cipherMethod == 0)
            {
                if (operation == "decode")
                {
                    if (comboBox4.SelectedIndex == 0)
                    {
                        Caesar.DLetter = null;
                        Caesar.DBigram = null;
                        Caesar.DTrigram = null;
                    }
                    else
                    {
                        string name = (string)comboBox4.SelectedItem;
                        FrequencyDictionary dict = Controller.Project.Dicts[name];
                        Caesar.DLetter = dict.Letters;
                        Caesar.DBigram = dict.Bigrams;
                        Caesar.DTrigram = dict.Trigrams;
                    }

                    CipherDel<int> shiftFind = null;
                    DictDel dictionary = null;
                    
                    if (analyzeMethod == 0)
                    {
                        shiftFind = Caesar.ShiftSearchChiSquared;
                        dictionary = Caesar.LettersDict;
                    }

                    if (analyzeMethod == 1)
                    {
                        shiftFind = Caesar.ShiftSearchFitnessMeasure;
                        dictionary = Caesar.BigramsDict;
                    }

                    if (analyzeMethod == 2)
                    {
                        shiftFind = Caesar.ShiftSearchFitnessMeasure;
                        dictionary = Caesar.TrigramsDict;
                    }
                    
                    FileData file = Controller.Project.GetFile(document);

                    int shift = 0;
                    string text = String.Empty;

                    if (file.Type == DataType.typeFile)
                    {
                        string path = file.FullPath;
                        int size = 0;

                        using (StreamReader reader = new StreamReader(path, Encoding.Default))
                        {
                            while (size < maxNeededSize && !reader.EndOfStream)
                            {
                                text += reader.ReadLine();
                                size = text.Length;
                            }
                            reader.Close();
                        }
                        Caesar.DetectLanguage(text);
                        shift = shiftFind(text, dictionary);
                        textBox1.Text = shift.ToString();

                        using (StreamReader reader = new StreamReader(path, Encoding.Default))
                        {
                            text = reader.ReadToEnd();
                            reader.Close();
                        }
                    }

                    if (file.Type == DataType.typeText)
                    {
                        text = ((TextData)file).Text;
                        Caesar.DetectLanguage(text);
                        shift = shiftFind(text, dictionary);
                        textBox1.Text = shift.ToString();
                    }

                    if (file.Type == DataType.typeWeb)
                    {
                        text = ((WebData)file).Text;
                        Caesar.DetectLanguage(text);
                        shift = shiftFind(text, dictionary);
                        textBox1.Text = shift.ToString();
                    }

                    result = Caesar.Decode(text, shift);
                    success = true;
                }

                if (operation == "encode")
                {
                    string key = textBox1.Text.Trim();
                    
                    if (key == String.Empty)
                    {
                        progressBar1.Visible = false;
                        MessageBox.Show("Ключ не должен быть пустым", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        success = false;
                        return;
                    }

                    if (!key.All(char.IsNumber))
                    {
                        progressBar1.Visible = false;
                        MessageBox.Show("Ключ должен быть числом", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        success = false;
                        return;
                    }

                    FileData file = Controller.Project.GetFile(document);

                    string text = String.Empty;

                    if (file.Type == DataType.typeFile)
                    {
                        string path = file.FullPath;
                        using (StreamReader reader = new StreamReader(path, Encoding.Default))
                        {
                            text = reader.ReadToEnd();
                            reader.Close();
                        }

                        Caesar.DetectLanguage(text);
                    }

                    if (file.Type == DataType.typeText)
                    {
                        text = ((TextData)file).Text;
                        Caesar.DetectLanguage(text);
                    }

                    if (file.Type == DataType.typeWeb)
                    {
                        text = ((WebData)file).Text;
                        Caesar.DetectLanguage(text);
                    }

                    int shift = int.Parse(key);

                    if (!(0 <= shift && shift <= Caesar.Power()))
                    {
                        progressBar1.Visible = false;
                        MessageBox.Show("Ключ должен быть числом в пределах от 0 до " + (Caesar.Power()) , "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        success = false;
                        return;
                    }

                    result = Caesar.Encode(text, shift);
                    success = true;
                }
            }
        }

        /// <summary>
        /// По окончании операции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            if (success)
            {
                button3.Enabled = true;
                button4.Enabled = true;
                MessageBox.Show("Операция завершена", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }

        /// <summary>
        /// Показать результат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            Form10 form = new Form10(result);
            form.ShowDialog();
        }

        /// <summary>
        /// Сохранить результат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.CreateNew))
                    {
                        using (StreamWriter writer = new StreamWriter(fs, Encoding.Default))
                        {
                            writer.Write(result);
                            writer.Close();
                        }
                        fs.Close();
                    }
                    MessageBox.Show("Файл успешно сохранен", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при сохранении файла!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Вставить текст
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2(form1, this);
            form.ShowDialog();
            form.Activate();
        }
    }
}
