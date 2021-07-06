using Core.Entities;
using System.Collections.Generic;

namespace Client.Models
{
    public class TripDetailModel
    {
        public Trip Trip { get; set; }
        public bool IsTourFollowed { get; set; }
        public IEnumerable<TourReview> Reviews { get; set; }
        public IEnumerable<Trip> Recommendations { get; set; }
    }
}
