﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GlazingArea.Forms
{
    
    public partial class glazingForm : Form
    {
        DoorUtils door1;
        int ctr = 0; double tarea = 0;
        const string layerNameConstant = "1 ALL-Elevation Medium";

        public glazingForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            JPXMLTools xmltools = new JPXMLTools();
            xmltools.Add(new JPXMlTool() {LayerName= "1 ALL-Elevation Medium" });
            xmltools.WtitetoXml(@"C: \Users\JyotiP\Documents\Visual Studio 2015\Projects\GlazingArea\GlazingArea\bin\Debug\Config\Layer.xml");
            JPXMLTools myxmltools = JPXMLTools.ReadXml(@"C: \Users\JyotiP\Documents\Visual Studio 2015\Projects\GlazingArea\GlazingArea\bin\Debug\Config\Layer.xml");

            //GlazingData gdata = new GlazingData();
            //gdata.WriteData();
            //gdata.ReadXML();
        }

        private void glazingForm_Load(object sender, EventArgs e)
        {

        }
    }
}
