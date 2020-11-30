using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Trip_Description
    {
        public string Shape_Id { get; set; }
        public string Route_Id { get; set; }
        public int Direction_Id { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public override string ToString()
        {
            return $"{ShortDescription} - {LongDescription}";
        }
    }
}
