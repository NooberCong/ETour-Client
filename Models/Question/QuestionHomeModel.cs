using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class QuestionHomeModel
    {
      public  IEnumerable<Question> questions { get; set; }
      public Question question { get; set; }
      public Customer customers { get; set; }
    }
}
