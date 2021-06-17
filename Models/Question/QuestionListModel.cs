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
      public Question _Question { get; set; }
      public Customer Customer { get; set; }
    }
}
