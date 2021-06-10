using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {

        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        public QuestionController(IQuestionRepository questionRepository, IUnitOfWork unitOfWork, ICustomerRepository customerRepository)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            _customerRepository = customerRepository;
        }
        // Display list of questions that user have asked
        // Return View(questionList)
        public IActionResult Index()
        {
            IEnumerable<Question> Questions = _questionRepository.Queryable.Include(q=> q.Author);
            Question Question=null;
            return View(new QuestionHomeModel
            {
                questions = Questions,
                question = Question
            }) ;
        }
        //ok

        // Display details of a specific question, including it's anwers
        // Return View(question)
        [HttpPost]
        public async Task<IActionResult> createQuestion(Question question)
        {
            string customerID = User.Claims.First(c1 => c1.Type == ClaimTypes.NameIdentifier).Value;
            question.AuthorID = customerID;
            await _questionRepository.AddAsync(question);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Index");

           
        }
    }
}
