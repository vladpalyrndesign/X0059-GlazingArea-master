using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qrndGlazingArea
{
    public class GBussinessLogic
    {
        public DataTable GetData()
        {
            try
            {
                Helper myHelper = new Helper();
                return myHelper.Read();
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetData(string elevName)
        {
            try
            {
                Helper myHelper = new Helper();
                return myHelper.Read(elevName);
            }
            catch
            {
                throw;
            }
        }
        public void SaveData(DataTable dt)
        {
            try
            {
                Helper myHelper = new Helper();
                myHelper.SaveData(dt);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
