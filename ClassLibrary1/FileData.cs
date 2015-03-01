using Counter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjectController
{
    /// <summary>
    /// Класс данных из файла
    /// Базовый для TextData и WebData
    /// </summary>
    [Serializable]
    public class FileData
    {
        /// <summary>
        /// Делегат подсчёта частот в тексте
        /// </summary>
        /// <param name="str">Строка</param>
        public delegate void CountDel(string str);

        /// <summary>
        /// Размер файла
        /// </summary>
        protected long size;

        /// <summary>
        /// Тип файла (текстовый, введёный вручную, веб-страница)
        /// </summary>
        protected string type;

        /// <summary>
        /// Путь к файлу
        /// </summary>
        protected string path;

        /// <summary>
        /// Имя файла
        /// </summary>
        protected string name;

        /// <summary>
        /// Расширение файла
        /// </summary>
        private string extension;

        /// <summary>
        /// MD5 файла
        /// </summary>
        protected string md5;

        /// <summary>
        /// Всего букв
        /// </summary>
        protected int totalLetters;

        /// <summary>
        /// Частоты букв
        /// </summary>
        protected Dictionary<string, int> letters;

        /// <summary>
        /// Частоты биграмм
        /// </summary>
        protected Dictionary<string, int> bigrams;

        /// <summary>
        /// Частоты триграмм
        /// </summary>
        protected Dictionary<string, int> trigrams;

        /// <summary>
        /// Получение размера файла в единицах измерения информации
        /// </summary>
        public string Size
        {
            get { return GetSizeFormat(size); }
        }

        /// <summary>
        /// Получение размера файла в байтах
        /// </summary>
        public long SizeNum
        {
            get { return size; }
        }

        /// <summary>
        /// Получение MD5 файла
        /// </summary>
        public string Md5
        {
            get { return md5; }
        }

        /// <summary>
        /// Получение типа файла
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// Получение имени файла
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Получение полного пути файла
        /// </summary>
        public string FullPath
        {
            get { return path; }
        }

        /// <summary>
        /// Получение расширения файла
        /// </summary>
        public string Extension
        {
            get { return extension; }
        }

        /// <summary>
        /// Получение частотного словаря букв
        /// </summary>
        public Dictionary<string, int> Letters
        {
            get { return letters; }
        }

        /// <summary>
        /// Получение частотного словаря биграмм
        /// </summary>
        public Dictionary<string, int> Bigrams
        {
            get { return bigrams; }
        }

        /// <summary>
        /// Получение частотного словаря триграмм
        /// </summary>
        public Dictionary<string, int> Trigrams
        {
            get { return trigrams; }
        }

        /// <summary>
        /// Конструктор по умолчанию (ничего не выполняет)
        /// </summary>
        public FileData() { }

        /// <summary>
        /// Конструктор получает основную информацию о файле
        /// </summary>
        /// <param name="path"></param>
        public FileData(string path)
        {
            FileInfo file = new FileInfo(path);
            this.type = DataType.typeFile;
            this.path = file.FullName;
            this.name = file.Name;
            this.size = file.Length;
            this.extension = file.Extension;
            this.md5 = GetMD5(path);
        }

        /// <summary>
        /// Подсчёт частот букв, биграмм и триграмм
        /// </summary>
        /// <param name="countLetters">Частоты букв</param>
        /// <param name="countBigrams">Частоты биграмм</param>
        /// <param name="countTrigrams">Частоты триграмм</param>
        public virtual void Count(bool countLetters, bool countBigrams, bool countTrigrams)
        {
            if (countLetters == false && countBigrams == false && countTrigrams == false)
            {
                return;
            }

            Letter letter = new Letter();
            Bigram bigram = new Bigram();
            Trigram trigram = new Trigram();

            CountDel Start = null;

            if (countLetters)
                Start += letter.Count;

            if (countBigrams)
                Start += bigram.Count;

            if (countTrigrams)
                Start += trigram.Count;


            if (!File.Exists(this.path))
            {
                Controller.Project.RemoveFile(this.Name);
                return;
            }

            using (StreamReader reader = new StreamReader(this.path, Encoding.Default, true))
            {
                while (!reader.EndOfStream)
                {
                    string temp = reader.ReadLine();
                    Start(temp);
                }
                reader.Close();
            }

            string language = null;

            if (countLetters && language == null)
            {
                letter.DetectLanguage();
                language = letter.Language;
            }

            if (countBigrams && language == null)
            {
                bigram.DetectLanguage();
                language = bigram.Language;
            }

            if (countTrigrams && language == null)
            {
                trigram.DetectLanguage();
                language = trigram.Language;
            }

            if (countLetters)
                letters = letter.GetData(language);

            if (countBigrams)
                bigrams = bigram.GetData(language);

            if (countTrigrams)
                trigrams = trigram.GetData(language);

            Controller.Project.Changed = true;
        }

        /// <summary>
        /// Подсчёт общего числа букв
        /// </summary>
        /// <returns>Число</returns>
        public int TotalLetters()
        {
            int sum = 0;
            if (letters != null)
                foreach (var p in letters)
                    sum += p.Value;
            return sum;
        }

        /// <summary>
        /// Подсчёт общего числа биграмм
        /// </summary>
        /// <returns>Число</returns>
        public int TotalBigrams()
        {
            int sum = 0;
            if (bigrams != null)
                foreach (var p in bigrams)
                    sum += p.Value;
            return sum;
        }

        /// <summary>
        /// Подсчёт общего числа триграмм
        /// </summary>
        /// <returns>Число</returns>
        public int TotalTrigrams()
        {
            int sum = 0;
            if (trigrams != null)
                foreach (var p in trigrams)
                    sum += p.Value;
            return sum;
        }

        /// <summary>
        /// Получение данных для listViewItem
        /// </summary>
        /// <returns>Данные в виде массива строк</returns>
        public string[] GetData()
        {
            string type = this.type;
            string name = this.name;
            string size = GetSizeFormat(this.size);
            string letters = this.letters == null ? "Нет" : "Да";
            string bigrams = this.bigrams == null ? "Нет" : "Да";
            string trigrams = this.trigrams == null ? "Нет" : "Да";
            return new string[] { type, name, size, letters, bigrams, trigrams };
        }

        /// <summary>
        /// Получение MD5 файла
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>MD5</returns>
        protected virtual string GetMD5(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] md5bytes = md5.ComputeHash(stream);
                    StringBuilder result = new StringBuilder(md5bytes.Length * 2);
                    for (int i = 0; i < md5bytes.Length; i++)
                        result.Append(md5bytes[i].ToString("x2"));
                    return result.ToString();
                }
            }
        }

        /// <summary>
        /// Получение размера файлах в единицах измерения
        /// </summary>
        /// <param name="length">Размер файла</param>
        /// <returns>Форматированный размер</returns>
        protected string GetSizeFormat(long length)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (length >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                length = length / 1024;
            }
            return String.Format("{0:0.##} {1}", length, sizes[order]);
        }
    }
}
