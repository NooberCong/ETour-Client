using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class TripDetailModel
    {
        public Trip Trip { get; set; }
        public bool IsTourFollowed { get; set; }
        public IEnumerable<Trip> Recommendations { get; set; }
    }
}
