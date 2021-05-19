using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class TripController : Controller
    {
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
        public IActionResult Details(int? id)
        {
            return View();
        }

        public class TripFilterParams
        {

        }
    }
}
