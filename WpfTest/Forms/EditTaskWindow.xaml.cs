using System;
using System.Windows;
using WpfTest.Entitys;

namespace WpfTest.Forms
{
    public partial class EditTaskWindow : Window
    {
        private Task task;
        private User user;
        private MainWindow mainWindow;
       

        public EditTaskWindow()
        {
            InitializeComponent();
        }


        public EditTaskWindow(User user, MainWindow mainWindow, Task task = null)
        {
            this.task = task;
            this.user = user;
            this.mainWindow = mainWindow;
            InitializeComponent();
            DisplayTask();
        }


        private void Button_Save_Task_Click(object sender, RoutedEventArgs e)
        {
            if (task.State_Id != stateTask.SelectedIndex + 1) {
                State.DoChangeState(task.State_Id, stateTask.SelectedIndex + 1, task);
            }
            task.UpdateTask(nameTask.Text, descriptionTask.Text, listExecutorTask.Text, pannedLaborIntensityTask.Text);
            mainWindow.RebootActive();
            Close();
        }


        private void DisplayTask()
        {
            nameTask.Text = task.Name;
            descriptionTask.Text = task.Description;
            listExecutorTask.Text = task.List_Exe;

            pannedLaborIntensityTask.Text = task.Planned_Labor_Intensity.ToString();
            stateTask.SelectedIndex = task.State_Id - 1;
        }


        private void NumberValidationPlannedLaborIntensity(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Task.NumberValidationPlannedLaborIntensity(e.Text);
        }
    }
}