using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

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
        public async Task<IActionResult> UpdateInfo()
        {
            // Lấy ID của ng dùng
            string customerID = User.Claims.First(cl => cl.Type == ClaimTypes.NameIdentifier).Value;

            // Vào database lấy các dữ liệu còn lại
            Customer customer = await _customerRepository.FindAsync(customerID);

            // Update dữ liệu, ví dụ name
            customer.Name = "Lai Dinh Thuan";

            // Lưu các thay đổi vào database
            await _unitOfWork.CommitAsync();

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
