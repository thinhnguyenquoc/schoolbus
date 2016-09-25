using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem
{

    abstract class Location
     {
         protected int id;

         public int Id
         {
             get { return id; }
         }
         private double lat;

         public double Lat
         {
             get { return lat; }
             set { lat = value; }
         }
         private double lon;

         public double Lon
         {
             get { return lon; }
             set { lon = value; }
         }
         private List<int> distanceToX;

         public List<int> DistanceToX
         {
             get { return distanceToX; }
             set { distanceToX = value; }
         }
         private List<int> durationToX;

         public List<int> DurationToX
         {
             get { return durationToX; }
             set { durationToX = value; }
         }
         public Location(int id,double lat,double lon) {
             this.id = id;
             this.lat = lat;
             this.lon = lon;
             this.distanceToX = new List<int>();
             this.durationToX = new List<int>();
         }
         public void AddDurationToX(int x) {
             this.durationToX.Add(x);
         }
         public int GetDistance(Location x) {
             return this.distanceToX[x.Id];
         }
         public int GetDuration(Location x)
         {
             return this.distanceToX[x.Id];
         }
    }
}
