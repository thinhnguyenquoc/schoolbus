using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class Station: Location
    {

        private int stoptime;

        public int Stoptime
        {
            get { return stoptime; }
            set { stoptime = value; }
        }
        List<StateStation> states;

        internal List<StateStation> States
        {
            get { return states; }
       
        }
        List<Student> students;

        internal List<Student> Students
        {
            get { return students; }
            
        }
        public bool isEmpty() { 
            return this.students.Count==0?true:false;
        }
        public Station(int id,double lat,double lon):base(id,lat,lon){
            this.students =new List<Student>();
            this.states = new List<StateStation>();
        }
        public void addStudent(Student student) {
            this.students.Add(student);
        }
        public void addState(StateStation state) {
            this.states.Add(state);
        }

    }
}
