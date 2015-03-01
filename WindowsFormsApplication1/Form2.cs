using ProjectController;
using System;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form2 : Form
    {
        /// <summary>
        /// Ссылка на 1-ю форму
        /// </summary>
        Form1 form1;

        /// <summary>
        /// Ссылка на 9-ю форму
        /// </summary>
        Form9 form9;

        /// <summary>
        /// Конструткор формы обновляет список категорий
        /// </summary>
        /// <param name="form1"></param>
        /// <param name="form9"></param>
        public Form2(Form1 form1, Form9 form9 = null)
        {
            InitializeComponent();
            UpdateCategories();
            this.form1 = form1;
            this.form9 = form9;
        }

        /// <summary>
        /// Обновление категорий
        /// </summary>
        /// <param name="category"></param>
        public void UpdateCategories(string category = null)
        {
            if (category == null)
            {
                category = Controller.Project.DefaultCategory;
            }

            comboBox1.Items.Clear();

            foreach (var cat in Controller.Project.GetAllCategories())
            {
                comboBox1.Items.Add(cat.Name);
            }

            comboBox1.SelectedItem = category;
        }

        /// <summary>
        /// Делегат для проверки строки на соответствие определенным критериям
        /// </summary>
        /// <param name="s">Строка</param>
        /// <returns>Результат проверки</returns>
        delegate bool Del(string s);

        /// <summary>
        /// Процесс добавление текста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Del ExistsLetter = (s) =>
            {
                for (int i = 0; i < s.Length; i++)
                    if (char.IsLetter(s[i]))
                        return true;
                return false;
            };

            Del IsEmpty = s => String.IsNullOrEmpty(s);

            string name = textBox2.Text.Trim();
            string category = (string)comboBox1.SelectedItem;
            string text = textBox1.Text.Trim();

            if (IsEmpty(name) || IsEmpty(text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ExistsLetter(name) || !ExistsLetter(text))
            {
                MessageBox.Show("Все поля должны содержать буквы!", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                Controller.Project.LoadTextInsert(name, text, category);
                form1.ActivateMenu();
                form1.UpdateCategories();
                form1.UpdateListView();
                if (form9 != null)
                {
                    form9.comboBox1_update(name);
                }
                MessageBox.Show("Текст был успешно добавлен!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Возможность использования Ctrl+A в textBox1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
            {
                textBox1.SelectAll();
            }
        }
    }
}
