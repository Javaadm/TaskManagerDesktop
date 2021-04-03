using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using WpfTest.Entitys;
using System;
using WpfTest.Forms;

namespace WpfTest
{
    /// <summary>
    /// Логика взаимодействия для AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        private AppContext db;
        private User user;
        private Task parent_task;
        private State state;

        public AddTaskWindow()
        {
            initialLocal();
        }


        public AddTaskWindow(User user, Task parent_task = null)
        {
            this.parent_task = parent_task;
            this.user = user;
            initialLocal();
        }

        private void initialLocal()
        {
            InitializeComponent();
            db = new AppContext();

            if (user == null)
            {
                using (AppContext context = new AppContext())
                {
                    user = context.Users.Where(b => b.Login == "admin").FirstOrDefault();
                }
               

                if (user == null)
                {
                    user = new User("admin", "admin");
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }

            using (AppContext context = new AppContext())
            {
                state = context.States.Where(b => b.id == 1).FirstOrDefault();
            }

            if (state == null)
            {
                state = new State("Назначена");
                db.States.Add(state);
                db.States.Add(new State("Выполняется"));
                db.States.Add(new State("Приостановлена"));
                db.States.Add(new State("Завершена"));
                db.SaveChanges();
            }

        }

        private void NumberValidationPlanned_labor_intensity(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Create_Task_Click(object sender, RoutedEventArgs e)
        {
            string name = textBoxName_add.Text.Trim();
            string description = textBoxDescription_add.Text.Trim();
            string list_exe = textBoxList_executor_add.Text.Trim();
            float planned_labor_intensity = float.Parse(textBoxPlanned_labor_intensity.Text.Trim());
            int? parent_task_id = null;

            if (parent_task != null)
            {
                parent_task_id = parent_task.id;
            }

            Task task = new Task(name, description, list_exe, planned_labor_intensity, user.id, state.id, parent_task_id);

            db.Tasks.Add(task);
            db.SaveChanges();


            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            Close();

        }
    }

}
