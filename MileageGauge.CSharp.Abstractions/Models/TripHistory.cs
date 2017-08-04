using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.Models
{
    public class TripHistory
    {
        public string Vin { get; set; }
        public decimal MilesTravelled { get; set; }
        public decimal GallonsUsed { get; set; }
    }
}
