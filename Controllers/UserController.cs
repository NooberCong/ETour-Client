using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Client.Controllers
{
    public class UserController : Controller
    {
        private readonly IBookingRepository _booking;
        private readonly ITripRepository _trip;
        private readonly ITourRepository _tour;
        private readonly ICustomerRepository _customer;
        private readonly IOrderRepository _order;
        public UserController(IOrderRepository order, ICustomerRepository customer,IBookingRepository booking, ITripRepository trip, ITourRepository tour)
        {
            _order = order;
            _booking=booking;
            _trip = trip;
            _tour = tour;
            _customer = customer;
        }

        // Display user main screen, including user details and list of upcoming trips
        // View(userHomeViewModel)
        public IActionResult Index()
        {
            IEnumerable<Booking> objList = _booking.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour);
            return View(objList);
        }

        // Display list of tours that user followed
        // Return View(listFavoriteTours)
        public IActionResult Favourite()
        {
            return View();
        }

        // Display user info edit screen
        // Return View(userInfo)
        public IActionResult UpdateInfo()
        {
            string customerID = User.Claims.First(c1 => c1.Type == ClaimTypes.NameIdentifier).Value;
            //Customer customer = await _customerRepository.FindAsync(customerID);
            return View();
        }

        // Action for editting user info, this action is called when a form is sent(POST) to the action
        // Param user represents the updated user instance, object is used as a placeholder class because User class is not written
        // Return View(userInfo) with update status (fail/sucess)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(ICustomerRepository cu)
        {   
            
            return View();
        }

        // Display trips that user has bought tickets for, including the option to review the trip (redirect to the tour details page)
        // Parameter pageNumber specify which set of trip record to display according to pageSize
        // Return View(tripHistoryList)
        public IActionResult OrderHistory()
        {
            IEnumerable<Order> objList= _order.Queryable.Include(bk=> bk.Bookings).ThenInclude(or=>or.Order).ThenInclude(cu=>cu.Customer);

            return View(objList);
        }

        public class UserHomeViewModel
        {
            // User details and Recently viewed trips go here
        }
    }
}
