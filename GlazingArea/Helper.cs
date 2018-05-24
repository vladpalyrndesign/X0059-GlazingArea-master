using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qrndGlazingArea
{
    public class Helper
    {
        public static string connectionString;
        SqlConnection con;
        DataTable dt;
        public Helper()
        {
             con = new SqlConnection();
             dt = new DataTable();
            connectionString = ConfigurationManager.ConnectionStrings["SampleDB"].ConnectionString;
            //connectionString = "Data Source=.\\SQLExpress;Initial Catalog=test;Integrated Security=true;Trusted_Connection=True;";
           
           
        }
        public DataTable Read()
        {
            con.ConnectionString = connectionString;
            if (ConnectionState.Closed == con.State)
                con.Open();
            SqlCommand cmd = new SqlCommand("select * from [test].[dbo].[rnGlazingTable]", con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public DataTable Read(string elName)
        {
            con.ConnectionString = connectionString;
            if (ConnectionState.Closed == con.State)
                con.Open();
            SqlCommand cmd = new SqlCommand("select * from [test].[dbo].[rnGlazingTable] where elevation="+ elName +" ", con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public void SaveData(DataTable dt)
        {
            con.ConnectionString = connectionString;
            if (ConnectionState.Closed == con.State)
                con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            try
            {
                    cmd.CommandText = @"INSERT INTO[dbo].[rnGlazingTable](
                                        [Project#],[Model#]
                                        ,[Elevation],[Front]
                                        ,[Rear],[Right]
                                        ,[Left],[TotalGlazingArea])
                                         VALUES (" +
                                      Convert.ToString(dt.Rows[0]["Project#"]) + "," +
                                      Convert.ToString(dt.Rows[0]["Model#"])+","+
                                      Convert.ToString(dt.Rows[0]["Elevation"])+","+
                                      (dt.Rows[0]["Front"]) + "," +
                                      (dt.Rows[0]["Rear"]) + "," +
                                      (dt.Rows[0]["Right"]) + "," +
                                      (dt.Rows[0]["Left"]) + "," +
                                      (dt.Rows[0]["TotalGlazingArea"])+
                                               " )";
                    cmd.ExecuteNonQuery();
                con.Close();
                
            }
            catch
            {
                throw;
            }
        }

    }
}
