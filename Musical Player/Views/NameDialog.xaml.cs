using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Musical_Player.Views
{
    /// <summary>
    /// Логика взаимодействия для NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        public NameDialog()
        {
            // Инициализация компонентов диалогового окна
            InitializeComponent();

            // Устанавливаем кнопку OK в неактивное состояние при создании окна
            OK.IsEnabled = false;
        }

        // Поле для хранения результата диалога (нажата кнопка OK или нет)
        private bool Result = false;

        // Обработчик события нажатия на кнопку OK
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем результат в true и закрываем окно
            Result = true;
            Close();
        }

        // Обработчик события закрытия окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Устанавливаем DialogResult в соответствии с результатом
            DialogResult = Result;
        }

        // Обработчик события изменения текста в поле ввода
        private void InputTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем, не является ли текстовое поле пустым или null
            if (InputTextbox.Text.Length == 0 || InputTextbox.Text == null)
            {
                // Если пусто, делаем кнопку OK неактивной
                OK.IsEnabled = false;
            }
            else
            {
                // Если есть текст, активируем кнопку OK
                OK.IsEnabled = true;
            }
        }

    }
}
