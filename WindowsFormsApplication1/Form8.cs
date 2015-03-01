using ProjectController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form8 : Form
    {
        /// <summary>
        /// Конструктор инициализирует список частотных словарей
        /// </summary>
        public Form8()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            
            if (Controller.Project.Dicts != null)
            {
                foreach (var item in Controller.Project.Dicts)
                {
                    string type = item.Value.Type == "document" ? "Документ" : "Категория";
                    listView1.Items.Add(new ListViewItem(new string[] { item.Key, type }));
                }
            }
        }

        /// <summary>
        /// Удаление частотного словаря
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                try
                {
                    string name = listView1.SelectedItems[0].SubItems[0].Text;
                    Controller.Project.DeleteDictionary(name);
                    listView1.Items.Clear();
                    foreach (var item in Controller.Project.Dicts)
                    {
                        string type = item.Value.Type == "document" ? "Документ" : "Категория";
                        listView1.Items.Add(new ListViewItem(new string[] { item.Key, type }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Экспорт в excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel file (*.xls)|*.xls";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    progressBar1.Visible = true;
                    backgroundWorker1.RunWorkerAsync(saveFileDialog1.FileName);
                }
            }
        }

        /// <summary>
        /// Процесс экспорта выполняется в фоне
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];

                string name = listView1.SelectedItems[0].SubItems[0].Text;
                FrequencyDictionary dict = Controller.Project.Dicts[name];

                double total = dict.Letters.Values.Sum();

                Dictionary<string, double> data = null;

                if (comboBox1.SelectedIndex == 0)
                    data = dict.Letters;
                if (comboBox1.SelectedIndex == 1)
                    data = dict.Bigrams;
                if (comboBox1.SelectedIndex == 2)
                    data = dict.Trigrams;

                data = data.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                int i = 1;
                foreach (var item in data)
                {
                    ws.Cells[i, 1] = item.Key;
                    ws.Cells[i, 2] = ((item.Value / total) * 100).ToString(new System.Globalization.CultureInfo("en-US").NumberFormat);
                    ws.Cells[i, 3] = item.Value;
                    i++;
                }

                wb.SaveAs((string)e.Argument);
                app.Quit();
            }
            catch
            {
                progressBar1.Visible = false;
                MessageBox.Show("Произошла ошибка при сохранении файла!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// По окончании процесса экспорта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            MessageBox.Show("Операция успешно завершена!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
