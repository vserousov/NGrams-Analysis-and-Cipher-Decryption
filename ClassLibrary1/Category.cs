using System;

namespace ProjectController
{
    /// <summary>
    /// Категория
    /// </summary>
    [Serializable]
    public class Category
    {
        /// <summary>
        /// Имя категории
        /// </summary>
        private string name;

        /// <summary>
        /// Задание и получение имени категории
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Конструктор категории
        /// </summary>
        /// <param name="name"></param>
        public Category(string name)
        {
            this.name = name;
        }
    }
}
