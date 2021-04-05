using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private TreeViewItem active_tree_element;
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

        internal void RebootActive()
        {
            if (active_task != null)
            {
                active_task = active_task.ReturnThisTaskActual();
                DisplayActiveTask();
                LoadTaskTree();
            }
            else
            {
                DisplayDefaultWindow();
            }
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTaskTree();

            Console.WriteLine(default(DateTimeOffset));
        }

        protected void LoadTaskTree()
        {
            Task[] tasks = user.GetArrayByIdTasks();
            treeTasks.Items.Clear();
            foreach (var task_item in tasks)
            {
                treeTasks.Items.Add(DoFillTree(task_item));
            }
        }

        protected void DisplayDefaultWindow()
        {
            LoadTaskTree();
            buttonAddTask.Visibility = Visibility.Hidden;
            buttonEditTask.Visibility = Visibility.Hidden;
            buttonDeleteTask.Visibility = Visibility.Hidden;

            TextDescriptionTask.Text = "";
            NameTask.Text = "Задача";
            ListExe.Text = "";
            StateTask.Text = "";
            LaborIntensityText.Text = "Фактическая / Плановая : ";
        }

        protected TreeViewItem DoFillTree(Task task)
        {

            Task[] children = user.GetArrayByIdTasks(task.id);
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
            item.MouseLeftButtonUp += new MouseButtonEventHandler(Item_GotFocus);
            //item.GotFocus += Item_GotFocus;
            return item;
        }

        TreeViewItem TryGetClickedItem(TreeView treeView, MouseButtonEventArgs e)
        {
            var hit = e.OriginalSource as DependencyObject;
            while (hit != null && !(hit is TreeViewItem))
                hit = System.Windows.Media.VisualTreeHelper.GetParent(hit);

            return hit as TreeViewItem;
        }

        private void Item_GotFocus(object sender, MouseButtonEventArgs e)
        {
          
            if (active_task == null)
            {
                buttonAddTask.Visibility = Visibility;
                buttonEditTask.Visibility = Visibility;
                buttonDeleteTask.Visibility = Visibility;
            }
            TreeViewItem clickedItem = TryGetClickedItem(treeTasks, e);

            if (clickedItem == null || clickedItem != sender)
                return;

            Console.WriteLine(clickedItem.Header);
            Console.WriteLine(((TreeViewItem)sender).Tag);
            var item = (TreeViewItem)sender;
            active_tree_element = item;

            active_task = (Task)item.Tag;

            DisplayActiveTask();
 
            Console.WriteLine(active_task.Name);
        }

        private void DisplayActiveTask()
        {
            TextDescriptionTask.Text = active_task.Description;
            NameTask.Text = active_task.Name;
            ListExe.Text = active_task.List_Exe;
            StateTask.Text = active_task.GetState();
            //todo переписать (каждое переключение) - это много запросов в базу данных 
            LaborIntensityText.Text = "Фактическая / Плановая : " + active_task.GetTimeRecurtionTree(IS_FIRST);
        }

        private void Button_Add_Task_Parent_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow(user, this);
            addTaskWindow.Show();
        }

        private void Button_Add_Task_Children_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow(user, this, active_task);
            addTaskWindow.Show();
        }

        private void Button_Edit_Task_Click(object sender, RoutedEventArgs e)
        {
            EditTaskWindow editTaskWindow = new EditTaskWindow(user, this, active_task);
            editTaskWindow.Show();
        }

        private void Button_Delete_Task_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Вы действительно хотите удалить " + active_task.Name + " и все вложеные в нее задачи?";
            string caption = "Удаление задачи";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            if (result == MessageBoxResult.Yes)
            {
                active_task.DeletedTask();
                active_task = null;
                RebootActive();
            }
        }
        
    }
}
