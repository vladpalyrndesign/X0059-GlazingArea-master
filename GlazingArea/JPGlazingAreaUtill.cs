using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qAAA = Autodesk.AutoCAD.ApplicationServices;
namespace qrndGlazingArea
{
    public static class JPGlazingAreaUtill
    {
        public enum CalcType { Glazing, Spatial }
        public static void GetSysVariable()
        {
            //TODO:VLAD CHECK FOR DRAWING UNNITSs
            UnitsValue unitValue = Application.DocumentManager.MdiActiveDocument.Database.Insunits;
            if (!unitValue.Equals(4))
            {
                double dTotalArea = 0;
                //'The unit was in milimeter, convert it to square foot
                if (unitValue.Equals(0) || unitValue.Equals(4))
                    dTotalArea = dTotalArea * 144 / 92903.04;

            }
        }
        public static double SelectObjectPolyLine(CalcType calcType)
        {
            double polyLinePerimeter = 0;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            //TODO:VLAD hardcoded names must be in configuration files
            //string sLayerName = "4 AREA-Floor";
            //string sLayerName3 = "4 AREA-Other";
            //string sLayerNames = sLayerName + "," + sLayerName3;

            //            if pline has "4 AREA-Floor" then takes area perimeter of wall for glazing calculation
            //              if pline has "4 AREA-Other" then takes length area of wall on spatial calculation

            string sLayerNames = "";

            //TODO:VLAD hardcoded names must be in configuration files
            if (calcType == CalcType.Glazing)
            {
                sLayerNames = "4 AREA-Floor";
            }
            else if (calcType == CalcType.Spatial)
            {
                sLayerNames = "4 AREA-Other";
            }


            #region selection 
            //TODO:VLAD changed 20180517
            //TypedValue[] acTypValAr = new TypedValue[] { new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE") };
            TypedValue[] acTypValAr = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue((int)DxfCode.LayoutName , "Model"),
                    new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
                    new TypedValue((int)DxfCode.LayerName, sLayerNames),
                    new TypedValue((int)DxfCode.Operator, "and>")};
            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
            PromptSelectionResult acSSPrompt;
            acSSPrompt = acDoc.Editor.GetSelection(acSelFtr);
            if (acSSPrompt.Status != PromptStatus.OK)
                return 0;
            #endregion selection 
            SelectionSet acSSet = acSSPrompt.Value;

            ObjectIdCollection ids = new ObjectIdCollection();
            foreach (SelectedObject acSSObj in acSSet)
            {
                if (acSSObj.ObjectId != ObjectId.Null && !acSSObj.ObjectId.IsErased && !acSSObj.ObjectId.IsNull)
                {
                    ids.Add(acSSObj.ObjectId);
                }
            }

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Request for objects to be selected in the drawing area

                // If the prompt status is OK, objects were selected

