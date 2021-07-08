using Core.Entities;
using Core.Helpers;

namespace Client.Models
{
    public class CustomerPointsModel
    {
        public int Points { get; set; }
        public PaginatedList<PointLog> PointLogs { get; set; }
    }
}
