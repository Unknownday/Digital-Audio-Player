using Musical_Player.Files_management;
using Musical_Player.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Musical_Player.Global
{
    /// <summary>
    /// Главная логика плеера
    /// </summary>
    public static class PlayerLogic
    {

        /// <summary>
        /// Возвращает отформатированное время для надписи со временем
        /// </summary>
        /// <param name="currMinutes">Текущая минута играющей песни</param>
        /// <param name="currSeconds">Текущая секунда играющей песни</param>
        /// <param name="totMinutes">Целых минут в песне</param>
        /// <param name="totSeconds">Количество секунд в песне</param>
        /// <returns>Отформатированную строку со временем песни, готовую для использования</returns>
        public static string GetDurationLabel(int currMinutes, int currSeconds, int totMinutes, int totSeconds)
        {
            // Форматируем текущую и общую продолжительность песни в виде "мм:сс"
            string currentDuration = $"{currMinutes:D2}:{currSeconds:D2}";
            string totalDuration = $"{totMinutes:D2}:{totSeconds:D2}";

            // Возвращаем строку с отформатированным временем в формате "текущая | общая"
            return $"{currentDuration} | {totalDuration}";
        }

        /// <summary>
        /// Получает названия всех песен в плейлисте
        /// </summary>
        /// <param name="playlistIndex">Индекс плейлиста</param>
        /// <returns>Список всех песен</returns>
        public static List<string> GetAllSongs(int playlistIndex)
        {

            // Читаем все строки из файла плейлиста
            var songPaths = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

            // Проверяем существование каждой песни в списке, удаляем отсутствующие
            for (int i = 0; i < songPaths.Length; i++)
            {
                if (!FileManager.ValidatePath(songPaths[i]))
                {
                    FileManager.DeleteSong(playlistIndex, i);
                }
            }

            // Очищаем списки в Config и добавляем существующие пути и названия песен
            Config.SongNames.Clear();
            Config.SongPaths.Clear();
            Config.SongPaths.AddRange(songPaths);
            Config.SongNames.AddRange(songPaths.Select(songPath => Path.GetFileName(songPath)));

            // Возвращаем отформатированный список названий песен
            return Config.SongPaths.Select((path, index) => FileManager.NameFilter($"{index + 1}. {Path.GetFileName(path)}")).ToList();
        }

        /// <summary>
        /// Перемешивает песни в плейлисте
        /// </summary>
        /// <param name="playlistIndex">Название плейлиста</param>
        public static void ShufflePlaylist(int playlistIndex)
        {
            // Читаем все строки из файла плейлиста
            var currentPlaylist = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

            // Вызываем метод Shuffle для случайного перемешивания элементов
            Shuffle(currentPlaylist);

            // Записываем перемешанный плейлист обратно в файл
            File.WriteAllLines(Config.PlaylistPaths[playlistIndex], currentPlaylist);
        }

        /// <summary>
        /// Случайным образом перемешивает индексы в массиве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Массив для перемешивания</param>
        public static void Shuffle<T>(T[] array)
        {
            // Инициализируем генератор случайных чисел
            Random random = new Random();

            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                // Генерируем случайный индекс
                int j = random.Next(0, i + 1);

                // Обмениваем значениями элементы array[i] и array[j]
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

    }
}
