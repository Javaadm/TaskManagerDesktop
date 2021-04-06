using System;
using System.Windows;

namespace WpfTest.Entitys
{
    public class State
    {
        public static int CONST__APPOINTED = 1;
        public static int CONST__PERFORMED = 2;
        public static int CONST__SUSPENDED = 3;
        public static int CONST__COMPLETED = 4;

        public int id { get; set; }
        private string state_value { get; set; }


        public string State_value
        {
            get => state_value;
            set => state_value = value;
        }


        public static void InitialStates()
        {
            State state;

            using (AppContext context = new AppContext())
            {
                state = GetState(context, 1);
                if (state == null)
                {
                    state = new State("Назначена");
                    context.States.Add(state);
                    context.States.Add(new State("Выполняется"));
                    context.States.Add(new State("Приостановлена"));
                    context.States.Add(new State("Завершена"));
                    context.SaveChanges();
                }
            }                  
        }


        public static State GetState(AppContext context, int stateId)
        {
            return context.States.Find(stateId);
        }


        public static void DoChangeState(int stateIdStart, int finshIdState, Task task)
        {
            using (AppContext context = new AppContext())
            {
                Task localTask = task.GetTaskById(context);

                if (finshIdState == CONST__APPOINTED || finshIdState == CONST__SUSPENDED)
                {
                    if (stateIdStart == CONST__PERFORMED)
                    {
                        localTask.Current_Time_Spend += localTask.GetSelfTime();
                    }

                    localTask.Date_Start = default(DateTimeOffset);
                    localTask.State_Id = finshIdState;
                }
                else if (finshIdState == CONST__PERFORMED)
                {
                    localTask.Date_Start = DateTimeOffset.Now;
                    localTask.State_Id = finshIdState;
                }
                else if (finshIdState == CONST__COMPLETED)
                {
                    if (localTask.CheckStateChildren() == true)
                    {
                        if (stateIdStart == CONST__PERFORMED)
                        {
                            localTask.Current_Time_Spend += localTask.GetSelfTime();
                        }

                        localTask.ChangeStateChildren();
                        localTask.Date_Start = default(DateTimeOffset);
                        localTask.State_Id = finshIdState;
                    }
                    else
                    {
                        string messageBoxText = "Нельзя завершить задачу, так как в текущей или в дачерней задаче статус - Назначена";
                        string caption = "Уведомление";
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Information;

                        MessageBox.Show(messageBoxText, caption, button, icon);
                    }
                }

                context.SaveChanges();
            }
        }


        public State() { }


        public State(string value)
        {
            this.state_value = value;
        }
    }
}
