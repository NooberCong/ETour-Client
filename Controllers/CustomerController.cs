using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IBookingRepository bookingRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;

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

        //GET- main screen
        [HttpPost]
        public async Task<IActionResult> Index(Customer customer)
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

            await _customerRepository.UpdateAsync(existingUser);
            await _unitOfWork.CommitAsync();
            return RedirectToAction("Index");
        }



        //Display order history
        public IActionResult BookingHistory()
        {
            IEnumerable<Booking> bookings = _bookingRepository.Queryable
                .Where(bk => bk.AuthorID == UserID)
                .Include(bk => bk.Author)
                .Include(bk => bk.CustomerInfos)
                .Include(bk => bk.Trip).ThenInclude(t => t.Tour)
                .AsEnumerable();

            return View(bookings);
        }


        //Display speccific booking detail
        public async Task<IActionResult> BookingDetail(int id)
        {
            var booking = await _bookingRepository.Queryable
                .Include(bk => bk.Author)
                .Include(bk => bk.CustomerInfos)
                .Include(bk => bk.Trip).ThenInclude(t => t.Tour)
                .FirstOrDefaultAsync(bk => bk.ID == id);

            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }



    }
}
