using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlazingArea
{
    class Door
    {
        public int doorCount { get; set; }
        public double doorLength { get; set; }
        public double doorWidth { get; set; }
        public string doorName { get; set; }
        public Point3d position { get; set; }
        public Extents3d doorExtents { get; set; }
        public double doorArea;
    }
     class DoorUtils
    {

        
        public double GetMspaceSlidingDoorArea()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            double area = 0.0;
            Point3d dMinPoint = new Point3d();
            Point3d dMaxPoint = new Point3d();
           // ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
            using (DocumentLock acLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {

                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                   // string blkName1 = "Altwood Garage Door - 8'";
                    //  string blkName2 = "Door - Single";
                    string blkName = "Sliding Door";
                    foreach (var btrId in bt)
                    {
                        if (true)
                        {

                        }
                        BlockTableRecord block = tr.GetObject(bt[blkName], OpenMode.ForRead) as BlockTableRecord;
                        if (!block.IsDynamicBlock)
                        {
                            continue;
                        }

                        var anonymousIds = block.GetAnonymousBlockIds();
                        var dynBlockRefs = new ObjectIdCollection();
                        int count = 0;
                        foreach (ObjectId anonymousBtrId in anonymousIds)
                        {
                            var anonymousBtr = (BlockTableRecord)tr.GetObject(anonymousBtrId, OpenMode.ForRead);
                            var blockRefIds = anonymousBtr.GetBlockReferenceIds(true, true);
                            foreach (ObjectId id in blockRefIds)
                            {
                                dynBlockRefs.Add(id);
                                count = dynBlockRefs.Count;
                                string name = anonymousBtr.Name;

                            }
                        }
                        BlockReference blk;
                        if (count > 0)
                        {
                            foreach (ObjectId id in dynBlockRefs)
                            {
                                BlockReference oEnt = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                blk = (BlockReference)oEnt;
                                Extents3d extents = blk.GeometricExtents;
                                dMinPoint = extents.MinPoint;
                                dMaxPoint = extents.MaxPoint;
                                double w = extents.MaxPoint.X - extents.MinPoint.X;
                                double h = extents.MaxPoint.Y - extents.MinPoint.Y;
                                area = w * h;
                                //area = Math.Round(w*h / 144, 2);

                            }
                        }

                    }
                    BlockTableRecord acblkTblRec = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    using (Polyline acPoly = new Polyline())
                    {
                        acPoly.SetDatabaseDefaults();
                        acPoly.AddVertexAt(0, new Point2d(dMinPoint.X, dMinPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(1, new Point2d(dMaxPoint.X, dMinPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(2, new Point2d(dMaxPoint.X, dMaxPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(3, new Point2d(dMinPoint.X, dMaxPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(4, new Point2d(dMinPoint.X, dMinPoint.Y), 0, 0, 0);
                       // acPoly.SetLayerId(layerId, true);
                        acblkTblRec.AppendEntity(acPoly);
                        tr.AddNewlyCreatedDBObject(acPoly, true);
                    }
                    tr.Commit();
                }
            }
            return area;
        }
        public  void GetDoor( ref int count, ref double area)
        {
            List<Door> doorList = GetMspaceDoorsBlockRefCount();
            foreach (Door door in doorList)
            {
                area = area + door.doorArea;
                count =door.doorCount;
            }
        }
        public static  List<Door> GetMspaceDoorsBlockRefCount()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId objId = ObjectId.Null;
            List<Door> doorList = new List<Door>();
            int count = 0;
            // string blkName = "Door - Single";
            // string blkName2 = "Sliding Door";
            //string blkName3 = "Altwood Garage Door - 8'";
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btrMSpace = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
               // BlockTableRecord btrMSpace = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                foreach (ObjectId id in btrMSpace)
                {
                    if (id.ObjectClass.DxfName == "INSERT")
                    {
                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        if (br.IsDynamicBlock)
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.DynamicBlockTableRecord, OpenMode.ForWrite);
                            if (btr.Name.Trim().ToUpper().Contains("DOOR"))
                            {
                                count++;
                                //objId = br.ObjectId;
                                Door d1 = new Door();
                                d1.doorCount = count;
                                d1.doorName = btr.Name;
                                d1.position = br.Position;
                                d1.doorExtents = br.GeometricExtents;
                                d1.doorWidth= d1.doorExtents.MaxPoint.X - d1.doorExtents.MinPoint.X;
                                d1.doorLength = d1.doorExtents.MaxPoint.Y - d1.doorExtents.MinPoint.Y;
                                d1.doorArea = Math.Round((d1.doorWidth * d1.doorLength),2);
                                doorList.Add(d1);
                                using (Polyline acPoly = new Polyline())
                                {
                                    acPoly.SetDatabaseDefaults();
                                    acPoly.AddVertexAt(0, new Point2d(d1.doorExtents.MinPoint.X, d1.doorExtents.MinPoint.Y), 0, 0, 0);
                                    acPoly.AddVertexAt(1, new Point2d(d1.doorExtents.MaxPoint.X, d1.doorExtents.MinPoint.Y), 0, 0, 0);
                                    acPoly.AddVertexAt(2, new Point2d(d1.doorExtents.MaxPoint.X, d1.doorExtents.MaxPoint.Y), 0, 0, 0);
                                    acPoly.AddVertexAt(3, new Point2d(d1.doorExtents.MinPoint.X, d1.doorExtents.MaxPoint.Y), 0, 0, 0);
                                    acPoly.AddVertexAt(4, new Point2d(d1.doorExtents.MinPoint.X, d1.doorExtents.MinPoint.Y), 0, 0, 0);
                                    acPoly.ColorIndex = 3;
                                    // acPoly.SetLayerId(layerId, true);
                                    btrMSpace.AppendEntity(acPoly);
                                    tr.AddNewlyCreatedDBObject(acPoly, true);
                                }
                            }

                        }
                    }
                }

                tr.Commit();
            }
            return doorList;
        }

    }
}
