using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace YpProektOne
{
    public partial class MainWindow : Window
    {
        // Строка подключения к базе данных
        private string lineConnection;
        // Общая сумма внесенных денежных средств
        private decimal totalAmount = 0;
        // Номер телефона пользователя
        private string phoneNumber;

        public MainWindow()
        {
            InitializeComponent();
            // Устанавливаем соединение с базой данных
            Connect("JEFFPHARAON\\SQLEXPRESS", "YpOne");
            // Загружаем список услуг из базы данных
            LoadServices();
        }

        // Метод для установки строки подключения к базе данных
        public void Connect(string servername, string dbname)
        {
            lineConnection = $"Data Source={servername};Initial Catalog={dbname};Integrated Security=True";
        }

        // Метод для загрузки списка услуг из базы данных
        private void LoadServices()
        {
            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                string query = "SELECT ServiceName FROM Services";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                List<string> services = new List<string>();
                while (reader.Read())
                {
                    services.Add(reader["ServiceName"].ToString());
                }
                // Устанавливаем источник данных для ListBox
                ServicesListBox.ItemsSource = services;
            }
        }

        // Обработчик события нажатия кнопки "Оплата услуг"
        private void PaymentServices_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на вкладку "Оплата услуг"
            MainTabControl.SelectedItem = PaymentServicesTab;
            PaymentServicesTab.Visibility = Visibility.Visible;
        }

        // Обработчик события нажатия кнопки "Qiwi кошелёк"
        private void QiwiWallet_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на вкладку "Ввод номера телефона"
            MainTabControl.SelectedItem = PhoneNumberInputTab;
            PhoneNumberInputTab.Visibility = Visibility.Visible;
        }

        // Обработчик события нажатия кнопки "Поиск"
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на вкладку "Поиск"
            MainTabControl.SelectedItem = SearchTab;
            SearchTab.Visibility = Visibility.Visible;
        }

        // Обработчик события нажатия кнопки "Сотовая связь"
        private void MobileServices_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на вкладку "Ввод номера телефона"
            MainTabControl.SelectedItem = PhoneNumberInputTab;
            PhoneNumberInputTab.Visibility = Visibility.Visible;
        }

        // Обработчик события нажатия кнопок с цифрами для ввода номера телефона
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // Добавляем нажатую цифру к номеру телефона
            PhoneNumberTextBox.Text += button.Content.ToString();
            // Проверяем валидность номера телефона
            ValidatePhoneNumber();
        }

        // Обработчик события нажатия кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на вкладку "Оплата услуг"
            MainTabControl.SelectedItem = PaymentServicesTab;
        }

        // Обработчик события нажатия кнопки "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PhoneNumberTextBox.Text.Length > 2)
            {
                // Удаляем последнюю цифру из номера телефона
                PhoneNumberTextBox.Text = PhoneNumberTextBox.Text.Substring(0, PhoneNumberTextBox.Text.Length - 1);
            }
            // Проверяем валидность номера телефона
            ValidatePhoneNumber();
        }

        // Обработчик события нажатия кнопки "Далее"
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем введенный номер телефона
            phoneNumber = PhoneNumberTextBox.Text;
            // Отображаем номер телефона на следующей вкладке
            PhoneNumberTextBlock.Text = $"Номер телефона: {phoneNumber}";
            // Переключаемся на вкладку "Ввод денежных средств"
            MainTabControl.SelectedItem = MoneyInputTab;
            MoneyInputTab.Visibility = Visibility.Visible;
        }

        // Метод для валидации номера телефона
        private void ValidatePhoneNumber()
        {
            // Кнопка "Далее" доступна только при вводе номера полностью из 11 цифр
            NextButton.IsEnabled = PhoneNumberTextBox.Text.Length == 12;
        }

        // Обработчик события нажатия кнопок для внесения денежных средств
        private void AddMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // Получаем сумму из текста кнопки и добавляем к общей сумме
            var amount = decimal.Parse(button.Content.ToString().Split(' ')[0]);
            totalAmount += amount;
            // Обновляем текст с общей суммой внесенных средств
            AmountTextBlock.Text = $"Внесено: {totalAmount} рублей";
            // Кнопка "Внести" доступна только если внесено больше 0 рублей
            SubmitButton.IsEnabled = totalAmount > 0;
        }

        // Обработчик события нажатия кнопки "Внести"
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Отображаем сообщение об успешном внесении средств
            MessageBox.Show("Денежные средства успешно внесены");
            // Сбрасываем общую сумму внесенных средств
            totalAmount = 0;
            // Обновляем текст с общей суммой внесенных средств
            AmountTextBlock.Text = "Внесено: 0 рублей";
            // Отключаем кнопку "Внести"
            SubmitButton.IsEnabled = false;
            // Переключаемся на вкладку "Оплата услуг"
            MainTabControl.SelectedItem = PaymentServicesTab;
        }

        // Обработчик события изменения текста в поле поиска
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            List<string> filteredServices = new List<string>();

            if (ServicesListBox != null && ServicesListBox.ItemsSource != null)
            {
                // Фильтруем список услуг по введенному тексту
                foreach (string service in ServicesListBox.ItemsSource)
                {
                    if (service.ToLower().Contains(searchText))
                    {
                        filteredServices.Add(service);
                    }
                }

                // Обновляем источник данных для ListBox
                ServicesListBox.ItemsSource = filteredServices;
            }
        }
    }
}

