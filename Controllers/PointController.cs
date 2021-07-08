using Client.Models;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Client.Controllers
{
    public class PointController : BaseController
    {
        private static readonly int _pageSize = 10;
        private readonly ICustomerRepository _customerRepository;
        public PointController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            Customer customer = await _customerRepository.FindAsync(UserID);
            IEnumerable<PointLog> logs = _customerRepository.GetPointsLogs(customer);
            
            return View(new CustomerPointsModel { 
                Points = customer.Points,
                PointLogs = PaginatedList<PointLog>.Create(logs.AsQueryable(), pageNumber, _pageSize)
            });
        }
    }
}
