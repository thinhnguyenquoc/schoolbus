using RouteProblem.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem.heuristic
{
     class  Heuristic:Route
    {
        private int mode;

        public int Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        private int limittime;

        public int Limittime
        {
            get { return limittime; }
            set { limittime = value; }
        }
        private School school;
        private List<Station> stations;
        private List<Bus> buses;
         public Heuristic(School school, List<Station> stations, List<Bus> buses)
        { 

        }
        public void Run()
        {
            switch(mode){
               case 1:  break;
               default: break;
            }
        }
        protected Bus SelectBus() {
            Bus bus = null;
            foreach (Bus i in this.buses)// Check Bus and select bus
                if (!i.IsComplete)
                {
                    bus = i;
                    break;
                }               
            return bus;
        }
        protected Station SelectStation() {
            Station station = null;
            foreach (Station si in this.stations)// check station and select station
                if (!si.isEmpty())
                {
                    station = si;
                    break;
                }
            return station;
        }
        public void Heuristic1() {
            Boolean flag=true;
            Bus bus=null;
            Station station=null;
            while (flag) { // initial 1 route
                if ((bus=SelectBus() )== null)
                    break;
                if ((station = this.SelectStation()) == null)
                    break;
                if (flag) {
                    bus.addStation(station);
                    while()// determine next station
                    {
                    }
                }
            }
        }
        public void PrintSolution()
        {
            throw new NotImplementedException();
        }

    }
}
