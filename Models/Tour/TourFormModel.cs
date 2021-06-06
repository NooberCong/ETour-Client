using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
         public class TourFormModel
    {
        public Tour Tour { get; set; }
        public IFormFileCollection Images { get; set; }
    }
    
    }

