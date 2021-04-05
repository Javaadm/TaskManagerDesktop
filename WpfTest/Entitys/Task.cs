using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace WpfTest.Entitys
{
    public class Task
    {
        public int id { get; set; }
        private string name;
        private string description;
        private string list_exe;
        private int planned_labor_intensity;
        private bool is_deleted;
        private int current_time_spend;

        private int? userId;
        private int? taskId;
        private int stateId;
        
        private DateTimeOffset date_cteate;
        private DateTimeOffset date_update;
        private DateTimeOffset date_start;
        private DateTimeOffset date_finish;

        public int? TaskId
        {
            get { return taskId; }
            set { taskId = value; }
        }

        private Task GetTaskById(AppContext context, int? id) {
            return context.Tasks.Find(id);
        }

        public Task GetThisTaskInContext(AppContext context) {
            return context.Tasks.Find(id);
        }

        private List<Task> GetAllChildrenTasksById(AppContext context, int? id_local = 0) {
            if (id == 0)
            {
                id_local = this.id;
            } 
            return context.Tasks.Where(b => b.TaskId == id_local && b.Is_Deleted == false).ToList<Task>();
        }

        internal Task ReturnThisTaskActual()
        {
            using (AppContext context = new AppContext()) {

               return GetTaskById(context, id);
            }
        }

        internal void UpdateTask(string nameTask, string descriptionTask, string listExecutorTask, string pannedLaborIntensityTask, int stateTask)
        {
            using (AppContext context = new AppContext())
            {
                Task task = GetTaskById(context, id);
                task.name = nameTask;
                task.description = descriptionTask;
                task.list_exe = listExecutorTask;
                task.stateId = stateTask;
                task.planned_labor_intensity = int.Parse(pannedLaborIntensityTask);
                context.SaveChanges();
            }
        }

        public int StateId
        {
            get { return stateId; }
            set { stateId = value; }
        }

        public int? UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Is_Deleted
        {
            get { return is_deleted; }
            set { is_deleted = value; }
        }
        
        public int Current_Time_Spend
        {
            get { return current_time_spend; }
            set { current_time_spend = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string List_Exe
        {
            get { return list_exe; }
            set { list_exe = value; }
        }

        public int Planned_Labor_Intensity
        {
            get { return planned_labor_intensity; }
            set { planned_labor_intensity = value; }
        }

        public DateTimeOffset Date_Cteate
        {
            get { return date_cteate; }
            set { date_cteate = value; }
        }

        public DateTimeOffset Date_Update
        {
            get { return date_update; }
            set { date_update = value; }
        }

        public DateTimeOffset Date_Start
        {
            get { return date_start; }
            set { date_start = value; }
        }

        public DateTimeOffset Date_Finish
        {
            get { return date_finish; }
            set { date_finish = value; }
        }


        public User GetUser()
        {
            User user;
            using (AppContext context = new AppContext())
            {
                user = context.Users.Find(UserId);
            }

            return user;
        }

        public string GetState()
        {
            string state;
            using (AppContext context = new AppContext())
            {
                state = context.States.Find(stateId).State_value;
            }

            return state;
        }

        public Task GetParentTask()
        {
            Task task;
            using (AppContext context = new AppContext())
            {
                task = GetTaskById(context, TaskId);
            }

            return task;
        }

        public List<Task> GetChildrenTasks()
        {
            List<Task> tasks;
            using (AppContext context = new AppContext())
            {
                tasks = GetAllChildrenTasksById(context, id);
                    
            }

            return tasks;
        }

        internal void DeletedTask()
        {
            using (AppContext context = new AppContext())
            {
                Task task = GetTaskById(context, id);

                task.is_deleted = true;
                context.SaveChanges();
                
            }
        }

        public float GetSelfTime()
        {
            if (date_cteate < date_finish)
                return (float)(date_finish - date_start).TotalHours;
            if (date_cteate < date_start)
                return (float)(DateTimeOffset.Now - date_start).TotalHours;

            return 0;
        } 

        private string FindSumTimeTask(string timeString)
        {
            timeString += " "; 
            int sumFact = 0;
            int sumPlan = 0;
            int counterNumbers = 0;
            string number = "";
            for (int i = 0; i < timeString.Length; i++)
            {
                char c = timeString[i];

                if( c>='0' && c <= '9')
                {
                    number += c;
                }
                else if ((number != "") )
                {
                    if(counterNumbers %2 == 0)
                    {
                        sumFact += int.Parse(number);
                    }
                    else
                    {
                        sumPlan += int.Parse(number);
                    }

                    number = "";
                    counterNumbers++;

                }
            }

            return sumFact.ToString() + " / " + sumPlan.ToString();
        }
       
        
        public string GetTimeRecurtionTree(bool isFirstStap)
        {
            string result = "";
            using (AppContext context = new AppContext())
            {

                List<Task> tasks = GetAllChildrenTasksById(context, id);

                foreach (var task_item in tasks)
                {
                    result = result + "  +  " + task_item.GetTimeRecurtionTree(false);
                }
            }
            if (isFirstStap)
            {
                result = " (" + GetSelfTime().ToString() + " / " + planned_labor_intensity.ToString() + ")" + result;
                string sum = FindSumTimeTask(result);
                result = sum + " = " + result;

            }
            else
            {
                result = GetSelfTime().ToString() + " / " + planned_labor_intensity.ToString() + result;
            }

            return result;
        }

        private bool DoFinishTaskAndChildren()
        {
            bool result;
            if (this.stateId == 1)
            {
                return false;
            }
            result = true;
            using (AppContext context = new AppContext())
            {
                List<Task> tasks = GetAllChildrenTasksById(context, id);

                foreach (var task_item in tasks)
                {
                    result = result && task_item.DoFinishTaskAndChildren();
                }
            }

            //todo do update
            return result;

        }

        public static bool NumberValidationPlannedLaborIntensity(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(text);
        }


        internal bool CheckStateChildren()
        {
            bool isNormal = stateId != 1;
            using (AppContext context = new AppContext())
            {

                List<Task> tasks = GetAllChildrenTasksById(context, id);
                foreach (var task_item in tasks)
                {
                    isNormal = isNormal && task_item.CheckStateChildren();
                }
            }
            return isNormal;
        }

        internal void ChangeStateChildren()
        {
            using (AppContext context = new AppContext())
            {

                List<Task> tasks = GetAllChildrenTasksById(context, id);
                foreach (var task_item in tasks)
                {
                    task_item.stateId = 4;
                }
                context.SaveChanges();
            }
        }

        public Task() { }

        public Task(string name, string description, string list_exe, int planned_labor_intensity, int? userId, int stateId, int? taskId = default(int))
        {
            this.name = name;
            this.description = description;
            this.list_exe = list_exe;
            this.planned_labor_intensity = planned_labor_intensity;
            this.date_cteate = DateTime.Now;
            this.date_update = DateTime.Now;
            this.taskId = taskId;
            this.stateId = stateId;
            this.userId = userId;
            this.is_deleted = false;
        }
    }
}

