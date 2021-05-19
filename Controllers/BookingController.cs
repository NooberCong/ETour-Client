using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class BookingController : Controller
    {
        // Display list of the tickets user had added to cart but not yet paid for
        // Return View(bookingList)
        public IActionResult Index()
        {
            return View();
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
