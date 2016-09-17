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

namespace GetLatitudeLongitude
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
            String sheetName = "TH KIM DONG";

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
                for (int i = 2; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null) //null is when the row only contains empty cells 
                    {
                        String diaChi = row.GetCell(4).ToString().Trim();
                        if (diaChi != "")
                        {
                            // parse diaChi
                            String[] items = diaChi.Split(',');
                            String quan = "", phuong = "", khupho = "", to = "";
                            foreach (String item in items)
                            {
                                if (item.Trim().Contains("Q. ")) quan = item.Trim().Substring(item.Trim().IndexOf("Q. "));
                                if (item.Trim().Contains("P. ")) phuong = item.Trim().Substring(item.Trim().IndexOf("P. "));
                                if (item.Trim().Contains("KP. ")) khupho = item.Trim().Substring(item.Trim().IndexOf("KP. "));
                                if (item.Trim().Contains("Tổ ")) to = item.Trim().Substring(item.Trim().IndexOf("Tổ "));
                            }
                            row.GetCell(5).SetCellValue(quan);
                            row.GetCell(6).SetCellValue(phuong);
                            row.GetCell(7).SetCellValue(khupho);
                            row.GetCell(8).SetCellValue(to);

                            // geocode diaChi
                            String geocode = await Geocode(diaChi);
                            if (geocode != null)
                            {
                                items = geocode.Split('\n');
                                String lat = items[0];
                                String lng = items[1];
                                String formatted_address = items[2];
                                row.GetCell(9).SetCellValue(lat);
                                row.GetCell(10).SetCellValue(lng);
                                row.GetCell(11).SetCellValue(formatted_address);
                            }
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
        static async Task<String> Geocode(String address)
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
                    JObject geometry = (JObject)top1["geometry"];
                    JObject location = (JObject)geometry["location"];
                    String lat = (String)location["lat"];
                    String lng = (String)location["lng"];
                    returnValue = lat + "\n" + lng + "\n" + formatted_address;
                }
            };
            String uri = String.Format("http://maps.googleapis.com/maps/api/geocode/json?address={0}", address);
            await client.DownloadStringTaskAsync(new Uri(uri));
            return returnValue;
        }
    }
}
