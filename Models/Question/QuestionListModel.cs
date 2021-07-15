using Core.Entities;
using System.Collections.Generic;

namespace Client.Models
{
    public class QuestionListModel
    {
        public IEnumerable<Question> Questions { get; set; }
        public Question Question { get; set; }
        public Customer Customer { get; set; }
        public Answer Answer { get; set; }
        public string UserID { get; set; }
        public string QAHubUrl { get; set; }
    }
}
