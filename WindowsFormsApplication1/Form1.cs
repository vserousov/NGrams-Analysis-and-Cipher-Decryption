using ProjectController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Заголовок формы по умолчанию
        /// </summary>
        const string formTitle = "Частотный анализ";
        
        /// <summary>
        /// Надпись для любой категории
        /// </summary>
        const string anyCategory = "Любая категория";
        
        /// <summary>
        /// Если истина, то анализируются все документы, иначе только выбранные
        /// </summary>
        bool allDocuments;

        /// <summary>
        /// Конструктор главной формы
        /// Создаёт новый проект
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Controller.New();
            UpdateMenuCheckers();
            UpdateMenuStrip();
            ComboBoxPrepare();
        }

        /// <summary>
        /// Обновление чекеров в меню
        /// </summary>
        public void UpdateMenuCheckers()
        {
            Dictionary<ToolStripMenuItem, bool> items = new Dictionary<ToolStripMenuItem,bool>();

            items.Add(lettersToolStripMenuItem, Controller.Project.CountLetters);
            items.Add(биграммToolStripMenuItem, Controller.Project.CountBigrams);
            items.Add(триграммToolStripMenuItem, Controller.Project.CountTrigrams);

            foreach (var pair in items)
            {
                pair.Key.DisplayStyle = pair.Value ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Text;
            }
        }

        /// <summary>
        /// Обработчик события для выбора категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubMenuItem_Click(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripItem;
            Controller.Project.ActiveCategory = clickedMenuItem.Text;
            UpdateMenuStrip();
        }

        /// <summary>
        /// Обновляем список категорий в меню
        /// </summary>
        public void UpdateMenuStrip()
        {
            ToolStripMenuItem parent = activeCategoryToolStripMenuItem;
            parent.DropDownItems.Clear();
            Image img = FrequencyAnalyze.Properties.Resources.check;
            foreach (var cat in Controller.Project.GetAllCategories())
            {
                ToolStripItem item = parent.DropDownItems.Add(cat.Name, img);
                if (cat.Name == Controller.Project.ActiveCategory)
                    item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                else
                    item.DisplayStyle = ToolStripItemDisplayStyle.Text;
                item.Click += SubMenuItem_Click;
            }
        }

        /// <summary>
        /// Обновляем список добавленных документов
        /// </summary>
        public void UpdateListView()
        {
            listView1.Items.Clear();

            List<FileData> allfiles;

            if (comboBox1.SelectedIndex == 0)
                allfiles = Controller.Project.GetAllFiles();
            else
                allfiles = Controller.Project.GetFiles((string)comboBox1.SelectedItem);

            foreach (var filedata in allfiles)
            {
                string[] temp = filedata.GetData();
                string format = (string)comboBox2.SelectedItem;

                if (format != DataType.typeAny && format != temp[0])
                    continue;
                
                ListViewItem added = listView1.Items.Add(new ListViewItem(temp));
                
                int imageIndex;
                
                switch (filedata.Type)
                {
                    case DataType.typeFile:
                        imageIndex = 0;
                        break;

                    case DataType.typeWeb:
                        imageIndex = 1;
                        break;

                    case DataType.typeText:
                        imageIndex = 2;
                        break;

                    default:
                        imageIndex = 0;
                        break;
                }
              
                listView1.Items[added.Index].ImageIndex = imageIndex;
            }
        }

        /// <summary>
        /// Обновляем список категорий
        /// </summary>
        /// <param name="category">активная категория</param>
        public void UpdateCategories(string category = null)
        {
            if (category == null)
            {
                category = anyCategory;
            }

            comboBox1.Items.Clear();
            comboBox1.Items.Add(anyCategory);
            foreach (var cat in Controller.Project.GetAllCategories())
            {
                comboBox1.Items.Add(cat.Name);
            }

            comboBox1.SelectedItem = category;
            UpdateMenuStrip();
        }

        /// <summary>
        /// Активируем пункты меню
        /// </summary>
        public void ActivateMenu()
        {
            if (label1.Visible == true)
            {
                label1.Visible = false;
                panel1.Visible = true;
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                toolStripButton3.Enabled = true;
                toolStripButton6.Enabled = true;
                analyzeToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Деактивируем пункты меню
        /// </summary>
        public void DeActivateMenu()
        {
            if (label1.Visible == false)
            {
                label1.Visible = true;
                panel1.Visible = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                toolStripButton3.Enabled = false;
                toolStripButton6.Enabled = false;
                analyzeToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Подготавливаем список форматов документов
        /// </summary>
        public void ComboBoxPrepare()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add(DataType.typeAny);
            comboBox2.Items.Add(DataType.typeFile);
            comboBox2.Items.Add(DataType.typeWeb);
            comboBox2.Items.Add(DataType.typeText);
            comboBox2.SelectedIndex = 0;
        }

        /// <summary>
        /// Создаём пунктирную рамку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label2_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(180, 180, 180), 3);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            Rectangle rect = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1,
                                           e.ClipRectangle.Height - 1);

            e.Graphics.DrawRectangle(p, rect);
        }

        /// <summary>
        /// Обработчик события при перетаскивании файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label1_DragEnter(object sender, DragEventArgs e)
        {
            //показываем рамку
            label2.Visible = true;
            //получаем список перетаскиваемых файлов
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //получаем информацию о файле
            FileInfo file = new FileInfo(files[0]);
            //формат переводим в нижний регистр
            string extension = file.Extension.ToLower();
            //проверяем, что количество файлов равно 1 и формат файла .txt
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true && files.Length == 1 && extension == ".txt")
            {
                //меняем курсор (информируем пользователя о возможности перетаскивания)
                e.Effect = DragDropEffects.All;
            }
            else
            {
                //меняем курсор, показывая, что нельзя перетащить
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Обработчик события для момента, когда файл был "сброшен"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label1_DragDrop(object sender, DragEventArgs e)
        {
            //скрываем рамку
            label2.Visible = false;
            //получаем список перетаскиваемых файлов
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);   
            //получаем информацию о файле
            FileInfo file = new FileInfo(files[0]);
            //формат переводим в нижний регистр
            string extension = file.Extension.ToLower();
            //проверяем, что количество файлов равно 1 и формат файла .txt
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true && files.Length == 1 && extension == ".txt")
            {
                //активируем пункты меню
                ActivateMenu();
                //загрузка и чтение файла
                Controller.Project.LoadTxtFile(files[0], Controller.Project.ActiveCategory);
                //обновляем ListView список файлов
                UpdateListView();
                //обновляем ComboBox список категорий
                UpdateCategories();
            }
        }

        /// <summary>
        /// Обработчик события, когда файл перетаскивается над label1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label1_DragOver(object sender, DragEventArgs e)
        {
            //показываем рамку
            label2.Visible = true;
        }

        /// <summary>
        /// Обработчик события, когда файл вне границ label1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label1_DragLeave(object sender, EventArgs e)
        {
            //скрываем рамку
            label2.Visible = false;
        }

        /// <summary>
        /// Меняем значения чекера меню буквы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lettersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //меняем логическое значение чекера
            Controller.Project.CountLetters = !Controller.Project.CountLetters;
            //обновляем меню
            UpdateMenuCheckers();
        }

        /// <summary>
        /// Меняем значение чекера меню биграммы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bigramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //меняем логическое значение чекера
            Controller.Project.CountBigrams = !Controller.Project.CountBigrams;
            //обновляем меню
            UpdateMenuCheckers();
        }

        /// <summary>
        /// Меняем значение чекера меню триграммы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trigramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //меняем логическое значение чекера
            Controller.Project.CountTrigrams = !Controller.Project.CountTrigrams;
            //обновляем меню
            UpdateMenuCheckers();
        }

        /// <summary>
        /// Вставить текст вручную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //создаем ссылку на вторую форму
            Form2 form = new Form2(this);
            //запускаем вторую форму
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Вставить текст вручную (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            //перенаправляем событие
            insertManualToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Обработчик события, когда файл перетаскивается над listView1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            //получаем список перетаскиваемых файлов
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //получаем информацию о файле
            FileInfo file = new FileInfo(files[0]);
            //формат переводим в нижний регистр
            string extension = file.Extension.ToLower();
            //проверяем, что количество файлов равно 1 и формат файла .txt
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true && files.Length == 1 && extension == ".txt")
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Обработчик события, когда файл брошен в listView1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            //получаем список перетаскиваемых файлов
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //получаем информацию о файле
            FileInfo file = new FileInfo(files[0]);
            //формат переводим в нижний регистр
            string extension = file.Extension.ToLower();
            
            //проверяем, что количество файлов равно 1 и формат файла .txt
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true && files.Length == 1 && extension == ".txt")
            {
                //Загрузка и чтение файла
                try
                {
                    Controller.Project.LoadTxtFile(files[0], Controller.Project.ActiveCategory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    UpdateListView();
                    UpdateCategories();
                }
            }
        }

        /// <summary>
        /// По клику на элемент в listView1 правой кнопкой мыши, показывается контекстное меню
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
        /// Удаление документа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            int id = listView1.SelectedItems[0].Index;
            string name = listView1.SelectedItems[0].SubItems[1].Text;
            DialogResult res = MessageBox.Show("Вы действительно хотите удалить документ " + name + "?",
                "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (res == DialogResult.Yes)
            {
                Controller.Project.RemoveFile(name);
                listView1.Items.RemoveAt(id);
            }
        }

        /// <summary>
        /// Смена категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateListView();
        }

        /// <summary>
        /// Читать документ из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Загрузка и чтение файла
                try
                {
                    Controller.Project.LoadTxtFile(openFileDialog1.FileName, Controller.Project.ActiveCategory);
                    ActivateMenu();
                    UpdateListView();
                    UpdateCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Смена формата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateListView();
        }

        /// <summary>
        /// Читать из файла (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            fromFileToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Читать из веб-страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromWebPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3(this);
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Открыть менеджер категорий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void categoryManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form = new Form4(this);
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Открыть менеджер категорий (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            categoryManagerToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Открыть из веб-страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            fromWebPageToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Открыть информацию об анализе документа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;

            string name = listView1.SelectedItems[0].SubItems[1].Text;
            FileData data = Controller.Project.GetFile(name);
            Form6 form = new Form6(data, this);
            form.Show();
            form.Activate();
        }

        /// <summary>
        /// Открыть информацию об анализе документа (по двойному клику мыши)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            toolStripMenuItem4_Click(sender, e);
        }

        /// <summary>
        /// Создать новый файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Controller.Project.Changed == true)
            {
                DialogResult res = MessageBox.Show("Вы действительно хотите создать новый проект, "
                 + "не сохранив этот?", "Подтвердите", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (res != DialogResult.Yes)
                    return;
            }

            Controller.New();
            this.Text = formTitle;
            DeActivateMenu();
            UpdateMenuCheckers();
            UpdateListView();
            UpdateCategories();
            ComboBoxPrepare();
        }

        /// <summary>
        /// Сохранить как
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Analyzing Results files (*.ares)|*.ares";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Controller.Save(saveFileDialog1.FileName);
                    Controller.Project.Path = saveFileDialog1.FileName;
                    Controller.Project.Changed = false;
                    FileInfo file = new FileInfo(Controller.Project.Path);
                    this.Text = formTitle + " - " + file.Name;
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при сохранении файла!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Сохранить файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Controller.Project.Path == null)
            {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }

            try
            {
                Controller.Save(Controller.Project.Path);
                Controller.Project.Changed = false;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении файла!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Открыть файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Analyzing Results files (*.ares)|*.ares";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Controller.Open(openFileDialog1.FileName);
                    Controller.Project.Path = openFileDialog1.FileName;
                    Controller.Project.Changed = false;
                    FileInfo file = new FileInfo(Controller.Project.Path);
                    this.Text = formTitle + " - " + file.Name;
                    ActivateMenu();
                    UpdateMenuCheckers();
                    UpdateListView();
                    UpdateCategories();
                    ComboBoxPrepare();
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при открытии файла!", "Внимание",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Создать файл (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Открыть файл (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Сохранить файл (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// О программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = "Программу разработал студент 1-го курса 171 группы " + Environment.NewLine
                        + "отделения программной инженерии Сероусов Виталий. " + Environment.NewLine
                        + "Научный руководитель: Лебедев А.Н.";
            MessageBox.Show(text, "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// О программе (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            aboutProgramToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Анализировать выбранное
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            progressBar1.Visible = true;
            allDocuments = false;
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Фоновый анализ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            bool letters = Controller.Project.CountLetters;
            bool bigrams = Controller.Project.CountBigrams;
            bool trigrams = Controller.Project.CountTrigrams;

            if (allDocuments == false)
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    FileData file = Controller.Project.GetFile(item.SubItems[1].Text);

                    if (file.Letters != null && file.Bigrams != null && file.Trigrams != null)
                        continue;

                    if (letters && file.Letters == null)
                        item.SubItems[3].Text = "Анализ...";
                    if (bigrams && file.Bigrams == null)
                        item.SubItems[4].Text = "Анализ...";
                    if (trigrams && file.Trigrams == null)
                        item.SubItems[5].Text = "Анализ...";

                    file.Count(letters, bigrams, trigrams);

                    if (letters && file.Letters != null)
                        item.SubItems[3].Text = "Да";
                    if (bigrams && file.Bigrams != null)
                        item.SubItems[4].Text = "Да";
                    if (trigrams && file.Trigrams != null)
                        item.SubItems[5].Text = "Да";
                }
            }
            else
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    FileData file = Controller.Project.GetFile(item.SubItems[1].Text);

                    if (file.Letters != null && file.Bigrams != null && file.Trigrams != null)
                        continue;

                    if (letters && file.Letters == null)
                        item.SubItems[3].Text = "Анализ...";
                    if (bigrams && file.Bigrams == null)
                        item.SubItems[4].Text = "Анализ...";
                    if (trigrams && file.Trigrams == null)
                        item.SubItems[5].Text = "Анализ...";

                    file.Count(letters, bigrams, trigrams);

                    if (letters && file.Letters != null)
                        item.SubItems[3].Text = "Да";
                    if (bigrams && file.Bigrams != null)
                        item.SubItems[4].Text = "Да";
                    if (trigrams && file.Trigrams != null)
                        item.SubItems[5].Text = "Да";
                }
            }
        }

       /// <summary>
       /// Фоновый анализ закончен
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
            progressBar1.Visible = false;
            UpdateListView();
        }

        /// <summary>
        /// Анализировать выбранное (меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (button1.Enabled == true)
            {
                button1_Click(sender, e);
            }
        }

        /// <summary>
        /// Анализировать все
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                progressBar1.Visible = true;
                allDocuments = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Анализировать все (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            toolStripMenuItem2_Click(sender, e);
        }

        /// <summary>
        /// Частотные словари
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void freqDictionariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form8 form = new Form8();
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Добавление частотного словаря из категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form7 form = new Form7("category");
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Добавление частотного словаря из документа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void documentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form7 form = new Form7("document");
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Добавление частотного словаря из документа (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            documentToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Частотные словари (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            freqDictionariesToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Дешифратор
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DechiperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form9 form = new Form9(this);
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Дешифратор (кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            DechiperToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
