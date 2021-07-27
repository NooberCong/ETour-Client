using Client.Models;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.InterfaceImpls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class CustomerController : BaseController
    {
        private static readonly int _pageSize = 10;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly IPostRepository<Post, Employee> _postRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IBookingRepository bookingRepository, ITourReviewRepository tourReviewRepository, IPostRepository<Post, Employee> postRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _postRepository = postRepository;
            _customerRepository = customerRepository;
            _tourReviewRepository = tourReviewRepository;
        }

        // Display user main screen, including user details and list of upcoming trips
        // View(userHomeViewModel)
        public async Task<IActionResult> Index()
        {
            IEnumerable<Booking> bookingList = _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour);

            Customer customer = await _customerRepository.FindAsync(UserID);

            return View(new CustomerHomeModel
            {
                Bookings = bookingList,
                Customer = customer
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Customer customer)
        {

            if (ModelState.IsValid)
            {
                var existingUser = await _customerRepository.FindAsync(UserID);

                existingUser.Name = customer.Name;
                existingUser.PhoneNumber = customer.PhoneNumber;
                existingUser.Address = customer.Address;
                existingUser.DOB = customer.DOB;

                await _customerRepository.UpdateAsync(existingUser);
                await _unitOfWork.CommitAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Points(int pageNumber = 1)
        {
            Customer customer = await _customerRepository.FindAsync(UserID);
            IEnumerable<PointLog> logs = _customerRepository.GetPointsLogs(customer)
                .OrderByDescending(pl => pl.LastUpdated);

            return View(new CustomerPointsModel
            {
                Points = customer.Points,
                PointLogs = PaginatedList<PointLog>.Create(logs.AsQueryable(), pageNumber, _pageSize)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Review(int bookingID, TourReview tourReview, string returnUrl)
        {
            returnUrl ??= Url.Action(nameof(Index));

            var booking = await _bookingRepository.Queryable
                .Include(bk => bk.Trip)
                .Include(bk => bk.Review)
                .FirstOrDefaultAsync(bk => bk.ID == bookingID);

            if (booking == null)
            {
                return NotFound();
            }


            if (!booking.CanBeReviewed(DateTime.Now))
            {
                return Forbid();
            }

            tourReview.BookingID = booking.ID;
            tourReview.LastUpdated = DateTime.Now;

            await _bookingRepository.UpdateAsync(booking);
            await _tourReviewRepository.AddAsync(tourReview);
            await _unitOfWork.CommitAsync();

            return LocalRedirect(returnUrl);
        }
    }
}
