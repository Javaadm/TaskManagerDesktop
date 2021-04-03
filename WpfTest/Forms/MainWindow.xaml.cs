using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfTest.Entitys;

namespace WpfTest.Forms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppContext db;
        private User user;
        private Task active_task;
        const bool IS_FIRST = true;


        public MainWindow()
        {
            initialLocal();
        }


        public MainWindow(User user)
        {
            this.user = user;
            initialLocal();
        }

        private void initialLocal()
        {
            InitializeComponent();
            db = new AppContext();

            if (user == null)
            {
                user = User.getAdminUser();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Task[] tasks = user.getArrayByIdTasks();

            foreach (var task_item in tasks)
            {
                treeTasks.Items.Add(DoFillTree(task_item));
            }


        }

        protected TreeViewItem DoFillTree(Task task)
        {

            Task[] children = user.getArrayByIdTasks(task.id);
            var item = new TreeViewItem()
            {
                Header = task.Name,
                Tag = task
            };

            foreach (var task_item in children)
            {

                TreeViewItem element_tree = DoFillTree(task_item);
                item.Items.Add(element_tree);
            }
            item.MouseDoubleClick += Item_GotFocus;
            //item.GotFocus += Item_GotFocus;
            return item;
        }

        private void Item_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(((TreeViewItem)sender).Tag);
            var item = (TreeViewItem)sender;
            active_task = (Task)item.Tag;

            TextDescriptionTask.Text = active_task.Description;
            NameTask.Text = active_task.Name;
            ListExe.Text = active_task.List_exe;
            StateTask.SelectedIndex = active_task.getStateIdFromZero();
            //todo переписать (каждое переключение) - это много запросов в базу данных 
            LaborIntensityPlanText.Text = "Плановая : " + active_task.getTimePlanRecurtionTree(IS_FIRST);
            LaborIntensityFactText.Text = "Фактическая : " + active_task.getTimeFactRecurtionTree(IS_FIRST);
            Console.WriteLine(active_task.Name);
        }

        private void Button_Add_Task_Parent_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow(user);
            addTaskWindow.Show();
            Close();
        }

        private void Button_Add_Task_Children_Click(object sender, RoutedEventArgs e)
        {

            // добавить проверку на пустой таск
            AddTaskWindow addTaskWindow = new AddTaskWindow(user, active_task);
            addTaskWindow.Show();
            Close();
        }
    }
}
