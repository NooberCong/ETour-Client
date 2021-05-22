using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class UserController : Controller
    {
        private static readonly int _historyPageSize = 5;

        // Display user main screen, including user details and list of upcoming trips
        // View(userHomeViewModel)
        public IActionResult Index()
        {
            return View();
        }

        // Display list of tours that user followed
        // Return View(listFavoriteTours)
        public IActionResult Favourite()
        {
            return View();
        }

        // Display user info edit screen
        // Return View(userInfo)
        public IActionResult UpdateInfo()
        {
            return View();
        }
        public IActionResult TicketLists()
        {
            return View();
        }
        // Action for editting user info, this action is called when a form is sent(POST) to the action
        // Param user represents the updated user instance, object is used as a placeholder class because User class is not written
        // Return View(userInfo) with update status (fail/sucess)
        [HttpPost]
        public IActionResult UpdateInfo(object user)
        {
            return View();
        }

        // Display trips that user has bought tickets for, including the option to review the trip (redirect to the tour details page)
        // Parameter pageNumber specify which set of trip record to display according to pageSize
        // Return View(tripHistoryList)
        public IActionResult TripHistory(int pageNumber=1)
        {
            return View();
        }

        public class UserHomeViewModel
        {
            // User details and Recently viewed trips go here
        }
    }
}
