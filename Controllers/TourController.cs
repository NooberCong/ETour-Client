using Core.Interfaces;
using Client.Models;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Client.Controllers
{
    public class TourController : BaseController
    {
        private static readonly int _pageSize = 10;
        private readonly ITourRepository _tourRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TourController(ITourRepository tourRepository, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _tourRepository = tourRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int pageNumber = 1)
        {

            IEnumerable<Tour> tours = _tourRepository.Queryable.Where(Tour => Tour.IsOpen).AsEnumerable();
            return View(new TourListModel
            {
                Tours = PaginatedList<Tour>.Create(tours.AsQueryable(), pageNumber, _pageSize)
            });
        }

        [Authorize]
        public async Task<IActionResult> ToggleFollow(int id)
        {
            var tour = await _tourRepository.FindAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.Queryable
                .Include(cus => cus.TourFollowings)
                .FirstOrDefaultAsync(cus => cus.ID == UserID);

            if (customer.IsFollowing(tour))
            {
                _customerRepository.UnFollow(customer, tour);
            }
            else
            {
                _customerRepository.Follow(customer, tour);
            }

            await _unitOfWork.CommitAsync();

            return RedirectToAction("Detail", "Trip", new { id = id });
        }
    }
}
