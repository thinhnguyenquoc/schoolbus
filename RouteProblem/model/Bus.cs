using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.model
{
    class Bus
    {
        private int id;
        public int Id
        {
            get { return id; }
            
        }
        private int capacity;

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
        private int curStudent;

        public int CurStudent
        {
            get { return curStudent; }
            set { curStudent = value; }
        }
        private int runningTime;

        public int RunningTime
        {
            get { return runningTime; }
            set { runningTime = value; }
        }
        private bool isComplete;

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; }
        }
        private List<Student> students;

        internal List<Student> Students
        {
            get { return students; }
           
        }
        public void addStudent(Student student) {
            this.students.Add(student);
        }
        private Path path;

        internal Path Path
        {
            get { return path; }
            
        }
        public void addStation(Station station){
        }

          
    }
}
