using System;
using System.Collections.Generic;

namespace ProjectController
{
    /// <summary>
    /// Класс проекта
    /// </summary>
    [Serializable]
    public class Project
    {
        /// <summary>
        /// Основные данные о категориях и файлах
        /// </summary>
        private Dictionary<Category, List<FileData>> data;

        /// <summary>
        /// Частотные словари
        /// </summary>
        private Dictionary<string, FrequencyDictionary> dicts;

        /// <summary>
        /// Изменение
        /// </summary>
        private bool changed;

        /// <summary>
        /// Путь
        /// </summary>
        private string path;

        /// <summary>
        /// Активная категория
        /// </summary>
        private string activeCategory;

        /// <summary>
        /// Категория по умолчанию
        /// </summary>
        private const string defaultCategory = "Общее";

        /// <summary>
        /// Подсчёт букв
        /// </summary>
        private bool countLetters;

        /// <summary>
        /// Подсчёт биграмм
        /// </summary>
        private bool countBigrams;

        /// <summary>
        /// Подсчёт триграмм
        /// </summary>
        private bool countTrigrams;

        /// <summary>
        /// Данные
        /// </summary>
        public Dictionary<Category, List<FileData>> Data
        {
            get { return data; }
        }

        /// <summary>
        /// Получение частотных словарей
        /// </summary>
        public Dictionary<string, FrequencyDictionary> Dicts
        {
            get { return dicts; }
        }

        /// <summary>
        /// Категория по умолчанию
        /// </summary>
        public string DefaultCategory
        {
            get { return defaultCategory; }
        }

        /// <summary>
        /// Изменение данных в проекте
        /// </summary>
        public bool Changed
        {
            get { return changed; }
            set { changed = value; }
        }

        /// <summary>
        /// Активная категория
        /// </summary>
        public string ActiveCategory
        {
            get { return activeCategory; }
            set { activeCategory = value; }
        }

        /// <summary>
        /// Путь файла проекта
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Выполнять подсчёт букв
        /// </summary>
        public bool CountLetters
        {
            get { return countLetters; }
            set { countLetters = value; }
        }

        /// <summary>
        /// Выполнять подсчёт биграмм?
        /// </summary>
        public bool CountBigrams
        {
            get { return countBigrams; }
            set { countBigrams = value; }
        }

        /// <summary>
        /// Выполнять подсчёт триграмм?
        /// </summary>
        public bool CountTrigrams
        {
            get { return countTrigrams; }
            set { countTrigrams = value; }
        }

        /// <summary>
        /// Конструктор проекта
        /// </summary>
        public Project()
        {
            data = new Dictionary<Category, List<FileData>>();
            changed = false;
            countLetters = true;
            countBigrams = true;
            countTrigrams = true;
            activeCategory = defaultCategory;
        }

        /// <summary>
        /// Загрузка txt-файла
        /// </summary>
        /// <param name="path">Путь</param>
        /// <param name="category">Категория</param>
        public void LoadTxtFile(string path, string category = defaultCategory)
        {
            changed = true;
            Category cat = GetCategory(category);
            FileData file = new FileData(path);

            if (file.SizeNum == 0)
                throw new System.ArgumentException("Файл не может быть пустым!");

            string startname = file.Name;
            int i = 0;
            while (!IsUniqueFileName(file.Name))
            {
                file.Name = "(" + (++i) + ")" + startname;
            }

            if (!IsUniqueFile(file.Md5))
                throw new System.ArgumentException("Файл с таким содержимым уже есть в базе!");

            AddFile(file, cat);
        }

        /// <summary>
        /// Добавление текста вручную
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="text">Текст</param>
        /// <param name="category">Категория</param>
        public void LoadTextInsert(string name, string text, string category)
        {
            changed = true;
            Category cat = GetCategory(category);
            FileData data = new TextData(name, text);

            if (!IsUniqueFileName(data.Name))
                throw new System.ArgumentException("Текст с таким заголовком уже существует!");

            if (!IsUniqueFile(data.Md5))
                throw new System.ArgumentException("Текст с таким содержимым уже существует!");

            AddFile(data, cat);
        }
        
