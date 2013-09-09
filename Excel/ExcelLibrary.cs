using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Web;

namespace Excel
{
    public class ExcelLibrary
    {
        public static List<Array> GetExcelData(string fileName, string sheetName, int columnCount)
        {
            var returnObject = new List<Array>();
            try
            {
                var oconn = new OleDbConnection (@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + HttpContext.Current.Server.MapPath(fileName) + ";Extended Properties=Excel 8.0");
                //After connecting to the Excel sheet here we are selecting the data 
                //using select statement from the Excel sheet
                //string selectString = "Select * from $Sheet1";
                var ocmd = new OleDbCommand(string.Format("Select * from [{0}$]", sheetName), oconn);
                oconn.Open();
                OleDbDataReader odr = ocmd.ExecuteReader();
                while (odr.Read())
                {
                    var strObjects = new List<string>();
                    for (int i = 0; i < columnCount; i++)
                    {
                        string val = odr.GetValue(i).ToString();
                        strObjects.Add(valid(odr, i));
                    }

                    returnObject.Add(strObjects.ToArray());
                    //Here using this method we are inserting the data into the database
                }
                oconn.Close();
            }
            catch
            {
            }
            return returnObject;
        }
        protected static string valid(OleDbDataReader myreader, int stval)//if any columns are 
        //found null then they are replaced by zero
        {
            var val = myreader[stval];
            return val != DBNull.Value ? val.ToString() : Convert.ToString(0);
        }
        //public static void ExportToSpreadsheet( table, string name)
        //{
        //    HttpContext context = HttpContext.Current;
        //    context.Response.Clear();

        //    foreach (DataColumn column in table.Columns)
        //    {
        //        context.Response.Write(column.ColumnName + ";");
        //    }

        //    context.Response.Write(Environment.NewLine);

        //    foreach (DataRow row in table.Rows)
        //    {
        //        for (int i = 0; i < table.Columns.Count; i++)
        //        {
        //            context.Response.Write(row[i].ToString().Replace(";", string.Empty) + ";");
        //        }
        //        context.Response.Write(Environment.NewLine);
        //    }

        //    context.Response.ContentType = "text/csv";
        //    context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + name + ".csv");
        //    context.Response.End();
        //}

    }
}
