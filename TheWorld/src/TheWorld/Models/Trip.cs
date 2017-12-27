using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class Trip
    {
        public int Id                   { get; set; }
        public string Name              { get; set; }
        public DateTime DateCreated     { get; set; }
        public string UserName          { get; set; }        // of whomever owns this trip.
        //ICollection of Stop Entity is going to allow us to add and remove them, and that is why we're using the ICollection Interface instead of something simple as IEnumerable is Read-Only.
        public ICollection<Stop> Stops  { get; set; }
    }
}
