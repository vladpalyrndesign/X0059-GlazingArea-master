using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GlazingAreaModel
{
    
    public class Glazing 
    {
        public string ProjectNumber { get; set; }
        public string ModelNumber { get; set; }
        public string Elevation { get; set; }
        public double TotalSpWallAraea { get; set; }
        public double SptLimitingDistance { get; set; }
        public double SpAllowablePercrntage { get; set; }
        public double SpAllowableOpenings { get; set; }
        public double SpActualOpenings { get; set; }
        public double FirstFloorPeripheral { get; set; }
        public double FirstFloorHeight { get; set; }
        public double SecondFloorPeripheral { get; set; }
        public double SecondFloorHeight { get; set; }
        public double ThirdFloorPeripheral { get; set; }
        public double ThirdFloorHeight { get; set; }
        public double FourthFloorPeripheral { get; set; }
        public double FourthFloorHeight { get; set; }
        public double PeriPheralSquareFootArea { get; set; }
        public double PeriPheralSquareMeterArea { get; set; }
        public double FrontArea { get; set; }
        public double RearArea { get; set; }
        public double LeftArea { get; set; }
        public double RightArea { get; set; }
        public double TotalGlazingSquareFootArea { get; set; }
        public double TotalGlazingSquareMeterArea { get; set; }
        public double TotalGlazingPercentage { get; set; }

    }
}
