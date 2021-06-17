using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class BookingFormModel
    {

        public Customer Customer { get; set; }
        public Trip Trip { get; set; }
        public Booking Booking { get; set; }
        public string[] CustomerNames { get; set; } = new string[] { };
        public CustomerInfo.CustomerSex[] CustomerSexes { get; set; } = new CustomerInfo.CustomerSex[] { };
        public DateTime[] CustomerDobs { get; set; } = new DateTime[] { };
        public CustomerInfo.CustomerAgeGroup[] CustomerAgeGroups { get; set; } = new CustomerInfo.CustomerAgeGroup[] { };
        public decimal[] SalePrices { get; set; }
        public decimal Total { get; set; }
    }
}
