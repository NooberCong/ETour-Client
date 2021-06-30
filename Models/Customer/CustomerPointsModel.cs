using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class CustomerPointsModel
    {
        public int Points { get; set; }
        public IEnumerable<PointLog> PointLogs { get; set; }
    }
}
