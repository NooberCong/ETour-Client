using Core.Entities;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class TourListModel
    {
        public PaginatedList<Tour> Tours { get; set; }
    }
}
