using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Musical_Player.LoadingLogic;

namespace Musical_Player.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            // Инициализация компонентов окна
            InitializeComponent();

            // Устанавливаем состояние флажка автоматического переключения песен в соответствии с текущим значением в Player
            AutoSongSwitchingSelector.IsChecked = Player.IsAutoSwitching;
            SwitchThemeCheckbox.IsChecked = Config.Theme == "Black" ? true : false;

            // Установка фона окна, если путь к изображению указан
            if (!string.IsNullOrEmpty(Config.BackgroundImagePath) && Config.BackgroundImagePath != "none")
            {
                SetWindowBackground(Config.BackgroundImagePath);
            }
            if(Config.BackgroundImagePath == "none")
            {
                this.Background = null;
                this.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            ResetBackgroundButton.Source = Config.IconsMap[6].ImageSource;
            
        }

        // Поле для хранения результата диалога (нажата кнопка OK или нет)
        private bool Result = false;

        // Обработчик события нажатия на кнопку смены фонового изображения
        private void SwitchBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем метод установки нового фонового изображения из класса FileManager
            FileManager.SetNewBackgroundImage();

            // Устанавливаем DialogResult в true и закрываем окно
            Result = true;
            Close();
        }

        // Обработчик события нажатия на кнопку выбора новой стандартной директории
        private void ChooseDefaultDirButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем метод установки новой стандартной директории из класса FileManager
            FileManager.SetNewDirectoryPath();

            // Устанавливаем DialogResult в true и закрываем окно
            Result = true;
            Close();
        }

        // Обработчик события изменения состояния флажка автоматического переключения песен
        private void AutoSongSwitchingSelector_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем значение AutoSongSwitchingSelector на противоположное к текущему
            AutoSongSwitchingSelector.IsChecked = !Player.IsAutoSwitching;

            // Обновляем значение свойства IsAutoSwitching в Player в соответствии с состоянием флажка
            Player.IsAutoSwitching = !Player.IsAutoSwitching;
        }

        // Обработчик события нажатия на кнопку закрытия окна
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем окно
            Close();
        }

        // Обработчик события закрытия окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Устанавливаем DialogResult в соответствии с результатом
            DialogResult = Result;
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

        // Обработчик события нажатия кнопки для сброса фона плеера.
        private void ResetBackgroundButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Устанавливает занчение для фона none
            Config.BackgroundImagePath = "none";

            // Устанавливаем DialogResult в true и закрываем окно
            Result = true;
            Close();
        }

        private void SwitchThemeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            SwitchThemeCheckbox.IsChecked = !SwitchThemeCheckbox.IsChecked;
            if (SwitchThemeCheckbox.IsChecked == true)
            {
                Config.Theme = "White";
            }
            else
            {
                Config.Theme = "Black";
            }

            SetupLogic.CreateIconsBitmap(Config.Theme);

            Result = true;
            Close();
        }
    }
}
