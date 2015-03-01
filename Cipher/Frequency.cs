using System.Collections.Generic;

namespace Cipher
{
    public struct Frequency
    {
        /// <summary>
        /// Частотный словарь русских букв
        /// </summary>
        public static Dictionary<string, double> RussianLetters = new Dictionary<string, double>
        {
            {"а", 7.92}, {"б", 1.71}, {"в", 4.33}, {"г", 1.74},
            {"д", 3.05}, {"е", 8.41}, {"ж", 1.05}, {"з", 1.75},
            {"и", 6.83}, {"й", 1.12}, {"к", 3.36}, {"л", 5.00},
            {"м", 3.26}, {"н", 6.72}, {"о", 11.08}, {"п", 2.81},
            {"р", 4.45}, {"с", 5.33}, {"т", 6.18}, {"у", 2.80},
            {"ф", 0.19}, {"х", 0.89}, {"ц", 0.36}, {"ч", 1.47},
            {"ш", 0.81}, {"щ", 0.37}, {"ъ", 0.02}, {"ы", 1.96},
            {"ь", 1.92}, {"э", 0.38}, {"ю", 0.61}, {"я", 2.13}
        };

        /// <summary>
        /// Частотный словарь английских букв
        /// </summary>
        public static Dictionary<string, double> EnglishLetters = new Dictionary<string, double>
        {
            {"a", 8.167}, {"b", 1.492}, {"c", 2.782}, {"d", 4.253},
            {"e", 12.702}, {"f", 2.228}, {"g", 2.015}, {"h", 6.094}, 
            {"i", 6.996}, {"j", 0.153}, {"k", 0.772}, {"l", 4.025}, 
            {"m", 2.406}, {"n", 6.749}, {"o", 7.507}, {"p", 1.929}, 
            {"q", 0.095}, {"r", 5.987}, {"s", 6.327}, {"t", 9.056}, 
            {"u", 2.758}, {"v", 0.978}, {"w", 2.360}, {"x", 0.150}, 
            {"y", 1.974}, {"z", 0.074}
        };

        /// <summary>
        /// Первая русская буква
        /// </summary>
        public static char russianStart = 'а';

        /// <summary>
        /// Последняя русская буква
        /// </summary>
        public static char russianEnd = 'я';

        /// <summary>
        /// Первая английская буква
        /// </summary>
        public static char englishStart = 'a';

        /// <summary>
        /// Последняя английская буква
        /// </summary>
        public static char englishEnd = 'z';

        /// <summary>
        /// Количество русских букв
        /// </summary>
        public static int russianNum = (int)russianEnd - (int)russianStart + 1;

        /// <summary>
        /// Количество английских букв
        /// </summary>
        public static int englishNum = (int)englishEnd - (int)englishStart + 1;
    }
}
