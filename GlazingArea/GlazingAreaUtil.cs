using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace qrndGlazingArea
{
    //class Doors
    //{
    //    public string doorName;
    //    public int doorCtr;
    //    public double doorArea;
    //    public ObjectId doorId;
    //    public Point3d doorPosition;
    //    //public double windowCtr;
    //    //public List<Window> windows;
        
    //}
    class WindowArea
    {
       // public string projectNo;
        //public string modelNo;
        public string parentBlock;
        public int WindowCtr;
        
        //public string WindowDim;
        
        public double totalArea;
        public List<Window> windows;

        public WindowArea()
        {
            windows = new List<Window>();
        }
    }

    class Window
    {
        public ObjectId winId;
        public string WindowName;
        public double WinArea;
    }

    class PageLimitBlock
    {
        public Point3d pageLimitBlkPosition;
        public string pageLimitBlkName;
        public ObjectId id;
        public Point3d minPt;
        public Point3d maxpt;
       
    }
  
    class TitleBlock
    {
        public string title;
        public string visibility;
        public ObjectId id;
        public Point3d location;
        public string layername;
    }
    class GlazingAreaUtil
    {

        public string GetProjectNo()
        {
            string sFilename, sPrjno = "";
            Document doc = Application.DocumentManager.MdiActiveDocument;
            sFilename = doc.Name;
            int iPos = sFilename.LastIndexOf("\\");
            if (iPos>-1)
            {
                sFilename = sFilename.Substring(iPos + 1);
                iPos = sFilename.IndexOf("-");
                if (iPos>0)
                {
                    sPrjno = sFilename.Substring(0, iPos).Trim();
                }
            }
            return sPrjno;
        }
        public bool GetPrjidModelno(out string sPrjno,out string sModelno)
        {
            sPrjno = "";
            sModelno = "";
            Document doc = Application.DocumentManager.MdiActiveDocument;
            string sFilename = doc.Name.ToUpper();
            // Get rid of directory string in the filename
            int iPos = sFilename.LastIndexOf("\\");
            if (iPos > -1)
                sFilename = sFilename.Substring(iPos + 1);
           
            //"12019-TH18-1-HL.DWG" => "12019-TH18-1-HL"
            iPos = sFilename.LastIndexOf(".DWG");
            if (iPos > -1) 
                sFilename = sFilename.Substring(0, iPos);
            // Get the info required
            // "12019-TH18-1-HL" sPrjno: 12019; sModelno: TH18-1
            iPos = sFilename.IndexOf("-");
            if (iPos == -1)
                return false;
            int iPos2 = sFilename.LastIndexOf("-");
            if (iPos2 == iPos)
            {
                sPrjno = sFilename.Substring(0, iPos);
                sModelno = sFilename.Substring(iPos + 1);
            }
            else if (iPos2 >= iPos)
            {
                sPrjno = sFilename.Substring(0, iPos);
                sModelno = sFilename.Substring(iPos + 1, iPos2 - iPos - 1);
            }
            else
                return false;
            return true;
        }

        public void removeItem()
        {
            List<WindowArea> windowsarea = new List<WindowArea>();
            windowsarea = GetMSpaceWindowsBlock();
           // ObjectId objId = SelectObjectOnScreen();
            foreach (WindowArea wArae in windowsarea)
            {
               //if( wArae.winId.Contains(objId))
               // {
                    
               // }
            }

        }
        //public Dictionary<string,double> AssignValues(Dictionary<string,double> dict)
        public void AssignValues(string ename,ref double fArea, ref double rArea, ref double riArea, ref double lArea)
        {
           double warea = 0;
            List<WindowArea> winarea = GetMSpaceWindowsBlock();
            double dArea= GetMspaceSlidingDoorArea();
            foreach (WindowArea wArea in winarea)
            {
                string sTemp = wArea.parentBlock;
                
                //for (int i = 0; i < sTemp.Length; i++)
                //{
                //    sTemp = sTemp.Remove(i);
                //}
                int idx = sTemp.IndexOf("SIDE");
               
                if (idx>0)
                {
                   sTemp= sTemp.Remove(idx-1,5);
                }
                sTemp = sTemp.Replace(" ", "");
                if (sTemp.EndsWith(ename)||sTemp.Contains(ename))
                {

                    // } 
                    // sTemp.Remove(0, 1);

                    //if (dict.ContainsKey(sTemp))
                    //{
                    //    dict[sTemp] = wArea.WinArea;
                    //}
                    switch (sTemp.Trim())
                    {
                        case "FRONTELEVATION'A'":
                            fArea = wArea.totalArea;
                            break;
                        case "FRONTELEVATION'B'":
                            fArea = wArea.totalArea;
                            break;
                        case "REARELEVATION'A'&'B'":
                            rArea = wArea.totalArea+dArea;
                            break;
                        case "REARELEVATION'A','B'&'C'":
                            rArea = wArea.totalArea+dArea ;
                            break;
                        case "REARELEVATION'A','B','C'&'D'":
                            rArea = wArea.totalArea+dArea;
                            break;
                        case "RIGHTELEVATION'A'":
                            riArea = wArea.totalArea;
                            break;
                        case "RIGHTELEVATION'B'":
                            riArea = wArea.totalArea;
                            break;
                        case "LEFTELEVATION'A'":
                            lArea = wArea.totalArea;
                            break;
                        case "LEFTELEVATION'B'":
                            lArea = wArea.totalArea;
                            break;
                        default:
                            warea = 0;
                            break;
                    }
                }
            }
           // return dict;
        }

        public double GetMspaceSlidingDoorArea()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            double area = 0.0;
            Point3d dMinPoint = new Point3d();
            Point3d dMaxPoint = new Point3d();
            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
            using (DocumentLock acLock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {

                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                   //string blkName = "Altwood Garage Door - 8'";
                   //  string blkName = "Door - Single";
                     string blkName = "Sliding Door";
                    foreach (var btrId in bt)
                    {
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
                        acPoly.SetLayerId(layerId, true);
                        acblkTblRec.AppendEntity(acPoly);
                        tr.AddNewlyCreatedDBObject(acPoly, true);
                    }
                    tr.Commit();
                }
            }
            return area;
        }
        public List<WindowArea> GetMSpaceWindowsBlock()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
          //  ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
            List<PageLimitBlock> newPageLimitBlocks = CreateNewPageLimitBlock();
            List<Point3d> position = new List<Point3d>();
            ObjectId objId = ObjectId.Null;
            List<WindowArea> windowsarea = new List<WindowArea>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
               
                // ed.WriteMessage("\nTotal Windows in\t:" + set.Count);
                TypedValue[] flt = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue(0,"INSERT"),
                    new TypedValue(410, "Model"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.BlockName,"*win" ),
                    new TypedValue((int)DxfCode.BlockName,"*-rnWindows" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
            };


                SelectionFilter filter = new SelectionFilter(flt);
                PromptSelectionOptions opts = new PromptSelectionOptions();
                PromptSelectionResult selRes = ed.SelectAll(filter);
                SelectionSet set = selRes.Value;

                foreach (PageLimitBlock nPgBlk in newPageLimitBlocks)
                {
                    int windowCount = 0;
                    double glazingArea = 0;
                    WindowArea winarea = new WindowArea(); 

                    foreach (ObjectId id in set.GetObjectIds())
                    {
                        BlockReference oEnt = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        BlockReference blk = (BlockReference)oEnt;
                        if ((nPgBlk.minPt.X <= blk.Position.X && blk.Position.X <= nPgBlk.maxpt.X) && (nPgBlk.minPt.Y <= blk.Position.Y && blk.Position.Y <= nPgBlk.maxpt.Y))
                        {
                            //blkName = blk.Name;
                           // objId = blk.Id;
                            windowCount++;
                            Window myWindow = new Window();
                            winarea.windows.Add(myWindow);
                            myWindow.winId = blk.Id;
                            myWindow.WindowName = blk.Name;
                            Extents3d extent = blk.GeometricExtents;
                            Point3d minPoint = extent.MinPoint;
                            Point3d maxPoint = extent.MaxPoint;
                            //using (Polyline acPoly = new Polyline())
                            //{
                            //    acPoly.SetDatabaseDefaults();
                            //    acPoly.AddVertexAt(0, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
                            //    acPoly.AddVertexAt(1, new Point2d(maxPoint.X, minPoint.Y), 0, 0, 0);
                            //    acPoly.AddVertexAt(2, new Point2d(maxPoint.X, maxPoint.Y), 0, 0, 0);
                            //    acPoly.AddVertexAt(3, new Point2d(minPoint.X, maxPoint.Y), 0, 0, 0);
                            //    acPoly.AddVertexAt(4, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
                            //    acPoly.SetLayerId(layerId, true);
                            //    //  acPoly.Layer = "4 AREA-Glazing Area";

                            //    //acPoly.ColorIndex = 3;
                            //    acblkTblRec.AppendEntity(acPoly);
                            //    tr.AddNewlyCreatedDBObject(acPoly, true);
                            //}

                            if (blk.AttributeCollection.Count > 0)
                        {
                            foreach (ObjectId objIdAttr in blk.AttributeCollection)
                            {
                                DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
                                AttributeReference ar = obj as AttributeReference;
                                if (ar != null && ar.Tag.Contains("X"))
                                {
                                    string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                                    string length = string.Empty;
                                    string width = string.Empty;
                                    List<string> dimofDiffWindows = new List<string>();
                                    if (attsValue.Length > 0)
                                    {
                                        int pos = attsValue.IndexOf("X");
                                        if (pos > -1)
                                        {
                                            length = attsValue.Substring(0, pos);
                                            width = attsValue.Substring(pos + 1, 2);
                                        }

                                        //attsValue = length + "X" + width;
                                    }
                                        // dimofDiffWindows.Add(attsValue);
                                        // glazingArea = FindDimAndAreaOfWindows(dimofDiffWindows);
                                        // myWindow.WinArea = glazingArea;
                                         myWindow.WinArea = Convert.ToDouble(length) * Convert.ToDouble(width);
                                         winarea.totalArea = winarea.totalArea +myWindow.WinArea;

                                        // winarea.totalArea = winarea.totalArea + glazingArea;
                                    }

                                }

                        }
                    }
                      
                    }
                        ed.WriteMessage("\n"+nPgBlk.pageLimitBlkName + " has " + windowCount + " windows having total area: "+glazingArea+" S.F");
                        winarea.parentBlock = nPgBlk.pageLimitBlkName;                        
                        winarea.WindowCtr = windowCount;                                               
                        windowsarea.Add(winarea);

                }
                tr.Commit();
                drawPolyLineAroundWindowsbyExploding();
            }
            return windowsarea;
        }

        public List<PageLimitBlock> CreateNewPageLimitBlock()
        {
            int count = 0;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Point3dCollection ptCol1 = new Point3dCollection();
            Point3dCollection ptCol2 = new Point3dCollection();
            //Dictionary<string, Point3d> dicmText = GetMSpaceText();
            // Dictionary<string, Point3d> dic = new Dictionary<string, Point3d>();
            List<TitleBlock> titleBlocks = GetMspaceTitleBlocks();
            List<PageLimitBlock> pagelimitblocks = GetMspacePageLimitBlock();
            List<PageLimitBlock> newPagelimitbloks = new List<PageLimitBlock>();
            foreach (PageLimitBlock pblk in pagelimitblocks)
            {
                //if (!pblk.pageLimitBlkName.Contains("ELEVATION"))
                //    continue;

                
                Point3d tempPtX = new Point3d(pblk.minPt.X,pblk.maxpt.X,0);
                Point3d tempPtY = new Point3d(pblk.minPt.Y, pblk.maxpt.Y, 0);
                foreach (TitleBlock tblk in titleBlocks)
                {
                    if (tblk.title.Contains("ELEVATION"))
                    {

                        //  }
                        if ((pblk.minPt.X <= tblk.location.X && tblk.location.X <= pblk.maxpt.X) && (pblk.minPt.Y <= tblk.location.Y && tblk.location.Y <= pblk.maxpt.Y))
                        {
                            count++;
                            PageLimitBlock pgBlock = new PageLimitBlock();
                            pgBlock.pageLimitBlkName = tblk.title;
                            // pgBlock.pageLimitBlkName = string.Concat(pblk.pageLimitBlkName, tblk.title);
                            pgBlock.minPt = pblk.minPt;
                            pgBlock.maxpt = pblk.maxpt;
                            pgBlock.id = pblk.id;
                            pgBlock.pageLimitBlkPosition = pblk.pageLimitBlkPosition;
                            newPagelimitbloks.Add(pgBlock);
                        }
                    }
                }
            }
            return newPagelimitbloks;
        }
        public static Dictionary<string,Point3d> GetMSpaceText()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            Dictionary<string, Point3d> dicMtext = new Dictionary<string, Point3d>();
            string tContent="" ;
            Point3d tLocation = new Point3d();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrMspace = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                foreach (ObjectId id in btrMspace)
                {
                    switch (id.ObjectClass.DxfName.ToUpper())
                    {
                        case "TEXT":
                            DBText text = (DBText)tr.GetObject(id, OpenMode.ForRead);
                            break;
                        case "MTEXT":
                            MText mtext = (MText)tr.GetObject(id, OpenMode.ForWrite);
                            if (mtext.Contents.Contains("ELEVATION"))
                            {
                                tContent = mtext.Contents;
                                tLocation = mtext.Location;
                                dicMtext[tContent] = tLocation;
                            }
                            break;
                        default:
                            break;
                    }
                }
                tr.Commit();
            }
            return dicMtext;
        }
        public List<PageLimitBlock> GetMspacePageLimitBlock()
        {
            List<PageLimitBlock> pageLimitBlocks = new List<PageLimitBlock>();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            // int midPtX = 0;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrMspace = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                foreach (ObjectId id in btrMspace)
                {
                    if (id.ObjectClass.DxfName == "INSERT")
                    {
                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        if (br.IsDynamicBlock)
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead);

                            if (btr.Name.ToUpper().Trim().StartsWith("PAGE LIMITS"))
                            {
                                PageLimitBlock pagelimitblock = new PageLimitBlock();
                                Extents3d extents = br.GeometricExtents;
                                //midPtX=Convert.ToInt32((extents.MinPoint.X+extents.MaxPoint.X/2));
                                pagelimitblock.minPt = extents.MinPoint;
                                pagelimitblock.maxpt = extents.MaxPoint;
                                pagelimitblock.id = id;
                                pagelimitblock.pageLimitBlkPosition = br.Position;
                                pagelimitblock.pageLimitBlkName = btr.Name;
                                pageLimitBlocks.Add(pagelimitblock);
                            }
                        }
                    }
                   // pageLimitBlocks.Add(pagelimitblock);
                }
               
                tr.Commit();
            }
            return pageLimitBlocks;
        }

        public List<TitleBlock> GetMspaceTitleBlocks()
        {
            List<TitleBlock> titleBlocks = new List<TitleBlock>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrMspace = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                foreach (ObjectId id in btrMspace)
                {
                    if (id.ObjectClass.DxfName != "INSERT")
                        continue;
                    BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                    if (!br.IsDynamicBlock)
                        continue;

                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead);
                    if (btr.Name.ToUpper().Trim() != "TITLE-PRIMARY1" )
                        continue;

                    TitleBlock titleBlock = new TitleBlock();
                    titleBlock.id = id;
                    titleBlock.location = br.Position;

                    titleBlock.visibility = "";
                    DynamicBlockReferencePropertyCollection pc = br.DynamicBlockReferencePropertyCollection;
                    foreach (DynamicBlockReferenceProperty prop in pc)
                    {
                        if (prop.PropertyName == "Visibility1")
                            titleBlock.visibility = prop.Value.ToString();
                    }

                    titleBlock.title = "";
                    AttributeCollection attCol = br.AttributeCollection;
                    foreach (ObjectId attId in attCol)
                    {
                        AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                        if (attRef.Tag.ToUpper().Trim() == "TITLE")
                            titleBlock.title = attRef.TextString.ToUpper().Trim();
                    }
                    titleBlock.layername = br.Layer;
                    if (titleBlock.layername.Equals("3 WD-Text"))
                    {
                        titleBlocks.Add(titleBlock);
                    }
                    //titleBlocks.Add(titleBlock);
                }
                tr.Commit();
            }
            return titleBlocks;
        }
        
        public List<string> GetElevation()
        {
            
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            //int count = 0;
            Dictionary<string, string> blkAtt = new Dictionary<string, string>();
            List<string> totalElevationInDWG = new List<string>();
            using (Transaction tr=db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrMSpace = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                foreach (ObjectId id in btrMSpace)
                {
                    if (id.ObjectClass.DxfName != "INSERT")
                        continue;
                    BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                    if (!br.IsDynamicBlock)
                        continue;
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead);
                    if (btr.Name.ToUpper().Trim() == "TITLE-PRIMARY1")
                    {
                        //count++;
                        ObjectId objId = br.ObjectId;
                        blkAtt = GetBlockAtts(objId);
                        string attVal;
                        int pos = 0;
                        if (blkAtt.TryGetValue("TITLE", out attVal) || blkAtt.TryGetValue("Title", out attVal))
                        {
                            if (attVal.Contains("REAR ELEVATION 'A' & 'B'"))
                            {
                                totalElevationInDWG.Add("A");
                                totalElevationInDWG.Add("B");
                            }
                            else if (attVal.Contains("REAR ELEVATION 'A', 'B' & 'C'"))
                            {
                                totalElevationInDWG.Add("A");
                                totalElevationInDWG.Add("B");
                                totalElevationInDWG.Add("C");
                            }
                            else if (attVal.Contains("REAR ELEVATION 'A' , 'B' , 'C' & 'D'"))
                            {
                                totalElevationInDWG.Add("A");
                                totalElevationInDWG.Add("B");
                                totalElevationInDWG.Add("C");
                                totalElevationInDWG.Add("D");
                            }

                           
                        }
                        //if (pos>0)
                        //{
                        //    string temp = attVal.Substring(pos);
                        //    string[] parts = temp.Split(',');
                        //    foreach (string part in parts)
                        //    {
                        //        string stemp = part.Trim();
                        //        stemp = stemp.Substring(1, stemp.Length - 2);

                        //        if (stemp.Contains("&"))
                        //        {
                        //            string[] pparts = stemp.Split('&');
                        //            totalElevationInDWG.Remove(part);
                        //            foreach (string item in pparts)
                        //            {
                        //                string sTemp= item.Trim();
                        //                sTemp = sTemp.Substring(1, sTemp.Length - 2);  //remove first character from the string
                        //                totalElevationInDWG.Add(sTemp);
                        //            }
                        //        }
                        //        totalElevationInDWG.Add(stemp);
                        //    }
                        //}
                    }
                }
                
              
            }
            return totalElevationInDWG;
        }

     
        public void getWinXdata()
        {
            Document doc =Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            PromptEntityOptions opt = new PromptEntityOptions("\nSelect entity: ");
            PromptEntityResult res =ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                Transaction tr = doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    DBObject obj = tr.GetObject(res.ObjectId, OpenMode.ForRead);
                    ResultBuffer rb = obj.XData;
                    if (rb == null)
                    {
                        ed.WriteMessage("\nEntity does not have XData attached.");
                    }
                    else
                    {
                        int n = 0;
                        foreach (TypedValue tv in rb)
                        {
                            ed.WriteMessage("\nTypedValue {0} - type: {1}, value: {2}", n++, tv.TypeCode, tv.Value);
                        }
                        rb.Dispose();
                    }
                }
            }
        }
        public void ListBlocks()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var modelSpace = (BlockTableRecord)tr.GetObject(
                    SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

                var brclass = RXObject.GetClass(typeof(BlockReference));
                var blocks = modelSpace
                    .Cast<ObjectId>()
                    .Where(id => id.ObjectClass == brclass)
                    .Select(id => (BlockReference)tr.GetObject(id, OpenMode.ForRead))
                    .GroupBy(br => ((BlockTableRecord)tr.GetObject(
                        br.DynamicBlockTableRecord, OpenMode.ForRead)).Name.StartsWith("P"));

                foreach (var group in blocks)
                {
                  
                    ed.WriteMessage($"\n{group.Key}: {group.Count()}");
                }
                tr.Commit();
            }
        }
        public static Dictionary<string, string> GetBlockAtts(ObjectId objId)
        {
            Dictionary<string, string> attDict = new Dictionary<string, string>();
            Document acNewDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDbNewDoc = acNewDoc.Database;
            using (DocumentLock acLckDoc = acNewDoc.LockDocument())
            {

                using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())
                {
                    Entity acEnt = (Entity)acTrans.GetObject(objId, OpenMode.ForRead);
                    BlockReference blk = (BlockReference)acEnt;
                    foreach (ObjectId objIdAttr in blk.AttributeCollection)
                    {
                        DBObject obj = acTrans.GetObject(objIdAttr, OpenMode.ForRead);
                        AttributeReference ar = obj as AttributeReference;

                        if (ar != null)
                        {
                            string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                            attDict.Add(ar.Tag, attsValue);
                        }
                    }
                }
            }
            return attDict;
        }

        public  void GetNameAndPosition(ObjectIdCollection blockIds)
        {
           // PageLimit pagelimits = new PageLimit();
            Document acNewDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDbNewDoc = acNewDoc.Database;
            using (DocumentLock acLckDoc = acNewDoc.LockDocument())
            {
                using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId objId in blockIds)
                    {
                        Entity acEnt = (Entity)acTrans.GetObject(objId, OpenMode.ForRead);
                        BlockReference blk = (BlockReference)acEnt;
                       // pagelimits.position = blk.Position;
                       // pagelimits.PageBlock = blk.Name;
                    }
                }
            }
           
        }
        public void countBlocks()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ObjectIdCollection blockIds = new ObjectIdCollection();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable BT = (BlockTable)tr.GetObject(db.BlockTableId,OpenMode.ForRead);
                TypedValue[] filterlist = new TypedValue[1];
                filterlist[0] = new TypedValue(0, "INSERT");
                SelectionFilter filter = new SelectionFilter(filterlist);
                PromptSelectionResult selRes = ed.SelectAll(filter);
                if (selRes.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("\nerror in getting the selectAll");
                    return;

                }
                //get the modelspace
                ObjectId ModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);
                foreach (ObjectId id in selRes.Value.GetObjectIds())
                {
                    BlockReference blockRef =tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                    if (blockRef.OwnerId != ModelSpaceId)
                        continue;
                    //note, no special case for dynamic blocks..
                    //please add extra checks if required for dynamic blocks.
                    if (!blockIds.Contains(blockRef.BlockTableRecord))
                        blockIds.Add(blockRef.BlockTableRecord);
                }
                tr.Commit();

            }
            ed.WriteMessage("Unqiue block reference in ModelSpace : "+ blockIds.Count.ToString() + "\n");

        }
        public void BlockrefCount()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            string blkName = "Altwood Garage Door - 8'";
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                if (!bt.Has(blkName))
                {
                    ed.WriteMessage("\nCannot find block called \"{0}\".", blkName);

                }
                else
                {
                    BlockTableRecord block = tr.GetObject(bt[blkName], OpenMode.ForRead) as BlockTableRecord;
                    ObjectIdCollection ids = block.GetBlockReferenceIds(true, true);
                    ed.WriteMessage("Number of reference is "+ ids.Count.ToString() + "\n");
                }
                tr.Commit();
            }
        }
        
        public void GetNumOfTitleBlocks()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            int count = 0;
           // string blkName = "Altwood Garage Door - 8'";
             string blkName = "Page Limits";
            using (Transaction tr=db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId,OpenMode.ForRead);
                if (!bt.Has(blkName))
                {
                    ed.WriteMessage("\nCannot find block called \"{0}\".", blkName);
                    
                }
                ObjectId ModelSpaceId =SymbolUtilityServices.GetBlockModelSpaceId(db);
                foreach (var btrId in bt)
                {
                    BlockTableRecord block = tr.GetObject(bt[blkName], OpenMode.ForRead) as BlockTableRecord;
                    if (!block.IsDynamicBlock)
                    {
                        continue;
                    }
                    var anonymousIds = block.GetAnonymousBlockIds();
                    var dynBlockRefs = new ObjectIdCollection();

                    foreach (ObjectId anonymousBtrId in anonymousIds)
                    {
                        var anonymousBtr = (BlockTableRecord)tr.GetObject(anonymousBtrId, OpenMode.ForRead);
                        var blockRefIds = anonymousBtr.GetBlockReferenceIds(true, true);

                        foreach (ObjectId id in blockRefIds)
                        {
                            
                            dynBlockRefs.Add(id);
                            count = dynBlockRefs.Count;
                        }
                    }
                    ed.WriteMessage("Number of reference is " + count.ToString() + "\n");
                    BlockReference blk;
                  
                  
                    if (count > 0)
                    {
                        foreach (ObjectId id in dynBlockRefs)
                        {
                            //PageLimit pagelimits = new PageLimit();
                            BlockReference oEnt = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                            blk = (BlockReference)oEnt;
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(blk.DynamicBlockTableRecord, OpenMode.ForRead);
                            if (blk.OwnerId != ModelSpaceId)
                            {
                                continue;
                            }
                            else
                            {
                                //pagelimits.PageBlock = btr.Name;
                               // pagelimits.position = blk.Position;
                                //pagelimitcoll.Add(pagelimits);
                            }

                        }
                    }
                }
                tr.Commit();
             
            }
           // return blockIds;
        }
        public List<string> selectElevation()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            List<string> elevationName = new List<string>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
             
                string blkName = "Title-Primary1";
               
                if (!bt.Has(blkName) )
                {
                    ed.WriteMessage("\nCannot find block called \"{0}\".", blkName);
                    
                }
                foreach (var btrId in bt)
                {
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
                         count=dynBlockRefs.Count;
                    }
                }
                  
                    BlockReference blk;
                    string attsValue="";
                    if (count > 0)
                    {
                        foreach (ObjectId id in dynBlockRefs)
                        {
                            BlockReference oEnt = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                            blk = (BlockReference)oEnt;
                            if (blk.AttributeCollection.Count>0)
                            {
                                foreach (ObjectId objIdAttr in blk.AttributeCollection)
                                {
                                    DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
                                    AttributeReference ar = obj as AttributeReference;
                                    if (ar != null && ar.Tag.Contains("Title"))
                                    {
                                         attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                                    }
                                    elevationName.Add(attsValue);
                                }
                            }

                        }
                    }
                }
                

                tr.Commit();
            }
            return elevationName;
        }
    

        public static double SelectedObjectonscreen()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
         
            string length=string.Empty;
            string width=string.Empty;
           
            List<string> dimofDiffWindows = new List<string>();
            double glazingArea = 0.0;
            using (Transaction tr =db.TransactionManager.StartTransaction())
                {
                    BlockTable BT =(BlockTable)tr.GetObject(db.BlockTableId,OpenMode.ForRead);
                    TypedValue[] filterlist1 = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue(0,"INSERT"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.BlockName,"*win" ),
                    new TypedValue((int)DxfCode.BlockName,"*-rnWindows" ),
                    new TypedValue((int)DxfCode.BlockName,"Altwood Garage Door - 8'" ),
                    new TypedValue((int)DxfCode.BlockName,"Door - Single" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
            };

                SelectionFilter filter1 = new SelectionFilter(filterlist1);
                PromptSelectionOptions opts =new PromptSelectionOptions();
                PromptSelectionResult selRes = ed.GetSelection(opts, filter1);
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                if (selRes.Status != PromptStatus.OK)
                    return -1;
                        SelectionSet set = selRes.Value;
                        foreach (ObjectId id in set.GetObjectIds())
                        {
                            BlockReference oEnt =(BlockReference)tr.GetObject(id,OpenMode.ForRead);
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
                    if (blk.AttributeCollection.Count>0)
                        {
                            foreach (ObjectId objIdAttr in blk.AttributeCollection)
                            {
                                DBObject obj = tr.GetObject(objIdAttr, OpenMode.ForRead);
                                AttributeReference ar = obj as AttributeReference;
                                if (ar!=null && ar.Tag.Contains("X"))
                                {  
                                    string attsValue = (ar.IsMTextAttribute) ? ar.MTextAttribute.Contents : ar.TextString;
                              
                                if (attsValue.Length>0)
                                {
                                    int pos = attsValue.IndexOf("X");
                                    if (pos>-1)
                                    {
                                         length = attsValue.Substring(0, pos);
                                         width = attsValue.Substring(pos + 1, 2);
                                    }
                                  
                                    attsValue = length + "X" + width;
                                }
                                dimofDiffWindows.Add(attsValue);
                                   
                                }
                              
                            }
                          
                        }
                       

                    }
                     
                glazingArea= FindDimAndAreaOfWindows(dimofDiffWindows);
                tr.Commit();

                }
            return glazingArea;
            }
        public double FindAreaOfWindow(string length,string width)
        {
            double area = 1;
            return area;
        }
        public static double FindDimAndAreaOfWindows(List<string> attVals)
        {
            double area = 1;
            double totalarea = 0.0;
            foreach (string attVal in attVals)
            {
                string[] dim = attVal.Split('X');
                for (int i = 1; i <= dim.Length; i++)
                {
                    area = area*double.Parse(dim[i - 1]) ;
                    
                }
                totalarea = totalarea + area;
               // area = 1;
                //totalarea = Math.Round(totalarea / 144,2);
            }
            return totalarea;
           
           
           
        }
        public void SelectObjectOnScreen(ref double polyLineArea)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

            TypedValue[] acTypValAr = new TypedValue[] {new TypedValue((int)DxfCode.Start, "POLYLINE,LWPOLYLINE") };
            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
           
           // ObjectId objId = ObjectId.Null;
            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
               
                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt;
                    acSSPrompt = acDoc.Editor.GetSelection(acSelFtr);

                if (acSSPrompt.Status != PromptStatus.OK)
                    return;

                // If the prompt status is OK, objects were selected
                SelectionSet acSSet = acSSPrompt.Value;
                // Step through the objects in the selection set
                foreach (SelectedObject acSSObj in acSSet)
                {
                    // Check to make sure a valid SelectedObject object was returned
                    if (acSSObj != null)
                    {
                        // Open the selected object for write
                        Entity acEnt = acTrans.GetObject(acSSObj.ObjectId,
                                                         OpenMode.ForWrite) as Entity;

                        if (acEnt!=null && acEnt is Polyline2d)                         //(acEnt != null && acEnt.Equals((Polyline2d)acEnt))
                        {
                            Polyline2d acPline = (Polyline2d)acEnt;
                            //Polyline2d acPline = acTrans.GetObject(acEnt.ObjectId, OpenMode.ForRead) as Polyline2d;
                            LayerTableRecord ltr = (LayerTableRecord)acTrans.GetObject(acPline.LayerId, OpenMode.ForRead);
                            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                            string lname = ltr.Name;
                            if (ltr.ObjectId != layerId)
                                continue;
                            polyLineArea =polyLineArea+ acPline.Area;
                            acPline.ColorIndex=1;
                        }
                        else if (acEnt != null && acEnt is Polyline)                                               //(acEnt != null && acEnt.Equals((Polyline)acEnt))
                        {
                            Polyline acPline = (Polyline)acEnt;
                            //Polyline acPline = acTrans.GetObject(acEnt.ObjectId, OpenMode.ForRead) as Polyline;
                            LayerTableRecord ltr = (LayerTableRecord)acTrans.GetObject(acPline.LayerId, OpenMode.ForRead);
                            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
                            string lname = ltr.Name;
                            if (ltr.ObjectId != layerId)
                                continue;
                            polyLineArea = polyLineArea+acPline.Area;
                            acPline.ColorIndex=1;
                        }
                        //if (acEnt != null && acEnt.Equals((BlockReference)acEnt))
                        //{
                        //    BlockReference blk = acTrans.GetObject(acEnt.ObjectId, OpenMode.ForRead) as BlockReference;

                        //    objId = blk.ObjectId;
                        //}
                       
                    }
                }
                acTrans.Commit();
            }
           // return objId;
        }
        public List<string> GetTitleBlockInfo()
        {
            List<string> projectInfo = new List<string>();
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
            string sModelno = "";string sPrjname = "";string sLocation = "";
            string sMarketname = "";string sClient = "";string sPrjno = "";
            using (DocumentLock dlock=acDoc.LockDocument())
            {
                using (Transaction tr=acCurDb.TransactionManager.StartTransaction())
                {
                    try
                    {
                        DBDictionary layoutDic = tr.GetObject(acCurDb.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary;
                        foreach (DBDictionaryEntry entry in layoutDic)
                        {
                            Layout layout = tr.GetObject(entry.Value, OpenMode.ForRead) as Layout;
                            if (layout.LayoutName=="A0")
                            {
                                BlockTableRecord btrLyt = (BlockTableRecord)tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead);
                                foreach (ObjectId id in btrLyt)
                                {
                                    if (id.ObjectClass.DxfName=="INSERT")
                                    {
                                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);
                                        //btr.HasAttributeDefinitions
                                        foreach (ObjectId idEmbedded in btr)
                                        {
                                            if (idEmbedded.ObjectClass.DxfName=="INSERT")
                                            {
                                                BlockReference brEmbedded = (BlockReference)tr.GetObject(idEmbedded, OpenMode.ForRead);
                                                AttributeCollection attColEmbedded = brEmbedded.AttributeCollection;
                                                foreach (ObjectId attId in attColEmbedded)
                                                {
                                                    AttributeReference attRefEmbedded = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                                                    string str = attRefEmbedded.Tag.ToUpper();
                                                    if (str == "MODEL")
                                                       sModelno = attRefEmbedded.TextString;
                                                    else if (str == "CLIENT")
                                                        sClient = attRefEmbedded.TextString;
                                                    else if (str == "PROJECT")
                                                        sPrjname = attRefEmbedded.TextString;
                                                    else if (str == "LOCATION")
                                                        sLocation = attRefEmbedded.TextString;
                                                    else if (str == "PROJECT#")
                                                        sPrjno = attRefEmbedded.TextString;
                                                    else if (str == "MARKETINGNAME") // "MODELNAME"
                                                        sMarketname = attRefEmbedded.TextString;
                                                }

                                                projectInfo.Add(sPrjname);
                                                projectInfo.Add(sPrjno);
                                                projectInfo.Add(sModelno);
                                                projectInfo.Add(sClient);
                                                projectInfo.Add(sLocation);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        acDocEd.WriteMessage("\nProblem due to " + ex.Message);
                        //throw;
                    }
                    finally { tr.Dispose(); }
                }
            }
            return projectInfo;
        }
        public void calculatePlineArea(SelectionSet acSSet)
        {
            foreach (SelectedObject acSSObj in acSSet)
            {
                if (acSSObj!=null)
                {
                    ObjectId objId = acSSObj.ObjectId;
                }
            }
        }
        public void  CalculatePolylineArea(Entity acEnt)
        {
            Polyline pLine = (Polyline)acEnt;
            double area = pLine.Area;

        }
        public void SetActiveLayout(string sLayoutname)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            LayoutManager layoutMgr = LayoutManager.Current;
            using (DocumentLock doclock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        layoutMgr.CurrentLayout = sLayoutname;
                        tr.Commit();
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        doc.Editor.WriteMessage("\nProblem due to " + ex.Message);
                    }
                    finally
                    {
                        tr.Dispose();
                    }
                }
            }
        }

        // code to find a block by name in Model Space
        public List<ObjectId> GetMspaceDoorsBlockRefCount()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId objId = ObjectId.Null;
            int count = 0;

            List<ObjectId> objIDs = new List<ObjectId>();
            string blkName = "Door - Single";
            string blkName2 = "Sliding Door";
            string blkName3 = "Altwood Garage Door - 8'";
           // Point3d blkPosition = new Point3d();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrMSpace = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                foreach (ObjectId id in btrMSpace)
                {
                    if (id.ObjectClass.DxfName == "INSERT")
                    {
                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        if (br.IsDynamicBlock)
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead);
                            if (btr.Name.Trim().ToUpper().Equals(blkName.Trim().ToUpper()))
                            {
                                count++;
                                objId = br.ObjectId;
                                objIDs.Add(objId);
                            }
                            else if(btr.Name.Trim().ToUpper().Equals(blkName2.Trim().ToUpper()))
                            {
                                count++;
                                objId = br.ObjectId;
                                objIDs.Add(objId);
                            }
                            else if (btr.Name.Trim().ToUpper().Equals(blkName3.Trim().ToUpper()))
                            {
                                count++;
                                objId = br.ObjectId;
                                objIDs.Add(objId);
                                // blkName = btr.Name;
                                //blkPosition = br.Position;
                                //ed.WriteMessage("EffectiveName:br.Name: " + br.Name + "\nbr.BlockName: " 
                                //+ br.BlockName + " \n btr.Name: " + btr.Name + "\ncount:" + count);
                            }
                           
                        }
                    }
                }
                
                tr.Commit();
            }
            return objIDs;
        }
        public void DrawPolylineAroundObject(ref double area)
        {
            List<ObjectId> objIDs = new List<ObjectId>();
            objIDs = GetMspaceDoorsBlockRefCount();
            Document acNewDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDbNewDoc = acNewDoc.Database;
            area = 0;
            foreach (ObjectId id in objIDs)
            {
                using (DocumentLock acLckDoc = acNewDoc.LockDocument())
                {
                    using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)acTrans.GetObject(acDbNewDoc.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord acblkTblRec = acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                        Entity acEnt = (Entity)acTrans.GetObject(id, OpenMode.ForRead);
                        BlockReference blk = (BlockReference)acEnt;
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
                            acPoly.ColorIndex = 3;
                            area += acPoly.Area;
                            //acPoly.SetLayerId(layerId, true);
                            acblkTblRec.AppendEntity(acPoly);
                            acTrans.AddNewlyCreatedDBObject(acPoly, true);
                        }
                        acTrans.Commit();
                    }
                }
            }
            drawPolyLineAroundWindowsbyExploding();
        }
        public void drawPolyLineAroundWindowsbyExploding()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
            // List<ObjectId> objIds = new List<ObjectId>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                    TypedValue[] flt = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue(0,"INSERT"),
                    new TypedValue(410, "Model"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.BlockName,"*win" ),
                    new TypedValue((int)DxfCode.BlockName,"*-rnWindows" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
            };
                SelectionFilter filter = new SelectionFilter(flt);
                PromptSelectionOptions opts = new PromptSelectionOptions();
                PromptSelectionResult selRes = ed.SelectAll(filter);
                SelectionSet set = selRes.Value;

                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
               // ObjectId objId = ObjectId.Null;
                foreach (ObjectId id in set.GetObjectIds())
                {
                    double maxArea = 0;
                    BlockReference oEnt = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                    BlockReference blk = (BlockReference)oEnt;
                   // objId = blk.ObjectId;
                    BlockReference brClone = (BlockReference)blk.Clone();
                    DBObjectCollection objs = new DBObjectCollection();
                    brClone.Explode(objs);
                    ed.WriteMessage("\n\n{0}", blk.Name);
                    foreach (DBObject obj in objs)
                    {
                        if (obj is Polyline2d)
                        {
                           Polyline2d pline = (Polyline2d)obj;
                            double dArea = pline.Area;
                            if (maxArea<dArea)
                            {
                                maxArea = dArea;
                                DBObjectCollection acDbObjColl = pline.GetOffsetCurves(0.001);
                                foreach (Entity acEnt in acDbObjColl)
                                {
                                    acEnt.LayerId = layerId;
                                    //acEnt.ColorIndex = 3;
                                    acblkTblRec.AppendEntity(acEnt);
                                    tr.AddNewlyCreatedDBObject(acEnt, true);
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
        }

        public void drawPolylinebyDefinition()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
          //  double maxArea = 0;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TypedValue[] acTypValAr = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue((int)DxfCode. Start,"INSERT"),
                    new TypedValue((int)DxfCode.LayoutName , "Model"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.LayerName,"1 ALL-Elevation Medium" ),
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName,"AutoWindows" ),
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName,"qrndsgn_AutoWindows" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
                    };
                SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
                PromptSelectionOptions opts = new PromptSelectionOptions();
                opts.AllowDuplicates = false;
                PromptSelectionResult selRes = ed.GetSelection(opts, acSelFtr);
                //PromptSelectionOptions opts = new PromptSelectionOptions();
                //PromptSelectionResult selRes = ed.SelectAll(filter);
                SelectionSet set = selRes.Value;
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (ObjectId blkId in set.GetObjectIds())
                {
                    if (blkId != null)
                    {
                        double maxArea = 0;
                        // Open the Block reference for read (no need for type checking because as the selection filter did it)
                        BlockReference acBlkRef = (BlockReference)tr.GetObject(blkId, OpenMode.ForRead) as BlockReference;
                        BlockTableRecord acBlkTblRec = (BlockTableRecord)tr.GetObject(acBlkRef.BlockTableRecord, OpenMode.ForRead);
                        // Step through the Block table record
                        ed.WriteMessage("\n\n{0}", acBlkRef.Name);
                       
                        foreach (ObjectId asObjId in acBlkTblRec)
                        {
                            Entity e = (Entity)tr.GetObject(asObjId, OpenMode.ForRead);
                            
                            if (e is Polyline2d)
                            {
                                Polyline2d pline = (Polyline2d)e;

                                double dArea = pline.Area;
                                // maxArea = dArea;
                                if (maxArea < dArea)
                                {
                                    maxArea = dArea;
                                    DBObjectCollection acDbObjColl = pline.GetOffsetCurves(0.001);
                                    foreach (Entity acEnt in acDbObjColl)
                                    {
                                        acEnt.ColorIndex = 3;
                                        acblkTblRec.AppendEntity(acEnt);
                                        tr.AddNewlyCreatedDBObject(acEnt, true);
                                    }

                                }

                            }
                            // if(e is Polyline)
                            //{
                            //    Polyline pline = (Polyline)e;
                            //    bool res = CCWHub.AlgebraicArea.IsLwPolylineCCW(pline);
                            //    if (res)
                            //    {


                            //        DBObjectCollection acDbObjColl = pline.GetOffsetCurves(0.001);
                            //        foreach (Entity acEnt in acDbObjColl)
                            //        {
                            //            acEnt.ColorIndex = 3;
                            //            acblkTblRec.AppendEntity(acEnt);
                            //            tr.AddNewlyCreatedDBObject(acEnt, true);
                            //        }
                            //    }
                            //}

                        }
                    }
                }
                tr.Commit();
            }
            }
       public void drawPolyLineAroundWindow()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId layerId = GetLayerId("4 AREA-Glazing Area");
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TypedValue[] flt = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<and"),
                    new TypedValue(0,"INSERT"),
                    new TypedValue(410, "Model"),
                    new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.BlockName,"*win" ),
                    new TypedValue((int)DxfCode.BlockName,"*-rnWindows" ),
                    new TypedValue((int)DxfCode.Operator,"or>"),
                    new TypedValue((int)DxfCode.Operator,"and>")
            };
                SelectionFilter filter = new SelectionFilter(flt);
                PromptSelectionOptions opts = new PromptSelectionOptions();
                PromptSelectionResult selRes = ed.SelectAll(filter);
                SelectionSet set = selRes.Value;
                BlockTable blkTbl;
                blkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acblkTblRec = tr.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (ObjectId id in set.GetObjectIds())
                {
                    BlockReference oEnt = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                    BlockReference blk = (BlockReference)oEnt;
                    Extents3d extent = blk.GeometricExtents;
                    Point3d minPoint = extent.MinPoint;
                    Point3d maxPoint = extent.MaxPoint;
                    using (Polyline acPoly=new Polyline())
                    {
                        acPoly.SetDatabaseDefaults();
                        acPoly.AddVertexAt(0, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(1, new Point2d(maxPoint.X, minPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(2, new Point2d(maxPoint.X, maxPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(3, new Point2d(minPoint.X, maxPoint.Y), 0, 0, 0);
                        acPoly.AddVertexAt(4, new Point2d(minPoint.X, minPoint.Y), 0, 0, 0);
                        acPoly.SetLayerId(layerId,true);
                      //  acPoly.Layer = "4 AREA-Glazing Area";
                        
                        acPoly.ColorIndex = 3;
                        acblkTblRec.AppendEntity(acPoly);
                        tr.AddNewlyCreatedDBObject(acPoly, true);
                    }
                  
                }
                tr.Commit();
            }
        }
        public static ObjectId GetLayerId(string sLayer)
        {
            ObjectId layerId = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            string sLayerUC = sLayer.ToUpper();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = (LayerTable)db.TransactionManager.GetObject(db.LayerTableId, OpenMode.ForRead);

                if (lt.Has(sLayerUC).Equals(false))
                {
                    LayerTableRecord ltr=new LayerTableRecord();
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 4);
                    ltr.Name = sLayerUC;
                    lt.UpgradeOpen();
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                    layerId = ltr.ObjectId;
                }
                else
                {
                    LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(lt[sLayerUC], OpenMode.ForRead);
                    layerId = ltr.ObjectId;
                }

                tr.Commit();
             
            }
            return layerId;
        }

    
    }
}
