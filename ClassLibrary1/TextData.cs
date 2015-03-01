using Counter;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjectController
{
    /// <summary>
    /// Текстовые данные введенные вручную
    /// </summary>
    [Serializable]
    public class TextData : FileData
    {
        /// <summary>
        /// Данные
        /// </summary>
        private string text;

        /// <summary>
        /// Свойство для получения данных
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Конструктор получает основную информацию
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="text">Текст</param>
        public TextData(string name, string text)
        {
            this.type = DataType.typeText;
            this.text = text;
            this.name = name;
            this.md5 = GetMD5(text);
            this.size = text.Length;
        }

        /// <summary>
        /// Подсчёт букв, биграмм, триграмм
        /// </summary>
        /// <param name="countLetters">Подсчёт букв</param>
        /// <param name="countBigrams">Подсчёт биграмм</param>
        /// <param name="countTrigrams">Подсчёт триграмм</param>
        public override void Count(bool countLetters, bool countBigrams, bool countTrigrams)
        {
            if (countLetters == false && countBigrams == false && countTrigrams == false)
            {
                return;
            }

            CountDel Start = null;
            Letter letter = new Letter();
            Bigram bigram = new Bigram();
            Trigram trigram = new Trigram();

            if (countLetters)
                Start += letter.Count;

            if (countBigrams)
                Start += bigram.Count;

            if (countTrigrams)
                Start += trigram.Count;

            Start(text);

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
        /// Получение MD5
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns></returns>
        protected override string GetMD5(string text)
        {
            using (var md5 = MD5.Create())
            {
                byte[] md5bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(text));
                StringBuilder result = new StringBuilder(md5bytes.Length * 2);
                for (int i = 0; i < md5bytes.Length; i++)
                    result.Append(md5bytes[i].ToString("x2"));
                return result.ToString();
            }
        }
    }
}
