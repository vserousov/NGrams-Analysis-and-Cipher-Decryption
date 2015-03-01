using ProjectController;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form3 : Form
    {
        /// <summary>
        /// Ссылка на 1-ю форму
        /// </summary>
        Form1 form1;

        /// <summary>
        /// URL-адрес
        /// </summary>
        string url;

        /// <summary>
        /// Категория
        /// </summary>
        string category;

        /// <summary>
        /// Начало
        /// </summary>
        string start;

        /// <summary>
        /// Конец
        /// </summary>
        string end;

        /// <summary>
        /// Результат
        /// </summary>
        bool completed;

        /// <summary>
        /// Конструктор формы обновляет список категорий
        /// </summary>
        /// <param name="form"></param>
        public Form3(Form1 form)
        {
            InitializeComponent();
            form1 = form;
            radioButton2.Checked = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            UpdateCategories();
        }

        /// <summary>
        /// Список категорий
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
        /// Обновление radioButton1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
            }
        }

        /// <summary>
        /// Обновление radioButton2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false)
            {
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox2.Text = "";
                textBox3.Text = "";
            }
        }

        /// <summary>
        /// Процесс добавления веб-страницы вызывает backgroundWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            bool show = radioButton1.Checked;

            url = textBox1.Text.Trim();
            category = (string)comboBox1.SelectedItem;
            start = show ? textBox2.Text.Trim() : null;
            end = show ? textBox3.Text.Trim() : null;

            if (show && (String.IsNullOrEmpty(start) || String.IsNullOrEmpty(end)) || String.IsNullOrEmpty(url))
            {
                MessageBox.Show("Заполните все доступные поля!", "Внимание!", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Uri uriResult;
            bool result = Uri.TryCreate(WebData.FormatUrl(url), UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;

            if (!result)
            {
                MessageBox.Show("Неверный формат URL!", "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            button1.Text = "Подождите...";
            button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Процесс парсинга веб-страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Controller.Project.LoadWebPage(url, category, start, end);
                completed = true;
            }
            catch (Exception ex)
            {
                completed = false;
                MessageBox.Show(ex.Message, "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// По окончанию парсинга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Добавить";
            button1.Enabled = true;

            if (completed == true)
            {
                form1.ActivateMenu();
                form1.UpdateCategories();
                form1.UpdateListView();
                MessageBox.Show("Веб-страница была успешно добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}
