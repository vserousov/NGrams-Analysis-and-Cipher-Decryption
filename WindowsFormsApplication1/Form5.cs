using ProjectController;
using System;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form5 : Form
    {
        /// <summary>
        /// Ссылка на 4-ю форму
        /// </summary>
        Form4 form4;

        /// <summary>
        /// Старое название категории
        /// </summary>
        string oldCategory;

        public Form5(Form4 form, string categoryName)
        {
            InitializeComponent();
            form4 = form;
            textBox1.Text = oldCategory = categoryName;
        }

        /// <summary>
        /// Изменить название категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string newcategory = textBox1.Text.Trim();

            if (String.IsNullOrEmpty(newcategory))
            {
                MessageBox.Show("Название категории не может быть пустым!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (oldCategory == textBox1.Text)
            {
                this.Close();
                return;
            }

            try
            {
                Controller.Project.ChangeCategoryName(oldCategory, textBox1.Text);
                form4.UpdateListView();
                form4.form1.UpdateCategories();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = oldCategory;
            }
        }
    }
}
