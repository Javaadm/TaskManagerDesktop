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

        private int? user_id;
        private int? task_id;
        private int state_id;

        private DateTimeOffset date_cteate;
        private DateTimeOffset date_update;
        private DateTimeOffset date_start;


        public int State_Id
        {
            get => state_id;
            set => state_id = value;
        }


        public int? User_Id
        {
            get => user_id;
            set => user_id = value;
        }


        public int? Task_Id
        {
            get => task_id;
            set => task_id = value;
        }


        public string Name
        {
            get => name;
            set => name = value;
        }


        public bool Is_Deleted
        {
            get => is_deleted;
            set => is_deleted = value;
        }


        public int Current_Time_Spend
        {
            get => current_time_spend;
            set => current_time_spend = value;
        }


        public string Description
        {
            get => description;
            set => description = value;
        }


        public string List_Exe
        {
            get => list_exe;
            set => list_exe = value;
        }


        public int Planned_Labor_Intensity
        {
            get => planned_labor_intensity;
            set => planned_labor_intensity = value;
        }


        public DateTimeOffset Date_Cteate
        {
            get => date_cteate;
            set => date_cteate = value;
        }


        public DateTimeOffset Date_Update
        {
            get => date_update;
            set => date_update = value;
        }


        public DateTimeOffset Date_Start
        {
            get => date_start;
            set => date_start = value;
        }


        public Task GetTaskById(AppContext context, int? id_local = 0)
        {
            if (id_local == 0)
            {
                id_local = this.id;
            }
            return context.Tasks.Find(id_local);
        }


        private List<Task> GetAllChildrenTasksById(AppContext context, int? idLocal = 0)
        {
            if (idLocal == 0)
            {
                idLocal = this.id;
            }
            return context.Tasks.Where(b => b.Task_Id == idLocal && b.Is_Deleted == false).ToList<Task>();
        }


        public static List<Task> GetAllTasksByUserIdAndTaskId(AppContext context, int localUserId, int? idLocal = null)
        {
            return context.Tasks.Where(b => b.Task_Id == idLocal && b.User_Id == localUserId && b.Is_Deleted == false).ToList<Task>();
        }


        internal Task ReturnThisTaskActual()
        {
            using (AppContext context = new AppContext())
            {
                return GetTaskById(context, id);
            }
        }


        internal void UpdateTask(string nameTask, string descriptionTask, string listExecutorTask, string pannedLaborIntensityTask)
        {
            using (AppContext context = new AppContext())
            {
                Task task = GetTaskById(context, id);
                task.name = nameTask;
                task.description = descriptionTask;
                task.list_exe = listExecutorTask;
                task.planned_labor_intensity = int.Parse(pannedLaborIntensityTask);
                context.SaveChanges();
            }
        }


        public User GetUser()
        {
            using (AppContext context = new AppContext())
            {
                return User.GetUserById(context, user_id);
            }
        }


        public string GetStateValue()
        {
            using (AppContext context = new AppContext())
            {
                return State.GetState(context, state_id).State_value;
            }
        }


        public Task GetParentTask()
        {
            using (AppContext context = new AppContext())
            {
                return GetTaskById(context, Task_Id);
            }
        }


        public List<Task> GetChildrenTasks()
        {
            using (AppContext context = new AppContext())
            {
                return GetAllChildrenTasksById(context, id);
            }
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


        internal void CreateThisTask()
        {
            using (AppContext context = new AppContext())
            {
                context.Tasks.Add(this);
                context.SaveChanges();
            }
        }


        public int GetSelfTime()
        {
            if (date_cteate < date_start)
            {
                return (int)(DateTimeOffset.Now - date_start).TotalHours + current_time_spend;
            }

            return current_time_spend;
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

                if (c >= '0' && c <= '9')
                {
                    number += c;
                }
                else if ((number != ""))
                {
                    if (counterNumbers % 2 == 0)
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

                foreach (Task taskItem in tasks)
                {
                    result = result + "  +  " + taskItem.GetTimeRecurtionTree(false);
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
            bool result = true;

            if (this.state_id == State.CONST__APPOINTED)
            {
                return false;
            }

            using (AppContext context = new AppContext())
            {
                List<Task> tasks = GetAllChildrenTasksById(context, id);

                foreach (Task taskItem in tasks)
                {
                    result = result && taskItem.DoFinishTaskAndChildren();
                }
            }

            return result;
        }


        public static bool NumberValidationPlannedLaborIntensity(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(text);
        }


        internal bool CheckStateChildren()
        {
            bool isNormal = state_id != State.CONST__APPOINTED;

            using (AppContext context = new AppContext())
            {
                List<Task> tasks = GetAllChildrenTasksById(context, id);

                foreach (Task taskItem in tasks)
                {
                    isNormal = isNormal && taskItem.CheckStateChildren();
                }
            }

            return isNormal;
        }


        internal void ChangeStateChildren()
        {
            using (AppContext context = new AppContext())
            {
                List<Task> tasks = GetAllChildrenTasksById(context, id);

                foreach (Task taskItem in tasks)
                {
                    taskItem.ChangeStateChildren();
                    taskItem.state_id = State.CONST__COMPLETED;
                    taskItem.current_time_spend += taskItem.GetSelfTime();
                    taskItem.Date_Start = default(DateTimeOffset);
                }

                context.SaveChanges();
            }
        }


        public Task() { }


        public Task(string name, string description, string listExe, int plannedLaborIntensity, int? userId, int? taskId = default(int))
        {
            this.name = name;
            this.description = description;
            this.list_exe = listExe;
            this.planned_labor_intensity = plannedLaborIntensity;
            this.date_cteate = DateTime.Now;
            this.date_update = DateTime.Now;
            this.task_id = taskId;
            this.state_id = State.CONST__APPOINTED;
            this.user_id = userId;
            this.is_deleted = false;
        }
    }
}

