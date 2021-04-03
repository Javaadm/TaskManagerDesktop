using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WpfTest.Entitys
{
    public class Task
    {
        public int id { get; set; }
        private string name;
        private string description;
        private string list_exe;
        private float planned_labor_intensity;

        private int? userId;
        public int? taskId;
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

        public int getStateIdFromZero()
        {
            return stateId - 1;
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

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string List_exe
        {
            get { return list_exe; }
            set { list_exe = value; }
        }

        public float Planned_labor_intensity
        {
            get { return planned_labor_intensity; }
            set { planned_labor_intensity = value; }
        }

        public DateTimeOffset Date_cteate
        {
            get { return date_cteate; }
            set { date_cteate = value; }
        }

        public DateTimeOffset Date_update
        {
            get { return date_update; }
            set { date_update = value; }
        }

        public DateTimeOffset Date_start
        {
            get { return date_start; }
            set { date_start = value; }
        }

        public DateTimeOffset Date_finish
        {
            get { return date_finish; }
            set { date_finish = value; }
        }


        public User GetUser()
        {
            User user;
            using (AppContext context = new AppContext())
            {
                user = context.Users.Where(b => b.id == UserId).FirstOrDefault();
            }

            return user;
        }

        public Task GetParentTask()
        {
            Task task;
            using (AppContext context = new AppContext())
            {
                task = context.Tasks.Where(b => b.id == TaskId).FirstOrDefault();
            }

            return task;
        }

        public List<Task> GetChildrenTasks()
        {
            List<Task> tasks;
            using (AppContext context = new AppContext())
            {
                tasks = context.Tasks.Where(b => b.TaskId == id).ToList<Task>();
            }

            return tasks;
        }

        public State GetState()
        {
            State state;
            using (AppContext context = new AppContext())
            {
                state = context.States.Where(b => b.id == StateId).FirstOrDefault();
            }

            return state;
        }

        public float getSelfTime()
        {
            if (date_cteate < date_finish)
                return (float)(date_finish - date_start).TotalHours;
            if (date_cteate < date_start)
                return (float)(DateTime.Now - date_start).TotalHours;

            return 0;
        } 


        public string getTimeFactRecurtionTree(bool isFirstStap)
        {
            string result = "";
            using (AppContext context = new AppContext())
            {
                var tasks = context.Tasks.Where(b => b.UserId == this.UserId && b.TaskId == this.id).ToArray();

                foreach (var task_item in tasks)
                {
                    result = result + " + " + task_item.getTimeFactRecurtionTree(false);
                }
            }
            if (isFirstStap)
            {
                
                result = " (" + getSelfTime().ToString() + ")" + result;
                string sum = new DataTable().Compute(result, null).ToString();
                result = sum + " = " + result;
            }
            else
            {
                result = getSelfTime().ToString() + result;
            }

            
            return result;
        }
        
        public string getTimePlanRecurtionTree(bool isFirstStap)
        {
            string result = "";
            using (AppContext context = new AppContext())
            {
                var tasks = context.Tasks.Where(b => b.UserId == this.UserId && b.TaskId == this.id).ToArray();

                foreach (var task_item in tasks)
                {
                    result = result + " + " + task_item.getTimePlanRecurtionTree(false);
                }
            }
            if (isFirstStap)
            {
                result = " (" + planned_labor_intensity.ToString() + ")" + result;
                string sum = new DataTable().Compute(result, null).ToString();
                result = sum + " = " + result;
            }
            else
            {
                result = planned_labor_intensity.ToString() + result;
            }

            return result;
        }

        public bool doFinishTaskAndChildren()
        {
            bool result;
            if (this.stateId == 1)
            {
                return false;
            }
            result = true;
            using (AppContext context = new AppContext())
            {
                var tasks = context.Tasks.Where(b => b.UserId == this.UserId && b.TaskId == this.id).ToArray();

                foreach (var task_item in tasks)
                {
                    result = result && task_item.doFinishTaskAndChildren();
                }
            }

            //todo do update
            return result;

        }


        public Task() { }

        public Task(string name, string description, string list_exe, float planned_labor_intensity, int? userId, int stateId, int? taskId = default(int))
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
        }
    }
}

