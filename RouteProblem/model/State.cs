using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class State
    {
        private int id;

        protected int Id
        {
            get { return id; }
            
        }
        public State(int id) {
            this.id = id;
        }
    }
}
