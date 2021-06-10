using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class TripController : Controller
    {
        private readonly ITripRepository _tripRepository;

        public TripController(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        private static readonly int _pageSize = 7;

        // Display list of trips
        // Parameter filterParams contains info about trip filters (price, starting time, end time, tour)
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(tourList)
        public IActionResult Index(TripFilterParams filterParams, int pageNumber=1)
        {
            return View();
        }

        // Display detail of a trip, including book button to book a ticket (add to cart)
        // Parameter id represent the id of the trip, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no trip with id is found, View(tripDetails) otherwise
        public async Task<IActionResult> Detail(int id)
        {
            var trip = await _tripRepository.Queryable
                .Include(tr => tr.Tour)
                .FirstOrDefaultAsync(tr => tr.ID == id);

            if (trip == null || !trip.IsOpen)
            {
                return NotFound();
            }

            return View(trip);
        }

        public class TripFilterParams
        {

        }
    }
}
