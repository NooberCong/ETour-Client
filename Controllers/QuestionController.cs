using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        // Display list of questions that user have asked
        // Return View(questionList)
        public IActionResult Index()
        {
            return View();
        }

        // Display details of a specific question, including it's anwers
        // Return View(question)
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
