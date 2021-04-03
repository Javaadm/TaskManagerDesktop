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
using WpfTest.Entitys;
using WpfTest.Forms;

namespace WpfTest
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxlogin_reg.Text.Trim();
            string pass = TextBoxpass_reg.Password.Trim();

            if (login.Length < 5)
            {
                TextBoxlogin_reg.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение логина");
            }
            else if (pass.Length < 5)
            {
                TextBoxpass_reg.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение пароля");
            }
            else
            {
                TextBoxlogin_reg.ToolTip = "";
                TextBoxlogin_reg.Background = Brushes.Transparent;
                TextBoxpass_reg.ToolTip = "";
                TextBoxpass_reg.Background = Brushes.Transparent;

                User authUser = null;
                using (AppContext context = new AppContext()){
                    authUser = context.Users.Where(b => b.Login == login && b.Password == pass).FirstOrDefault();

                }

                if (authUser != null) {
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
