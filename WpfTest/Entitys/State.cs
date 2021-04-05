using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTest.Entitys
{
    public class State
    {
        public int id { get; set; }
        private string state_value { get; set; }

        public string State_value
        {
            get { return state_value; }
            set { state_value = value; }
        }

        static public void DoChangeState(int stateIdStart, int finshIdState, Task task)
        {
            using (AppContext context = new AppContext())
            {
                var localTask = task.GetThisTaskInContext(context);
                if (finshIdState == 1 || finshIdState == 3)
                {
                    if (stateIdStart == 2)
                    {
                        localTask.Current_Time_Spend += (int)(DateTimeOffset.Now - localTask.Date_Start).TotalHours;
                    }

                    localTask.Date_Start = default(DateTimeOffset);

                }
                else if (finshIdState == 2)
                {
                    localTask.Date_Start = DateTimeOffset.Now;

                }
                else if (finshIdState == 4)
                {
                    if (localTask.CheckStateChildren() == true) {
                        if (stateIdStart == 2)
                        {
                            localTask.Current_Time_Spend += (int)(DateTimeOffset.Now - localTask.Date_Start).TotalHours;
                        }
                        localTask.ChangeStateChildren();
                        localTask.Date_Start = default(DateTimeOffset);
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
