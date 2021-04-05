using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using WpfTest.Entitys;
using System;
using WpfTest.Forms;

namespace WpfTest
{

    public partial class AddTaskWindow : Window
    {
        private AppContext db;
        private User user;
        private Task parent_task;
        private State state;
        private MainWindow mainWindow;

        public AddTaskWindow()
        {
            initialLocal();
        }


        public AddTaskWindow(User user, MainWindow mainWindow, Task parent_task = null)
        {
            this.mainWindow = mainWindow;
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

        private void NumberValidationPlannedLaborIntensity(object sender, TextCompositionEventArgs e)
        { 
            e.Handled = Task.NumberValidationPlannedLaborIntensity(e.Text);
        }

        private void Button_Create_Task_Click(object sender, RoutedEventArgs e)
        {
            string name = textBoxName_add.Text.Trim();
            string description = textBoxDescription_add.Text.Trim();
            string list_exe = textBoxList_executor_add.Text.Trim();
            int planned_labor_intensity = int.Parse(textBoxPlanned_labor_intensity.Text.Trim());
            int? parent_task_id = null;

            if (parent_task != null)
            {
                parent_task_id = parent_task.id;
            }

            Task task = new Task(name, description, list_exe, planned_labor_intensity, user.id, state.id, parent_task_id);

            db.Tasks.Add(task);
            db.SaveChanges();

            mainWindow.RebootActive();
            Close();

        }
    }

}
