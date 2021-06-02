using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class UserController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork,IOrderRepository orderRepository, ICustomerRepository customerRepository,IBookingRepository bookingRepository, ITripRepository tripRepository, ITourRepository tourRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _bookingRepository=bookingRepository;
            _tripRepository = tripRepository;
            _tourRepository = tourRepository;
            _customerRepository = customerRepository;
            
        }

        // Display user main screen, including user details and list of upcoming trips
        // View(userHomeViewModel)
        public async Task<IActionResult> Index()
        {
            string customerID = User.Claims.First(c1 => c1.Type == ClaimTypes.NameIdentifier).Value;
            IEnumerable<Booking> bookingList = _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour);
            Customer customer = await _customerRepository.FindAsync(customerID);
            return View(new UserHomeModel
            {
                booking = bookingList,
                customer = customer
            }) ;
        }

        [HttpPost]
        public async Task<IActionResult> Index(Customer cs)
        {
            string customerID = User.Claims.First(c1 => c1.Type == ClaimTypes.NameIdentifier).Value;
            Customer Customer = await _customerRepository.FindAsync(customerID);
            Customer = cs;
            IEnumerable<Booking> bookingList = _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour);
            await _unitOfWork.CommitAsync();
            return View(new UserHomeModel
            {
                booking = bookingList,
                customer = Customer
            });
        }
        // Display list of tours that user followed
        // Return View(listFavoriteTours)
        public IActionResult Favourite()
        {
            return View();
        }

        // Display user info edit screen
        // Return View(userInfo)
   
        // Action for editting user info, this action is called when a form is sent(POST) to the action
        // Param user represents the updated user instance, object is used as a placeholder class because User class is not written
        // Return View(userInfo) with update status (fail/sucess)
        [HttpPost]
        [ValidateAntiForgeryToken]
       

        // Display trips that user has bought tickets for, including the option to review the trip (redirect to the tour details page)
        // Parameter pageNumber specify which set of trip record to display according to pageSize
        // Return View(tripHistoryList)
        public IActionResult OrderHistory()
        {
            IEnumerable<Order> objList= _orderRepository.Queryable.Include(cu => cu.Customer);

            return View(objList);
        }

        public class UserHomeViewModel
        {
            // User details and Recently viewed trips go here
        }
    }
}
