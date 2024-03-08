using Musical_Player.Config_management;
using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using Musical_Player.Views;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Musical_Player.Global.Enums;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Windows.Forms.Timer;
using TreeView = System.Windows.Controls.TreeView;

namespace MusicalPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Переменная для отслеживания удаления плейлиста
        private bool isDeleted = false;

        // Переменные для отслеживания выбора плейлиста и песни
        private bool isPlaylistChoosed = false;
        private bool isSongChoosed = false;

        // Таймер для обновления интерфейса
        private Timer durationTimer;

        // Конструктор класса MainWindow
        public MainWindow()
        {
            // Инициализация компонентов программы
            InitializeComponent();

            UpdateUi();

            //Установка фона окна, если путь к изображению указан
            if (!string.IsNullOrEmpty(Config.BackgroundImagePath) && Config.BackgroundImagePath != "none")
            {
                SetWindowBackground(Config.BackgroundImagePath);
            }
            if (Config.BackgroundImagePath == "none")
            {
                SetWindowBackground(Path.Combine(Config.DefaultPath, "Icons", $"{Config.Theme}Background.png"));
            }

            // Разрешение перетаскивания файлов
            AllowDrop = true;

            // Привязка функции к событию "перетаскивание файла"
            Drop += MainWindow_Drop;

            // Проверка существования стандартного пути к плейлистам
            FileManager.ValidateDefaultPath(Config.DefaultPath);

            // Обновление списка плейлистов
            UpdatePlaylistList();

            // Создание таймера для обновления интерфейса
            durationTimer = new Timer();

            // Привязывание события при обновлении таймера
            durationTimer.Tick += new EventHandler(UpdateDuration);

            // Установка интервала таймера в миллисекундах
            durationTimer.Interval = 1;

            // Запуск таймера
            durationTimer.Start();

            // Установка значения для надписи с текущей громкостью
            VolumeLabel.Content = $"{Config.LastVolume}%";

            // Установка значения для ползунка громкости
            VolumeSlider.Value = Config.LastVolume;

            // Восстановление предыдущего плейлиста
            PlaylistsList.SelectedIndex = Config.LastPlaylist;

            Player.CurrentSongVolume = (float)(Config.LastVolume/100);

            // Восстановление предыдущей песни
            SongList.SelectedIndex = Config.LastSong;
        }

        // Обработчик события "Drop" при перетаскивании файлов в окно
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            // Скрываем подсказку
            TipLabel.Visibility = Visibility.Hidden;

            // Проверяем, выбран ли плейлист
            if (!isPlaylistChoosed) { MessageBox.Show("Выберите плейлист сначала!", "Musical player"); return; }

            // Получаем список файлов, перетащенных в окно
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Проверяем, является ли каждый файл поддерживаемым аудиоформатом
            if (files.Any(file => !FileManager.IsSupportedAudioFormat(file))) { return; }

            // Добавляем каждый поддерживаемый аудиофайл в выбранный плейлист
            files.Where(FileManager.IsSupportedAudioFormat).ToList().ForEach(file => FileManager.AddSong(file, PlaylistsList.SelectedItem.ToString()));

            // Обновляем список песен в интерфейсе
            UpdateSongList();
        }

        // Обработчик щелчка мыши на кнопке информации о программе
        private void ProgramInfoButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Создаем и отображаем окно с информацией
            var infoWindow = new InfoWindow();
            infoWindow.Show();
        }

        // Обработчик щелчка мыши на кнопке отключения звука
        private void MuteButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Сохраняем текущую громкость
                var tempSoundValue = Player.CurrentSongVolume;

                // Отключаем звук
                Player.CurrentSongVolume = Player.AudioValueBackup;

                // Сохраняем текущую громкость для восстановления
                Player.AudioValueBackup = tempSoundValue;

                // Обновляем состояние плеера
                Player.IsMuted = !Player.IsMuted;

                // Устанавливаем значение для слайдера звука
                VolumeSlider.Value = Player.CurrentSongVolume * 100;

                // Устанавливаем значение для надписи громкости
                VolumeLabel.Content = $"{Convert.ToInt32(Math.Abs(Player.CurrentSongVolume * 100))}%";

                // Устанавливаем новое значение громкости для плеера
                Player.AudioPlayer.Volume = Player.CurrentSongVolume;
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке добавления плейлиста
        private void AddPlaylistButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Создаем новый плейлист
                FileManager.CreatePlaylist();

                // Обновляем список плейлистов
                UpdatePlaylistList();
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке удаления плейлиста
        private void DeletePlaylistButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Удаляем выбранный плейлист
                FileManager.DeletePlaylist(PlaylistsList.SelectedIndex);

                isSongChoosed = false;
                isPlaylistChoosed = false;
                isDeleted = true;
                PlaylistsList.SelectedIndex = -1;

                // Обновляем список плейлистов
                UpdatePlaylistList();
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке перемещения песни вверх в очереди
        private void MoveUpButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                var currentIndex = SongList.SelectedIndex;
                if (currentIndex - 1 < 0) { return; }
                // Перемещаем песню в очереди вверх
                FileManager.MoveSongInQueue(SongList.SelectedIndex, PlaylistsList.SelectedIndex, Enums.MoveDirections.Up);
               
                // Обновляем список песен
                UpdateSongList();

                SongList.SelectedIndex = currentIndex - 1;
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке перемещения песни вниз в очереди
        private void MoveDownButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                var currentIndex = SongList.SelectedIndex;
                if (currentIndex + 1 >= SongList.Items.Count) { return; }
                // Перемещаем песню в очереди вниз
                FileManager.MoveSongInQueue(currentIndex, PlaylistsList.SelectedIndex, Enums.MoveDirections.Down);

                // Обновляем список песен
                UpdateSongList();

                SongList.SelectedIndex = currentIndex + 1;
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке добавления песни в плейлист
        private void AddSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист
            if (!isPlaylistChoosed) { return; }

            try
            {
                // Добавляем песню в плейлист
                FileManager.AddSongToPlaylist(PlaylistsList.SelectedIndex);

                // Обновляем список песен
                UpdateSongList();
            }
            catch (Exception ex)
            {
                // Выводим сообщение об ошибке
                MessageBox.Show(ex.Message);
            }
        }

        // Обработчик щелчка мыши на кнопке удаления песни из плейлиста
        private void DeleteSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                var currentIndex = SongList.SelectedIndex;
                // Удаляем выбранную песню из плейлиста
                FileManager.DeleteSong(PlaylistsList.SelectedIndex, currentIndex);

                // Обновляем список песен
                UpdateSongList();

                SongList.SelectedIndex = currentIndex;
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке проигрывания следующей песни
        private void PlayNextSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Получаем индекс следующей песни
                var nextIndex = SongList.SelectedIndex += 1;

                // Проверяем, не выходит ли индекс за пределы списка песен
                if (nextIndex >= SongList.Items.Count) { SongList.SelectedIndex = 0; return; }

                // Устанавливаем индекс следующей песни
                SongList.SelectedIndex = nextIndex;

                // Инициализируем и воспроизводим песню
                InitSong(SongList.SelectedIndex);

                // Обновление максимальной длины ползунка
                DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                PlaySong();
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке приостановки проигрывания
        private void PauseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Обновляем текущее состояние плеера
                Player.CurrentPlayerState = PlayerStates.OnPause;

                // Приостанавливаем проигрывание
                Player.AudioPlayer.Pause();
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке проигрывания
        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // В зависимости от текущего состояния плеера выполняем соответствующие действия
                switch (Player.CurrentPlayerState)
                {
                    case PlayerStates.Idle:
                        // Инициализируем песню
                        InitSong(SongList.SelectedIndex);

                        // Устанавливаем максимальное значение слайдера длительности равным длительности песни
                        DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                        // Воспроизводим песню
                        PlaySong();

                        // Обновляем надпись с названием песни
                        SongNameLabel.Content = Player.CurrentSongName;

                        // Запускаем таймер обновления интерфейса
                        durationTimer.Start();
                        break;
                    case PlayerStates.InProgress:
                        // Инициализируем песню
                        InitSong(SongList.SelectedIndex);

                        // Устанавливаем максимальное значение слайдера длительности равным длительности песни
                        DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                        // Воспроизводим песню
                        PlaySong();

                        // Обновляем надпись с названием песни
                        SongNameLabel.Content = Player.CurrentSongName;

                        // Запускаем таймер обновления интерфейса
                        durationTimer.Start();
                        break;
                    case PlayerStates.OnPause:
                        // Обновляем состояние плеера
                        Player.CurrentPlayerState = PlayerStates.InProgress;

                        // Возобновляем проигрывание песни
                        Player.AudioPlayer.Play();

                        // Запускаем таймер обновления интерфейса
                        durationTimer.Start();
                        break;
                }
            }
            catch { }
        }

        // Обработчик щелчка мыши на кнопке проигрывания предыдущей песни
        private void PlayPreviosSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, выбран ли плейлист и песня
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Получаем индекс следующей песни
                var nextIndex = SongList.SelectedIndex -= 1;

                // Проверяем, не выходит ли индекс за пределы списка песен
                if (nextIndex < 0) { SongList.SelectedIndex = SongList.Items.Count - 1; return; }

                // Устанавливаем индекс следующей песни
                SongList.SelectedIndex = nextIndex;

                // Инициализируем и воспроизводим песню
                InitSong(SongList.SelectedIndex);

                // Обновление максимальной длины ползунка
                DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                PlaySong();
            }
            catch { }
        }

        // Обработчик изменения выбранного плейлиста
        private void PlaylistsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Проверяем, не было ли удаления плейлиста
                if (!isDeleted)
                {
                    // Проверяем, выбран ли плейлист
                    if (PlaylistsList.SelectedIndex == -1) return;

                    // Устанавливаем флаг выбора плейлиста
                    isPlaylistChoosed = true;

                    // Обновляем список песен в интерфейсе
                    UpdateSongList();
                }
                if (isDeleted)
                {
                    SongList.ItemsSource = null; 
                }
            }
            catch { }
        }

        // Обработчик обновления длительности песни
        private void UpdateDuration(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, установлена ли текущая песня
                if (Player.CurrentSong != null)
                {
                    // Обновляем надпись с длительностью
                    DurationLabel.Content = PlayerLogic.GetDurationLabel(Player.CurrentSong.CurrentTime.Minutes,
                            Player.CurrentSong.CurrentTime.Seconds,
                            Player.CurrentSong.TotalTime.Minutes,
                            Player.CurrentSong.TotalTime.Seconds
                        );

                    // Устанавливаем значение слайдера длительности
                    DurationSlider.Value = Player.CurrentSong.CurrentTime.Seconds + 60 * Player.CurrentSong.CurrentTime.Minutes;

                    // Отображаем текущий статус в заголовке окна
                    string playbackState = Player.CurrentPlayerState == PlayerStates.InProgress ? "▶" : "||";
                    Title = Player.IsAutoSwitching ? $"{playbackState} {Player.CurrentSongName} |⇌|" : $"{playbackState} {Player.CurrentSongName}";

                    // Обновляем надпись с названием песни
                    SongNameLabel.Content = Player.CurrentSongName;

                    // Обновляем надпись с громкостью
                    VolumeLabel.Content = $"{(int)Math.Round(Math.Abs(Player.CurrentSongVolume * 100))}%";
                }
            }
            catch { }
        }

        // Метод для воспроизведения следующей песни с учетом разрешения на автопереключение
        private void PlayNextTrack(bool Allow)
        {
            // Проверка, включено ли автовоспроизведение следующей песни
            if (!Allow) { return; }

            // Остановка таймера обновления интерфейса
            durationTimer.Stop();

            // Обнуление ползунка продолжительности песни
            DurationSlider.Value = 0;

            // Инициализация переменной эмулирующей следующий индекс
            int nextIndex = SongList.SelectedIndex + 1;

            // Обработка исключений
            if (nextIndex > Config.SongPaths.Count || nextIndex > SongList.Items.Count || nextIndex == -1) { return; }

            // Инициализация новой песни
            InitSong(nextIndex);

            // Обновление максимальной длины ползунка
            DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

            // Запуск песни
            PlaySong();

            // Обновление надписи с названием песни
            SongNameLabel.Content = Player.CurrentSongName;

            // Увеличение индекса в списке песен
            SongList.SelectedIndex += 1;

            // Запуск таймера обновления интерфейса
            durationTimer.Start();
        }

        // Обработчик события перемещения ползунка продолжительности
        private void Slider_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Обновление текущего времени плеера
            Player.CurrentSong.CurrentTime = TimeSpan.FromSeconds(DurationSlider.Value);
        }

        // Обработчик изменения значения ползунка продолжительности
        private void DurationBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Проверка на окончание песни, чтобы не запустить новую, пока играет старая
            if (DurationSlider.Value == DurationSlider.Maximum)
            {
                // Автоматический запуск следующей песни
                PlayNextTrack(Player.IsAutoSwitching);
            }
        }

        // Метод для инициализации песни по указанному индексу
        public void InitSong(int songIndex)
        {
            // Проверка, существует ли песня по указанному пути, чтобы избежать исключения NullReferenceException
            if (!FileManager.ValidatePath(Config.SongPaths[songIndex])) return;

            // Остановка проигрывания
            Player.AudioPlayer.Stop();

            // Переопределение песни в модели плеера
            Player.CurrentSong = new AudioFileReader(Config.SongPaths[songIndex]);

            // Инициализация плеера в проигрывателе
            Player.AudioPlayer.Init(Player.CurrentSong);

            // Очистка названия песни от расширения и запись в модель плеера
            Player.CurrentSongName = FileManager.NameFilter(Config.SongNames[songIndex]);
        }

        // Статический метод для воспроизведения песни
        public static void PlaySong()
        {
            // Остановка проигрывания песни
            Player.AudioPlayer.Stop();

            // Проверка, не является ли песня null для избежания исключения NullReferenceException
            if (Player.CurrentSong == null) { return; }

            // Обновление статуса плеера
            Player.CurrentPlayerState = PlayerStates.InProgress;

            // Запуск проигрывания песни
            Player.AudioPlayer.Play();
        }

        // Обработчик изменения выбранной песни в списке
        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверка, не выходит ли песня за границы массива с песнями
            if (SongList.SelectedIndex == -1 || SongList.SelectedIndex > SongList.Items.Count || !isPlaylistChoosed) return;

            // Подтверждение, что песня выбрана
            isSongChoosed = true;
        }

        // Обработчик клика по кнопке перемешивания плейлиста
        private void ShuffeButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, выбран ли плейлист для избежания исключения
            if (!isPlaylistChoosed) { return; }

            // Перемешивание плейлиста
            PlayerLogic.ShufflePlaylist(PlaylistsList.SelectedIndex);

            // Обновление списка песен
            UpdateSongList();
        }

        // Обработчик события перетаскивания объектов в окно
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            // Обновление видимости надписи с подсказкой
            TipLabel.Visibility = Visibility.Visible;
        }

        // Обработчик события окончания перетаскивания объектов в окно
        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            // Обновление видимости надписи с подсказкой
            TipLabel.Visibility = Visibility.Hidden;
        }

        // Обработчик изменения значения ползунка громкости
        private void audioBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                // Сохранение текущей громкости
                Player.CurrentSongVolume = (float)(VolumeSlider.Value / 100);

                // Присвоение нового значения громкости плееру
                Player.AudioPlayer.Volume = Player.CurrentSongVolume;

                if (VolumeLabel != null)
                {
                    // Обновление надписи содержащей громкость плеера
                    VolumeLabel.Content = $"{(int)Math.Round(Math.Abs(Player.CurrentSongVolume * 100))}%";
                }
            }
            catch { }
        }

        // Обработчик события закрытия окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Сохранение конфига
            ConfigManager.SaveConfig(PlaylistsList.SelectedIndex, SongList.SelectedIndex, (int)VolumeSlider.Value);
        }

        // Метод для установки изображения в качестве фона окна
        private void SetWindowBackground(string path)
        {
            // Создание ImageBrush
            ImageBrush imageBrush = new ImageBrush();

            // Создание объекта BitmapImage для загрузки изображения
            BitmapImage bitmapImage = new BitmapImage(new Uri(path));

            // Установка изображения в ImageBrush
            imageBrush.ImageSource = bitmapImage;

            imageBrush.Stretch = Stretch.UniformToFill;

            // Установка ImageBrush в качестве фона для окна
            this.Background = imageBrush;
        }

        // Обработчки клика по кнопке открытия настроек
        private void SettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            var result = settingsWindow.ShowDialog();

            if (result == true)
            {

                //Установка фона окна, если путь к изображению указан
                if (!string.IsNullOrEmpty(Config.BackgroundImagePath) && Config.BackgroundImagePath != "none")
                {
                    SetWindowBackground(Config.BackgroundImagePath);
                }
                if (Config.BackgroundImagePath == "none")
                {
                    SetWindowBackground(Path.Combine(Config.DefaultPath, "Icons", $"{Config.Theme}Background.png"));
                }
                UpdateUi();
                UpdatePlaylistList();
                PlaylistsList.SelectedIndex = 0;
            }
        }

        // Обработчик двойного нажатия на список песен
        private void SongList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Проверяем находится ли индекс в границах массива
            if(SongList.SelectedIndex > 0 || SongList.SelectedIndex < SongList.Items.Count)
            {
                // Инициализируем и воспроизводим песню
                InitSong(SongList.SelectedIndex);

                // Обновление максимальной длины ползунка
                DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;
                PlaySong();
            }
        }

        // Метод обновления списка песен
        private void UpdateSongList()
        {
            SongList.ItemsSource = PlayerLogic.GetAllSongs(PlaylistsList.SelectedIndex);
        }

        // Метод обновления списка плейлистов
        private void UpdatePlaylistList()
        {
            PlaylistsList.ItemsSource = FileManager.GetPlaylists();
        }

        // Обновление интерфейса
        private void UpdateUi()
        {
            PlayButton.Source = Config.IconsMap[1].ImageSource;
            PauseButton.Source = Config.IconsMap[2].ImageSource;
            PlayNextSongButton.Source = Config.IconsMap[3].ImageSource;
            PlayPreviosButton.Source = Config.IconsMap[4].ImageSource;
            DeletePlaylistButton.Source = Config.IconsMap[5].ImageSource;
            DeleteSongButton.Source = Config.IconsMap[5].ImageSource;
            ProgramInfoButton.Source = Config.IconsMap[7].ImageSource;
            AddPlaylistButton.Source = Config.IconsMap[8].ImageSource;
            AddSongButton.Source = Config.IconsMap[8].ImageSource;
            MuteButton.Source = Config.IconsMap[9].ImageSource;
            MoveDownButton.Source = Config.IconsMap[11].ImageSource;
            MoveUpButton.Source = Config.IconsMap[10].ImageSource;
            SettingsButton.Source = Config.IconsMap[12].ImageSource;

            var color = System.Drawing.Color.FromName(Config.Theme);
            PlaylistsList.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SongList.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            PlaylistLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SongLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            ShuffeButton.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            VolumeLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            DurationLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SongNameLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }
    }
}