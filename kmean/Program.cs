using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using System.Diagnostics;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace kmean
{
    public class myGeo
    {
        public int Id { get; set; }
        public string address { get; set; }
        public string gaddress { get; set; }
        public GeoCoordinate geo { get; set; }
    }

    public class myresult
    {
        public int[] Cluster { get; set; }
        public GeoCoordinate[] Centroids { get; set; }
    }
    class Program
    {
        static int maxcapacity = 5;
        static void Main(string[] args)
        {
            try
            {
                HSSFWorkbook wb = null;
                // create xls if not exists
                if (File.Exists("../../data/data.xls"))
                {
                    FileStream file = new FileStream("../../data/data.xls", FileMode.Open, FileAccess.Read);
                    wb = new HSSFWorkbook(file);
                }
                ISheet sheet = wb.GetSheetAt(0);

                List<myGeo> geos = new List<myGeo>();
                for (int i = 2; i <= sheet.LastRowNum; i++)
                {
                    myGeo geo = new myGeo();
                    IRow row = sheet.GetRow(i);
                    geo.geo = new GeoCoordinate();
                    if (row.Cells[9] != null && row.Cells[9].CellType != CellType.Blank)
                    {
                        if (row.Cells[9].CellType == CellType.String && !string.IsNullOrEmpty(row.Cells[9].StringCellValue))
                        {
                            geo.geo.Latitude = Convert.ToDouble(row.Cells[9].StringCellValue);
                        }
                        else if (row.Cells[9].CellType == CellType.Numeric)
                        {
                            geo.geo.Latitude = Convert.ToDouble(row.Cells[9].NumericCellValue);
                        }
                        if (row.Cells[10].CellType == CellType.String && !string.IsNullOrEmpty(row.Cells[10].StringCellValue))
                        {
                            geo.geo.Longitude = Convert.ToDouble(row.Cells[10].StringCellValue);
                        }
                        else if (row.Cells[10].CellType == CellType.Numeric)
                        {
                            geo.geo.Longitude = Convert.ToDouble(row.Cells[10].NumericCellValue);
                        }
                        geo.address = row.Cells[4].StringCellValue;
                        geo.gaddress = row.Cells[11].StringCellValue;
                        geo.Id = i;
                        geo.geo.Speed = i;
                        geos.Add(geo);
                    }
                }
                GeoCoordinate[] rawData = new GeoCoordinate[geos.Count];
                for (int i = 0; i < geos.Count; i++)
                {
                    rawData[i] = geos[i].geo;
                }

                int numClusters = Convert.ToInt32(Convert.ToDouble(geos.Count() / maxcapacity)) + 1;
                int maxCount = 80;
                Debug.WriteLine("\nk = " + numClusters + " and maxCount = " + maxCount);
                myresult result = Cluster(rawData, numClusters, maxCount);
                Debug.WriteLine("\nClustering complete");
                Debug.WriteLine("\nClustering in internal format: \n");
                Debug.WriteLine("\nClustered data:");

                //ShowClustering(rawData, numClusters, result, geos);
                StoreBusStop(wb, rawData, numClusters, result, geos);
                StoreStation(wb, rawData, numClusters, result, geos);
                SaveExcel(wb);

                /*GeoCoordinate outlier = Outlier(rawData, clustering, numClusters, 0);
                Debug.WriteLine("Outlier for cluster 0 is:");
                ShowVector(outlier);*/

                Debug.WriteLine("\nEnd demo\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        } // Main

        // Nguyen
        static void SaveExcel(HSSFWorkbook hssfworkbook)
        {
            String filePath_output = "../../data/data_output.xls";
            FileStream fs = new FileStream(filePath_output, FileMode.OpenOrCreate);
            hssfworkbook.Write(fs);
            fs.Close();
        }


        // 14 short static method definitions here         

        static myresult Cluster(GeoCoordinate[] rawData, int numClusters, int maxCount)
        {
            bool changed = true;
            int ct = 0;
            int numTuples = rawData.Length;
            int[] clustering = InitClustering(numTuples, numClusters, 0);
            GeoCoordinate[] means = Allocate(numClusters);
            GeoCoordinate[] centroids = Allocate(numClusters);
            UpdateMeans(rawData, clustering, means);
            UpdateCentroids(rawData, clustering, means, centroids);
            while (changed == true && ct < maxCount)
            {
                ++ct;
                changed = Assign(rawData, clustering, centroids);
                UpdateMeans(rawData, clustering, means);
                UpdateCentroids(rawData, clustering, means, centroids);
            }
            myresult result = new myresult();
            result.Centroids = centroids;
            result.Cluster = clustering;
            return result;
        }

        static bool Assign(GeoCoordinate[] rawData, int[] clustering, GeoCoordinate[] centroids)
        {
            int numClusters = centroids.Length;
            bool changed = false;
            double[] distances = new double[numClusters];
            for (int i = 0; i < rawData.Length; ++i)
            {
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(rawData[i], centroids[k]);
                var list = distances.ToList();
                list.Sort();
                var found = false;
                for (int l = 0; l < list.Count; l++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (distances[j] == list[l])
                        {
                            if (AvailableToAssign(j, clustering))
                            {
                                if (AvailableToLeave(clustering[i], clustering))
                                {
                                    if (j != clustering[i])
                                    {
                                        changed = true;
                                        clustering[i] = j;
                                        found = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    List<int> indexs = new List<int>();
                                    for (int inf = 0; inf < clustering.Count(); inf++)
                                    {
                                        if (clustering[inf] == j)
                                        {
                                            indexs.Add(inf);
                                        }
                                    }
                                    foreach (var h in indexs)
                                    {
                                        if (distances[j] < Distance(rawData[h], centroids[j]))
                                        {
                                            if (j != clustering[i])
                                            {
                                                changed = true;
                                                var temp = clustering[i];
                                                clustering[i] = j;
                                                clustering[h] = temp;
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                List<int> indexs = new List<int>();
                                for (int inf = 0; inf < clustering.Count(); inf++)
                                {
                                    if (clustering[inf] == j)
                                    {
                                        indexs.Add(inf);
                                    }
                                }
                                foreach (var h in indexs)
                                {
                                    if (distances[j] < Distance(rawData[h], centroids[j]))
                                    {
                                        if (j != clustering[i])
                                        {
                                            changed = true;
                                            var temp = clustering[i];
                                            clustering[i] = j;
                                            clustering[h] = temp;
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (found)
                        break;
                }


                //int newCluster = MinIndex(distances);

            }
            return changed;
        }

        static void UpdateCentroids(GeoCoordinate[] rawData, int[] clustering, GeoCoordinate[] means, GeoCoordinate[] centroids)
        {
            for (int k = 0; k < centroids.Length; ++k)
            {
                GeoCoordinate centroid = ComputeCentroid(rawData, clustering, k, means);
                centroids[k] = centroid;
            }
        }

        static GeoCoordinate ComputeCentroid(GeoCoordinate[] rawData, int[] clustering, int cluster, GeoCoordinate[] means)
        {
            GeoCoordinate centroid = new GeoCoordinate();
            double minDist = double.MaxValue;
            for (int i = 0; i < rawData.Length; ++i) // walk thru each data tuple
            {
                int c = clustering[i];
                if (c != cluster) continue;
                double currDist = Distance(rawData[i], means[cluster]);
                if (currDist < minDist)
                {
                    minDist = currDist;
                    centroid.Latitude = rawData[i].Latitude;
                    centroid.Longitude = rawData[i].Longitude;
                }
            }
            return centroid;
        }

        static double Distance(GeoCoordinate tuple, GeoCoordinate vector)
        {
            return tuple.GetDistanceTo(vector);
        }

        static void UpdateMeans(GeoCoordinate[] rawData, int[] clustering, GeoCoordinate[] means)
        {
            int numClusters = means.Length;
            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means.Length; ++j)
                {
                    means[j].Latitude = 0.0;
                    means[j].Longitude = 0.0;
                }
            int[] clusterCounts = new int[numClusters];
            double[] lats = new double[means.Count()];
            double[] lngs = new double[means.Count()];
            for (int i = 0; i < means.Count(); i++)
            {
                lats[i] = 0.0;
                lngs[i] = 0.0;
            }
            for (int i = 0; i < rawData.Length; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
                lats[cluster] += rawData[i].Latitude;
                lngs[cluster] += rawData[i].Longitude;
            }
            for (int k = 0; k < means.Length; ++k)
            {
                means[k].Latitude = lats[k] / clusterCounts[k];// danger
                means[k].Longitude = lngs[k] / clusterCounts[k];// danger
            }
            return;
        }

        static GeoCoordinate[] Allocate(int numClusters)
        {
            GeoCoordinate[] result = new GeoCoordinate[numClusters];
            for (int k = 0; k < numClusters; ++k)
                result[k] = new GeoCoordinate();
            return result;
        }

        static int[] InitClustering(int numTuples, int numClusters, int randomSeed)
        {
            Random random = new Random(randomSeed);
            int[] clustering = new int[numTuples];
            for (int i = 0; i < numClusters; ++i)
                clustering[i] = i;
            for (int i = numClusters; i < clustering.Length; ++i)
            {
                var cl = random.Next(0, numClusters);
                if (AvailableToAssign(cl, clustering))
                {
                    if (AvailableToLeave(clustering[i], clustering))
                    {
                        clustering[i] = cl;
                    }
                }
            }
            return clustering;
        }

        static bool AvailableToLeave(int cluster, int[] clustering)
        {
            int count = 0;
            for (int k = 0; k < clustering.Count(); k++)
            {
                if (clustering[k] == cluster)
                {
                    count += 1;
                }
            }
            if (count <= 1)
                return false;
            else
                return true;
        }

        static bool AvailableToAssign(int cluster, int[] clustering)
        {
            int count = 0;
            for (int k = 0; k < clustering.Count(); k++)
            {
                if (clustering[k] == cluster)
                {
                    count += 1;
                }
            }
            if (count >= maxcapacity)
                return false;
            else
                return true;
        }

        static GeoCoordinate Outlier(GeoCoordinate[] rawData, int[] clustering, int numClusters, int cluster)
        {
            GeoCoordinate outlier = new GeoCoordinate();
            double maxDist = 0.0;
            GeoCoordinate[] means = Allocate(numClusters);
            GeoCoordinate[] centroids = Allocate(numClusters);
            UpdateMeans(rawData, clustering, means);
            UpdateCentroids(rawData, clustering, means, centroids);
            for (int i = 0; i < rawData.Length; ++i)
            {
                int c = clustering[i];
                if (c != cluster) continue;
                double dist = Distance(rawData[i], centroids[cluster]);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    outlier = rawData[i];
                }
            }
            return outlier;
        }

        static void ShowMatrix(GeoCoordinate[] matrix, int numRows)
        {
            for (int i = 0; i < numRows; ++i)
            {
                Debug.Write("[" + i.ToString().PadLeft(2) + "]  ");
                Debug.Write(matrix[i].Latitude.ToString() + "  ");
                Debug.Write(matrix[i].Longitude.ToString() + "  ");
                Debug.WriteLine("");
            }
        }

        static void ShowClustering2(GeoCoordinate[] rawData, int numClusters, myresult result, List<myGeo> goes)
        {
            for (int k = 0; k < numClusters; ++k) // Each cluster
            {
                int count = 0;
                for (int i = 0; i < rawData.Length; ++i)
                { // Each tuple                         
                    if (result.Cluster[i] == k)
                    {
                        count++;
                    }
                }
                var geo = goes.Find(x => x.Id == Convert.ToInt32(result.Centroids[k].Speed));
                Debug.Write(k.ToString() + "\t");
                Debug.Write(geo.address.ToString() + "\t");
                Debug.Write(geo.gaddress.ToString() + "\t");
                Debug.Write(result.Centroids[k].Latitude.ToString() + "\t");
                Debug.Write(result.Centroids[k].Longitude.ToString() + "\t");
                Debug.Write(count.ToString() + "\t");
                Debug.WriteLine("");
            }
        }

        static void ShowClustering(GeoCoordinate[] rawData, int numClusters, myresult result, List<myGeo> goes)
        {
            for (int k = 0; k < numClusters; ++k) // Each cluster
            {
                for (int i = 0; i < rawData.Length; ++i) // Each tuple
                    if (result.Cluster[i] == k)
                    {
                        var geo = goes.Find(x => x.Id == Convert.ToInt32(rawData[i].Speed));
                        Debug.Write(rawData[i].Speed.ToString() + "\t");
                        Debug.Write(geo.address.ToString() + "\t");
                        Debug.Write(geo.gaddress.ToString() + "\t");
                        Debug.Write(rawData[i].Latitude.ToString() + "\t");
                        Debug.Write(rawData[i].Longitude.ToString() + "\t");
                        Debug.Write(result.Centroids[k].Latitude.ToString() + "\t");
                        Debug.Write(result.Centroids[k].Longitude.ToString() + "\t");
                        Debug.WriteLine("");
                    }
                Debug.WriteLine("");
            }
        }

        static void StoreBusStop(HSSFWorkbook wb, GeoCoordinate[] rawData, int numClusters, myresult result, List<myGeo> goes)
        {
            // Output
            if (wb == null) return;

            //Create new Excel sheet
            ISheet sheet = null;
            sheet = wb.CreateSheet("BusStop");
            ////(Optional) set the width of the columns
            sheet.SetColumnWidth(0, 10 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 50 * 256);
            sheet.SetColumnWidth(3, 15 * 256);
            sheet.SetColumnWidth(4, 15 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 15 * 256);

            //Create a header row
            var headerRow = sheet.CreateRow(0);
            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue("Itemid");
            headerRow.CreateCell(1).SetCellValue("Addr");
            headerRow.CreateCell(2).SetCellValue("Google Addr");
            headerRow.CreateCell(3).SetCellValue("RawLat");
            headerRow.CreateCell(4).SetCellValue("RawLong");
            headerRow.CreateCell(5).SetCellValue("CentroidsLat");
            headerRow.CreateCell(6).SetCellValue("CentroidsLong");

            int j = 1;
            for (int k = 0; k < numClusters; ++k) // Each cluster
            {
                for (int i = 0; i < rawData.Length; ++i) // Each tuple
                    if (result.Cluster[i] == k)
                    {
                        var row = sheet.CreateRow(j++);
                        var geo = goes.Find(x => x.Id == Convert.ToInt32(rawData[i].Speed));
                        row.CreateCell(0).SetCellValue(rawData[i].Speed.ToString());
                        row.CreateCell(1).SetCellValue(geo.address.ToString());
                        row.CreateCell(2).SetCellValue(geo.gaddress.ToString());
                        row.CreateCell(3).SetCellValue(rawData[i].Latitude.ToString());
                        row.CreateCell(4).SetCellValue(rawData[i].Longitude.ToString());
                        row.CreateCell(5).SetCellValue(result.Centroids[k].Latitude.ToString());
                        row.CreateCell(6).SetCellValue(result.Centroids[k].Longitude.ToString());
                    }
            }

        }

        static void StoreStation(HSSFWorkbook wb, GeoCoordinate[] rawData, int numClusters, myresult result, List<myGeo> goes)
        {
            // Output
            if (wb == null) return;

            //Create new Excel sheet
            ISheet sheet = null;
            sheet = wb.CreateSheet("Station");
            ////(Optional) set the width of the columns
            sheet.SetColumnWidth(0, 10 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 10 * 256);

            //Create a header row
            var headerRow = sheet.CreateRow(0);
            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue("Itemid");
            headerRow.CreateCell(1).SetCellValue("Lat");
            headerRow.CreateCell(2).SetCellValue("Long");
            headerRow.CreateCell(3).SetCellValue("Nb");

            for (int k = 0; k < numClusters; ++k) // Each cluster
            {
                var row = sheet.CreateRow(k + 1);
                int count = 0;
                for (int i = 0; i < rawData.Length; ++i)
                { // Each tuple                         
                    if (result.Cluster[i] == k)
                    {
                        count++;
                    }
                }
                row.CreateCell(0).SetCellValue(k.ToString());
                row.CreateCell(1).SetCellValue(result.Centroids[k].Latitude.ToString());
                row.CreateCell(2).SetCellValue(result.Centroids[k].Longitude.ToString());
                row.CreateCell(3).SetCellValue(count.ToString());
            }
        }
    }
}
