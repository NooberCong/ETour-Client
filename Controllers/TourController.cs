using Core.Interfaces;
using Client.Models;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Client.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourRepository _tourRepository;

        public TourController(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
           
        }

        public IActionResult Index()
        {

            IEnumerable<Tour> tourList =_tourRepository.QueryFiltered(Tour => Tour.IsOpen);
            return View(new TourListModel
            {
                Tours = tourList,
            });
        }


        // Display list of tours
        // Parameter filterParams contains info about tour filters (startplace, destination)
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(tourList)


        // Display detail of a tour
        // Parameter id represent the id of the tour, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no tour with id is found, View(tourDetails) otherwise


        // Action for following (if not followed) or unfollowing a tour
        // Parameter id represent the id of the tour, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no tour with id is found, redirect to Details of tour otherwise


        public class TourFilterParams
        {

        }
    }
}
