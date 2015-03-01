using System.Collections.Generic;
using System.Linq;

namespace Counter
{
    /// <summary>
    /// Абстрактный класс Ngram - является родительским для Letter, Bigram, Trigram
    /// </summary>
    abstract public class Ngram
    {
        /// <summary>
        /// Первая русская буква
        /// </summary>
        protected const char startRussian = 'а';
        
        /// <summary>
        /// Последняя русская буква
        /// </summary>
        protected const char endRussian = 'я';

        /// <summary>
        /// Первая английская буква
        /// </summary>
        protected const char startEnglish = 'a';

        /// <summary>
        /// Последняя английская буква
        /// </summary>
        protected const char endEnglish = 'z';

        /// <summary>
        /// Буква ё
        /// </summary>
        protected const char special = 'ё';
        
        /// <summary>
        /// Буква е (для замены ё)
        /// </summary>
        protected const char specialReplace = 'е';

        /// <summary>
        /// Текущий язык
        /// </summary>
        protected string language = null;

        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        /// <summary>
        /// Подсчёт
        /// </summary>
        /// <param name="str">Строка</param>
        public abstract void Count(string str);

        /// <summary>
        /// Получить данные в словаре
        /// </summary>
        /// <returns>Данные в словаре</returns>
        public abstract Dictionary<string, int> GetData();

        /// <summary>
        /// Получить данные в текстовом формате
        /// </summary>
        /// <param name="format">Формат</param>
        /// <returns>Текстовые данные</returns>
        public abstract string GetTextData(string format);

        /// <summary>
        /// Проверка, является ли буква русской
        /// </summary>
        /// <param name="sym">Символ</param>
        /// <returns>Результат проверки</returns>
        protected bool IsRussian(char sym)
        {
            return startRussian <= sym && sym <= endRussian || sym == special;
        }

        /// <summary>
        /// Проверка, является ли буква английской
        /// </summary>
        /// <param name="sym">Символ</param>
        /// <returns>Результат проверки</returns>
        protected bool IsEnglish(char sym)
        {
            return startEnglish <= sym && sym <= endEnglish;
        }

        /// <summary>
        /// Проверка, является ли буквой
        /// </summary>
        /// <param name="sym">Символ</param>
        /// <returns>Результат проверки</returns>
        protected bool IsLetter(char sym)
        {
            sym = char.ToLower(sym);
            return IsRussian(sym) || IsEnglish(sym);
        }

        /// <summary>
        /// Определение языка
        /// </summary>
        public void DetectLanguage()
        {
            Dictionary<string, int> data = GetData();
            int sum = data.Values.Sum();
            Dictionary<string, int> temp = data.Where(x => GetLanguage(x.Key[0]) == "russian")
                                        .ToDictionary(x => x.Key, x => x.Value);
            int russian = temp.Values.Sum();

            if (russian > sum - russian)
                language = "russian";
            else
                language = "english";
        }

        /// <summary>
        /// Получить язык
        /// </summary>
        /// <param name="c">Символ</param>
        /// <returns>Язык</returns>
        protected string GetLanguage(char c)
        {
            char cl = char.ToLower(c);
            if (IsRussian(cl))
                return "russian";
            else if (IsEnglish(cl))
                return "english";
            else
                return "undefined";
        }

        /// <summary>
        /// Проверить, одинаковый ли язык у разных букв
        /// </summary>
        /// <param name="c">Разные буквы</param>
        /// <returns>Результат проверки</returns>
        protected bool IsSameLanguage(params char[] c)
        {
            string[] languages = new string[c.Length];
            
            for (int i = 0; i < c.Length; i++)
            {
                languages[i] = this.GetLanguage(c[i]);
            }

            for (int i = 0; i < c.Length - 1; i++)
            {
                if (languages[i] == languages[i + 1])
                    continue;
                else
                    return false;
            }
            return true;
        }
    }
}
