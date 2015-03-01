using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjectController
{
    /// <summary>
    /// Управление данными проекта
    /// </summary>
    public static class Controller
    {
        /// <summary>
        /// Текущий открытый проект
        /// </summary>
        private static Project project;

        /// <summary>
        /// Получение текущего проекта
        /// </summary>
        public static Project Project
        {
            get { return project; }
        }

        /// <summary>
        /// Создать новый проект
        /// </summary>
        public static void New()
        {
            project = new Project();
            Category cat = new Category(project.DefaultCategory);
            project.AddCategory(cat);
            project.Changed = false;
        }

        /// <summary>
        /// Открыть проект
        /// </summary>
        /// <param name="path">Путь</param>
        public static void Open(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
  
            if (!File.Exists(path))
            {
                throw new Exception("Файла не существует!");
            }
            
            using (var fStream = File.OpenRead(path))
            {
                project = (Project)formatter.Deserialize(fStream);
                fStream.Close();
            }

            project.Changed = false;
        }

        /// <summary>
        /// Сохранить проект
        /// </summary>
        /// <param name="path">Путь</param>
        public static void Save(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, project);
                fStream.Close();
            }
        }
    }
}
