using Core.Entities;
using Core.Helpers;
using Core.Value_Objects;
using Infrastructure.InterfaceImpls;

namespace Client.Models
{
    public class BlogListModel
    {
        public PaginatedList<IPost<Employee>> Posts { get; set; }
        public BlogFilterParams FilterParams { get; set; }
    }
}
