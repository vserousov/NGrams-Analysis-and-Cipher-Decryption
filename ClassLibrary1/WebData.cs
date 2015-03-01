using Counter;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectController
{
    /// <summary>
    /// Класс веб-данные
    /// </summary>
    [Serializable]
    public class WebData : FileData
    {
        /// <summary>
        /// Текстовые данные
        /// </summary>
        private string text;

        /// <summary>
        /// Свойство для текстовых данных
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Конструктор парсит страницы и получает основную информацию
        /// </summary>
        /// <param name="url">URL-адрес</param>
        public WebData(string url)
        {
            type = DataType.typeWeb;
            name = url;
            text = GetPage(url);
            text = ClearTags(text);
            size = text.Length;
            md5 = GetMD5(text);
        }

        /// <summary>
        /// Конструктор парсит страницы и получает основную информацию, вырезая текст по указанным границам
        /// </summary>
        /// <param name="url">URL-адрес</param>
        /// <param name="start">Начало текста</param>
        /// <param name="end">Конец текста</param>
        public WebData(string url, string start, string end)
        {
            type = DataType.typeWeb;
            name = url;
            text = GetPage(url);
            text = CutText(text, start, end);
            text = ClearTags(text);
            size = text.Length;
            md5 = GetMD5(text);
        }

        /// <summary>
        /// Вырезание текста по указанным границам
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="start">Начало</param>
        /// <param name="end">Конец</param>
        /// <returns>Текст</returns>
        private string CutText(string text, string start, string end)
        {
            StringSplitOptions option = StringSplitOptions.None;
            string[] startArr = new string[] { start };
            string[] endArr = new string[] { end };
            string temp = text;
            temp = temp.Split(startArr, option)[1];
            temp = temp.Split(endArr, option)[0];
            return temp;
        }

        /// <summary>
        /// Очистка от HTML-тегов
        /// </summary>
        /// <param name="code">HTML-код</param>
        /// <returns>Чистый текст</returns>
        private string ClearTags(string code)
        {
            string clean = Regex.Replace(code, "<[^>]+>", string.Empty);
            return clean;
        }

        /// <summary>
        /// Получение страницы
        /// </summary>
        /// <param name="url">URL-Адрес</param>
        /// <returns>HTML-код</returns>
        private string GetPage(string url)
        {
            try
            {
                return MakeRequest(url, "text/html");
            }
            catch
            {
                throw new Exception("Невозможно получить веб-страницу!");
            }
        }

        /// <summary>
        /// Получение MD5
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns>MD5</returns>
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

            if (countLetters)
                letters = letter.GetData();

            if (countBigrams)
                bigrams = bigram.GetData();

            if (countTrigrams)
                trigrams = trigram.GetData();

            Controller.Project.Changed = true;
        }

        /// <summary>
        /// Отправить запрос
        /// </summary>
        /// <param name="url">URL-адрес</param>
        /// <param name="contentType">Тип контента</param>
        /// <returns>HTML-код</returns>
        public string MakeRequest(string url, string contentType)
        {
            Uri uri = new Uri(FormatUrl(url));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 10000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.116 Safari/537.36";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string Charset = response.CharacterSet;
            Encoding encoding = Encoding.GetEncoding(Charset);
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream, encoding))
            {
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

        /// <summary>
        /// Добавление http:// в случае, если требуется
        /// </summary>
        /// <param name="Url">URL-адрес</param>
        /// <returns>URL-адрес с http://</returns>
        public static string FormatUrl(string Url)
        {
            Url = Url.ToLower();
            if (!(Url.StartsWith("http://") || Url.StartsWith("https://")))
               Url = "http://" + Url;
            return Url;
        }
    }
}