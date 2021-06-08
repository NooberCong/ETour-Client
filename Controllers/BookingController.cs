using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]

    
    public class BookingController : Controller
    {
        private readonly IETourLogger _eTourLogger;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        // Display list of the tickets user had added to cart but not yet paid for
        // Return View(bookingList)
        public BookingController(IUnitOfWork unitOfWork,IBookingRepository bookingrepository, IETourLogger eTourLogger)
        {
            _bookingRepository = bookingrepository;
            _unitOfWork = unitOfWork;
            _eTourLogger = eTourLogger;
        }
        public IActionResult Index()
        {
            IEnumerable<Booking> bookingList = _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour);

            return View(bookingList);
        }

        // Display a form for user to enter essential information to add a new booking to cart
        // Parameter tripId represents the id of the trip user want to book, it is null if not provided or cannot be converted to int
        // Return NotFound() if tripId is null or no trip with tripId is found, View(bookingModel) otherwise
        public IActionResult Create(int? tripId)
        {
            return View();
        }

        // This action is called when user submit booking form to add new booking to cart
        // Parameter tripId represents the id of the trip user want to book, it is null if not provided or cannot be converted to int
        // Parameter model represent the model bound values that supports form validation
        // Return NotFound() if tripId is null or no trip with tripId is found, View(bookingModel) otherwise
        [HttpPost]
        public IActionResult Create(int? tripId, BookingModel model)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id, string returnUrl)
        {
            returnUrl ??= Url.Action("Index");

            Booking booking = await _bookingRepository.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            await _bookingRepository.DeleteAsync(booking);
            await _eTourLogger.LogAsync(Log.LogType.Deletion, $"{User.Identity.Name} deleted booking {booking.ID}");
            await _unitOfWork.CommitAsync();

            TempData["StatusMessage"] = "Discount deleted successfully";
            return LocalRedirect(returnUrl);
        }



        // Display payment details and a form for user to make payment
        // Return View(paymentModel)
        public IActionResult Checkout()
        {
            return View();
        }

        public class BookingModel
        {
            // Trip detail and essential user info go here
        }

        public class PaymentModel
        {
            // Payment details go here
        }
    }
}
