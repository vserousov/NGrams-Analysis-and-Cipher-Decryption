using System;
using System.Collections.Generic;

namespace Counter
{
    /// <summary>
    /// Класс для работы с биграммами
    /// </summary>
    public class Bigram : Ngram
    {
        /// <summary>
        /// Биграммы и их частоты
        /// </summary>
        private Dictionary<string, int> data = new Dictionary<string, int>();
        
        /// <summary>
        /// Биграмма и позиция
        /// </summary>
        private Dictionary<string, List<int>> pos = new Dictionary<string, List<int>>();

        /// <summary>
        /// Подсчёт
        /// </summary>
        /// <param name="str">Строка</param>
        public override void Count(string str)
        {
            for (int i = 0; i < str.Length - 1; i++)
            {
                char c1 = char.ToLower(str[i]);
                char c2 = char.ToLower(str[i + 1]);

                if (!IsLetter(c1) || !IsLetter(c2))
                    continue;

                if (!IsSameLanguage(c1, c2))
                    continue;

                string bigram = c1.ToString() + c2.ToString();

                if (data.ContainsKey(bigram))
                {
                    data[bigram]++;
                }
                else
                {
                    data.Add(bigram, 1);
                    pos.Add(bigram, new List<int>());
                }

                pos[bigram].Add(i);
            }
        }

        /// <summary>
        /// Получить данные в словаре
        /// </summary>
        /// <returns>Данные в словаре</returns>
        public override Dictionary<string, int> GetData()
        {
            return data;
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

            foreach (var item in data)
                if (del(item.Key[0]))
                    temp.Add(item.Key, item.Value);

            return temp;
        }

        /// <summary>
        /// Получить данные в текстовом формате
        /// </summary>
        /// <param name="format">Формат</param>
        /// <returns>Текстовые данные</returns>
        public override string GetTextData(string format)
        {
            string result = "";
            foreach (var e in data)
            {
                result += String.Format(format, e.Key, e.Value);
            }
            return result;
        }
    }
}
