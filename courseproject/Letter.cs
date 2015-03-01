using System;
using System.Collections.Generic;

namespace Counter
{
    /// <summary>
    /// Делегат для проверки языка символа
    /// </summary>
    /// <param name="s">Символ</param>
    /// <returns>Язык</returns>
    delegate bool LangDel(char s);

    /// <summary>
    /// Класс для работы с буквами
    /// </summary>
    public class Letter : Ngram
    {
        /// <summary>
        /// Количество русских букв
        /// </summary>
        private const int numRussian = (int)endRussian - (int)startRussian + 1;

        /// <summary>
        /// Количество английских букв
        /// </summary>
        private const int numEnglish = (int)endEnglish - (int)startEnglish + 1;
        
        /// <summary>
        /// Код первой русской буквы
        /// </summary>
        private const int russianBegins = (int)startRussian;
        
        /// <summary>
        /// Код первой английской буквы
        /// </summary>
        private const int englishBegins = (int)startEnglish;

        /// <summary>
        /// Массив данных о частоте
        /// </summary>
        private int[] counter;

        /// <summary>
        /// Конструктор задаёт размер массива данных
        /// </summary>
        public Letter()
        {
            counter = new int[numRussian + numEnglish];
        }

        /// <summary>
        /// Подсчёт
        /// </summary>
        /// <param name="str">Строка</param>
        public override void Count(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char symbol = char.ToLower(str[i]);
                if (IsLetter(symbol))
                {
                    int index = Index(symbol);
                    counter[index]++;
                }
            }
        }

        /// <summary>
        /// Получить данные в словаре
        /// </summary>
        /// <returns>Словарь данных</returns>
        public override Dictionary<string, int> GetData()
        {
            var temp = new Dictionary<string, int>();

            for (int i = 0; i < counter.Length; i++)
            {
                temp.Add(Symbol(i).ToString(), counter[i]);
            }

            return temp;
        }

        /// <summary>
        /// Получить данные
        /// </summary>
        /// <param name="language">Язык</param>
        /// <returns>Словарь данных</returns>
        public Dictionary<string, int> GetData(string lang)
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();

            LangDel del;

            if (lang == "russian")
                del = IsRussian;
            else
                del = IsEnglish;

            for (int i = 0; i < counter.Length; i++)
                if (del(Symbol(i)))
                    temp.Add(Symbol(i).ToString(), counter[i]);

            return temp;
        }

        /// <summary>
        /// Получить данные в текстовом формате
        /// </summary>
        /// <param name="format">Формат данных</param>
        /// <returns>Текстовые данные</returns>
        public override string GetTextData(string format)
        {
            string result = "";
            for (int i = 0; i < counter.Length; i++)
            {
                result += String.Format(format, Symbol(i), counter[i]);
            }
            return result;
        }

        /// <summary>
        /// Индекс буквы
        /// </summary>
        /// <param name="sym">Буква</param>
        /// <returns>Индекс</returns>
        private int Index(char sym)
        {
            char letter = sym == special ? specialReplace : sym; //заменяем букву ё на е
            int symCode = (int)letter;
            int result = IsRussian(letter) ? symCode - russianBegins : symCode - englishBegins + numRussian;
            return result;
        }

        /// <summary>
        /// Получить символ по индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns>Символ</returns>
        private char Symbol(int index)
        {
            if (index >= numRussian)
                return (char)(index + englishBegins - numRussian);
            else
                return (char)(index + russianBegins);
        }
    }
}
