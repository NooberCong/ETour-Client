using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Hubs;
using Infrastructure.InfrasUtils;
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
                UserID = UserID,
                QAHubUrl = $"https://{HostHelper.CompanyHostUrl()}{QAHub.PATH}"
            });
        }
        //ok

        // Display details of a specific question, including it's anwers
        // Return View(question)
        [HttpPost]
        public async Task<IActionResult> Create(Question question)
        {
            if (ModelState.IsValid)
            {
                question.OwnerID = UserID;
                question.LastUpdated = DateTime.Now;
                question.Status = Question.QuestionStatus.Pending;
                question.Priority = Question.QuestionPriority.Low;

                await _questionRepository.AddAsync(question);
                await _unitOfWork.CommitAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            Question question = await _questionRepository.Queryable.Include(q => q.Owner).Include(q => q.Answers).FirstOrDefaultAsync(q => q.ID == id);

            return View(new QuestionListModel
                {
                    Question = question
                });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAns(Answer answer)
        {
            var question = await _questionRepository.Queryable
                .Include(q => q.Owner)
                .FirstOrDefaultAsync(q => q.ID == answer.QuestionID);

            if (question == null)
            {
                return NotFound();
            }

            // Can only be answered by the same user who asked
            if (question.OwnerID != UserID)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                answer.LastUpdated = DateTime.Now;
                answer.Author = question.Owner.Name;
                answer.QuestionID = question.ID;
                answer.AuthoredByCustomer = true;

                await _answerRepository.AddAsync(answer);
                await _unitOfWork.CommitAsync();
            }

            return RedirectToAction("Detail", new { id = question.ID });
        }

    }
}
