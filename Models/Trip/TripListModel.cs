using Core.Entities;
using Core.Helpers;
using Core.Value_Objects;
using System.Collections.Generic;

namespace Client.Models
{
    public class TripListModel
    {
        public IEnumerable<Tour> Tours { get; set; }
        public TripFilterParams FilterParams { get; set; }
        public PaginatedList<Trip> Trips { get; set; }
    }
}
