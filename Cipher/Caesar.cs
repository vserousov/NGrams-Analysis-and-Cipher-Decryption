using System;
using System.Collections.Generic;
using System.Linq;

namespace Cipher
{
    /// <summary>
    /// Делегат для передачи метода, возвращающий словарь
    /// </summary>
    /// <returns>Словарь частот</returns>
    public delegate Dictionary<string, double> DictDel();

    /// <summary>
    /// Символ в строку с определенными преобразованиями
    /// </summary>
    /// <param name="s">Символ</param>
    /// <returns></returns>
    public delegate string MyDel(char s);

    /// <summary>
    /// Класс шифрования и дешифрования Шифра Цезаря
    /// Сдвиг производится в левую сторону
    /// </summary>
    public static class Caesar
    {
        /// <summary>
        /// Текущий активный язык
        /// </summary>
        static string language = "russian";

        /// <summary>
        /// Свойство для изменения\получения текущего языка
        /// </summary>
        public static string Language
        {
            set { language = value; }
            get { return language; }
        }

        /// <summary>
        /// Пользовательский словарь букв
        /// </summary>
        public static Dictionary<string, double> DLetter
        {
            get;
            set;
        }

        /// <summary>
        /// Пользовательский словарь биграмм
        /// </summary>
        public static Dictionary<string, double> DBigram
        {
            get;
            set;
        }

        /// <summary>
        /// Пользовательский словарь букв
        /// </summary>
        public static Dictionary<string, double> DTrigram
        {
            get;
            set;
        }