                // Step through the objects in the selection set
                foreach (ObjectId id in ids)
                {
                    // Check to make sure a valid SelectedObject object was returned

                    // Open the selected object for write
                    Entity acEnt = acTrans.GetObject(id, OpenMode.ForRead) as Entity;
                    string entLayer = acEnt.Layer;


                    //TODO:VLAD CHANGE THIS BLOCK LATER >>>
                    // LayerList.Contains(acEnt.Layer)
                    // acEnt != null =>acEnt != Object.Null
                    // check one var polyLinePerimeter += Length +=Area ///??????????????
                    if (acEnt is Polyline2d)                         //(acEnt != null && acEnt.Equals((Polyline2d)acEnt))
                    {
                        Polyline2d acPline = (Polyline2d)acEnt;
                        //TODO:VLAD CHNAGED 

                        //LayerTableRecord ltr = (LayerTableRecord)acTrans.GetObject(acPline.LayerId, OpenMode.ForRead);///???
                        //if (entLayer.Equals(sLayerName, StringComparison.InvariantCultureIgnoreCase))
                        if (calcType == CalcType.Glazing)
                        {
                            polyLinePerimeter = acPline.Length / 12; ///??????????????
                        }
                        //else if (entLayer.Equals(sLayerName3, StringComparison.InvariantCultureIgnoreCase))
                        else if (calcType == CalcType.Spatial)
                        {
                            polyLinePerimeter = acPline.Area / 144;///??????????????
                        }
                    }
                    else if (acEnt is Polyline)                                               //(acEnt != null && acEnt.Equals((Polyline)acEnt))
                    {
                        Polyline acPline = (Polyline)acEnt;
                        //TODO:VLAD CHNAGED  2018-05-18
                        //LayerTableRecord ltr = (LayerTableRecord)acTrans.GetObject(acPline.LayerId, OpenMode.ForRead);
                        //if (entLayer.Equals(sLayerName, StringComparison.InvariantCultureIgnoreCase))
                        if (calcType == CalcType.Glazing)
                        {
                            polyLinePerimeter = acPline.Length / 12;
                        }
                        // else if (entLayer.Equals(sLayerName3, StringComparison.InvariantCultureIgnoreCase))
                        else if (calcType == CalcType.Spatial)
                        {
                            polyLinePerimeter = acPline.Area / 144;
                        }
                    }
                    //TODO:VLAD ELEMINATE THIS BLOCK LATER <<<

                }
            }
            return polyLinePerimeter;
        }
        public static double ConvertSqFtToSqMeter(double inches)
        {
            //TODO:VLAD CHANGED
            //double totalMM = (inches / 10.764);
            return (inches / 10.764);
        }
        public static double ConvertInchesToFoot(double inches)
        {
            //TODO:VLAD CHANGED
            return (inches / 12.0);
        }
        public static double ConvertInchesToSqrFoot(double inches)
        {
            //TODO:VLAD CHANGED
            return inches / 144.0;
        }
        public static ObjectIdCollection SelectionSet2ObjectIdCollection(SelectionSet ss)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            foreach (SelectedObject s in ss)
            {
                if (s.ObjectId != ObjectId.Null && !s.ObjectId.IsErased && !s.ObjectId.IsNull)
                {
                    ids.Add(s.ObjectId);
                }
            }
            return ids;
        }

        //TODO:VLAD completely refactored function: 
        //removed multiple selection 
        // only one dimension can be selected
        public static double SelectOneRotatedDimension()
        {
            double height = 0;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            #region entity RotatedDimension selection
            PromptEntityOptions opts = new PromptEntityOptions("\nSelect Dimension: ");
            opts.SetRejectMessage("\nOnly Rotated Dimensions could be selected.");
            opts.AddAllowedClass(typeof(RotatedDimension), false);

            PromptEntityResult per = ed.GetEntity(opts);
            if (!qACAutils.acSSet.CheckObjectId(per.ObjectId)) return 0;
            ObjectId id = per.ObjectId;
            #endregion entity RotatedDimension selection
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                RotatedDimension acRotDim = acTrans.GetObject(id, OpenMode.ForRead) as RotatedDimension;
                height = ConvertInchesToFoot(acRotDim.Measurement);
            }
            return height;
        }
        public static ObjectId GetLayerId(string sLayer)
        {
            ObjectId layerId = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)db.TransactionManager.GetObject(db.LayerTableId, OpenMode.ForRead);
                if (lt.Has(sLayer))
                {
                    layerId = lt[sLayer];
                }
                else
                {
                    lt.UpgradeOpen();
                    //TODO:VLAD document is not locked transaction are not permitted. will break AutoCAD
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 4);//???
                    ltr.Name = sLayer;
                    layerId = lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                    tr.Commit();
                }
            }
            return layerId;
        }
        public static ResultBuffer GetXData(ObjectId objId)
        {
            Document acDdoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDdoc.Database;
            ResultBuffer rb = new ResultBuffer();
            Document doc = qAAA.Application.DocumentManager.MdiActiveDocument;
            using (DocumentLock acDoc = qAAA.Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    DBObject obj = tr.GetObject(objId, OpenMode.ForRead);
                    rb = obj.XData;
                }
            }
            return rb;
        }
        public static double SelectGlazingBlockFromTheScreen()
        {
            Document acDdoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDdoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] acTypValAr = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue((int)DxfCode. Start,"INSERT"),
                    new TypedValue((int)DxfCode.LayoutName , "Model"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.LayerName,"1 ALL-Elevation Medium" ),
                    new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
                    };

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.AllowDuplicates = false;
            PromptSelectionResult selRes = acDocEd.GetSelection(opts, acSelFtr);
            if (selRes.Status != PromptStatus.OK)
                return -1;
            SelectionSet acSSet = selRes.Value;
            double totalWinArea = 0;
            List<double> winAreaList = new List<double>();
            using (DocumentLock acDocLock = acDdoc.LockDocument())
            {
                using (Transaction tr = acDdoc.TransactionManager.StartTransaction())
                {
                    BlockTable blkTbl;
                    blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    foreach (ObjectId objId in acSSet.GetObjectIds())
                    {
                        if (objId != null)
                        {
                            //ResultBuffer rb = new ResultBuffer();
                            //rb = GetXData(objId);
                            BlockReference oEnt = (BlockReference)tr.GetObject(objId, OpenMode.ForRead);
                            BlockReference blk = (BlockReference)oEnt;
                            Extents3d extent = blk.GeometricExtents;
                            double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
                            double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
                            double winArea = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
                            winAreaList.Add(winArea);
                        }
                    }
                    tr.Commit();
                    totalWinArea = ConvertInchesToSqrFoot(totalWinArea + winAreaList.Sum());
                }
            }

            return totalWinArea;
        }



        public static double getGlazingArea(bool drawFrameAroundWindow)
        {
            Document acDdoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDdoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            double dTotalGlazingArea = 0;
            double dWinAreaFromNewBlock = 0;
            //TypedValue[] acTypValAr = new TypedValue[] {
            //        new TypedValue((int)DxfCode.Operator, "<and"),
            //        new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
            //        new TypedValue((int)DxfCode. Start,"INSERT"),
            //        new TypedValue((int)DxfCode.LayoutName , "Model"),
            //        new TypedValue((int)DxfCode.Operator, "<or"),
            //        new TypedValue((int)DxfCode.LayerName,"1 ALL-Elevation Medium" ),

            //        new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
            //        new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
            //        new TypedValue((int)DxfCode.Operator,"or>"),
            //        new TypedValue((int)DxfCode.Operator,"and>")
            //        };

            TypedValue[] acTypValAr = new TypedValue[] {
                                        new TypedValue((int)DxfCode.Operator, "<or"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
                                        new TypedValue((int)DxfCode.LayerName,"AL-OPGN,AL-OPNG,1 ALL-Elevation Medium" ),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode. Start,"INSERT"),
                                        new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode. Start,"INSERT"),
                                        new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator,"or>"),
                    };



            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.AllowDuplicates = false;
            PromptSelectionResult selRes = acDocEd.GetSelection(opts, acSelFtr);
            if (selRes.Status != PromptStatus.OK)
                return -1;
            SelectionSet acSSet = selRes.Value;

            using (DocumentLock acDocLock = acDdoc.LockDocument())
            {
                using (Transaction tr = acDdoc.TransactionManager.StartTransaction())
                {
                    BlockTable blkTbl;
                    blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        //TODO:VLAD changed everwhere 
                        //ObjectId id = acSSObj.ObjectId
                        //(acSSObj.ObjectId != ObjectId.Null && !acSSObj.ObjectId.IsErased && !acSSObj.ObjectId.IsNull!)

                        if (acSSObj != null)// TODO:VLAD ELIMINATE
                        {
                            // Open the selected object for write
                            Entity acEnt = tr.GetObject(acSSObj.ObjectId, OpenMode.ForWrite) as Entity;
                            if (acEnt != null && acEnt.Equals((BlockReference)acEnt))
                            {
                                BlockReference blk = tr.GetObject(acEnt.ObjectId, OpenMode.ForRead) as BlockReference;


                            }
                            else if (acEnt != null && acEnt is Polyline2d)
                            {

                            }
                            else if (acEnt != null && acEnt is Polyline)
                            {

                            }
                        }
                    }


                    tr.Commit();
                }
            }
            dTotalGlazingArea = dWinAreaFromNewBlock;
            return dTotalGlazingArea;
        }

        public static double GetAllowedGlazingPercentage(double dWallArea, double dDist)
        {
            //            ' Interpolate allowable percentage of glazing area
            //' Based on:
            //' ---------------------------------------------------------------
            //'                       Table 9.10.15.4
            //' Maximum Area of Glazed Openings in Exterior Walls of Buildings
            //'               Containing Only Dwelling Units
            //' ---------------------------------------------------------------'
            //'
            double getAllowedGlazingPercentage = 0.0;
            double[] dDistance = new double[14];
            double[] dArea = new double[9];
            double[] dCol = new double[] { 3.916, 4.917, 6.583 , 8.333 ,
                                            9.833,13.083,19.667,19.667,
                                            19.667,39.333,54.5,65.583
                                           };

            double[] dRow = new double[] { 107, 160, 215, 267,
                                            323,431,538,1080,9999};

            double[,] dTable = new double[,] {
                { 8,12,21,33,55,96,100,100,100,100,100,100 },
                { 8,10,17,25,37,67,100,100,100,100,100,100 },
                { 8,10,15,21,30,53,100,100,100,100,100,100 },
                { 8,9,13,19,26,45,100,100,100,100,100,100 },
                { 7,9,12,17,23,39,88,100,100,100,100,100 },
                { 7,8,11,15,20,32,69,100,100,100,100,100 },
                { 7,8,10,14,18,28,57,100,100,100,100,100 },
                { 7,8,9,11,13,18,34,56,84,100,100,100 },
                 { 7,7,8,9,10,12,19,28,40,55,92,100 }
            };

            if (dDist < 3.196 || dWallArea < 107)
                getAllowedGlazingPercentage = 0.0;
            else if (dDist > 65.583)
                getAllowedGlazingPercentage = 100.0;
            int iRow = 0, iCol = 0;
            for (int i = 1; i < 8; i++)
            {
                if (dWallArea <= dRow[i])
                {
                    iRow = i - 1;
                    break;
                }

            }
            for (int i = 1; i < 10; i++)
            {
                if (dDist <= dCol[i])
                {
                    iCol = i - 1;
                    break;
                }
            }


            //        ' Interpolate
            double dPct1, dPct2, dPct;
            if (iRow == 7)
            {
                dPct = Interpolate(dDist, dCol[iCol], dCol[iCol + 1], dTable[8, iCol], dTable[8, iCol + 1]);
                getAllowedGlazingPercentage = dPct;
            }
            dPct1 = Interpolate(dWallArea, dRow[iRow], dRow[iRow + 1], dTable[iRow, iCol], dTable[iRow + 1, iCol]);
            dPct2 = Interpolate(dWallArea, dRow[iRow], dRow[iRow + 1], dTable[iRow, iCol + 1], dTable[iRow + 1, iCol + 1]);
            dPct = Interpolate(dDist, dCol[iCol], dCol[iCol + 1], dPct1, dPct2);
            getAllowedGlazingPercentage = Math.Round(dPct, 2);
            return getAllowedGlazingPercentage;

        }

        public static double Interpolate(double dInput, double d1, double d2, double v1, double v2)
        {
            double interpolate;
            if (Math.Abs(d1 - d2) < 0.01)
                interpolate = v1;
            else
                interpolate = v1 + (v2 - v1) * (dInput - d1) / (d2 - d1);

            return interpolate;
        }
        public static double GetGlazingArea(bool drawFrame)
        {
            UnitsValue unitValue = Application.DocumentManager.MdiActiveDocument.Database.Insunits;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] acTypValAr = new TypedValue[] {

                                    new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode.LayoutName , "Model"),
                                        new TypedValue((int)DxfCode.Operator, "<or"),
                                            new TypedValue((int)DxfCode.Operator, "<and"),
                                                new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
                                                new TypedValue((int)DxfCode.LayerName,"AL-OPGN,AL-OPNG,1 ALL-Elevation Medium" ),
                                            new TypedValue((int)DxfCode.Operator,"and>"),
                                            //new TypedValue((int)DxfCode.Operator, "<and"),
                                            //new TypedValue((int)DxfCode. Start,"INSERT"),
                                            //new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
                                            //new TypedValue((int)DxfCode.Operator,"and>"),
                                            //new TypedValue((int)DxfCode.Operator, "<and"),
                                            //new TypedValue((int)DxfCode. Start,"INSERT"),
                                            //new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
                                            //new TypedValue((int)DxfCode.Operator,"and>"),
                                            //new TypedValue((int)DxfCode.Operator, "<and"),
                                            new TypedValue((int)DxfCode. Start,"INSERT"),
                                            //new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator,"or>"),
                                    new TypedValue((int)DxfCode.Operator,"and>")
                                    };

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.AllowDuplicates = false;
            PromptSelectionResult selRes = ed.GetSelection(opts, acSelFtr);
            if (selRes.Status != PromptStatus.OK)
                return -1;

            ObjectIdCollection ids = qACAutils.acSSet.SelectionSet2ObjectIdCollection(selRes.Value);
            if (ids == null || ids.Count == 0)
                return -1;

            double totalArea = 0;
            double maxArea = 0;

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                foreach (ObjectId id in ids)
                {
                    double area = 0;
                    // Check to make sure a valid SelectedObject object was returned
                    Entity acEnt = tr.GetObject(id, OpenMode.ForWrite) as Entity;

                    if (acEnt is Polyline2d)
                    {
                        Polyline2d acPline = (Polyline2d)acEnt;
                        Polyline2d acPlineClone = acPline.Clone() as Polyline2d;
                        DBObjectCollection acDbObjColl = acPlineClone.GetOffsetCurves(-1);
                        foreach (Entity acEnt1 in acDbObjColl)
                        {
                            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                            acEnt1.SetLayerId(layerId, true);
                            // Add each offset object
                            acblkTblRec.AppendEntity(acEnt1);
                            tr.AddNewlyCreatedDBObject(acEnt1, true);
                        }
                        area = acPline.Area;
                        area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                        totalArea = totalArea + area;
                    }
                    else if (acEnt is Polyline)
                    {
                        Polyline acPline = (Polyline)acEnt;
                        Polyline acPlineClone = acPline.Clone() as Polyline;
                        DBObjectCollection acDbObjColl = acPlineClone.GetOffsetCurves(-1);
                        foreach (Entity acEnt1 in acDbObjColl)
                        {
                            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                            acEnt1.SetLayerId(layerId, true);
                            // Add each offset object
                            acblkTblRec.AppendEntity(acEnt1);
                            tr.AddNewlyCreatedDBObject(acEnt1, true);
                        }
                        area = acPline.Area;
                        area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                        totalArea = totalArea + area;

                    }
                    else if (acEnt.Equals((BlockReference)acEnt))
                    {
                        BlockReference blk = tr.GetObject(id, OpenMode.ForRead) as BlockReference;

                        ResultBuffer xd = qACAutils.acSSet.GetXDataByAppName(acEnt.ObjectId, "qrndsgn_AutoWindows");
                        Boolean isWindow = false;
                        if (xd != null && xd.AsArray().Count() > 0) isWindow = true;
                        else
                        {
                            xd = qACAutils.acSSet.GetXDataByAppName(acEnt.ObjectId, "AutoWindows");
                            if (xd != null && xd.AsArray().Count() > 0) isWindow = true;
                        }
                        xd.Dispose();

                        #region Jyoti code has been commented
                        //BlockTableRecord blkRec = (BlockTableRecord)tr.GetObject(blk.BlockTableRecord, OpenMode.ForRead);

                        // double maxArea = 0;
                        //foreach (ObjectId asObjId in blkRec)
                        //{

                        //    Entity e = (Entity)tr.GetObject(asObjId, OpenMode.ForRead);
                        //    if (e is Polyline2d)
                        //    {
                        //        Polyline2d pline = (Polyline2d)e;

                        //        double dArea = pline.Area;
                        //        // maxArea = dArea;
                        //        if (maxArea < dArea)
                        //        {
                        //            maxArea = dArea;
                        //            DBObjectCollection acDbObjColl = pline.GetOffsetCurves(-1);
                        //            foreach (Entity acEntPoly in acDbObjColl)
                        //            {
                        //                ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                        //                acEntPoly.SetLayerId(layerId, true);
                        //                // acEntPoly.ColorIndex = 3;
                        //                acblkTblRec.AppendEntity(acEntPoly);
                        //                tr.AddNewlyCreatedDBObject(acEntPoly, true);
                        //            }

                        //        }

                        //    }
                        //}


                        //if (blk.AttributeCollection.Count > 0)
                        //{
                        //    foreach (ObjectId objIdAttr in blk.AttributeCollection)
                        //    {
                        //        DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
                        //        AttributeReference ar = obj as AttributeReference;
                        //        if (ar != null && ar.Tag.Contains("X"))
                        //        {
                        //            int offset = 0;
                        //            string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                        //            string[] stemp = attsValue.Split('X');
                        //            if (drawFrame)
                        //            {
                        //                offset = 0;
                        //                area = CalCulateWindowArea(stemp, offset);
                        //            }
                        //            else
                        //            {
                        //                offset = 2;
                        //                area = CalCulateWindowArea(stemp, offset);
                        //            }
                        //            ///TODO:VLAD WINGEN INSERT ????
                        //            area = 11d;
                        //            // area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                        //            totalArea = totalArea + area;
                        //        }
                        //    }
                        //}
                        #endregion Jyoti code has been commented

                        //window object
                        if (isWindow)
                        {
                            ObjectIdCollection idsBlPlns = qACAutils.acSSet.GetOffestFromArcs(acEnt.ObjectId);
                            foreach (ObjectId idsBlPln in idsBlPlns)
                            {
                                Polyline p = tr.GetObject(idsBlPln, OpenMode.ForWrite) as Polyline;

                                DBObjectCollection acDbObjColl = p.GetOffsetCurves(-1);
                                foreach (Entity acEnt1 in acDbObjColl)
                                {
                                    ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                                    acEnt1.SetLayerId(layerId, true);
                                    // Add each offset object
                                    ObjectId offsetedP =  acblkTblRec.AppendEntity(acEnt1);
                                    tr.AddNewlyCreatedDBObject(acEnt1, true);
                                    Polyline pp = tr.GetObject(offsetedP, OpenMode.ForRead) as Polyline;
                                    totalArea += (!unitValue.Equals(4)) ? pp.Area / 144 : pp.Area / 25.4 / 25.4 / 144;
                                }

                                
                                p.Erase();
                            }
                        }
                        else
                        {
                            Extents3d extent = blk.GeometricExtents;
                            Point3d dMinPoint = extent.MinPoint;
                            Point3d dMaxPoint = extent.MaxPoint;
                            using (Polyline acPoly = new Polyline())
                            {
                                acPoly.SetDatabaseDefaults();
                                acPoly.AddVertexAt(0, new Point2d(dMinPoint.X, dMinPoint.Y), 0, 0, 0);
                                acPoly.AddVertexAt(1, new Point2d(dMaxPoint.X, dMinPoint.Y), 0, 0, 0);
                                acPoly.AddVertexAt(2, new Point2d(dMaxPoint.X, dMaxPoint.Y), 0, 0, 0);
                                acPoly.AddVertexAt(3, new Point2d(dMinPoint.X, dMaxPoint.Y), 0, 0, 0);
                                acPoly.AddVertexAt(4, new Point2d(dMinPoint.X, dMinPoint.Y), 0, 0, 0);
                                DBObjectCollection acDbObjColl = acPoly.GetOffsetCurves(1);
                                foreach (Entity acEnt1 in acDbObjColl)
                                {
                                    ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                                    acEnt1.SetLayerId(layerId, true);
                                    // Add each offset object
                                    acblkTblRec.AppendEntity(acEnt1);
                                    tr.AddNewlyCreatedDBObject(acEnt1, true);
                                }
                            }

                            double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
                            double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
                            area = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
                            area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                            totalArea = totalArea + area;
                        }

                    }



                }

                //foreach (ObjectId objId in acSSet.GetObjectIds())
                //{
                //    if (objId != null)
                //    {
                //        double winarea = 0;
                //        BlockReference oEnt = (BlockReference)tr.GetObject(objId, OpenMode.ForRead);
                //        BlockReference blk = (BlockReference)oEnt;
                //        if (blk.AttributeCollection.Count > 0)
                //        {
                //            foreach (ObjectId objIdAttr in blk.AttributeCollection)
                //            {
                //                DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
                //                AttributeReference ar = obj as AttributeReference;
                //                if (ar != null && ar.Tag.Contains("X"))
                //                {
                //                    int offset = 0;
                //                    string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                //                    string[] stemp = attsValue.Split('X');

                //                    if (drawFrame)
                //                    {
                //                        offset = 0;
                //                        winarea = CalCulateWindowArea(stemp,offset);
                //                    }
                //                    else
                //                    {
                //                        offset = 2;
                //                        winarea = CalCulateWindowArea(stemp,offset);
                //                    }
                //                    winarea = (!unitValue.Equals(4)) ? winarea / 144 : winarea / 25.4 / 25.4 / 144;
                //                   // winarea = (winarea * 100 + 0.5) / 100;
                //                    totalWinArea = totalWinArea + winarea;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            Extents3d extent = blk.GeometricExtents;
                //            double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
                //            double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
                //             winarea = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
                //            winarea = (!unitValue.Equals(4)) ? winarea / 144 : winarea / 25.4 / 25.4 / 144;
                //            totalWinArea = totalWinArea + winarea;
                //        }


                //        }
                //        }
                tr.Commit();
            }
            //return 11d;
            return totalArea;
        }
        //public static double GetGlazingArea(bool drawFrame)
        //{
        //    UnitsValue unitValue = Application.DocumentManager.MdiActiveDocument.Database.Insunits;

        //    Document acDdoc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = acDdoc.Database;
        //    Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
        //    TypedValue[] acTypValAr = new TypedValue[] {

        //                            new TypedValue((int)DxfCode.Operator, "<and"),
        //                                new TypedValue((int)DxfCode.LayoutName , "Model"),
        //                                new TypedValue((int)DxfCode.Operator, "<or"),
        //                                    new TypedValue((int)DxfCode.Operator, "<and"),
        //                                        new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
        //                                        new TypedValue((int)DxfCode.LayerName,"AL-OPGN,AL-OPNG,1 ALL-Elevation Medium" ),
        //                                    new TypedValue((int)DxfCode.Operator,"and>"),
        //                                    //new TypedValue((int)DxfCode.Operator, "<and"),
        //                                    //new TypedValue((int)DxfCode. Start,"INSERT"),
        //                                    //new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
        //                                    //new TypedValue((int)DxfCode.Operator,"and>"),
        //                                    //new TypedValue((int)DxfCode.Operator, "<and"),
        //                                    //new TypedValue((int)DxfCode. Start,"INSERT"),
        //                                    //new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
        //                                    //new TypedValue((int)DxfCode.Operator,"and>"),
        //                                    //new TypedValue((int)DxfCode.Operator, "<and"),
        //                                    new TypedValue((int)DxfCode. Start,"INSERT"),
        //                                    //new TypedValue((int)DxfCode.Operator,"and>"),
        //                                new TypedValue((int)DxfCode.Operator,"or>"),
        //                            new TypedValue((int)DxfCode.Operator,"and>")
        //                            };

        //    SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
        //    PromptSelectionOptions opts = new PromptSelectionOptions();
        //    opts.AllowDuplicates = false;
        //    PromptSelectionResult selRes = acDocEd.GetSelection(opts, acSelFtr);
        //    if (selRes.Status != PromptStatus.OK)
        //        return -1;
        //    SelectionSet acSSet = selRes.Value;
        //    double totalArea = 0;
        //    double maxArea = 0;
        //    using (Transaction tr = acDdoc.TransactionManager.StartTransaction())
        //    {
        //        BlockTable blkTbl;
        //        blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        //        BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

        //        foreach (SelectedObject acSSObj in acSSet)
        //        {
        //            double area = 0;
        //            // Check to make sure a valid SelectedObject object was returned
        //            if (acSSObj != null)
        //            {
        //                Entity acEnt = tr.GetObject(acSSObj.ObjectId,
        //                                                OpenMode.ForWrite) as Entity;
        //                if (acEnt != null && acEnt is Polyline2d)
        //                {
        //                    Polyline2d acPline = (Polyline2d)acEnt;
        //                    Polyline2d acPlineClone = acPline.Clone() as Polyline2d;
        //                    DBObjectCollection acDbObjColl = acPlineClone.GetOffsetCurves(-1);
        //                    foreach (Entity acEnt1 in acDbObjColl)
        //                    {
        //                        ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
        //                        acEnt1.SetLayerId(layerId, true);
        //                        // Add each offset object
        //                        acblkTblRec.AppendEntity(acEnt1);
        //                        tr.AddNewlyCreatedDBObject(acEnt1, true);
        //                    }
        //                    area = acPline.Area;
        //                    area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
        //                    totalArea = totalArea + area;
        //                }
        //                else if (acEnt != null && acEnt is Polyline)
        //                {
        //                    Polyline acPline = (Polyline)acEnt;
        //                    Polyline acPlineClone = acPline.Clone() as Polyline;
        //                    DBObjectCollection acDbObjColl = acPlineClone.GetOffsetCurves(-1);
        //                    foreach (Entity acEnt1 in acDbObjColl)
        //                    {
        //                        ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
        //                        acEnt1.SetLayerId(layerId, true);
        //                        // Add each offset object
        //                        acblkTblRec.AppendEntity(acEnt1);
        //                        tr.AddNewlyCreatedDBObject(acEnt1, true);
        //                    }
        //                    area = acPline.Area;
        //                    area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
        //                    totalArea = totalArea + area;

        //                }
        //                else if ((acEnt != null && acEnt.Equals((BlockReference)acEnt)))
        //                {
        //                    BlockReference blk = tr.GetObject(acEnt.ObjectId, OpenMode.ForRead) as BlockReference;
        //                    BlockTableRecord blkRec = (BlockTableRecord)tr.GetObject(blk.BlockTableRecord, OpenMode.ForRead);
        //                    // double maxArea = 0;
        //                    foreach (ObjectId asObjId in blkRec)
        //                    {

        //                        Entity e = (Entity)tr.GetObject(asObjId, OpenMode.ForRead);
        //                        if (e is Polyline2d)
        //                        {
        //                            Polyline2d pline = (Polyline2d)e;

        //                            double dArea = pline.Area;
        //                            // maxArea = dArea;
        //                            if (maxArea < dArea)
        //                            {
        //                                maxArea = dArea;
        //                                DBObjectCollection acDbObjColl = pline.GetOffsetCurves(-1);
        //                                foreach (Entity acEntPoly in acDbObjColl)
        //                                {
        //                                    ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
        //                                    acEntPoly.SetLayerId(layerId, true);
        //                                    // acEntPoly.ColorIndex = 3;
        //                                    acblkTblRec.AppendEntity(acEntPoly);
        //                                    tr.AddNewlyCreatedDBObject(acEntPoly, true);
        //                                }

        //                            }

        //                        }
        //                    }

        //                    if (blk.AttributeCollection.Count > 0)
        //                    {
        //                        foreach (ObjectId objIdAttr in blk.AttributeCollection)
        //                        {
        //                            DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
        //                            AttributeReference ar = obj as AttributeReference;
        //                            if (ar != null && ar.Tag.Contains("X"))
        //                            {
        //                                int offset = 0;
        //                                string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
        //                                string[] stemp = attsValue.Split('X');
        //                                if (drawFrame)
        //                                {
        //                                    offset = 0;
        //                                    area = CalCulateWindowArea(stemp, offset);
        //                                }
        //                                else
        //                                {
        //                                    offset = 2;
        //                                    area = CalCulateWindowArea(stemp, offset);
        //                                }
        //                                ///TODO:VLAD WINGEN INSERT ????
        //                                area = 11d;
        //                                // area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
        //                                totalArea = totalArea + area;
        //                            }
        //                        }
        //                    }

        //                    else
        //                    {
        //                        Extents3d extent = blk.GeometricExtents;
        //                        Point3d dMinPoint = extent.MinPoint;
        //                        Point3d dMaxPoint = extent.MaxPoint;
        //                        using (Polyline acPoly = new Polyline())
        //                        {
        //                            acPoly.SetDatabaseDefaults();
        //                            acPoly.AddVertexAt(0, new Point2d(dMinPoint.X, dMinPoint.Y), 0, 0, 0);
        //                            acPoly.AddVertexAt(1, new Point2d(dMaxPoint.X, dMinPoint.Y), 0, 0, 0);
        //                            acPoly.AddVertexAt(2, new Point2d(dMaxPoint.X, dMaxPoint.Y), 0, 0, 0);
        //                            acPoly.AddVertexAt(3, new Point2d(dMinPoint.X, dMaxPoint.Y), 0, 0, 0);
        //                            acPoly.AddVertexAt(4, new Point2d(dMinPoint.X, dMinPoint.Y), 0, 0, 0);
        //                            DBObjectCollection acDbObjColl = acPoly.GetOffsetCurves(1);
        //                            foreach (Entity acEnt1 in acDbObjColl)
        //                            {
        //                                ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
        //                                acEnt1.SetLayerId(layerId, true);
        //                                // Add each offset object
        //                                acblkTblRec.AppendEntity(acEnt1);
        //                                tr.AddNewlyCreatedDBObject(acEnt1, true);
        //                            }
        //                        }

        //                        double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
        //                        double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
        //                        area = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
        //                        area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
        //                        totalArea = totalArea + area;
        //                    }

        //                }

        //            }

        //        }

        //        //foreach (ObjectId objId in acSSet.GetObjectIds())
        //        //{
        //        //    if (objId != null)
        //        //    {
        //        //        double winarea = 0;
        //        //        BlockReference oEnt = (BlockReference)tr.GetObject(objId, OpenMode.ForRead);
        //        //        BlockReference blk = (BlockReference)oEnt;
        //        //        if (blk.AttributeCollection.Count > 0)
        //        //        {
        //        //            foreach (ObjectId objIdAttr in blk.AttributeCollection)
        //        //            {
        //        //                DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
        //        //                AttributeReference ar = obj as AttributeReference;
        //        //                if (ar != null && ar.Tag.Contains("X"))
        //        //                {
        //        //                    int offset = 0;
        //        //                    string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
        //        //                    string[] stemp = attsValue.Split('X');

        //        //                    if (drawFrame)
        //        //                    {
        //        //                        offset = 0;
        //        //                        winarea = CalCulateWindowArea(stemp,offset);
        //        //                    }
        //        //                    else
        //        //                    {
        //        //                        offset = 2;
        //        //                        winarea = CalCulateWindowArea(stemp,offset);
        //        //                    }
        //        //                    winarea = (!unitValue.Equals(4)) ? winarea / 144 : winarea / 25.4 / 25.4 / 144;
        //        //                   // winarea = (winarea * 100 + 0.5) / 100;
        //        //                    totalWinArea = totalWinArea + winarea;
        //        //                }
        //        //            }
        //        //        }
        //        //        else
        //        //        {
        //        //            Extents3d extent = blk.GeometricExtents;
        //        //            double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
        //        //            double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
        //        //             winarea = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
        //        //            winarea = (!unitValue.Equals(4)) ? winarea / 144 : winarea / 25.4 / 25.4 / 144;
        //        //            totalWinArea = totalWinArea + winarea;
        //        //        }


        //        //        }
        //        //        }
        //        tr.Commit();
        //    }
        //    //return 11d;
        //    return totalArea;
        //}
        //This routine is used to calculate total area of glazing
        //windows by using attribute
        public static double GetGlazingArea1(bool drawFrame)
        {
            UnitsValue unitValue = Application.DocumentManager.MdiActiveDocument.Database.Insunits;

            Document acDdoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDdoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] acTypValAr = new TypedValue[] {
                                        new TypedValue((int)DxfCode.Operator, "<or"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE"),
                                        new TypedValue((int)DxfCode.LayerName,"AL-OPGN,AL-OPNG,1 ALL-Elevation Medium" ),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode. Start,"INSERT"),
                                        new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode. Start,"INSERT"),
                                        new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator, "<and"),
                                        new TypedValue((int)DxfCode. Start,"INSERT"),
                                        new TypedValue((int)DxfCode.Operator,"and>"),
                                        new TypedValue((int)DxfCode.Operator,"or>"),
                    };

            double totalArea = 0;
            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.AllowDuplicates = false;
            PromptSelectionResult selRes = acDocEd.GetSelection(opts, acSelFtr);
            if (selRes.Status != PromptStatus.OK)
                return totalArea;
            //acDocEd.WriteMessage("Number of objects selected: 0");


            SelectionSet acSSet = selRes.Value;

            double maxArea = 0;
            using (Transaction tr = acDdoc.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                foreach (SelectedObject acSSObj in acSSet)
                {
                    double area = 0;
                    // Check to make sure a valid SelectedObject object was returned
                    if (acSSObj != null)
                    {
                        Entity acEnt = tr.GetObject(acSSObj.ObjectId,
                                                        OpenMode.ForWrite) as Entity;
                        if (acEnt != null && acEnt is Polyline2d)
                        {
                            Polyline2d acPline = (Polyline2d)acEnt;
                            area = acPline.Area;
                            area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                            totalArea = totalArea + area;
                        }
                        else if (acEnt != null && acEnt is Polyline)
                        {
                            Polyline acPline = (Polyline)acEnt;
                            area = acPline.Area;
                            area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                            totalArea = totalArea + area;

                        }
                        else if ((acEnt != null && acEnt.Equals((BlockReference)acEnt)))
                        {
                            BlockReference blk = tr.GetObject(acEnt.ObjectId, OpenMode.ForRead) as BlockReference;

                            ResultBuffer xd = qACAutils.acSSet.GetXDataByAppName(acEnt.ObjectId, "qrndsgn_AutoWindows");
                            Boolean isWindow = false;
                            if (xd != null && xd.AsArray().Count() > 0) isWindow = true;
                            else
                            {
                                xd = qACAutils.acSSet.GetXDataByAppName(acEnt.ObjectId, "AutoWindows");
                                if (xd != null && xd.AsArray().Count() > 0) isWindow = true;
                            }
                            xd.Dispose();

                            //area = 11d; new file >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                            if (isWindow)
                            {
                                ObjectIdCollection ids = qACAutils.acSSet.GetOffestFromArcs(acEnt.ObjectId);
                                foreach (ObjectId id in ids)
                                {
                                    Polyline p = tr.GetObject(id, OpenMode.ForWrite) as Polyline;
                                    totalArea += (!unitValue.Equals(4)) ? p.Area / 144 : p.Area / 25.4 / 25.4 / 144;
                                    p.Erase();
                                }
                            }

                            else
                            {
                                Extents3d extent = blk.GeometricExtents;
                                double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
                                double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
                                area = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
                                area = (!unitValue.Equals(4)) ? area / 144 : area / 25.4 / 25.4 / 144;
                                totalArea = totalArea + area;
                            }

                        }

                    }

                }

                //foreach (ObjectId objId in acSSet.GetObjectIds())
                //{
                //    if (objId != null)
                //    {
                //        double winarea = 0;
                //        BlockReference oEnt = (BlockReference)tr.GetObject(objId, OpenMode.ForRead);
                //        BlockReference blk = (BlockReference)oEnt;
                //        if (blk.AttributeCollection.Count > 0)
                //        {
                //            foreach (ObjectId objIdAttr in blk.AttributeCollection)
                //            {
                //                DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
                //                AttributeReference ar = obj as AttributeReference;
                //                if (ar != null && ar.Tag.Contains("X"))
                //                {
                //                    int offset = 0;
                //                    string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                //                    string[] stemp = attsValue.Split('X');

                //                    if (drawFrame)
                //                    {
                //                        offset = 0;
                //                        winarea = CalCulateWindowArea(stemp,offset);
                //                    }
                //                    else
                //                    {
                //                        offset = 2;
                //                        winarea = CalCulateWindowArea(stemp,offset);
                //                    }
                //                    winarea = (!unitValue.Equals(4)) ? winarea / 144 : winarea / 25.4 / 25.4 / 144;
                //                   // winarea = (winarea * 100 + 0.5) / 100;
                //                    totalWinArea = totalWinArea + winarea;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            Extents3d extent = blk.GeometricExtents;
                //            double winWidth = (extent.MaxPoint.X - extent.MinPoint.X);
                //            double winHeight = (extent.MaxPoint.Y - extent.MinPoint.Y);
                //             winarea = (extent.MaxPoint.X - extent.MinPoint.X) * (extent.MaxPoint.Y - extent.MinPoint.Y);
                //            winarea = (!unitValue.Equals(4)) ? winarea / 144 : winarea / 25.4 / 25.4 / 144;
                //            totalWinArea = totalWinArea + winarea;
                //        }


                //        }
                //        }
                tr.Commit();
            }
            //return 11d;
            return totalArea;
        }
        public static void drawFrameAroundWindow(ObjectId objId)
        {
            Document acDdoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDdoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction tr = acDdoc.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                BlockReference oEnt = (BlockReference)tr.GetObject(objId, OpenMode.ForRead);
                BlockReference blk = (BlockReference)oEnt;
                Extents3d extent = blk.GeometricExtents;
                Point3d minPoint = extent.MinPoint;
                Point3d maxPoint = extent.MaxPoint;
                using (Polyline acPoly = new Polyline())
                {
                    acPoly.SetDatabaseDefaults();
                    acPoly.AddVertexAt(0, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
                    acPoly.AddVertexAt(1, new Point2d(maxPoint.X, minPoint.Y), 0, 0, 0);
                    acPoly.AddVertexAt(2, new Point2d(maxPoint.X, maxPoint.Y), 0, 0, 0);
                    acPoly.AddVertexAt(3, new Point2d(minPoint.X, maxPoint.Y), 0, 0, 0);
                    acPoly.AddVertexAt(4, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
                    ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                    acPoly.SetLayerId(layerId, true);
                    acblkTblRec.AppendEntity(acPoly);
                    tr.AddNewlyCreatedDBObject(acPoly, true);
                }
                tr.Commit();
            }
        }

        public static double SelectBlockSpatialFromTheScreen(bool drawFrame)
        {
            // UnitsValue unitValue = Application.DocumentManager.MdiActiveDocument.Database.Insunits;

            Document acDdoc = Application.DocumentManager.MdiActiveDocument;
            Database db = acDdoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            TypedValue[] acTypValAr = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue((int)DxfCode. Start,"INSERT"),
                    new TypedValue((int)DxfCode.LayoutName , "Model"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                  //  new TypedValue((int)DxfCode.LayerName,lname ),
                    new TypedValue((int)DxfCode.LayerName,"1 ALL-Elevation Medium" ),
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
                    };

            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.AllowDuplicates = false;
            PromptSelectionResult selRes = acDocEd.GetSelection(opts, acSelFtr);
            if (selRes.Status != PromptStatus.OK)
                return -1;
            SelectionSet acSSet = selRes.Value;
            double totalWinArea = 0;
            List<double> winAreaList = new List<double>();
            using (Transaction tr = acDdoc.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (ObjectId objId in acSSet.GetObjectIds())
                {
                    if (objId != null)
                    {
                        BlockReference oEnt = (BlockReference)tr.GetObject(objId, OpenMode.ForRead);
                        BlockReference blk = (BlockReference)oEnt;
                        ResultBuffer rb = new ResultBuffer();
                        rb = GetXData(objId);
                        string[] stemp = new string[2];
                        TypedValue[] type = rb.AsArray();
                        foreach (TypedValue tVal in rb)
                        {
                            if (tVal.TypeCode.Equals(1001) || tVal.TypeCode.Equals(1000))
                            {
                                string tval = tVal.Value.ToString();
                                if (tval.Equals("qrndsgn_AutoWindows"))
                                {

                                    continue;
                                }
                                else if (tval.Contains('!'))
                                {
                                    stemp = tVal.Value.ToString().Split('!');
                                    break;
                                }
                                if (tval.Equals("AutoWindows"))
                                {
                                    continue;
                                    // TypedValue[] type = rb.AsArray();
                                }
                                else if (tval.Contains('X'))
                                {
                                    stemp = tVal.Value.ToString().Split('X');
                                    break;
                                }
                            }
                        }
                        if (drawFrame == true)
                        {
                            BlockReference brClone = (BlockReference)blk.Clone();
                            DBObjectCollection objs = new DBObjectCollection();
                            brClone.Explode(objs);
                            foreach (DBObject obj in objs)
                            {
                                if (obj is Polyline)
                                {
                                    rb = obj.GetXDataForApplication("qrndsgn_Misc");
                                    //rb = GetXData(obj.ObjectId);
                                    Polyline pline = (Polyline)obj;
                                    if (rb != null)
                                    {

                                        //}
                                        foreach (TypedValue tVal in rb)
                                        {
                                            if (tVal != null && tVal.Value.ToString().Equals("OUTLINE"))
                                            {
                                                // Polyline newPline = pline;
                                                DBObjectCollection acDbObjColl = pline.GetOffsetCurves(-1);
                                                foreach (Entity acEntPoly in acDbObjColl)
                                                {
                                                    ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                                                    acEntPoly.SetLayerId(layerId, true);
                                                    // acEntPoly.ColorIndex = 3;
                                                    acblkTblRec.AppendEntity(acEntPoly);
                                                    tr.AddNewlyCreatedDBObject(acEntPoly, true);
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }

                        ////////////double winArea = 0;
                        ////////////if (!drawFrame)
                        ////////////{
                        ////////////    winArea = double.Parse(stemp[1]);
                        ////////////}
                        ////////////else
                        ////////////{
                        ////////////    winArea = double.Parse(stemp[0]);
                        ////////////}

                        ///////////// winAreaList.Add(winArea);
                        //foreach (TypedValue tVal in rb)
                        //{
                        //    string[] stemp = new string[2];
                        //    if (tVal.TypeCode.Equals(1000) && tVal.Value.ToString().Contains('!'))
                        //    {
                        //        double winArea1 = 0; double winArea2 = 0;

                        //        stemp = tVal.Value.ToString().Split('!');
                        //        winArea1 = double.Parse(stemp[0]);
                        //        winArea2 = double.Parse(stemp[1]);
                        //        if (!drawFrame)
                        //            winAreaList.Add(winArea2);
                        //        else
                        //            winAreaList.Add(winArea1);
                        //    }
                        //    else if (tVal.TypeCode.Equals(1000) && tVal.Value.ToString().Contains('X'))
                        //    {
                        //        int offset = 1;
                        //        stemp = tVal.Value.ToString().Split('X');
                        //        double winArea = CalCulateWindowArea(stemp, offset);
                        //        winAreaList.Add(winArea / 144);

                        //    }
                        //}


                    }
                }
                totalWinArea = winAreaList.Sum();
                // totalWinArea = ConvertInchesToSqrFoot(totalWinArea + winAreaList.Sum());
            }
            //if (!unitValue.Equals(4))
            //{
            //    double dTotalArea = 0;
            //    //'The unit was in milimeter, convert it to square foot
            //    if (unitValue.Equals(0) || unitValue.Equals(4))
            //        dTotalArea = dTotalArea * 144 / 92903.04;

            //}
            return totalWinArea;

        }
        public static double CalCulateWindowArea(string[] winDim, int offSet)
        {
            double area = 1;
            foreach (string dim in winDim)
            {
                string sTemp = dim.Substring(0, 2);
                double side = double.Parse(sTemp);
                area = area * (side - offSet);
            }
            return area;
        }
        public static void drawTable()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                ObjectId idLayer = GetLayerIdFromDb(db, "3 WD-Text");
                PromptPointResult pPtRes;
                PromptPointOptions pPtOpts = new PromptPointOptions("");

                // Prompt for the start point
                pPtOpts.Message = "\nPick a point: ";
                pPtRes = doc.Editor.GetPoint(pPtOpts);
                Point3d ptStart = pPtRes.Value;

                // Exit if the user presses ESC or cancels the command
                if (pPtRes.Status == PromptStatus.Cancel) return;
                //  DrawRectangleInLayerWH returnPnt(0), returnPnt(1), 130#, -44.5, 0#, "3 WD-Text"
            }
        }
        public static void DrawRectangelInLayer(Point3d pt, double w, double v, int a, string s)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Polyline pline = new Polyline();

            //         Dim oPline As AcadLWPolyline
            // Dim points(0 To 7) As Double


            // points(0) = cornerX: points(1) = cornerY
            // points(2) = cornerX + dWidth: points(3) = cornerY
            // points(4) = cornerX + dWidth: points(5) = cornerY + dHeight
            // points(6) = cornerX: points(7) = cornerY + dHeight


            // Set oPline = ThisDrawing.ModelSpace.AddLightWeightPolyline(points)
            // oPline.Layer = sLayername
            // oPline.Closed = True
            // 'oPline.Linetype = "CONTINUOUS"


            // Dim i As Integer
            // For i = 1 To 4
            //     oPline.SetWidth i -1, dLineWidth, dLineWidth
            //Next i
        }

        public static void CreateMyTable(List<string> tblList)
        {
            // based on code written by Kean Walmsley
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                Table tbl = new Table();
                ObjectId idLayer = GetLayerIdFromDb(db, "3 WD-Text");
                if (!idLayer.IsNull)
                    tbl.SetLayerId(idLayer, true);
                ObjectId idTextstyle = GetTextstyleIdFromDb(db, "STANDARD");
                tbl.TableStyle = db.Tablestyle;
                tbl.Position = ed.GetPoint("\nPick a point: ").Value;
                TableStyle ts = (TableStyle)tr.GetObject(tbl.TableStyle, OpenMode.ForRead);
                double textht = ts.TextHeight(RowType.DataRow);
                int rows = 3;
                int columns = 6;
                //insert rows
                tbl.InsertRows(1, textht * 2, rows);
                // insert columns
                tbl.InsertColumns(1, textht * 15, columns);// first column is already exist, thus we'll have 5 columns
                                                           //create range to merge the cells in the first row
                CellRange range = CellRange.Create(tbl, 0, 0, 0, columns);
                tbl.MergeCells(range);

                // set style for title row
                tbl.Cells[0, 0].Style = "Title";
                tbl.Cells[0, 0].TextString = "ALLOWABLE UNPROTECTED OPENINGS";

                tbl.Rows[0].Height = textht * 2;
                tbl.InsertRows(1, textht * 2, 1);
                // set style for header row
                tbl.Rows[1].Style = "Header";
                tbl.Rows[1].Height = textht * 2;
                //create contents in the first cell and set textstring
                tbl.Cells[1, 0].Contents.Add();
                tbl.Cells[1, 0].Contents[0].TextString = "Total Wall Area";
                //tbl.Cells[1, 1].Contents.Add();
                //tbl.Cells[1, 1].Contents[0].TextString = "";
                tbl.Cells[1, 3].Contents.Add();
                tbl.Cells[1, 3].Contents[0].TextString = tblList[0];
                tbl.Cells[1, 4].Contents.Add();
                tbl.Cells[1, 4].Contents[0].TextString = "S.F";
                tbl.Cells[1, 5].Contents.Add();
                tbl.Cells[1, 5].Contents[0].TextString = tblList[1];
                tbl.Cells[1, 6].Contents.Add();
                tbl.Cells[1, 6].Contents[0].TextString = "m2";
                tbl.Cells[2, 0].Contents.Add();
                tbl.Cells[2, 0].Contents[0].TextString = "Limiting Distance";
                tbl.Cells[2, 1].Contents.Add();
                tbl.Cells[2, 1].Contents[0].TextString = tblList[2];
                tbl.Cells[2, 2].Contents.Add();
                tbl.Cells[2, 2].Contents[0].TextString = "S.F";
                tbl.Cells[2, 3].Contents.Add();
                tbl.Cells[2, 3].Contents[0].TextString = tblList[3];
                tbl.Cells[2, 4].Contents.Add();
                tbl.Cells[2, 4].Contents[0].TextString = "m2";
                tbl.Cells[2, 5].Contents.Add();
                tbl.Cells[2, 5].Contents[0].TextString = tblList[4];
                tbl.Cells[2, 6].Contents.Add();
                tbl.Cells[2, 6].Contents[0].TextString = "%";
                tbl.Cells[3, 0].Contents.Add();
                tbl.Cells[3, 0].Contents[0].TextString = "Allowable Openings";
                tbl.Cells[3, 3].Contents.Add();
                tbl.Cells[3, 3].Contents[0].TextString = tblList[5];
                tbl.Cells[3, 4].Contents.Add();
                tbl.Cells[3, 4].Contents[0].TextString = "S.F";
                tbl.Cells[3, 5].Contents.Add();
                tbl.Cells[3, 5].Contents[0].TextString = tblList[6];
                tbl.Cells[3, 6].Contents.Add();
                tbl.Cells[3, 6].Contents[0].TextString = "S.F";
                tbl.Cells[4, 0].Contents.Add();
                tbl.Cells[4, 0].Contents[0].TextString = "Actual openings";
                tbl.Cells[4, 3].Contents.Add();
                tbl.Cells[4, 3].Contents[0].TextString = tblList[7];
                tbl.Cells[4, 4].Contents.Add();
                tbl.Cells[4, 4].Contents[0].TextString = "S.F";
                tbl.Cells[4, 5].Contents.Add();
                tbl.Cells[4, 5].Contents[0].TextString = tblList[8];
                tbl.Cells[4, 6].Contents.Add();
                tbl.Cells[4, 6].Contents[0].TextString = "S.F";
                //tbl.Cells[2, 2].Contents.Add();
                //tbl.Cells[2, 2].Contents[0].TextString = tblList[1];
                //List<string> dataList = new List<string>() { "Toatl Wall Area", "0", "0", tblList[0], "S.F", "97.45" };
                //for (int c = 0; c <= columns; c++)
                //{

                //    //for all of the rest cells just set textstring(or value)
                //    for (int j = 0; j <= dataList.Count; j++)
                //        {
                //            if (c == j)
                //            {
                //                tbl.Cells[1, c].TextString = dataList.ElementAt(j);
                //            }

                //        }
                //}

                //for (int r = 2; r < rows + 2; r++)//exact number of data rows + title row + header row
                //{
                //    // set style for data row
                //    tbl.Rows[r].Style = "Data";
                //    tbl.Rows[r].Height = textht * 1.25;
                //    //create contents in the first cell and set textstring
                //    tbl.Cells[r, 0].Contents.Add();
                //    for (int c = 0; c <= columns; c++)
                //    {
                //        //for all of the rest cells just set textstring (or value)
                //        for (int j = 0; j <= dataList.Count; j++)
                //        {
                //            if (c == j)
                //            {
                //                tbl.Cells[r, c].TextString = dataList.ElementAt(j);
                //            }

                //        }
                //    }
                //}

                tbl.Columns[0].Width = 67;
                tbl.Columns[1].Width = 15.25;
                tbl.Columns[2].Width = 9.5;
                tbl.Columns[3].Width = 23;
                tbl.Columns[4].Width = 12.75;
                tbl.Columns[5].Width = 20.25;
                tbl.Columns[6].Width = 13.25;

                //  foreach (Column col in tbl.Columns)
                //   col.Width = textht * 10;

                tbl.GenerateLayout();
                btr.AppendEntity(tbl);
                tr.AddNewlyCreatedDBObject(tbl, true);
                tr.Commit();


            }
        }




        public static ObjectId GetTextstyleIdFromDb(Database db, string sTextstyle)
        {
            ObjectId textstyleId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = (TextStyleTable)db.TransactionManager.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                TextStyleTableRecord tstr;
                foreach (ObjectId id in tst)
                {
                    tstr = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    if (tstr.Name.ToUpper() == sTextstyle)
                    {
                        textstyleId = id;
                        break;
                    }
                }
                tr.Commit();
            }
            return textstyleId;
        }
        public static ObjectId GetLayerIdFromDb(Database db, string sLayer)
        {
            string sLayerUC = sLayer.ToUpper();
            ObjectId layerId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)db.TransactionManager.GetObject(db.LayerTableId, OpenMode.ForRead);
                LayerTableRecord ltr;

                foreach (ObjectId id in lt)
                {
                    ltr = (LayerTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    if (ltr.Name.ToUpper() == sLayerUC)
                    {
                        layerId = id;
                        break;
                    }
                }
            }
            return layerId;
        }


    }
}

