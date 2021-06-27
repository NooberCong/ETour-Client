using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IBookingRepository bookingRepository, ITourReviewRepository tourReviewRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
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

            if (!ModelState.IsValid)
            {
                return View(
                new CustomerHomeModel
                {
                    Bookings = _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour),
                    Customer = customer
                });
            }

            var existingUser = await _customerRepository.FindAsync(UserID);

            existingUser.Name = customer.Name;
            existingUser.PhoneNumber = customer.PhoneNumber;
            existingUser.Address = customer.Address;
            existingUser.DOB = customer.DOB;

            await _customerRepository.UpdateAsync(existingUser);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Review(int bookingID, TourReview tourReview, string returnUrl)
        {
            returnUrl ??= Url.Action("Index");

            var booking = await _bookingRepository.Queryable
                .Include(bk => bk.Trip)
                .FirstOrDefaultAsync(bk => bk.ID == bookingID);

            if (booking == null)
            {
                return NotFound();
            }


            var customer = await _customerRepository.Queryable
                .Include(cus => cus.Bookings)
                .ThenInclude(bk => bk.Trip)
                .FirstOrDefaultAsync(cus => cus.ID == UserID);

            if (!booking.CanBeReviewed(DateTime.Now))
            {
                return Forbid();
            }

            tourReview.OwnerID = customer.ID;
            tourReview.TourID = booking.Trip.TourID;
            tourReview.LastUpdated = DateTime.Now;
            booking.Reviewed = true;

            await _bookingRepository.UpdateAsync(booking);
            await _tourReviewRepository.AddAsync(tourReview);
            await _unitOfWork.CommitAsync();

            return LocalRedirect(returnUrl);
        }
    }
}
