using GlazingAreaModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlazingADODataAcessLayer
{
    public class GlazingRepository
    {
        private readonly string _connStr;
        public GlazingRepository()
        {
            // _connStr = "Data Source=.\\SQLExpress;Initial Catalog=RN_GLAZING_DB;Integrated Security=true;Trusted_Connection=True; ";
            _connStr = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        }
        public void Add(Glazing items)
        {

            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("usp_InsertAreaByProjectNoModelNoAndElevation", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProjectNo", items.ProjectNumber);
                command.Parameters.AddWithValue("@ModelNo", items.ModelNumber);
                command.Parameters.AddWithValue("@Elevation", items.Elevation);
                command.Parameters.AddWithValue("@TotalSpatialWallArea", items.TotalSpWallAraea);
                command.Parameters.AddWithValue("@SpLimitingDistance", items.SptLimitingDistance);
                command.Parameters.AddWithValue("@SpAllowablePercentage", items.SpAllowablePercrntage);
                command.Parameters.AddWithValue("@SpAllowableOpening", items.SpAllowableOpenings);
                command.Parameters.AddWithValue("@SpActualOpening", items.SpActualOpenings);
                command.Parameters.AddWithValue("@FirstFloorPeripheral", items.FirstFloorPeripheral);
                command.Parameters.AddWithValue("@FirstFloorHeight", items.FirstFloorHeight);
                command.Parameters.AddWithValue("@SecondFloorPeripheral", items.SecondFloorPeripheral);
                command.Parameters.AddWithValue("@SecondFloorHeight", items.SecondFloorHeight);
                command.Parameters.AddWithValue("@ThirdFloorPeripheral", items.ThirdFloorPeripheral);
                command.Parameters.AddWithValue("@ThirdFloorHeight", items.ThirdFloorHeight);
                command.Parameters.AddWithValue("@FourhtFloorPeripheral", items.FourthFloorPeripheral);
                command.Parameters.AddWithValue("@FourhtFloorHeight", items.FourthFloorHeight);
                command.Parameters.AddWithValue("@PeriPheralSquareFootArea", items.PeriPheralSquareFootArea);
                command.Parameters.AddWithValue("@PeriPheralSquareMeterArea", items.PeriPheralSquareMeterArea);
                command.Parameters.AddWithValue("@FrontArea", items.FrontArea);
                command.Parameters.AddWithValue("@RearArea", items.RearArea);
                command.Parameters.AddWithValue("@LeftArea", items.LeftArea);
                command.Parameters.AddWithValue("@RightArea", items.RightArea);
                command.Parameters.AddWithValue("@TotalGlazingSquareFootArea", items.TotalGlazingSquareFootArea);
                command.Parameters.AddWithValue("@TotalGlazingSquareMeterArea", items.TotalGlazingSquareMeterArea);
                command.Parameters.AddWithValue("@TotalGlazingPercentage", items.TotalGlazingPercentage);
                try
                {
                   // connection.Open();
                    command.ExecuteNonQuery();
                    
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }

               
            }



        }
        public Glazing Select(string ProjectNumber,string ModelNumber,string Elevation)
        {
            Glazing item = new Glazing();
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                SqlCommand command = new SqlCommand("usp_SelectAreaByProjectNoModelNoAndElevation", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProjectNo", ProjectNumber);
                command.Parameters.AddWithValue("@ModelNo", ModelNumber);
                command.Parameters.AddWithValue("@Elevation", Elevation);
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        
                        item.ProjectNumber =Convert.ToString( dataReader["ProjectNumber"]);
                        item.ModelNumber = Convert.ToString(dataReader["ModelNumber"]);
                        item.Elevation = Convert.ToString(dataReader["Elevation"]);
                        item.TotalSpWallAraea= Math.Round(Convert.ToDouble(dataReader["TotalSpatialWallArea"]),4);
                        item.SptLimitingDistance = Math.Round(Convert.ToDouble(dataReader["SptLimitingDistance"]),4);
                        item.SpAllowablePercrntage= Math.Round(Convert.ToDouble(dataReader["SpAllowablePercentage"]),4);
                        item.SpAllowableOpenings = Math.Round(Convert.ToDouble(dataReader["SpAllowableOpening"]),4);
                        item.SpActualOpenings = Math.Round(Convert.ToDouble(dataReader["SpActualOpening"]),4);
                        item.FirstFloorPeripheral = Math.Round(Convert.ToDouble(dataReader["FirstFloorPeripheral"]),4);
                        item.FirstFloorHeight = Math.Round(Convert.ToDouble(dataReader["FirstFloorHeight"]),4);
                        item.SecondFloorPeripheral = Math.Round(Convert.ToDouble(dataReader["SecondFloorPeripheral"]),4);
                        item.SecondFloorHeight = Math.Round(Convert.ToDouble(dataReader["SecondFloorHeight"]),4);
                        item.ThirdFloorPeripheral = Math.Round(Convert.ToDouble(dataReader["ThirdFloorPeripheral"]),4);
                        item.ThirdFloorHeight = Math.Round(Convert.ToDouble(dataReader["ThirdFloorHeight"]),4);
                        item.FourthFloorPeripheral = Math.Round(Convert.ToDouble(dataReader["FourhtFloorPeripheral"]),4);
                        item.FourthFloorHeight = Math.Round(Convert.ToDouble(dataReader["FourhtFloorHeight"]),4);
                        item.PeriPheralSquareFootArea = Math.Round(Convert.ToDouble(dataReader["PeriPheralSquareFootArea"]),4);
                        item.PeriPheralSquareMeterArea = Math.Round(Convert.ToDouble(dataReader["PeriPheralSquareMeterArea"]),4);
                        item.FrontArea = Math.Round(Convert.ToDouble(dataReader["FrontArea"]),4);
                        item.RearArea = Math.Round(Convert.ToDouble(dataReader["RearArea"]),4);
                        item.LeftArea = Math.Round(Convert.ToDouble(dataReader["LeftArea"]),4);
                        item.RightArea = Math.Round(Convert.ToDouble(dataReader["RightArea"]),4);
                        item.TotalGlazingSquareFootArea= Math.Round(Convert.ToDouble(dataReader["TotalGlazingSquareFootArea"]),4);
                        item.TotalGlazingSquareMeterArea = Math.Round(Convert.ToDouble(dataReader["TotalGlazingSquareMeterArea"]),4);
                        item.TotalGlazingPercentage = Math.Round(Convert.ToDouble(dataReader["TotalGlazingPercentage"]),4);
                    }
                }
            }
            return item;
        }



        

        
    }
}
