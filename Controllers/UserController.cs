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
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IBookingRepository bookingRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
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
                Bookings = bookingList,
                Customer = customer
            });
        }

        //GET- main screen
        [HttpPost]
        public async Task<IActionResult> Index(Customer customer)
        {
            string customerID = User.Claims.First(c1 => c1.Type == ClaimTypes.NameIdentifier).Value;
            customer.ID = customerID;

            await _customerRepository.UpdateAsync(customer);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Index");
        }



        //Display order history
        public IActionResult BookingHistory()
        {
            IEnumerable<Booking> bookings = _bookingRepository.Queryable.Include(bk => bk.Author);

            return View(bookings);
        }


        //Display speccific booking detail
        public async Task<IActionResult> BookingDetail(int id)
        {
            var booking = await _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(t => t.Tour).Include(bk=>bk.Order).ThenInclude(o=>o.Customer).FirstOrDefaultAsync(bk=>bk.ID==id);
        
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
       
       

    }
}
