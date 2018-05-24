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
        double polyLinePeriMeter,height;
        const double oneSqMeter = 10.764;
        const double defaultArea = 0.0;
        string elevationName=string.Empty;
        string sPrjnoFromDwg = string.Empty, sModelnoFromDwg = string.Empty;
        List<string> tblList;


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
            txtBoxGZFstFloorPeriPheral.Text = Convert.ToString(myGlazing.FirstFloorPeripheral);
            txtBoxGzFstFloorHeight.Text = Convert.ToString(myGlazing.FirstFloorHeight);
            txtBoxGzWDSecFloorPeripheral.Text = Convert.ToString(myGlazing.SecondFloorHeight);
            txtBoxGzThrdFloorPeripheral.Text = Convert.ToString(myGlazing.ThirdFloorPeripheral);
            txtBoxGzThrdFloorHeight.Text = Convert.ToString(myGlazing.ThirdFloorHeight);
            txtBoxForthFloorPeripheral.Text = Convert.ToString(myGlazing.FourthFloorPeripheral);
            txtBoxGzforthFloorHeight.Text = Convert.ToString(myGlazing.FourthFloorHeight);
            txtBoxGzTotalSFArea.Text = Convert.ToString(myGlazing.PeriPheralSquareFootArea);
            txtBoxGzTotalSMArea.Text = Convert.ToString(myGlazing.PeriPheralSquareMeterArea);
            txtBoxFront.Text = Convert.ToString(myGlazing.FrontArea);
            textBoxRear.Text = Convert.ToString(myGlazing.RearArea);
            txtBoxLeft.Text = Convert.ToString(myGlazing.LeftArea);
            txtBoxRight.Text = Convert.ToString(myGlazing.RightArea);
            txtBoxSFTotal.Text = Convert.ToString(myGlazing.TotalGlazingSquareFootArea);
            txtBoxSMTotal.Text = Convert.ToString(myGlazing.TotalGlazingSquareMeterArea);
        }
        public void RefreshMy()
        {
            polyLinePeriMeter = 0;  height = 0;
           
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

        private void RiPlusBtn_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxRight.Text = Convert.ToString((Convert.ToDouble(txtBoxFront.Text) + totalSquareFootArea));
            //  double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);

        }

        private void FrontSelBtn_Click(object sender, EventArgs e)
        {
            //JPXMLTools xmltools = new JPXMLTools();
            //xmltools.Add(new JPXMlTool() { LayerName = "1 ALL-Elevation Medium" });
            //xmltools.WtitetoXml(@"C: \Users\JyotiP\Documents\Visual Studio 2015\Projects\GlazingArea\GlazingArea\bin\Debug\Config\Layer.xml");
            //JPXMLTools myxmltools = JPXMLTools.ReadXml(@"C: \Users\JyotiP\Documents\Visual Studio 2015\Projects\GlazingArea\GlazingArea\bin\Debug\Config\Layer.xml");
            //foreach (GlazingArea.JPXMlTool item in myxmltools)
            //{
            //    layerName = item.LayerName;
            //}
            // glazeArea = Math.Round(JPGlazingAreaUtill.SelectBlockSpatialFromTheScreen(g_drawFrameAroundWindow), 2);

            bool g_drawFrameAroundWindow = true;
            double glazeArea = 0;
            string layerName = string.Empty;
            glazeArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxFront.Text = Convert.ToString(glazeArea);
        }
        private void btndrawTable_Click(object sender, EventArgs e)
        {
            if (tblList!=null)
            {
                JPGlazingAreaUtill.CreateMyTable(tblList);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Table is Empty!!");
            }
            
        }
        private void btnGzFrstFlrPhSelect_Click(object sender, EventArgs e)
        {
            height = 0; polyLinePeriMeter = 0;
            JPGlazingAreaUtill.SelectObjectPolyLine(ref polyLinePeriMeter);
            polyLinePeriMeter = Math.Round(polyLinePeriMeter, 2);
            txtBoxGZFstFloorPeriPheral.Text =Convert.ToString(polyLinePeriMeter);
           
        }
        private void btnGzScndFlrPhSelect_Click(object sender, EventArgs e)
        {
            polyLinePeriMeter = 0; height = 0;
            JPGlazingAreaUtill.SelectObjectPolyLine(ref polyLinePeriMeter);
            polyLinePeriMeter = Math.Round(polyLinePeriMeter, 2);
            txtBoxGzWDSecFloorPeripheral.Text = Convert.ToString(polyLinePeriMeter);
            
        }

        private void btnScndFloorHeightSelect_Click(object sender, EventArgs e)
        {
            height = 0;
            JPGlazingAreaUtill.SelectObject(ref height);
            height = Math.Round(height, 2);
            txtBoxGzSecFloorHeight.Text = Convert.ToString(height);
            
        }

        private void btnGzThrdFlrPhSelect_Click(object sender, EventArgs e)
        {
            polyLinePeriMeter = 0;
            height = 0;
            JPGlazingAreaUtill.SelectObjectPolyLine(ref polyLinePeriMeter);
            polyLinePeriMeter = Math.Round(polyLinePeriMeter, 2);
            txtBoxGzThrdFloorPeripheral.Text= Convert.ToString(polyLinePeriMeter);
           
        }

        private void btnThirdFloorHeightSelect_Click(object sender, EventArgs e)
        {
            height = 0;
            JPGlazingAreaUtill.SelectObject(ref height);
            height = Math.Round(height, 2);
            txtBoxGzThrdFloorHeight.Text = Convert.ToString(height);
           
        }

        private void btnGzForthFlrPhSelect_Click(object sender, EventArgs e)
        {
            polyLinePeriMeter = 0;
            height = 0;
            JPGlazingAreaUtill.SelectObjectPolyLine(ref polyLinePeriMeter);
            polyLinePeriMeter = Math.Round(polyLinePeriMeter, 2);
            txtBoxForthFloorPeripheral.Text= Convert.ToString(polyLinePeriMeter);
           
        }

        private void btnForthFloorHeightSelect_Click(object sender, EventArgs e)
        {
            height = 0;
            JPGlazingAreaUtill.SelectObject(ref height);
            height = Math.Round(height, 2);
            txtBoxGzforthFloorHeight.Text = Convert.ToString(height);
          
        }

        private void btnGzCalTotalArea_Click(object sender, EventArgs e)
        {
            double dTotalArea = 0;
            double dTmp = 0;
            dTmp = Convert.ToDouble(txtBoxGZFstFloorPeriPheral.Text) * Convert.ToDouble(txtBoxGzFstFloorHeight.Text);
            dTotalArea = dTotalArea + dTmp;
            dTmp = Convert.ToDouble(txtBoxGzWDSecFloorPeripheral.Text) * Convert.ToDouble(txtBoxGzSecFloorHeight.Text);
            dTotalArea = dTotalArea + dTmp;
            dTmp = Convert.ToDouble(txtBoxGzThrdFloorPeripheral.Text) * Convert.ToDouble(txtBoxGzThrdFloorHeight.Text);
            dTotalArea = dTotalArea + dTmp;
            dTmp = Convert.ToDouble(txtBoxForthFloorPeripheral.Text) * Convert.ToDouble(txtBoxGzforthFloorHeight.Text);
            dTotalArea = dTotalArea + dTmp;
            txtBoxGzTotalSFArea.Text = Convert.ToString(dTotalArea);
            txtBoxGzTotalSMArea.Text = Convert.ToString(Math.Round((dTotalArea / oneSqMeter), 2));

   
        }

        private void btnSPWallAreaSelect_Click(object sender, EventArgs e)
        {
            polyLinePeriMeter = 0;
            JPGlazingAreaUtill.SelectObjectPolyLine(ref polyLinePeriMeter);
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

        private void btnFrontPlus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxFront.Text = Convert.ToString((Convert.ToDouble(txtBoxFront.Text) + totalSquareFootArea));
        }

        private void btnRearSelect_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double glazeArea = 0;
           // glazeArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            glazeArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            textBoxRear.Text = Convert.ToString(glazeArea);
        }

        private void btnLeftSelect_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double glazeArea = 0;
           // glazeArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            glazeArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxLeft.Text = Convert.ToString(glazeArea);
        }

        private void btnRightSelect_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double glazeArea = 0;
            glazeArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxRight.Text = Convert.ToString(glazeArea);

        }

        private void btnRearPlus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            textBoxRear.Text = Convert.ToString((Convert.ToDouble(txtBoxFront.Text) + totalSquareFootArea));
        }

        private void btnLeftPlus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxLeft.Text = Convert.ToString((Convert.ToDouble(txtBoxFront.Text) + totalSquareFootArea));
          
        }
        private void btnCalTotalArea_Click(object sender, EventArgs e)
        {
           double totalSfArea = (Convert.ToDouble(txtBoxFront.Text) +
                               Convert.ToDouble(textBoxRear.Text) +
                               Convert.ToDouble(txtBoxLeft.Text) +
                               Convert.ToDouble(txtBoxRight.Text));
            txtBoxSFTotal.Text = Convert.ToString(totalSfArea);
            txtBoxSMTotal.Text = Convert.ToString(Math.Round((totalSfArea / oneSqMeter),2));
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
            glaze.FirstFloorPeripheral= getDoubleValue(txtBoxGZFstFloorPeriPheral.Text );
            glaze.FirstFloorHeight= getDoubleValue(txtBoxGzFstFloorHeight.Text);
            glaze.SecondFloorPeripheral= getDoubleValue(txtBoxGzWDSecFloorPeripheral.Text);
            glaze.SecondFloorHeight= getDoubleValue(txtBoxGzSecFloorHeight.Text);
            glaze.ThirdFloorPeripheral= getDoubleValue(txtBoxGzThrdFloorPeripheral.Text);
            glaze.ThirdFloorHeight= getDoubleValue(txtBoxGzThrdFloorHeight.Text );
            glaze.FourthFloorPeripheral= getDoubleValue(txtBoxForthFloorPeripheral.Text);
            glaze.FourthFloorHeight= getDoubleValue(txtBoxGzforthFloorHeight.Text);
            glaze.PeriPheralSquareFootArea= getDoubleValue(txtBoxGzTotalSFArea.Text);
            glaze.PeriPheralSquareMeterArea= getDoubleValue(txtBoxGzTotalSMArea.Text );
            glaze.FrontArea= getDoubleValue(txtBoxFront.Text);
            glaze.RearArea= getDoubleValue(textBoxRear.Text);
            glaze.LeftArea= getDoubleValue(textBoxRear.Text);
            glaze.RightArea= getDoubleValue(txtBoxRight.Text);
            glaze.TotalGlazingSquareFootArea= getDoubleValue(txtBoxSFTotal.Text);
            glaze.TotalGlazingSquareMeterArea= getDoubleValue(txtBoxSMTotal.Text);
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

        private void btnSelectFrstFloorHeight_Click(object sender, EventArgs e)
        {
            height = 0;
            JPGlazingAreaUtill.SelectObject(ref height);
            height = Math.Round(height, 2);
            txtBoxGzFstFloorHeight.Text = Convert.ToString(height);
           // firstFloorArea = firstFloorArea + polyLinePeriMeter * height;
        }

        private void comboBoxElevation_SelectedIndexChanged(object sender, EventArgs e)
        {
            elevationName = comboBoxElevation.Items[comboBoxElevation.SelectedIndex].ToString().Trim();
        }

       
        private void btnFrontMinus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
             txtBoxFront.Text = Convert.ToString((Convert.ToDouble(txtBoxFront.Text) - totalSquareFootArea));

        }
        private void btnRearMinus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            textBoxRear.Text = Convert.ToString((Convert.ToDouble(textBoxRear.Text) - totalSquareFootArea));
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

        private void txtBoxSPAllpowablePct_TextChanged(object sender, EventArgs e)
        {
            double dTmp = 0;
            dTmp = Math.Round(Convert.ToDouble(txtBoxSPTotalWallArea.Text) * Convert.ToDouble(txtBoxSPAllpowablePct.Text) / 100,2);
            txtBoxSPAllowableOpenings.Text = Convert.ToString(dTmp);
          
        }

        private void txtBoxSPAllowableOpenings_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void btnLeftMinus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxLeft.Text = Convert.ToString((Convert.ToDouble(txtBoxLeft.Text) - totalSquareFootArea));
           
        }
        private void btnRightMinus_Click(object sender, EventArgs e)
        {
            bool g_drawFrameAroundWindow = true;
            double totalSquareFootArea = Math.Round(JPGlazingAreaUtill.GetGlazingArea(g_drawFrameAroundWindow), 2);
            txtBoxRight.Text = Convert.ToString((Convert.ToDouble(txtBoxRight.Text) - totalSquareFootArea));
            
        }
    }
}





