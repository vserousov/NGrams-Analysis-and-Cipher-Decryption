using System.Windows.Forms;

namespace FrequencyAnalyze
{
    public partial class Form10 : Form
    {
        /// <summary>
        /// Конструктор заполняет поле полученными данными
        /// </summary>
        /// <param name="result"></param>
        public Form10(string result)
        {
            InitializeComponent();
            richTextBox1.Text = result;
        }
    }
}
