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
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        AppContext db;

        public RegWindow()
        {
            InitializeComponent();

            db = new AppContext();
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxlogin_reg.Text.Trim();
            string pass = TextBoxpass_reg.Password.Trim();
            string pass2= TextBoxpass_reg2.Password.Trim();
            
            if(login.Length < 5)
            {
                TextBoxlogin_reg.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение логина");
            }
            else if(pass.Length < 5)
            {
                TextBoxpass_reg.ToolTip = "Слишком короткое значение";
                MessageBox.Show("Слишком короткое значение пароля");
            }
            else if(pass != pass2)
            {
                MessageBox.Show("Не совпадают пароли");
                TextBoxpass_reg.ToolTip = "Не совпадает";
            }
            else
            {
                TextBoxlogin_reg.ToolTip = "";
                TextBoxlogin_reg.Background = Brushes.Transparent;
                TextBoxpass_reg.ToolTip = "";
                TextBoxpass_reg.Background = Brushes.Transparent;
                TextBoxpass_reg2.ToolTip = "";
                TextBoxpass_reg2.Background = Brushes.Transparent;

                MessageBox.Show("Все хорошо!");

                User user = new User(login, pass);

                db.Users.Add(user);
                db.SaveChanges();

                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();
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
