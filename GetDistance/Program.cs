using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetDistance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PROCESSING ...");
            Task.Run(async () =>
            {
                HSSFWorkbook hssfworkbook = await ReadExcel();
                SaveExcel(hssfworkbook);
                Console.WriteLine("OK BABY !!!");
            }).Wait();
        }

        static async Task<HSSFWorkbook> ReadExcel()
        {
            String filePath = "../../data/data.xls";
            String sheetName = "Sheet1";

            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);

            String fileExt = System.IO.Path.GetExtension(filePath);
            ISheet sheet = null;
            if (fileExt == ".xls")
                sheet = hssfworkbook.GetSheet(sheetName);
            else if (fileExt == ".xlsx")
                sheet = hssfworkbook.GetSheet(sheetName);
            if (sheet != null)
            {
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null) //null is when the row only contains empty cells 
                    {
                        // read from excel
                        String lat1 = row.GetCell(0).ToString().Trim();
                        String lng1 = row.GetCell(1).ToString().Trim();
                        String lat2 = row.GetCell(3).ToString().Trim();
                        String lng2 = row.GetCell(4).ToString().Trim();

                        // Reverse geocoding
                        String address1 = await ReverseGeocode(lat1, lng1);
                        String address2 = await ReverseGeocode(lat2, lng2);

                        // Get distance
                        String distanceDuration = await GetDistanceDuration(address1, address2);

                        // write to excel
                        if (distanceDuration != null)
                        {
                            String[] items = distanceDuration.Split('\n');
                            ICell cell = row.GetCell(6);
                            if (cell == null) cell = row.CreateCell(6);
                            cell.SetCellValue(items[0]);
                            cell = row.GetCell(7);
                            if (cell == null) cell = row.CreateCell(7);
                            cell.SetCellValue(items[1]);
                        }
                    }
                }
                file.Close();
            }
            return hssfworkbook;
        }

        static void SaveExcel(HSSFWorkbook hssfworkbook)
        {
            String filePath_output = "../../data/data_output.xls";
            FileStream fs = new FileStream(filePath_output, FileMode.OpenOrCreate);
            hssfworkbook.Write(fs);
            fs.Close();
        }

        /* ref: https://developers.google.com/maps/documentation/geocoding/intro */
        static async Task<String> ReverseGeocode(String lat, String lng)
        {
            String returnValue = null;
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject jsResult = JObject.Parse(json);
                String status = (String)jsResult["status"];
                if ("OK".Equals(status))
                {
                    JArray results = (JArray)jsResult["results"];
                    JObject top1 = (JObject)results[0];
                    String formatted_address = (String)top1["formatted_address"];
                    formatted_address = Encoding.UTF8.GetString(Encoding.Default.GetBytes(formatted_address)); // convert to UTF8
                    returnValue = formatted_address;
                }
            };
            String uri = String.Format("http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}", lat, lng);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }

        /* ref: https://developers.google.com/maps/documentation/distance-matrix/start */
        static async Task<String> GetDistanceDuration(String from, String to)
        {
            String returnValue = null;
            WebClient client = new WebClient();
            client.Headers["Accept"] = "application/json";
            client.DownloadStringCompleted += (s1, e1) =>
            {
                String json = e1.Result.ToString();
                JObject result = JObject.Parse(json);
                String status = (String)result["status"];
                if ("OK".Equals(status))
                {
                    JArray rows = (JArray)result["rows"];
                    JObject row = (JObject)rows[0];
                    JArray elements = (JArray)row["elements"];
                    JObject element = (JObject)elements[0];
                    String status2 = (String)element["status"];
                    if ("OK".Equals(status2))
                    {
                        JObject distance = (JObject)element["distance"];
                        long dist = (long)distance["value"];
                        JObject duration = (JObject)element["duration"];
                        long dur = (long)duration["value"];
                        returnValue = dist + "\n" + dur;
                    }
                }
            };
            String uri = String.Format("http://maps.googleapis.com/maps/api/distancematrix/json?origins={0}&destinations={1}", from, to);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }
    }
}