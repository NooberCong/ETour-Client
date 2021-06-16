using Core.Entities;
using System.Collections.Generic;

namespace Client.Models
{
    public class CustomerHomeModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public Trip Trips { get; set; }
        public IEnumerable<Booking> UpcomingTrips { get; set; }

    }
}
