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

        public State() { }

        public State(string value)
        {
            this.state_value = value;
        }
    }
}
