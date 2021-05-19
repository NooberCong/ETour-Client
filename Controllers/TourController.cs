using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class TourController : Controller
    {
        private static readonly int _pageSize = 5;

        // Display list of tours
        // Parameter filterParams contains info about tour filters (startplace, destination)
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(tourList)
        public IActionResult Index(TourFilterParams filterParams, int pageNumber=1)
        {
            return View();
        }

        // Display detail of a tour
        // Parameter id represent the id of the tour, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no tour with id is found, View(tourDetails) otherwise
        public IActionResult Details(int? id)
        {
            return View();
        }

        // Action for following (if not followed) or unfollowing a tour
        // Parameter id represent the id of the tour, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no tour with id is found, redirect to Details of tour otherwise
        public IActionResult Follow(int? id)
        {
            return RedirectToAction("Details");
        }

        public class TourFilterParams
        {

        }
    }
}
