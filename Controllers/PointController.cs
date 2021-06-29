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
    public class PointController : BaseController
    {
        private readonly ITripRepository _tripRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository; 
        private readonly IUnitOfWork _unitOfWork;
        public PointController(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IBookingRepository bookingRepository, ITripRepository tripRepository)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _tripRepository = tripRepository;
        }
        public async Task<IActionResult> Index()
        {
          Customer customer = await _customerRepository.FindAsync(UserID);
            IEnumerable<PointLog> log =  _customerRepository.GetPointsLogs(customer);
           
            return View(log);
        }
    }
}
