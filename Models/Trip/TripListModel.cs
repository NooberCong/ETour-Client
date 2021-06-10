using Core.Entities;
using Core.Value_Objects;
using System.Collections.Generic;

namespace Client.Models
{
    public class TripListModel
    {
        public IEnumerable<Tour> Tours { get; set; }
        public TripFilterParams FilterParams { get; set; }
        public IEnumerable<Trip> Trips { get; set; }
    }
}
