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
        private readonly IAnswerRepository _answerRepository;
        public QuestionController(IQuestionRepository questionRepository, IUnitOfWork unitOfWork, IAnswerRepository answerRepository)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }
        // Display list of questions that user have asked
        // Return View(questionList)
        public IActionResult Index()
        {
            IEnumerable<Question> Questions = _questionRepository.Queryable.Include(q => q.Owner);
            return View(new QuestionListModel
            {
                Questions = Questions,
            });
        }
        //ok

        // Display details of a specific question, including it's anwers
        // Return View(question)
        [HttpPost]
        public async Task<IActionResult> Create(Question _Question)
        {
            _Question.OwnerID = UserID;
            _Question.LastUpdated = DateTime.Now;
            
            await _questionRepository.AddAsync(_Question);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Index");


        }
        
        public async Task<IActionResult> Detail (int id)
        {
          Question question = await _questionRepository.Queryable.Include(q=> q.Owner).Include(q=>q.Answers).FirstOrDefaultAsync(q => q.ID == id);
           
            return View(
                new QuestionListModel
                {
                    _Question = question
                }
                ); ;

        }

        [HttpPost]
        public async Task<IActionResult> CreateAns(Question _Question, Answer _Answer)
        {
            
            _Answer.LastUpdated = DateTime.Now;
           
            _Answer.Author = _Question.Author.Name;
            _Answer.QuestionID = _Question.ID;
            _Answer.AuthoredByCustomer = true;
            await _answerRepository.AddAsync(_Answer);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Detail", new {id= _Question.ID });


        }

    }
}
