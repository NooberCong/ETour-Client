using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class BookingHistoryModel
    {
        public IEnumerable<Booking> Bookings { get; set; }
        public TourReview TourReview { get; set; }
    }
}