        /// <summary>
        /// Функция шифрования текста шифром Цезаря
        /// </summary>
        /// <param name="text">Открытый текст</param>
        /// <param name="shift">Сдвиг</param>
        /// <returns>Шифротекст</returns>
        public static string Encode(string text, int shift)
        {
            char[] str = RemoveSpecialCharacters(text).ToCharArray();
            int power = Power();
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetter(str[i]) && LetterMatchesLanguage(str[i]))
                {
                    char sym = str[i];
                    char start = FirstLetter(sym);
                    str[i] = (char)(Mod(sym + shift - start, power) + start);
                }
            }
            return new string(str);
        }

        /// <summary>
        /// Функция дешифрования текста в шифре Цезаря
        /// </summary>
        /// <param name="text">Шифротекст</param>
        /// <param name="shift">Сдвиг</param>
        /// <returns>Открытый текст</returns>
        public static string Decode(string text, int shift)
        {
            char[] str = text.ToCharArray();
            int power = Power();
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetter(str[i]) && LetterMatchesLanguage(str[i]))
                {
                    char sym = str[i];
                    char start = FirstLetter(sym);
                    str[i] = (char)(Mod(sym - shift - start, power) + start);
                }
            }
            return new string(str);
        }

        /// <summary>
        /// Поиск сдвига для Шифра Цезаря на основе частотности монограмм(букв) и распределения хи-квадрат 
        /// </summary>
        /// <param name="text">Зашифрованный текст</param>
        /// <returns>Числовой сдвиг</returns>
        public static int ShiftSearchChiSquared(string text, DictDel DictLetters)
        {
            int shift = 0;
            double optsum = -1;

            Dictionary<string, double> dict = DictLetters();

            for (int i = 0; i < Power(); i++)
            {
                string decoded = Decode(text, i);

                //Подсчёт букв
                Dictionary<string, double> letters = new Dictionary<string, double>();
                for (int j = 0; j < decoded.Length; j++)
                {
                    char l = char.ToLower(decoded[j]);
                    string ch = l.ToString();
                    if (char.IsLetter(l) && LetterMatchesLanguage(l))
                    {
                        if (letters.ContainsKey(ch))
                            letters[ch]++;
                        else
                            letters.Add(ch, 1);
                    }
                }

                //Подсчёт отклонения
                double dictMax = dict.Values.Max();
                double lettersMax = letters.Values.Max();
                double ratio = dictMax / lettersMax;

                double sum = 0;
                foreach (var item in letters)
                {
                    double E = dict[item.Key.ToLower()];
                    sum += Math.Pow(item.Value * ratio - E, 2) / E;
                }

                if (sum < optsum || i == 0)
                {
                    optsum = sum;
                    shift = i;
                }
            }
            return shift;
        }
        
        /// <summary>
        /// Поиск оптимального ключа для шифра Цезаря на основе резульатов подчёта частот триграмм
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Ключ</returns>
        public static int ShiftSearchFitnessMeasure(string str, DictDel NGram)
        {
            //Генерируем словарь логарифмической вероятности появления триграммы
            Dictionary<string, double> ngrams = NGram();
            Dictionary<string, double> ngramsFitness = new Dictionary<string, double>();
            double total = ngrams.Values.Sum();

            foreach (var ngram in ngrams)
            {
                double p = ngram.Value / total;
                ngramsFitness.Add(ngram.Key, Math.Log10(p));
            }

            //Округляем величину, если частотность триграммы равна 0, чтобы не получить минус бесконечность
            double floor = Math.Log10(0.01 / total);

            double maxScore = new double();
            int shift = new int();
            bool started = false;

            for (int i = 0; i < Power(); i++)
            {
                string decoded = Decode(str, i);
                double score = Score(decoded, ngramsFitness, floor);

                if (started == false)
                {
                    maxScore = score;
                    shift = i;
                    started = true;
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    shift = i;
                }
            }
            return shift;
        }

        /// <summary>
        /// Подсчёт логарифмической вероятности того, что текст открытый
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="trigrams">Триграмма</param>
        /// <param name="floor">Округление</param>
        /// <returns>Логарифмическая вероятность</returns>
        static double Score(string str, Dictionary<string, double> ngrams, double floor)
        {
            MyDel getStr = s => char.ToLower(s).ToString();
            double sum = 0;

            List<string> temp = ngrams.Keys.ToList();
            int len = temp[0].Length;
            for (int i = 0; i < str.Length - len + 1; i++)
            {
                string curngram = null;
                
                for (int j = 0; j < len; j++)
                {
                    curngram += getStr(str[i + j]);
                }

                if (ngrams.ContainsKey(curngram))
                    sum += ngrams[curngram];
                else
                    sum += floor;
            }
            return sum;
        }
        
        /// <summary>
        /// Определение языка
        /// </summary>
        /// <param name="str">строка</param>
        public static void DetectLanguage(string str)
        {
            int numRussian = 0;
            int numEnglish = 0;

            char rusStart = Frequency.russianStart;
            char rusEnd = Frequency.russianEnd;
            char engStart = Frequency.englishStart;
            char engEnd = Frequency.englishEnd;

            str = RemoveSpecialCharacters(str);

            for (int i = 0; i < str.Length; i++)
            {
                if (rusStart <= str[i] && str[i] <= rusEnd)
                    numRussian++;
                if (engStart <= str[i] && str[i] <= engEnd)
                    numEnglish++;
            }

            if (numRussian > numEnglish)
                language = "russian";
            else
                language = "english";
        }

        /// <summary>
        /// Проверка на совпадение языка и буквы
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        static bool LetterMatchesLanguage(char letter)
        {
            letter = char.ToLower(letter);
            if (language == "russian")
                return Frequency.russianStart <= letter && letter <= Frequency.russianEnd;
            return Frequency.englishStart <= letter && letter <= Frequency.englishEnd;
        }

        /// <summary>
        /// Удаление особых символов
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns>Очищенный текст</returns>
        static string RemoveSpecialCharacters(string text)
        {
            if (language == "russian")
            {
                text = text.Replace("ё", "е");
                text = text.Replace("Ё", "Е");
            }
            return text;
        }

        /// <summary>
        /// Получение первой буквы в алфавите в нужном регистре
        /// </summary>
        /// <param name="sym">Символ</param>
        /// <returns>Буква</returns>
        static char FirstLetter(char sym)
        {
            char start = language == "russian" ? Frequency.russianStart : Frequency.englishStart;
            return char.IsUpper(sym) ? char.ToUpper(start) : start;
        }

        /// <summary>
        /// Получение мощности алфавита
        /// </summary>
        /// <returns>Числовая мощность</returns>
        public static int Power()
        {
            return language == "russian" ? Frequency.russianNum : Frequency.englishNum;
        }

        /// <summary>
        /// Получение частотного словаря букв
        /// </summary>
        /// <returns>Словарь(буква, частота)</returns>
        public static Dictionary<string, double> LettersDict()
        {
            if (DLetter == null)
            {
                return language == "russian" ? Frequency.RussianLetters : Frequency.EnglishLetters;
            }
            else
            {
                double sum = DLetter.Values.Sum();
                Dictionary<string, double> freq = new Dictionary<string,double>();
                foreach (var item in DLetter)
                {
                    if (!freq.ContainsKey(item.Key))
                        freq.Add(item.Key, (item.Value / sum) * 100);
                }
                return freq;
            }
        }

        /// <summary>
        /// Получение частотного словаря биграмм
        /// </summary>
        /// <returns>Словарь(биграмма, частота)</returns>
        public static Dictionary<string, double> BigramsDict()
        {
            if (DBigram == null)
            {
                Dictionary<string, double> bigrams = new Dictionary<string, double>();

                string temp;
                if (language == "russian")
                    temp = Properties.Resources.russian_bigrams;
                else
                    temp = Properties.Resources.english_bigrams;

                StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
                string[] lines = temp.Split(new string[] { Environment.NewLine }, options);

                foreach (string line in lines)
                {
                    string[] item = line.Split(' ');
                    bigrams.Add(item[0].ToLower(), double.Parse(item[1]));
                }

                return bigrams;
            }
            else
            {
                return DBigram;
            }
        }
        
        /// <summary>
        /// Получение частотного словаря триграмм
        /// </summary>
        /// <returns>Словарь(триграмма, частота)</returns>
        public static Dictionary<string, double> TrigramsDict()
        {
            if (DTrigram == null)
            {
                Dictionary<string, double> trigrams = new Dictionary<string, double>();

                string temp;
                if (language == "russian")
                    temp = Properties.Resources.russian_trigrams;
                else
                    temp = Properties.Resources.english_trigrams;

                StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
                string[] lines = temp.Split(new string[] { Environment.NewLine }, options);

                foreach (string line in lines)
                {
                    string[] item = line.Split(' ');
                    trigrams.Add(item[0].ToLower(), double.Parse(item[1]));
                }

                return trigrams;
            }
            else
            {
                return DTrigram;
            }
        }

        /// <summary>
        /// Деление по модулю
        /// </summary>
        /// <param name="m">Число</param>
        /// <param name="n">По модулю</param>
        /// <returns></returns>
        static int Mod(int m, int n)
        {
            return (Math.Abs(m * n) + m) % n;
        }
    }
}
