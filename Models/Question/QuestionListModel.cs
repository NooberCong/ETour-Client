using Core.Entities;
using System.Collections.Generic;

namespace Client.Models
{
    public class QuestionListModel
    {
        public IEnumerable<Question> Questions { get; set; }
        public Question _Question { get; set; }
        public Customer Customer { get; set; }
        public Answer _Answer { get; set; }
        public string UserID { get; set; }
    }
}
