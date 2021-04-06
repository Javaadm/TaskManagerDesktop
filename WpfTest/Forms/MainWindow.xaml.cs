using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfTest.Entitys;

namespace WpfTest.Forms
{
    public partial class MainWindow : Window
    {
        private User user;
        private Task activeTask;
        private TreeViewItem activeTreeElement;
        private const bool CONST__IS_FIRST = true;


        public MainWindow()
        {
            InitialLocal();
        }


        public MainWindow(User user)
        {
            this.user = user;
            InitialLocal();
        }


        private void InitialLocal()
        {
            InitializeComponent();

            if (user == null)
            {
                user = User.GetAdminUser();
            }

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTaskTree();
        }


        internal void RebootActive()
        {
            if (activeTask != null)
            {
                activeTask = activeTask.ReturnThisTaskActual();
                DisplayActiveTask();
                LoadTaskTree();
            }
            else
            {
                DisplayDefaultWindow();
            }
        }


        protected void LoadTaskTree()
        {
            List<Task> tasks = user.GetArrayByIdTasks();
            treeTasks.Items.Clear();

            foreach (Task task_item in tasks)
            {
                treeTasks.Items.Add(DoFillTree(task_item));
            }
        }


        private void DisplayActiveTask()
        {
            descriptionTask.Text = activeTask.Description;
            nameTask.Text = activeTask.Name;
            listExeTask.Text = activeTask.List_Exe;
            stateTask.Text = activeTask.GetStateValue();
            laborIntensityTask.Text = "Фактическая / Плановая : " + activeTask.GetTimeRecurtionTree(CONST__IS_FIRST);
        }


        protected void DisplayDefaultWindow()
        {
            LoadTaskTree();
            buttonAddTask.Visibility = Visibility.Hidden;
            buttonEditTask.Visibility = Visibility.Hidden;
            buttonDeleteTask.Visibility = Visibility.Hidden;

            descriptionTask.Text = "";
            nameTask.Text = "Задача";
            listExeTask.Text = "";
            stateTask.Text = "";
            laborIntensityTask.Text = "Фактическая / Плановая : ";
        }


        protected TreeViewItem DoFillTree(Task task)
        {
            List<Task> children = user.GetArrayByIdTasks(task.id);
            TreeViewItem item = new TreeViewItem()
            {
                Header = task.Name,
                Tag = task
            };

            foreach (Task task_item in children)
            {
                TreeViewItem elementTree = DoFillTree(task_item);
                item.Items.Add(elementTree);
            }

            item.MouseLeftButtonUp += new MouseButtonEventHandler(ItemButtonUp);
            return item;
        }


        private TreeViewItem TryGetClickedItem(TreeView treeView, MouseButtonEventArgs e)
        {
            DependencyObject hit = e.OriginalSource as DependencyObject;
            while (hit != null && !(hit is TreeViewItem))
            {
                hit = System.Windows.Media.VisualTreeHelper.GetParent(hit);
            }

            return hit as TreeViewItem;
        }


        private void ItemButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (activeTask == null)
            {
                buttonAddTask.Visibility = Visibility;
                buttonEditTask.Visibility = Visibility;
                buttonDeleteTask.Visibility = Visibility;
            }

            TreeViewItem clickedItem = TryGetClickedItem(treeTasks, e);

            if (clickedItem == null || clickedItem != sender)
            {
                return;
            }

            TreeViewItem item = (TreeViewItem)sender;
            activeTreeElement = item;

            activeTask = (Task)item.Tag;

            DisplayActiveTask();
        }


        private void Button_Add_Task_Parent_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow(user, this);
            addTaskWindow.Show();
        }


        private void Button_Add_Task_Children_Click(object sender, RoutedEventArgs e)
        {
            AddTaskWindow addTaskWindow = new AddTaskWindow(user, this, activeTask);
            addTaskWindow.Show();
        }


        private void Button_Edit_Task_Click(object sender, RoutedEventArgs e)
        {
            EditTaskWindow editTaskWindow = new EditTaskWindow(user, this, activeTask);
            editTaskWindow.Show();
        }


        private void Button_Delete_Task_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Вы действительно хотите удалить " + activeTask.Name + " и все вложеные в нее задачи?";
            string caption = "Удаление задачи";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            if (result == MessageBoxResult.Yes)
            {
                activeTask.DeletedTask();
                activeTask = null;
                RebootActive();
            }
        }
    }
}
