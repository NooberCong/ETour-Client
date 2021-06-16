using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]
    public class QuestionController : BaseController
    {

        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public QuestionController(IQuestionRepository questionRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
        }
        // Display list of questions that user have asked
        // Return View(questionList)
        public IActionResult Index()
        {
            IEnumerable<Question> Questions = _questionRepository.Queryable.Include(q => q.Author);
            return View(new QuestionListModel
            {
                Questions = Questions,
            });
        }
        //ok

        // Display details of a specific question, including it's anwers
        // Return View(question)
        [HttpPost]
        public async Task<IActionResult> Create(Question question)
        {
            question.AuthorID = UserID;
            question.LastUpdated = DateTime.Now;
            await _questionRepository.AddAsync(question);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Index");


        }
    }
}