///some useful code for future
//if (drawFrame == true)
//                            {
//                                BlockReference brClone = (BlockReference)blk.Clone();
//DBObjectCollection objs = new DBObjectCollection();
//brClone.Explode(objs);
//                                ResultBuffer rb = new ResultBuffer();
//                               // rb = GetXData(objId);
//                                foreach (DBObject obj in objs)
//                                {
//                                    if (obj is Polyline)
//                                    {
//                                        rb = obj.GetXDataForApplication("qrndsgn_Misc");
//                                        //rb = GetXData(obj.ObjectId);
//                                        Polyline pline = (Polyline)obj;
//                                        if (rb != null)
//                                        {

//                                            //}
//                                            foreach (TypedValue tVal in rb)
//                                            {
//                                                if (tVal != null && tVal.Value.ToString().Equals("OUTLINE"))
//                                                {
//                                                    // Polyline newPline = pline;
//                                                    DBObjectCollection acDbObjColl = pline.GetOffsetCurves(1);
//                                                    foreach (Entity acEntPoly in acDbObjColl)
//                                                    {
//                                                        ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
//acEntPoly.SetLayerId(layerId, true);
//                                                        // acEntPoly.ColorIndex = 3;
//                                                        acblkTblRec.AppendEntity(acEntPoly);
//                                                        tr.AddNewlyCreatedDBObject(acEntPoly, true);
//                                                    }

//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }