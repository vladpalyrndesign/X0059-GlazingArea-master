using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qrndGlazingArea
{
    public class Commands
    {
        //
        [CommandMethod("qrnGlazingArea",CommandFlags.Modal)]
        public static void ShowGlazingAreaForm()
        {
           // GlazingArea.Forms.glazingForm frm = new GlazingArea.Forms.glazingForm(); //door form
            // qrndGlazingArea.frmGlazingArea frm = new frmGlazingArea();
            frmGlazingArea frm= new frmGlazingArea();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(frm);
            if (!frm.IsDisposed)
            {
                frm.Dispose();
            }
        }

        /*
         * 
         * 
         * 
         */
    }
}
