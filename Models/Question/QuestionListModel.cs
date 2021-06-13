using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class QuestionListModel
    {
      public  IEnumerable<Question> Questions { get; set; }
      public Question Question_ { get; set; }
      public Customer Customer { get; set; }
    }
}
