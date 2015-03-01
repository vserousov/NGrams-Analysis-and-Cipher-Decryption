using ProjectController;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form4 : Form
    {
        public Form1 form1;
        
        /// <summary>
        /// Конструктор менеджера категорий обновляет список категорий
        /// </summary>
        /// <param name="form"></param>
        public Form4(Form1 form)
        {
            InitializeComponent();
            form1 = form;
            UpdateListView();
        }

        /// <summary>
        /// Процесс обновления списка категорий
        /// </summary>
        public void UpdateListView()
        {
            listView1.Items.Clear();
            List<Category> categories = Controller.Project.GetAllCategories();
            foreach (var category in categories)
            {
                string name = category.Name;
                int count = Controller.Project.GetFiles(category.Name).Count;
                listView1.Items.Add(new ListViewItem(new string[] { name, count.ToString() }));
            }
        }

        /// <summary>
        /// Делегат для проверки строки на соответствие определенным критериям
        /// </summary>
        /// <param name="s">Строка</param>
        /// <returns>Результат проверки</returns>
        delegate bool Del(string s);

        /// <summary>
        /// Добавление новой категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string category = textBox1.Text.Trim();

            Del ExistsLetter = (s) =>
            {
                for (int i = 0; i < s.Length; i++)
                    if (char.IsLetter(s[i]))
                        return true;
                return false;
            };

            Del IsEmpty = s => String.IsNullOrEmpty(s);

            if (IsEmpty(category))
            {
                MessageBox.Show("Заполните поле \"название\"!", "Информация", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!ExistsLetter(category))
            {
                MessageBox.Show("Поле \"название\" должно содержать буквы!", "Информация", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                Controller.Project.AddCategory(new Category(category));
                UpdateListView();
                form1.UpdateCategories();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                textBox1.Text = "";
            }
        }

        /// <summary>
        /// Показ контекстного меню при клике на элемент listView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true && listView1.SelectedItems.Count == 1)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        /// <summary>
        /// Процесс удаление выбранной категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;

            int id = listView1.SelectedItems[0].Index;
            string name = listView1.SelectedItems[0].SubItems[0].Text;
            DialogResult res = MessageBox.Show("Вы действительно хотите удалить категорию " + name + "?",
                "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (res == DialogResult.Yes)
            {
                try
                {
                    Controller.Project.RemoveCategory(name);
                    listView1.Items.RemoveAt(id);
                    form1.UpdateListView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Перенаправление на процесс удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        /// <summary>
        /// Открытие формы для изменения категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;

            string name = listView1.SelectedItems[0].SubItems[0].Text;

            if (Controller.Project.DefaultCategory == name)
            {
                MessageBox.Show("Встроенную категорию нельзя изменить!", "Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Form5 form = new Form5(this, name);
            form.ShowDialog();
        }

        /// <summary>
        /// Перенаправление для изменения категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }
    }
}
