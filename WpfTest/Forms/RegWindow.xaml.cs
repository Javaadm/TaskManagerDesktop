using System.Windows;
using System.Windows.Media;
using WpfTest.Entitys;
using WpfTest.Forms;

namespace WpfTest
{
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = loginReg.Text.Trim();
            string pass = passReg.Password.Trim();
            string pass2 = passReg2.Password.Trim();

            if (login.Length < 5)
            {
                loginReg.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение логина");
            }
            else if (pass.Length < 5)
            {
                passReg.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение пароля");
            }
            else if (pass != pass2)
            {
                MessageBox.Show("Не совпадают пароли");
                passReg.ToolTip = "Не совпадает";
            }
            else
            {
                loginReg.ToolTip = "";
                loginReg.Background = Brushes.Transparent;
                passReg.ToolTip = "";
                passReg.Background = Brushes.Transparent;
                passReg2.ToolTip = "";
                passReg2.Background = Brushes.Transparent;

                User user = User.RegUser(login, pass);

                if (user != null)
                {
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    string messageBoxText = "Пользователь уже зарегестрирован!";
                    string caption = "Проблема с регистрацией";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Question;

                    MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                }
               
            }
        }


        private void Button_Window_Auth_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            Close();

        }
    }
}
