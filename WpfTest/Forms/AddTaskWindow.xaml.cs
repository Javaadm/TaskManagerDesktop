using System.Windows;
using System.Windows.Input;
using WpfTest.Entitys;
using WpfTest.Forms;

namespace WpfTest
{
    public partial class AddTaskWindow : Window
    {
        private User user;
        private Task parentTask;
        private MainWindow mainWindow;


        public AddTaskWindow()
        {
            InitialLocal();
        }


        public AddTaskWindow(User user, MainWindow mainWindow, Task parentTask = null)
        {
            this.mainWindow = mainWindow;
            this.parentTask = parentTask;
            this.user = user;
            InitialLocal();
        }


        private void InitialLocal()
        {
            InitializeComponent();
            State.InitialStates();
        }


        private void NumberValidationPlannedLaborIntensity(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Task.NumberValidationPlannedLaborIntensity(e.Text);
        }


        private void Button_Create_Task_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTask.Text.Trim();
            string description = descriptionTask.Text.Trim();
            string listExe = listExecutorTask.Text.Trim();
            int plannedLaborIntensity = int.Parse(plannedLaborIntensityTask.Text.Trim());

            Task task = new Task(name, description, listExe, plannedLaborIntensity, user.id, parentTask?.id);

            task.CreateThisTask();

            mainWindow.RebootActive();
            Close();

        }
    }

}
