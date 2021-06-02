using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class UserHomeModel
    {
        public Customer customer { get; set; }
        public IEnumerable<Booking> booking { get; set; }

    }
}