        /// <summary>
        /// Загрузка веб-страницы
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <param name="category">Категория</param>
        /// <param name="start">Начало</param>
        /// <param name="end">Конец</param>
        public void LoadWebPage(string url, string category, string start = null, string end = null)
        {
            changed = true;
            Category cat = GetCategory(category);
            FileData data;

            if (start == null || end == null)
                data = new WebData(url);
            else
                data = new WebData(url, start, end);

            string urlToSimilar = data.Name.Replace("http://", "");
            urlToSimilar = urlToSimilar.Replace("/", "");
            urlToSimilar = urlToSimilar.Replace("?", "");
            urlToSimilar = urlToSimilar.Replace("www.", "");

            if (!IsUniqueWebPage(urlToSimilar))
                throw new System.ArgumentException("Веб-страница с таким адресом уже есть в базе!");

            if (!IsUniqueFile(data.Md5))
                throw new System.ArgumentException("Веб-страница с таким содержимым уже существует!");

            AddFile(data, cat);
        }

        /// <summary>
        /// Добавление частотного словаря
        /// </summary>
        /// <param name="name">Имя словаря</param>
        /// <param name="fname">Имя файла или категории</param>
        /// <param name="type">Тип</param>
        public void AddDictionary(string name, string fname, string type)
        {
            if (dicts == null)
                dicts = new Dictionary<string, FrequencyDictionary>();

            if (dicts.ContainsKey(name))
                throw new System.ArgumentException("Словарь с таким названием уже существует!");

            if (type == "document")
            {
                FileData file = GetFile(fname);
                FrequencyDictionary dict = new FrequencyDictionary("document");

                dict.AddLetters(file.Letters);
                dict.AddBigrams(file.Bigrams);
                dict.AddTrigrams(file.Trigrams);

                dicts.Add(name, dict);
            }
            else
            {
                List<FileData> files = GetFiles(fname);

                int count = files.Count;
                Dictionary<string, int>[] dletters = new Dictionary<string, int>[count];
                Dictionary<string, int>[] dbigrams = new Dictionary<string, int>[count];
                Dictionary<string, int>[] dtrigrams = new Dictionary<string, int>[count];
                
                int i = 0;
                foreach (FileData file in files)
                {
                    dletters[i] = file.Letters;
                    dbigrams[i] = file.Bigrams;
                    dtrigrams[i] = file.Trigrams;
                    i++;
                }

                FrequencyDictionary dict = new FrequencyDictionary("category");

                dict.AddLetters(dletters);
                dict.AddBigrams(dbigrams);
                dict.AddTrigrams(dtrigrams);

                dicts.Add(name, dict);
            }
        }

        /// <summary>
        /// Удаление частотного словаря
        /// </summary>
        /// <param name="name">Имя словаря</param>
        public void DeleteDictionary(string name)
        {
            if (!dicts.ContainsKey(name))
                throw new System.ArgumentException("Неверное имя");

            dicts.Remove(name);
        }

