using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class UserHomeModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public Trip Trips { get; set; }
        public IEnumerable<Order> Orders { get; set; }

    }
}
