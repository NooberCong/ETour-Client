using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class CustomerInfosModel
    {
        public List<CustomerInfo.CustomerAgeGroup> AgeGroups { get; set; }
        public decimal[] SalePrices { get; set; }
    }
}
