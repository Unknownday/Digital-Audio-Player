using Microsoft.Win32;
using Musical_Player.Global;
using Musical_Player.Models;
using Musical_Player.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace Musical_Player.Files_management
{
    /// <summary>
    /// Работа с файлами
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Фильтр названий файлов. Очищает название файла от всего не нужного
        /// </summary>
        /// <param name="name">Название файла</param>
        /// <returns>Очищенное название</returns>
        public static string NameFilter(string name)
        {
            // Список форматов файлов, которые будут удалены из названия
            List<string> DeletingFormats = new List<string>()
            {
                ".mp3",
                ".wav",
                ".ogg",
                ".playlist",
            };

            // Итерация по каждому формату и удаление из названия
            foreach (string to_replace in DeletingFormats)
            {
                name = name.Replace(to_replace, "");
            }
            return name; // Возвращаем отфильтрованное название
        }

        /// <summary>
        /// Создает новый плейлист с указанным пользователем названием
        /// </summary>
        public static void CreatePlaylist()
        {
            // Открываем диалог для получения от пользователя названия плейлиста
            var nameDialog = new NameDialog();
            bool? dialogResult = nameDialog.ShowDialog();

            // Если пользователь подтверждает диалог, приступаем к созданию плейлиста
            if (dialogResult == true)
            {
                // Название плейлиста по умолчанию
                string playlistName = "Новый плейлист";

                // Используем введенное пользователем название, если оно доступно, иначе используем название по умолчанию
                playlistName = nameDialog.InputTextbox.Text == null ? "Новый плейлист" : nameDialog.InputTextbox.Text + ".playlist";

                // Проверяем, существует ли уже плейлист с таким же названием
                if (File.Exists(Path.Combine(Config.DefaultPath, playlistName)))
                {
                    MessageBox.Show("Плейлист с таким названием уже существует! Выберите другое!", "Музыкальный плеер");
                    return;
                }

                // Создаем пустой файл плейлиста
                File.Create(Path.Combine(Config.DefaultPath, playlistName)).Close();
            }
        }

        /// <summary>
        /// Добавляет новую песню в плейлист
        /// </summary>
        /// <param name="playlistIndex">Индекс плейлиста</param>
        public static void AddSongToPlaylist(int playlistIndex)
        {
            // Открываем диалог для выбора песен
            OpenFileDialog opnFileDlg = new OpenFileDialog();
            opnFileDlg.Filter = "(mp3),(wav),(ogg)|*.mp3;*.wav;*.ogg;";
            opnFileDlg.Multiselect = true;

            // Если файлы выбраны, добавляем их пути в файл плейлиста
            if (opnFileDlg.ShowDialog() == true)
            {
                foreach (string fileName in opnFileDlg.FileNames)
                {
                    File.AppendAllText(Config.PlaylistPaths[playlistIndex], fileName + "\n");
                }
            }
        }

        /// <summary>
        /// Удаляет песню из плейлиста
        /// </summary>
        /// <param name="playlistIndex">Индекс плейлиста</param>
        /// <param name="songIndex">Местоположение песни в списке</param>
        public static void DeleteSong(int playlistIndex, int songIndex)
        {
            // Читаем текущий плейлист
            var curentPlaylist = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

            // Создаем новый плейлист, исключая указанную песню
            var newPlaylist = curentPlaylist.Where((x, index) => index != songIndex).ToList();

            // Записываем новый плейлист обратно в файл
            File.WriteAllLines(Config.PlaylistPaths[playlistIndex], newPlaylist);
        }

        /// <summary>
        /// Удаляет плейлист
        /// </summary>
        /// <param name="playlistIndex">Индекс плейлиста</param>
        public static void DeletePlaylist(int playlistIndex)
        {
            // Удаляем файл плейлиста
            File.Delete(Config.PlaylistPaths[playlistIndex]);
        }

        /// <summary>
        /// Перемещает песню вверх или вниз по списку 
        /// </summary>
        /// <param name="songIndex">Местоположение песни в списке</param>
        /// <param name="playlistIndex">Индекс плейлиста</param>
        /// <param name="direction">Направление перемещения (вниз = 0, вверх = 1)</param>
        public static void MoveSongInQueue(int songIndex, int playlistIndex, Enums.MoveDirections direction)
        {
            try
            {
                // Читаем текущий плейлист
                var currentPlaylist = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

                // Рассчитываем новый индекс в зависимости от указанного направления
                int newIndex = direction == Enums.MoveDirections.Down ? songIndex + 1 : songIndex - 1;

                // Проверяем, находится ли новый индекс в пределах допустимых значений
                if (newIndex >= 0 && newIndex < currentPlaylist.Length)
                {
                    // Меняем местами текущую песню и ту, которая находится на новом индексе
                    Swap(currentPlaylist, songIndex, newIndex);

                    // Записываем модифицированный плейлист обратно в файл
                    File.WriteAllLines(Config.PlaylistPaths[playlistIndex], currentPlaylist);
                }
                else
                {
                    // Выводим сообщение об ошибке для недопустимой операции перемещения
                    Console.WriteLine("Недопустимая операция перемещения песни.");
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем любые исключения и выводим сообщение об ошибке
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает названия плейлистов 
        /// </summary>
        /// <returns>Список плейлистов</returns>
        public static List<string> GetPlaylists()
        {
            // Очищаем список путей песен в конфигурации
            Config.SongPaths.Clear();

            // Получаем все файлы с расширением ".playlist" в стандартной директории
            IEnumerable<string> playlists = Directory.EnumerateFiles(Config.DefaultPath, "*.playlist");

            // Очищаем и добавлем пути плейлистов в список 
            Config.PlaylistPaths.Clear();
            Config.PlaylistPaths.AddRange(playlists);

            // Извлекаем безопасные имена файлов и добавляем их в результат и пути плейлистов в конфигурацию
            return playlists.Select(playlist => NameFilter(Path.GetFileName(playlist))).ToList();
        }

        /// <summary>
        /// Меняет местами элементы в массиве
        /// </summary>
        /// <param name="array">Массив в котором надо поменять элементы местами</param>
        /// <param name="index1">Первый индекс для замены</param>
        /// <param name="index2">Второй индекс для замены</param>
        private static void Swap<T>(T[] array, int index1, int index2)
        {
            // Временная переменная для обмена значениями местами
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        /// <summary>
        /// Проверяет существует ли песня по данному пути
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True если песня существует. False если песня не существует</returns>
        public static bool ValidatePath(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Проверка существует ли нужная директория
        /// </summary>
        public static void ValidateDefaultPath(string path)
        {
            // Создаем директории, если они не существуют
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(Path.Combine(path, "Icons")))
            {
                Directory.CreateDirectory(Path.Combine(path, "Icons"));
            }

            if (!File.Exists(Path.Combine(path, "Icons", "BlackIconSet.png")) 
                || !File.Exists(Path.Combine(path, "Icons", "WhiteIconSet.png")) 
                || !File.Exists(Path.Combine(path, "Icons", "WhiteBackground.png")) 
                || !File.Exists(Path.Combine(path, "Icons", "BlackBackground.png")))
            {
                DownloadIconSet();
            }
        }

        /// <summary>
        /// Добавляет песню в плейлист новую песню из файла. Используется для Drag&Drop.
        /// </summary>
        /// <param name="path">Путь к песне</param>
        /// <param name="playlistName">Название плейлиста в который надо добавить песню</param>
        public static void AddSong(string path, string playlistName)
        {
            // Проверяем путь и добавляем песню в плейлист
            if (!ValidatePath(path))
            {
                return;
            }

            File.AppendAllText(Path.Combine(Config.DefaultPath, playlistName + ".playlist"), path + "\n");
        }

        /// <summary>
        /// Проверка поддерживается-ли формат песни при Drag&Drop
        /// </summary>
        /// <param name="filePath">Путь к песне для проверки</param>
        /// <returns>True если формат поддерживается. False если формат не поддерживается</returns>
        public static bool IsSupportedAudioFormat(string filePath)
        {
            // Поддерживаемые форматы аудио
            string[] supportedFormats = { ".mp3", ".wav", ".ogg" };
            string fileExtension = Path.GetExtension(filePath);

            // Проверяем, есть ли расширение файла в списке поддерживаемых форматов
            return supportedFormats.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Устанавливает новую стандартную директорию
        /// </summary>
        public static void SetNewDirectoryPath()
        {
            // Открываем диалог выбора директории
            var dialog = new FolderBrowserDialog();
            DialogResult dialogResult = dialog.ShowDialog();

            // Если выбрана новая директория, обновляем конфигурацию
            if (DialogResult.OK == dialogResult)
            {
                var newDefaultPath = dialog.SelectedPath;
                ValidateDefaultPath(newDefaultPath);
                var files = Directory.GetFiles(Config.DefaultPath, "*.playlist");

                // Копируем существующие файлы плейлистов в новую директорию и обновляем конфигурацию
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(newDefaultPath, Path.GetFileName(file)));
                    File.Delete(file);
                }

                foreach (var image in Directory.GetFiles(Path.Combine(Config.DefaultPath, "Icons"))) 
                {
                    var imageFile = File.ReadAllLines(image);
                    File.WriteAllLines(Path.Combine(newDefaultPath, "Icons", Path.GetFileName(image)), imageFile);
                }

                Config.DefaultPath = newDefaultPath;
            }
        }

        /// <summary>
        /// Устанавливает новое изображение фона
        /// </summary>
        public static void SetNewBackgroundImage()
        {
            // Открываем диалог выбора файла для нового фонового изображения
            OpenFileDialog opnFileDlg = new OpenFileDialog();
            opnFileDlg.Filter = "(png),(jpg)|*.png;*.jpg;";

            // Если выбрано новое изображение, обновляем конфигурацию
            if (opnFileDlg.ShowDialog() == true && opnFileDlg.FileNames.Length != 0)
            {
                Config.BackgroundImagePath = opnFileDlg.FileNames[0];
            }
        }

        public static void DownloadIconSet()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    // Загружаем файл по URL в указанную директорию
                    client.DownloadFile("https://github.com/Unknownday/C-Audio-Player/blob/main/BlackIconSet.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "BlackIconSet.png"));
                    client.DownloadFile("https://github.com/Unknownday/C-Audio-Player/blob/main/WhiteIconSet.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "WhiteIconSet.png"));
                    client.DownloadFile("https://github.com/Unknownday/C-Audio-Player/blob/main/BlackBackground.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "BlackBackground.png"));
                    client.DownloadFile("https://github.com/Unknownday/C-Audio-Player/blob/main/WhiteBackground.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "WhiteBackground.png"));

                    Console.WriteLine("Файл успешно скачан.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
                }
            }
        }
    }
}
