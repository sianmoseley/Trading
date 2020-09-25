using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.OleDb;
using TradingPlatform1.Models;
using LINQtoCSV;


namespace TradingPlatform1.Controllers
{
    public class ImportController : Controller
    {
        TradeContext db = new TradeContext();

        // GET: Import
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {

            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        ViewBag.Message = "Please select the excel file with .xls or .xlsx extension";
                        return View();
                    }

                    string folderPath = Server.MapPath("~/UploadedFiles/");
                    //Check Directory exists else create one
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    //Save file to folder
                    var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Get file extension

                    string excelConString = "";

                    //Get connection string using extension 
                    switch (fileExtension)
                    {
                        //If uploaded file is Excel 1997-2007.
                        case ".xls":
                            excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                            break;
                        //If uploaded file is Excel 2007 and above
                        case ".xlsx":
                            excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                            break;
                    }

                    //Read data from first sheet of excel into datatable
                    DataTable dt = new DataTable();
                    excelConString = string.Format(excelConString, filePath);

                    using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                    {
                        using (OleDbCommand excelDbCommand = new OleDbCommand())
                        {
                            using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                            {
                                excelDbCommand.Connection = excelOledbConnection;

                                excelOledbConnection.Open();
                                //Get schema from excel sheet
                                DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                //Get sheet name
                                string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                excelOledbConnection.Close();

                                //Read Data from First Sheet.
                                excelOledbConnection.Open();
                                excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                excelDataAdapter.SelectCommand = excelDbCommand;
                                //Fill datatable from adapter
                                excelDataAdapter.Fill(dt);
                                excelOledbConnection.Close();
                            }
                        }
                    }

                    //Insert records to Trade table.
                    using (var context = new TradeContext())
                    {
                        //Loop through datatable and add trade data to trade table. 
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            context.Trades.Add(GetTradeFromExcelRow(row));
                        }
                        context.SaveChanges();
                    }
                    ViewBag.Message = "Data Imported Successfully.";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }

        private static DataTable GetSchemaFromExcel(OleDbConnection excelOledbConnection)
        {
            return excelOledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        }

        //Convert each datarow into trade object
        private Trade GetTradeFromExcelRow(System.Data.DataRow row)
        {
            return new Trade
            {
                ClientID = (int)Session["clientID"],
                TradeType = row[1].ToString(),
                Instrument = row[2].ToString(),
                Volume = int.Parse(row[3].ToString()),
                Price = double.Parse(row[4].ToString()),
            };
        }


        //////////UPLOAD CSV FILE CODE - FOR THIS TO WORK, I HAD TO EDIT THE CSV 
        //////////FILE TO HAVE ALL THE SAME HEADINGS AS TRADE 

        //[HttpPost]
        //public ActionResult UploadCSV(HttpPostedFileBase postedFile)
        //{

        //    if (postedFile != null)
        //    {
        //        try
        //        {
        //            CsvFileDescription csvFileDescription = new CsvFileDescription
        //            {
        //                SeparatorChar = ',',
        //                FirstLineHasColumnNames = true
        //            };
        //            CsvContext csvContext = new CsvContext();
        //            StreamReader streamReader = new StreamReader(postedFile.InputStream);
        //            IEnumerable<Trade> list = csvContext.Read<Trade>(streamReader, csvFileDescription);
        //            db.Trades.AddRange(list);
        //            db.SaveChanges();
        //            ViewBag.Message = "Data Imported Successfully.";

        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Please select the file first to upload.";
        //    }
        //    return View();
        //}


    }
}