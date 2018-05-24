using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GlazingArea
{


    /*
     * 
     * asdfasasdfldas;lgfk;asldfk';aslk
     * /
    public class JPXMlTool
    {
        public string LayerName { get; set; }
    }
    [Serializable]
    public class JPXMLTools : List<JPXMlTool>
    {
        public void WtitetoXml(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static JPXMLTools ReadXml(string path)
        {
            XmlSerializer reader = new XmlSerializer(typeof(JPXMLTools));
            using (FileStream input = File.OpenRead(path))
            {
                return reader.Deserialize(input) as JPXMLTools;
            }
        }


        //public void WriteXMl()
        //{
        //    JPXMlTool xmltool = new JPXMlTool();
        //    // xmltool.LayerName = "1 ALL - Elevation Medium";
        //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(JPXMlTool));
        //    TextWriter txtWriter = new StreamWriter(@"C: \Users\JyotiP\Documents\Visual Studio 2015\Projects\GlazingArea\GlazingArea\bin\Debug\Config\Layer.xml");
        //    xmlSerializer.Serialize(txtWriter, xmltool);
        //    txtWriter.Close();
        //}
    }
}
