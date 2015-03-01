using ProjectController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form7 : Form
    {
        /// <summary>
        /// Конструктор инициализирует список документов\категорий
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selected"></param>
        public Form7(string type = null, string selected = null)
        {
            InitializeComponent();

            if (type == "category")
            {
                radioButton1.Checked = true;
                foreach (var category in Controller.Project.GetAllCategories())
                {
                    bool add = true;
                    List<FileData> files = Controller.Project.GetFiles(category.Name);

                    foreach (FileData file in files)
                    {
                        if (file.Letters == null || file.Bigrams == null || file.Trigrams == null)
                            add = false;
                    }
                    
                    if (add && files.Count > 0)
                    {
                        int id = comboBox1.Items.Add(category.Name);
                        if (category.Name == selected)
                            comboBox1.SelectedIndex = id;
                    }
                }
            }

            if (type == "document")
            {
                radioButton2.Checked = true;
                foreach (var document in Controller.Project.GetAllFiles())
                {
                    if (document.Letters == null || document.Bigrams == null || document.Trigrams == null)
                        continue;
                    int id = comboBox1.Items.Add(document.Name);
                    if (document.Name == selected)
                        comboBox1.SelectedIndex = id;
                }
            }

            string str = (string)comboBox1.SelectedItem;
            if ((str == String.Empty || str == null || str == "") && comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            radioButton1.CheckedChanged += radioButton_Update;
            radioButton2.CheckedChanged += radioButton_Update;
        }

        /// <summary>
        /// Изменение типа словаря
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_Update(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            if (radioButton1.Checked == true)
            {
                foreach (var category in Controller.Project.GetAllCategories())
                {
                    bool add = true;

                    List<FileData> files = Controller.Project.GetFiles(category.Name);

                    foreach (FileData file in files)
                    {
                        if (file.Letters == null || file.Bigrams == null || file.Trigrams == null)
                        {
                            add = false;
                        }
                    }

                    if (add && files.Count > 0)
                    {
                        comboBox1.Items.Add(category.Name);
                    }
                }
            }
            else
            {
                foreach (var document in Controller.Project.GetAllFiles())
                {
                    if (document.Letters == null || document.Bigrams == null || document.Trigrams == null)
                        continue;
                    else
                        comboBox1.Items.Add(document.Name);
                }
            }

            if (comboBox1.Items.Count > 0)
               comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Добавление частотного словаря
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text.Trim();
            string type = radioButton1.Checked ? "category" : "document";
            
            if (comboBox1.Items.Count == 0)
            {
                MessageBox.Show("Нечего добавить в словарь!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!text.Any(char.IsLetter))
            {
                MessageBox.Show("Название должно содержать буквы!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Controller.Project.AddDictionary(text, (string)comboBox1.SelectedItem, type);
                MessageBox.Show("Словарь успешно добавлен!", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
