using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectController
{
    [Serializable]
    public class FrequencyDictionary
    {
        /// <summary>
        /// Тип словаря: категория - category, документ - document
        /// </summary>
        string type;

        /// <summary>
        /// Частотный словарь букв
        /// </summary>
        Dictionary<string, double> letters;
        
        /// <summary>
        /// Частотный словарь биграмм
        /// </summary>
        Dictionary<string, double> bigrams;

        /// <summary>
        /// Частотный словарь триграмм
        /// </summary>
        Dictionary<string, double> trigrams;

        /// <summary>
        /// Тип словаря
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// Частотный словарь букв
        /// </summary>
        public Dictionary<string, double> Letters
        {
            get { return letters; }
        }

        /// <summary>
        /// Частотный словарь биграмм
        /// </summary>
        public Dictionary<string, double> Bigrams
        {
            get { return bigrams; }
        }

        /// <summary>
        /// Частотный словарь триграмм
        /// </summary>
        public Dictionary<string, double> Trigrams
        {
            get { return trigrams; }
        }

        /// <summary>
        /// Конструткор задаёт тип частного словаря
        /// </summary>
        /// <param name="type">Тип частотного словаря</param>
        public FrequencyDictionary(string type)
        {
            this.type = type;
        }

        /// <summary>
        /// Добавление букв в частотный словарь
        /// </summary>
        /// <param name="dictionaries">Словари</param>
        public void AddLetters(params Dictionary<string, int>[] dictionaries)
        {
            letters = new Dictionary<string, double>();

            if (dictionaries.Length > 1)
            {
                foreach (Dictionary<string, int> dict in dictionaries)
                {
                    foreach (var item in dict)
                    {
                        if (letters.ContainsKey(item.Key))
                            letters[item.Key] += item.Value;
                        else
                            letters.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                foreach (var item in dictionaries[0])
                {
                    letters.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Добавление биграмм в частотный словарь
        /// </summary>
        /// <param name="dictionaries">Словари</param>
        public void AddBigrams(params Dictionary<string, int>[] dictionaries)
        {
            bigrams = new Dictionary<string, double>();

            if (dictionaries.Length > 1)
            {
                foreach (Dictionary<string, int> dict in dictionaries)
                {
                    foreach (var item in dict)
                    {
                        if (bigrams.ContainsKey(item.Key))
                            bigrams[item.Key] += item.Value;
                        else
                            bigrams.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                foreach (var item in dictionaries[0])
                {
                    bigrams.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Добавление триграмм в частотный словарь
        /// </summary>
        /// <param name="dictionaries">Словарь</param>
        public void AddTrigrams(params Dictionary<string, int>[] dictionaries)
        {
            trigrams = new Dictionary<string, double>();

            if (dictionaries.Length > 1)
            {
                foreach (Dictionary<string, int> dict in dictionaries)
                {
                    foreach (var item in dict)
                    {
                        if (trigrams.ContainsKey(item.Key))
                            trigrams[item.Key] += item.Value;
                        else
                            trigrams.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                foreach (var item in dictionaries[0])
                {
                    trigrams.Add(item.Key, item.Value);
                }
            }
        }
    }
}
