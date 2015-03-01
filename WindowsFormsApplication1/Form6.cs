using ProjectController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FrequencyAnalyze
{
    public partial class Form6 : Form
    {
        /// <summary>
        /// Данные о документе
        /// </summary>
        FileData currentFile;

        /// <summary>
        /// Ссылка на 1-ю форму
        /// </summary>
        Form1 form1;

        /// <summary>
        /// Тип данных
        /// </summary>
        string type;

        /// <summary>
        /// Тип N-граммы
        /// </summary>
        string element;

        /// <summary>
        /// Конструктор инициализирует основные данные о файле
        /// </summary>
        /// <param name="file">Документ</param>
        /// <param name="form">Форма</param>
        public Form6(FileData file, Form1 form)
        {
            InitializeComponent();
            form1 = form;
            currentFile = file;
            label5.Text = type = file.Type;
            label6.Text = file.Name;

            this.Text += " - " + file.Name;

            Category cat = Controller.Project.GetCategoryByFileName(file.Name);

            label7.Text = cat.Name;
            label8.Text = file.Size;

            radioButton1.Checked = true;

            if (type == DataType.typeFile)
                label2.Text = "Имя файла:";

            if (type == DataType.typeText)
                label2.Text = "Заголовок:";

            if (type == DataType.typeWeb)
                label2.Text = "Веб-адрес:";

            UpdateCheckers();

            element = "letters";

            if (!checkBox1.Enabled && !checkBox2.Enabled && !checkBox3.Enabled)
            {
                button1.Enabled = false;
                button4.Enabled = true;
            }
        }

        /// <summary>
        /// Обновить чекеры
        /// </summary>
        private void UpdateCheckers()
        {
            if (currentFile.Letters == null)
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = true;
            }
            else
            {
                checkBox1.Checked = true;
                checkBox1.Enabled = false;
            }

            if (currentFile.Bigrams == null)
            {
                checkBox2.Checked = false;
                checkBox2.Enabled = true;
            }
            else
            {
                checkBox2.Checked = true;
                checkBox2.Enabled = false;
            }

            if (currentFile.Trigrams == null)
            {
                checkBox3.Checked = false;
                checkBox3.Enabled = true;
            }
            else
            {
                checkBox3.Checked = true;
                checkBox3.Enabled = false;
            }
        }

        /// <summary>
        /// Запуск операции подсчёта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Заполнение ListView таблицой букв\биграмм\триграмм
        /// </summary>
        /// <param name="type">Тип N-граммы</param>
        private void ShowGram(string type)
        {
            listView1.Items.Clear();
            Dictionary<string, int> data;
            int total;
            string stype;
            switch (type)
            {
                case "letters":
                    data = currentFile.Letters;
                    total = currentFile.TotalLetters();
                    stype = "букв";
                    break;

                case "bigrams":
                    data = currentFile.Bigrams;
                    total = currentFile.TotalBigrams();
                    stype = "биграмм";
                    break;

                case "trigrams":
                    data = currentFile.Trigrams;
                    total = currentFile.TotalTrigrams();
                    stype = "триграмм";
                    break;

                default:
                    data = currentFile.Letters;
                    total = currentFile.TotalLetters();
                    stype = "букв";
                break;
            }

            if (data == null)
            {
                label9.Visible = true;
                label9.Text = "Нет данных о частоте " + stype;
                button2.Enabled = false;
                button3.Enabled = false;
                return;
            }
            else if (total == 0)
            {
                label9.Visible = true;
                label9.Text = "В тексте нет " + stype;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                label9.Visible = false;
                button2.Enabled = true;
                button3.Enabled = true;
            }

            element = type;

            if (checkBox4.Checked)
                data = data.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (var elements in data)
            {
                string ngram = elements.Key;
                double frequency = total == 0 ? total : ((double)elements.Value / (double)total) * 100.0;
                string freq = String.Format("{0:F3}", frequency);
                string count = elements.Value.ToString();
                listView1.Items.Add(new ListViewItem(new string[] { ngram, freq, count }));
            }
        }

        /// <summary>
        /// Обновление графика
        /// </summary>
        /// <param name="type">Тип N-граммы</param>
        private void UpdateGraph(string type)
        {
            chart1.Series["Series1"].Points.Clear();
            Dictionary<string, int> data;
            int total;
            switch (type)
            {
                case "letters":
                    data = currentFile.Letters;
                    total = currentFile.TotalLetters();
                    chart1.Titles[0].Text = "Частоты букв, %";
                    break;

                case "bigrams":
                    data = currentFile.Bigrams;
                    total = currentFile.TotalBigrams();
                    chart1.Titles[0].Text = "Частоты биграмм, %";
                    break;

                case "trigrams":
                    data = currentFile.Trigrams;
                    total = currentFile.TotalTrigrams();
                    chart1.Titles[0].Text = "Частоты триграмм, %";
                    break;

                default:
                    data = currentFile.Letters;
                    total = currentFile.TotalLetters();
                    chart1.Titles[0].Text = "Частоты букв, %";
                    break;
            }

            if (data == null || total == 0)
            {
                return;
            }

            data = data.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            int i = 0;
            foreach (var dat in data)
            {
                if (i > 10)
                    break;

                double frequency = ((double)dat.Value / (double)total) * 100.0;
                DataPoint point = chart1.Series["Series1"].Points.Add(frequency);
                point.Label = dat.Key;

                i++;
            }
        }

        /// <summary>
        /// Обновление radioButton1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                listView1.Columns[0].Text = "Буква";
                element = "letters";
                ShowGram("letters");
                UpdateGraph("letters");
            }
        }

        /// <summary>
        /// Обновление radioButton2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                listView1.Columns[0].Text = "Биграмма";
                element = "bigrams";
                ShowGram(element);
                UpdateGraph(element);
            }
        }

        /// <summary>
        /// Обновление radioButton3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                listView1.Columns[0].Text = "Триграмма";
                element = "trigrams";
                ShowGram(element);
                UpdateGraph(element);
            }                
        }

        /// <summary>
        /// Обновление checkBox4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            ShowGram(element);
        }

        /// <summary>
        /// Процесс выполнения подсчёта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked)
            {
                return;
            }

            if (checkBox1.Enabled == false && checkBox2.Enabled == false && checkBox3.Enabled == false)
            {
                return;
            }

            bool countLetters = checkBox1.Checked && checkBox1.Enabled;
            bool countBigrams = checkBox2.Checked && checkBox2.Enabled;
            bool countTrigrams = checkBox3.Checked && checkBox3.Enabled;

            currentFile.Count(countLetters, countBigrams, countTrigrams);
        }

        /// <summary>
        /// Выполнение кода после операции подсчёта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateCheckers();
            ShowGram(element);
            UpdateGraph(element);
            form1.UpdateListView();
            progressBar1.Visible = false;

            if (!checkBox1.Enabled && !checkBox2.Enabled && !checkBox3.Enabled)
            {
                button1.Enabled = false;
                button4.Enabled = true;
            }
            else
            {
                button1.Enabled = true;
                button4.Enabled = false;
            }
        }

        /// <summary>
        /// Закрытие формы - если backgroudWorker работает, то закрыть нельзя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form6_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy)
                e.Cancel = true;
        }

        /// <summary>
        /// Экспорт в excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel file (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                progressBar1.Visible = true;
                backgroundWorker2.RunWorkerAsync(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Экспорт графика в png
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG Image (*.png)|*.png";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    chart1.SaveImage(saveFileDialog1.FileName, ChartImageFormat.Png);
                }
                catch
                {
                    MessageBox.Show("Произошла ошибка при сохранении файла!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Создание частотного словаря - открытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Form7 form = new Form7("document", currentFile.Name);
            form.ShowDialog();
            form.Activate();
        }

        /// <summary>
        /// Процесс экспорта данных в Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
                int j = 1;
                foreach (ListViewItem item in listView1.Items)
                {
                    int i = 1;
                    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                    {
                        if (i == 2)
                            ws.Cells[j, i] = double.Parse(subItem.Text).ToString(new System.Globalization.CultureInfo("en-US").NumberFormat);
                        else
                            ws.Cells[j, i] = subItem.Text;
                        i++;
                    }
                    j++;
                }
                wb.SaveAs((string)e.Argument);
                app.Quit();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении файла!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        /// <summary>
        /// По окончании экспорта данных в excel проинформировать пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            MessageBox.Show("Файл успешно экспортирован в таблицу Excel", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Открытие дешифратора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Form9 form = new Form9(form1, currentFile.Name);
            this.Close();
            form.ShowDialog();
            form.Activate();
        }
    }
}
