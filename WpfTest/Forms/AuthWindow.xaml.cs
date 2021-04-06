using System.Windows;
using System.Windows.Media;
using WpfTest.Entitys;
using WpfTest.Forms;

namespace WpfTest
{
    public partial class AuthWindow : Window
    {

        public AuthWindow()
        {
            InitializeComponent();
        }


        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string login = loginAuth.Text.Trim();
            string password = passwordAuth.Password.Trim();

            if (login.Length < 5)
            {
                loginAuth.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение логина");
            }
            else if (password.Length < 5)
            {
                passwordAuth.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение пароля");
            }
            else
            {
                loginAuth.ToolTip = "";
                loginAuth.Background = Brushes.Transparent;
                passwordAuth.ToolTip = "";
                passwordAuth.Background = Brushes.Transparent;

                User authUser = User.AuthByLoginAndPassword(login, password);

                if (authUser != null)
                {
                    MainWindow mainWindow = new MainWindow(authUser);
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Пользователь не найден!");
                }
            }
        }


        private void Button_Windows_Reg_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new RegWindow();
            regWindow.Show();
            Close();
        }
    }
}
