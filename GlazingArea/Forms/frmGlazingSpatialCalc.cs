using GlazingADODataAcessLayer;
using GlazingArea;
using GlazingAreaModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qrndGlazingArea
{



    public partial class frmGlazingArea : Form
    {
        GlazingAreaUtil gArea;
        //double polyLinePeriMeter,height;
        const double oneSqMeter = 10.764;
        const double defaultArea = 0.0;
        string elevationName=string.Empty;
        string sPrjnoFromDwg = string.Empty, sModelnoFromDwg = string.Empty;
        List<string> tblList;

        public Dictionary<string, double> glbGLZWD_Doubles = new Dictionary<string, double>();


        public List<string> GLZWDnames = new List<string>() { "GLZWDAP1", "GLZWDH1", "GLZWDAP2", "GLZWDH2", "GLZWDAP3", "GLZWDH3", "GLZWDAP4", "GLZWDH4", "GLZWDTotalArea" };
        public List<string> GAnames = new List<string>() { "GAFront", "GARear", "GALeft", "GARight", "GASFTotal", "GASMTotal" };
        public Dictionary<string, double> glbGA_Doubles = new Dictionary<string, double>();

        

        public frmGlazingArea()
        { 
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            gArea = new GlazingAreaUtil();
            RefreshMy();
           // int GlazingOffset = 1;
            //bool isChangingMetricTotal = false;
            //bool g_drawFrameAroundWindow = true;
           
            //  getItem();


            //init dictionary for double values
            foreach(string str in GLZWDnames)
            {
                glbGLZWD_Doubles.Add(str, 0);
            }

            foreach (string str in GAnames)
            {
                glbGA_Doubles.Add(str, 0);
            }
        }

        public  void getItem()
        {
            GlazingRepository glazrRepository = new GlazingRepository();
            Glazing myGlazing = new Glazing();
            myGlazing = glazrRepository.Select(sPrjnoFromDwg, sModelnoFromDwg, elevationName);
            txtBoxSPTotalWallArea.Text = Convert.ToString(myGlazing.TotalSpWallAraea);
            txtBoxSPLimitingDistance.Text = Convert.ToString(myGlazing.SptLimitingDistance);
            txtBoxSPAllpowablePct.Text = Convert.ToString(myGlazing.SpAllowablePercrntage);
            txtBoxSPAllowableOpenings.Text = Convert.ToString(myGlazing.SpAllowableOpenings);
            txtBoxSPActualOpenings.Text = Convert.ToString(myGlazing.SpActualOpenings);
            txtGLZWDAP1.Text = Convert.ToString(myGlazing.FirstFloorPeripheral);
            txtGLZWDH1.Text = Convert.ToString(myGlazing.FirstFloorHeight);
            txtGLZWDAP2.Text = Convert.ToString(myGlazing.SecondFloorHeight);
            txtGLZWDAP3.Text = Convert.ToString(myGlazing.ThirdFloorPeripheral);
            txtGLZWDH3.Text = Convert.ToString(myGlazing.ThirdFloorHeight);
            txtGLZWDAP4.Text = Convert.ToString(myGlazing.FourthFloorPeripheral);
            txtGLZWDH4.Text = Convert.ToString(myGlazing.FourthFloorHeight);
            txtGLZWDTotalArea.Text = Convert.ToString(myGlazing.PeriPheralSquareFootArea);
            txtGLZWDTotalSMArea.Text = Convert.ToString(myGlazing.PeriPheralSquareMeterArea);
            txtGAFront.Text = Convert.ToString(myGlazing.FrontArea);
            txtGARear.Text = Convert.ToString(myGlazing.RearArea);
            txtGALeft.Text = Convert.ToString(myGlazing.LeftArea);
            txtGARight.Text = Convert.ToString(myGlazing.RightArea);
            txtGASFTotal.Text = Convert.ToString(myGlazing.TotalGlazingSquareFootArea);
            txtGASMTotal.Text = Convert.ToString(myGlazing.TotalGlazingSquareMeterArea);
        }
        public void RefreshMy()
        {  
            if (!gArea.GetPrjidModelno(out sPrjnoFromDwg, out sModelnoFromDwg))
            {
                MessageBox.Show("Can't get project number from active file.", "RN Page/Layout Manager");
                return;
            }
            this.Text = string.Concat(sPrjnoFromDwg, "-");
            this.Text = string.Concat(this.Text, sModelnoFromDwg);
            comboBoxElevation.DataSource = gArea.GetElevation();
        }
       


        //private void fetchButton_Click(object sender, EventArgs e)
        //{
        //    //GlazingRepository glazrRepository = new GlazingRepository();
        //    //Glazing myGlazing = new Glazing();
        //    //myGlazing = glazrRepository.Select(sPrjnoFromDwg, sModelnoFromDwg, elevationName);
        //    //txtBoxSPTotalWallArea.Text = Convert.ToString(Math.Round(myGlazing.TotalSpWallAraea, 3));
        //    //txtBoxSPLimitingDistance.Text = Convert.ToString(Math.Round(myGlazing.SptLimitingDistance, 3));
        //    //txtBoxSPAllpowablePct.Text = Convert.ToString(Math.Round(myGlazing.SpAllowablePercrntage, 3));
        //    //txtBoxSPActualOpenings.Text = Convert.ToString(Math.Round(myGlazing.SpActualOpenings, 3));
        //    //txtBoxGZFstFloorPeriPheral.Text = Convert.ToString(Math.Round(myGlazing.FirstFloorPeripheral, 3));
        //    //txtBoxGzFstFloorHeight.Text = Convert.ToString(Math.Round(myGlazing.FirstFloorHeight, 3));
        //    //txtBoxGzWDSecFloorPeripheral.Text = Convert.ToString(Math.Round(myGlazing.SecondFloorHeight, 3));
        //    //txtBoxGzThrdFloorPeripheral.Text = Convert.ToString(Math.Round(myGlazing.ThirdFloorPeripheral, 3));
        //    //txtBoxGzThrdFloorHeight.Text = Convert.ToString(Math.Round(myGlazing.ThirdFloorHeight, 3));
        //    //txtBoxForthFloorPeripheral.Text = Convert.ToString(Math.Round(myGlazing.FourthFloorPeripheral, 3));
        //    //txtBoxGzforthFloorHeight.Text = Convert.ToString(Math.Round(myGlazing.FourthFloorHeight, 3));
        //    //txtBoxGzTotalSFArea.Text = Convert.ToString(Math.Round(myGlazing.PeriPheralSquareFootArea, 3));
        //    //txtBoxGzTotalSMArea.Text = Convert.ToString(Math.Round(myGlazing.PeriPheralSquareMeterArea, 3));
        //    //txtBoxFront.Text = Convert.ToString(Math.Round(myGlazing.FrontArea, 3));
        //    //textBoxRear.Text = Convert.ToString(Math.Round(myGlazing.RearArea, 3));
        //    //txtBoxLeft.Text = Convert.ToString(Math.Round(myGlazing.LeftArea, 3));
        //    //txtBoxRight.Text = Convert.ToString(Math.Round(myGlazing.RightArea, 3));
        //    //txtBoxSFTotal.Text = Convert.ToString(Math.Round(myGlazing.TotalGlazingSquareFootArea, 3));
        //    //txtBoxSMTotal.Text = Convert.ToString(Math.Round(myGlazing.TotalGlazingSquareMeterArea, 3));
        //}





        private void btndrawTable_Click(object sender, EventArgs e)
        {
            if (tblList != null)
            {
                JPGlazingAreaUtill.CreateMyTable(tblList);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Table is Empty!!");
            }

        }

        //
        //public List<string> GAnames = new List<string>() { "GAFront", "GARear", "GALeft", "GARight", "GASFTotal", "GASMTotal" };
        //public Dictionary<string, double> glbGA_Doubles = new Dictionary<string, double>();

        #region Glazzing Area >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        #region Front
        private void btnGAFrontSelect_Click(object sender, EventArgs e)
        {//select
            glbGA_Doubles["GAFront"]= JPGlazingAreaUtill.GetGlazingArea(true);            
            txtGAFront.Text = Math.Round(glbGA_Doubles["GAFront"], 2).ToString();
        }
        private void btnGAFrontPlus_Click(object sender, EventArgs e)
        {//plus   
            glbGA_Doubles["GAFront"] +=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGAFront.Text = Math.Round(glbGA_Doubles["GAFront"], 2).ToString();
        }
        private void btnGAFrontMinus_Click(object sender, EventArgs e)
        {//minus
            glbGA_Doubles["GAFront"] -=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGAFront.Text = Math.Round(glbGA_Doubles["GAFront"], 2).ToString();
            if (glbGA_Doubles["GAFront"] < 0)
            {
                System.Windows.Forms.MessageBox.Show("Please pay attenation: Area is negative");
            }
        }
        #endregion Front

        #region Rear
        private void btnGARearSelect_Click(object sender, EventArgs e)
        {//select
            glbGA_Doubles["GARear"] =JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGARear.Text = Math.Round(glbGA_Doubles["GARear"], 2).ToString();
        }

        private void bbtnGARearPlus_Click(object sender, EventArgs e)
        {//plus   
            glbGA_Doubles["GARear"] +=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGARear.Text = Math.Round(glbGA_Doubles["GARear"], 2).ToString();
        }

        private void btnGARearMinus_Click(object sender, EventArgs e)
        {//minus
            glbGA_Doubles["GARear"] -=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGARear.Text = Math.Round(glbGA_Doubles["GARear"], 2).ToString();
            if (glbGA_Doubles["GARear"] < 0)
            {
                System.Windows.Forms.MessageBox.Show("Please pay attenation: Area is negative");
            }
        }
        #endregion Rear

        #region Left
        private void btnGALeftSelect_Click(object sender, EventArgs e)
        {//select
            glbGA_Doubles["GALeft"] =JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGALeft.Text = Math.Round(glbGA_Doubles["GALeft"], 2).ToString();
        }
        private void btnGALeftPlus_Click(object sender, EventArgs e)
        {//plus   
            glbGA_Doubles["GALeft"] +=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGALeft.Text = Math.Round(glbGA_Doubles["GALeft"], 2).ToString();
        }
        private void btnGALeftMinus_Click(object sender, EventArgs e)
        {//minus
            glbGA_Doubles["GALeft"] -=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGALeft.Text = Math.Round(glbGA_Doubles["GALeft"], 2).ToString();
            if (glbGA_Doubles["GALeft"] < 0)
            {
                System.Windows.Forms.MessageBox.Show("Please pay attenation: Area is negative");
            }
        }

        #endregion Left

        #region Right
        private void btnGARightSelect_Click(object sender, EventArgs e)
        {
            glbGA_Doubles["GARight"] =JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGARight.Text = Math.Round(glbGA_Doubles["GARight"], 2).ToString();
        }
        private void btnGARightPlus_Click(object sender, EventArgs e)
        {
            glbGA_Doubles["GARight"] +=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGARight.Text = Math.Round(glbGA_Doubles["GARight"], 2).ToString();

        }
        private void btnGARightMinus_Click(object sender, EventArgs e)
        {
            glbGA_Doubles["GARight"] -=JPGlazingAreaUtill.GetGlazingArea(true);;
            txtGARight.Text = Math.Round(glbGA_Doubles["GARight"], 2).ToString();
            if (glbGA_Doubles["GARight"] < 0)
            {
                System.Windows.Forms.MessageBox.Show("Please pay attenation: Area is negative");
            }
        }
        #endregion Right

        private void UpdateGAArea()
        {
            glbGA_Doubles["GASFTotal"] = 0;
            foreach (string key in new List<string>() { GAnames[0], GAnames[1], GAnames[2], GAnames[3] })
            {
                glbGA_Doubles["GASFTotal"]  += glbGA_Doubles[key];
            }
            glbGA_Doubles["GASMTotal"] = glbGA_Doubles["GASFTotal"] / oneSqMeter;
            txtGASFTotal.Text = (Math.Round( glbGA_Doubles["GASFTotal"],2)).ToString();
            txtGASMTotal.Text = (Math.Round(glbGA_Doubles["GASMTotal"],2)).ToString();
        }
        private void txtGAFront_TextChanged(object sender, EventArgs e)
        {
            UpdateGAArea();
        }
        private void txtGARear_TextChanged(object sender, EventArgs e)
        {
            UpdateGAArea();
        }
        private void txtGALeft_TextChanged(object sender, EventArgs e)
        {
            UpdateGAArea();
        }
        private void txtGASFTotal_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtGARight_TextChanged(object sender, EventArgs e)
        {
            UpdateGAArea();
        }
        #endregion Glazzing Area >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>



        #region glazing Total Peripheral Wall Area >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        // 1 floor area
        private void btnGLZWDAP1_Click(object sender, EventArgs e)
        { 
            glbGLZWD_Doubles["GLZWDAP1"] = JPGlazingAreaUtill.SelectObjectPolyLine(JPGlazingAreaUtill.CalcType.Glazing);           
            txtGLZWDAP1.Text =Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDAP1"], 2));
        }
        // 1 floor height
        private void btnGLZWDH1_Click(object sender, EventArgs e)
        {   
            glbGLZWD_Doubles["GLZWDH1"] = JPGlazingAreaUtill.SelectOneRotatedDimension();
            txtGLZWDH1.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDH1"], 2));
        }
        // 2 floor area
        private void btnGzScndFlrPhSelect_Click(object sender, EventArgs e)
        {
            glbGLZWD_Doubles["GLZWDAP2"] = JPGlazingAreaUtill.SelectObjectPolyLine(JPGlazingAreaUtill.CalcType.Glazing); 
            txtGLZWDAP2.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDAP2"], 2));
        }
        // 2 floor height
        private void bbtnGLZWDH2_Click(object sender, EventArgs e)
        {  
            glbGLZWD_Doubles["GLZWDH2"] = JPGlazingAreaUtill.SelectOneRotatedDimension();
            txtGLZWDH2.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDH2"], 2));
        }
        // 3 floor area
        private void btnGLZWDAP3_Click(object sender, EventArgs e)
        {
            glbGLZWD_Doubles["GLZWDAP3"] = JPGlazingAreaUtill.SelectObjectPolyLine(JPGlazingAreaUtill.CalcType.Glazing); 
            txtGLZWDAP3.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDAP3"], 2));
        }
        // 3 floor height
        private void btnGLZWDH3_Click(object sender, EventArgs e)
        {
            glbGLZWD_Doubles["GLZWDH3"] = JPGlazingAreaUtill.SelectOneRotatedDimension(); ;
            txtGLZWDH3.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDH3"], 2));
        }
        // 4 floor area
        private void btnGLZWDAP4_Click(object sender, EventArgs e)
        {   
            glbGLZWD_Doubles["GLZWDAP4"] = JPGlazingAreaUtill.SelectObjectPolyLine(JPGlazingAreaUtill.CalcType.Glazing); 
            txtGLZWDAP4.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDAP4"], 2));
        }
        // 4 floor height
        private void btnForthFloorHeightSelect_Click(object sender, EventArgs e)
        {   
            glbGLZWD_Doubles["GLZWDH4"] = JPGlazingAreaUtill.SelectOneRotatedDimension();
            txtGLZWDH4.Text = Convert.ToString(Math.Round(glbGLZWD_Doubles["GLZWDH4"], 2));
        }
        private void btnGzCalTotalArea_Click(object sender, EventArgs e)
        {
            this.GLZWDCalculateSummary();
        }
        private void GLZWDCalculateSummary()
        {
            double dTotalArea =
              glbGLZWD_Doubles["GLZWDAP1"] * glbGLZWD_Doubles["GLZWDH1"] +
              glbGLZWD_Doubles["GLZWDAP2"] * glbGLZWD_Doubles["GLZWDH2"] +
              glbGLZWD_Doubles["GLZWDAP3"] * glbGLZWD_Doubles["GLZWDH3"] +
              glbGLZWD_Doubles["GLZWDAP4"] * glbGLZWD_Doubles["GLZWDH4"];
            txtGLZWDTotalArea.Text = Convert.ToString(Math.Round(dTotalArea, 2));
            txtGLZWDTotalSMArea.Text = Convert.ToString(Math.Round((dTotalArea / oneSqMeter), 2));
        }



 
        private void txtGLZWDAP1_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }

        private void txtGLZWDH1_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }

        private void txtGLZWDAP2_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }

        private void txtGLZWDH2_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }

        private void txtGLZWDAP3_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }

        private void txtGLZWDH3_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }



        private void txtGLZWDH4_TextChanged(object sender, EventArgs e)
        {
            GLZWDCalculateSummary();
        }


        #endregion glazing Total Peripheral Wall Area >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


        private void txtBoxSPAllowableOpenings_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSPWallAreaSelect_Click(object sender, EventArgs e)
        {
            double polyLinePeriMeter = JPGlazingAreaUtill.SelectObjectPolyLine(JPGlazingAreaUtill.CalcType.Spatial);
            polyLinePeriMeter = Math.Round(polyLinePeriMeter, 2);
            txtBoxSPTotalWallArea.Text = Convert.ToString(polyLinePeriMeter);
        }

        private void btnSPActualOpSelect_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = false;
           // double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.SelectBlockSpatialFromTheScreen(g_drawFrameAroundWindow), 2);
             double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxSPActualOpenings.Text = Convert.ToString(totalSquareFootArea);
            tblList = new List<string>();
            tblList.Add(txtBoxSPTotalWallArea.Text);
            tblList.Add(Convert.ToString(Math.Round((Convert.ToDouble(txtBoxSPTotalWallArea.Text) * 0.09290304),2)));
            tblList.Add(txtBoxSPLimitingDistance.Text);
            tblList.Add(Convert.ToString(Math.Round((Convert.ToDouble(txtBoxSPLimitingDistance.Text)* 0.3048),1)));
            tblList.Add(txtBoxSPAllpowablePct.Text);
            tblList.Add(txtBoxSPAllowableOpenings.Text);
            tblList.Add(Convert.ToString(Math.Round((Convert.ToDouble(txtBoxSPAllowableOpenings.Text) * 0.09290304),2)));
            tblList.Add(txtBoxSPActualOpenings.Text);
            tblList.Add(Convert.ToString(Math.Round((Convert.ToDouble(txtBoxSPActualOpenings.Text) * 0.09290304),2)));
            //my method
        }

  







   

        private void btnSave_Click(object sender, EventArgs e)
        {
            Glazing glaze = new Glazing();
            glaze.ProjectNumber = sPrjnoFromDwg;
            glaze.ModelNumber = sModelnoFromDwg;
            glaze.Elevation = elevationName;
            glaze.TotalSpWallAraea = getDoubleValue(txtBoxSPTotalWallArea.Text);
            glaze.SptLimitingDistance = getDoubleValue(txtBoxSPLimitingDistance.Text);
            glaze.SpAllowablePercrntage = getDoubleValue(txtBoxSPAllpowablePct.Text);
            glaze.SpAllowableOpenings = getDoubleValue(txtBoxSPAllowableOpenings.Text);
            glaze.SpActualOpenings = getDoubleValue(txtBoxSPActualOpenings.Text);
            glaze.FirstFloorPeripheral= getDoubleValue(txtGLZWDAP1.Text );
            glaze.FirstFloorHeight= getDoubleValue(txtGLZWDH1.Text);
            glaze.SecondFloorPeripheral= getDoubleValue(txtGLZWDAP2.Text);
            glaze.SecondFloorHeight= getDoubleValue(txtGLZWDH2.Text);
            glaze.ThirdFloorPeripheral= getDoubleValue(txtGLZWDAP3.Text);
            glaze.ThirdFloorHeight= getDoubleValue(txtGLZWDH3.Text );
            glaze.FourthFloorPeripheral= getDoubleValue(txtGLZWDAP4.Text);
            glaze.FourthFloorHeight= getDoubleValue(txtGLZWDH4.Text);
            glaze.PeriPheralSquareFootArea= getDoubleValue(txtGLZWDTotalArea.Text);
            glaze.PeriPheralSquareMeterArea= getDoubleValue(txtGLZWDTotalSMArea.Text );
            glaze.FrontArea= getDoubleValue(txtGAFront.Text);
            glaze.RearArea= getDoubleValue(txtGARear.Text);
            glaze.LeftArea= getDoubleValue(txtGARear.Text);
            glaze.RightArea= getDoubleValue(txtGARight.Text);
            glaze.TotalGlazingSquareFootArea= getDoubleValue(txtGASFTotal.Text);
            glaze.TotalGlazingSquareMeterArea= getDoubleValue(txtGASMTotal.Text);
            //gz.TotalGlazingPercentage= Convert.ToDouble();todo
            //result = (gridCol1 ) ? Convert.ToDouble(gridCol1) : defaultArea
            GlazingRepository glazrRepository = new GlazingRepository();
            glazrRepository.Add(glaze);
            System.Windows.Forms.MessageBox.Show("Record is added successfully");
            
        }

        private double getDoubleValue(string text)
        {
            if (String.IsNullOrEmpty(text))
                return defaultArea;
            double result = 0;
            Double.TryParse(text, out result);

            if (result == 0)
                return defaultArea;
            else
                return result;
        }

        private void comboBoxElevation_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            elevationName = comboBoxElevation.Items[comboBoxElevation.SelectedIndex].ToString().Trim();
          
        }



        private void comboBoxElevation_SelectedIndexChanged(object sender, EventArgs e)
        {
            elevationName = comboBoxElevation.Items[comboBoxElevation.SelectedIndex].ToString().Trim();
        }

       



        //private void button1_Click(object sender, EventArgs e)
        //{
        //    gArea.drawPolylinebyDefinition();
        //}

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void txtBoxSPLimitingDistance_TextChanged(object sender, EventArgs e)
        {
            txtBoxSPAllpowablePct.Text = Convert.ToString(JPGlazingAreaUtill.GetAllowedGlazingPercentage(Convert.ToDouble(txtBoxSPTotalWallArea.Text), Convert.ToDouble(txtBoxSPLimitingDistance.Text)));
            //txtAllowablePctL.Text = GetAllowedGlazingPercentage(Val(txtTotalWallAreaL.Text), Val(txtLimitingDistL.Text)
        }

        private void txtBoxSPTotalWallArea_TextChanged(object sender, EventArgs e)
        {
            txtBoxSPAllpowablePct.Text =Convert.ToString(JPGlazingAreaUtill.GetAllowedGlazingPercentage(Convert.ToDouble(txtBoxSPTotalWallArea.Text), Convert.ToDouble(txtBoxSPLimitingDistance.Text)));
            double dTmp = 0;
            dTmp = Math.Round(Convert.ToDouble(txtBoxSPTotalWallArea.Text) * Convert.ToDouble(txtBoxSPAllpowablePct.Text) / 100,2);
            txtBoxSPAllowableOpenings.Text = Convert.ToString(dTmp);
        }

        private void txtBoxSPActualOpenings_TextChanged(object sender, EventArgs e)
        {

        }

     

        private void txtBoxSPAllpowablePct_TextChanged(object sender, EventArgs e)
        {
            double dTmp = 0;
            dTmp = Math.Round(Convert.ToDouble(txtBoxSPTotalWallArea.Text) * Convert.ToDouble(txtBoxSPAllpowablePct.Text) / 100,2);
            txtBoxSPAllowableOpenings.Text = Convert.ToString(dTmp);
        }



    }
}





