using System;
using System.Collections.Generic;

namespace Counter
{
    /// <summary>
    /// Класс для работы с триграммами
    /// </summary>
    public class Trigram : Ngram
    {
        /// <summary>
        /// Триграммы и их частоты
        /// </summary>
        private Dictionary<string, int> data = new Dictionary<string, int>();

        /// <summary>
        /// Триграммы и расстояние между ними
        /// </summary>
        private Dictionary<string, List<int>> pos = new Dictionary<string, List<int>>();

        /// <summary>
        /// Подсчёт
        /// </summary>
        /// <param name="str">Строка</param>
        public override void Count(string str)
        {
            for (int i = 0; i < str.Length - 2; i++)
            {
                char c1 = char.ToLower(str[i]);
                char c2 = char.ToLower(str[i + 1]);
                char c3 = char.ToLower(str[i + 2]);

                if (!IsLetter(c1) || !IsLetter(c2) || !IsLetter(c3))
                    continue;

                if (!IsSameLanguage(c1, c2, c3))
                    continue;

                string trigram = c1.ToString() + c2.ToString() + c3.ToString();
  
                if (data.ContainsKey(trigram))
                {
                    data[trigram]++;
                }
                else
                {
                    data.Add(trigram, 1);
                    pos.Add(trigram, new List<int>());
                }

                pos[trigram].Add(i);
            }
        }

        /// <summary>
        /// Получить данные в словаре
        /// </summary>
        /// <returns>Словарь данных</returns>
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
