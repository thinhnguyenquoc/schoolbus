using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{   
    class StateStation: State
    {
        private List<StateStation> states;
        private List<Student> students;
        public StateStation(Station station,int idBus):base(idBus){
              
        }
    }
}
