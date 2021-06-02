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
        public async Task<IActionResult> Index(Customer? cs)
        {
            
            Customer Customer = await _customerRepository.FindAsync(cs.ID);
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

     
       
        public IActionResult OrderHistory()
        {
            IEnumerable<Order> OrderList= _orderRepository.Queryable.Include(cu => cu.Customer);

            return View(OrderList);
        }

        public class UserHomeViewModel
        {
            // User details and Recently viewed trips go here
        }
    }
}
