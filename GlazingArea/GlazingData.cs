using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlazingArea
{
    [Serializable]
    public class GlazingData
    {
         public class Layer
        {
            public string layerName { get; set; }
        }
        public void WriteData()
        {
            Layer myLayer=new Layer();
            myLayer.layerName = "1 ALL-Elevation Medium";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Layer));
            //TODO:VLAD changed it
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, myLayer);
            file.Close();
        }

        public void ReadXML()
        {
            // First write something so that there is something to read ...  
            var l = new Layer { layerName = "1 ALL-Elevation Medium" };
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(Layer));
            var wfile = new System.IO.StreamWriter(@"c:\temp\SerializationOverview.xml");
            writer.Serialize(wfile, l);
            wfile.Close();

            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Layer));
            System.IO.StreamReader file = new System.IO.StreamReader(
                @"c:\temp\SerializationOverview.xml");
            Layer overview = (Layer)reader.Deserialize(file);
            file.Close();

           // Console.WriteLine(overview.layerName);

        }

    }
}
