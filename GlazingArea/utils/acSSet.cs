using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;


namespace qACAutils
{
    public class acSSet
    {
        

        public static Boolean CheckObjectId(ObjectId id)
        {
            if (id == ObjectId.Null || id.IsErased || id.IsNull)
                return false;
            else
                return true;
        }

        public static ResultBuffer GetXDataByAppName(ObjectId objId, string AppName)
        {
            ResultBuffer rb = new ResultBuffer();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (DocumentLock acDoc = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    DBObject obj = tr.GetObject(objId, OpenMode.ForRead);
                    rb = obj.GetXDataForApplication(AppName);
                }
            }
            return rb;
        }
        public static ObjectIdCollection GetOffestFromArcs(ObjectId id)
        {
            return GlazingSpatialCalculator.rnWindows.GetAreaPlineFromWindow(id, "0", 0, Math.PI / 9, 0.0001);
        }

        public static ObjectIdCollection SelectionSet2ObjectIdCollection(SelectionSet sset)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            foreach (SelectedObject acSSObj in sset)
            {
                if (qACAutils.acSSet.CheckObjectId(acSSObj.ObjectId))
                    ids.Add(acSSObj.ObjectId);
            }
            return ids;
        }
    }
}