        /// <summary>
        /// Уникальность файла
        /// </summary>
        /// <param name="md5">MD5</param>
        /// <returns>Уникальность</returns>
        public bool IsUniqueFile(string md5)
        {
            foreach (var elements in data)
            {
                foreach (var file in elements.Value)
                {
                    if (file.Md5 == md5)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка уникальности категории
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <returns>Уникальность</returns>
        public bool IsUniqueCategoryName(string name)
        {
            try
            {
                GetCategory(name);
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Проверка уникальности веб-страницы
        /// </summary>
        /// <param name="name">Адрес</param>
        /// <returns>Уникальность</returns>
        public bool IsUniqueWebPage(string name)
        {
            foreach (var elements in data)
            {
                foreach (var page in elements.Value)
                {
                    string urlToCompare = page.Name.Replace("http://", "");
                    urlToCompare = urlToCompare.Replace("/", "");
                    urlToCompare = urlToCompare.Replace("?", "");
                    urlToCompare = urlToCompare.Replace("www.", "");
                    if (urlToCompare == name)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка уникальности имени файла
        /// </summary>
        /// <param name="name">Имя файла</param>
        /// <returns>Уникальность</returns>
        public bool IsUniqueFileName(string name)
        {
            try
            {
                GetFile(name);
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Получить список всех файлов
        /// </summary>
        /// <returns></returns>
        public List<FileData> GetAllFiles()
        {
            List<FileData> files = new List<FileData>();
            foreach (var elements in data)
            {
                foreach (var file in elements.Value)
                {
                    files.Add(file);
                }
            }
            return files;
        }

        /// <summary>
        /// Получить список файлов в категории
        /// </summary>
        /// <param name="category">Имя категории</param>
        /// <returns>Список файлов</returns>
        public List<FileData> GetFiles(string category)
        {
            List<FileData> files = new List<FileData>();
            foreach (var elements in data)
            {
                if (elements.Key.Name == category)
                {
                    foreach (var file in elements.Value)
                    {
                        files.Add(file);
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Получить список всех категорий
        /// </summary>
        /// <returns>Список категорий</returns>
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            foreach (var elements in data)
            {
                categories.Add(elements.Key);
            }
            return categories;
        }

        /// <summary>
        /// Получить объект категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <returns>Категория</returns>
        public Category GetCategory(string categoryName)
        {
            Category category = null;
            foreach (var elements in data)
            {
                if (elements.Key.Name == categoryName)
                {
                    category = elements.Key;
                    break;
                }
            }

            if (category == null)
            {
                throw new System.ArgumentException("Категория не найдена!");
            }

            return category;
        }

        /// <summary>
        /// Получить объект категории
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>Категория</returns>
        public Category GetCategoryByFileName(string filename)
        {
            Category cat = null;
            foreach (var elements in data)
            {
                bool b = false;

                foreach (var file in elements.Value)
                {
                    if (file.Name == filename)
                    {
                        cat = elements.Key;
                        b = true;
                        break;
                    }
                }

                if (b == true)
                {
                    break;
                }
            }

            if (cat == null)
            {
                throw new System.ArgumentException("Категории не существует!");
            }

            return cat;
        }

        /// <summary>
        /// Получить объект файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>Файл</returns>
        public FileData GetFile(string filename)
        {
            FileData filedata = null;
            foreach (var elements in data)
            {
                bool b = false;

                foreach (var file in elements.Value)
                {
                    if (file.Name == filename)
                    {
                        filedata = file;
                        b = true;
                        break;
                    }
                }
                
                if (b == true)
                {
                    break;
                }
            }

            if (filedata == null)
            {
                throw new System.ArgumentException("Файла не существует!");
            }

            return filedata;
        }

        /// <summary>
        /// Изменение названия категории
        /// </summary>
        /// <param name="oldname">Старое имя</param>
        /// <param name="newname">Новое имя</param>
        public void ChangeCategoryName(string oldname, string newname)
        {
            changed = true;
            Category cat = GetCategory(oldname);
            if (IsUniqueCategoryName(newname))
            {
                cat.Name = newname;
            }
            else
            {
                throw new System.ArgumentException("Категория с таким названием уже существует!");
            }
        }
        
        /// <summary>
        /// Добавление категории
        /// </summary>
        /// <param name="cat">Категория</param>
        public void AddCategory(Category cat)
        {
            changed = true;
            if (data.ContainsKey(cat) || !IsUniqueCategoryName(cat.Name))
            {
                throw new System.ArgumentException("Категория уже существует!");
            }

            data.Add(cat, new List<FileData>());
        }

        /// <summary>
        /// Удаление категорие
        /// </summary>
        /// <param name="category">Имя категории</param>
        public void RemoveCategory(string category)
        {
            changed = true;
            if (category == defaultCategory)
            {
                throw new System.ArgumentException("Нельзя удалить встроенную категорию!");
            }

            foreach (var elements in data)
            {
                if (elements.Key.Name == category)
                {
                    data.Remove(elements.Key);
                    return;
                }
            }
        }

        /// <summary>
        /// Добавление файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="cat">Категория</param>
        public void AddFile(FileData file, Category cat)
        {
            changed = true;
            if (data.ContainsKey(cat))
            {
                data[cat].Add(file);
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        public void RemoveFile(string filename)
        {
            changed = true;
            foreach (var elements in data)
            {
                foreach (FileData f in elements.Value)
                {
                    if (f.Name == filename)
                    {
                        elements.Value.Remove(f);
                        return;
                    }
                }
            }
        }
    }
}
