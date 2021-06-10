using Client.Models;
using Core.Interfaces;
using Core.Services;
using Core.Value_Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class TripController : Controller
    {
        private readonly ITripRepository _tripRepository;
        private readonly ITourRepository _tourRepository;
        private readonly TripFilterService _tripFilterService;

        public TripController(ITripRepository tripRepository, ITourRepository tourRepository, TripFilterService tripFilterService)
        {
            _tripRepository = tripRepository;
            _tourRepository = tourRepository;
            _tripFilterService = tripFilterService;
        }

        private static readonly int _pageSize = 7;

        // Display list of trips
        // Parameter filterParams contains info about trip filters (price, starting time, end time, tour)
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(tourList)

        public async Task<IActionResult> Index(TripFilterParams filterParams, int pageNumber = 1)
        {
            var trips = await _tripRepository.Queryable
                .Where(tr => tr.IsOpen)
                .Include(tr => tr.Tour)
                .Include(tr => tr.TripDiscounts)
                .ThenInclude(td => td.Discount)
                .Include(tr => tr.Bookings)
                .ThenInclude(bk => bk.CustomerInfos)
                .ToListAsync();

            var filteredTrips = trips.Where(_tripFilterService.BuildFilterPredicate(filterParams));

            var tours = _tourRepository.QueryFiltered(tour => tour.IsOpen);

            return View(new TripListModel
            {
                Trips = filteredTrips,
                Tours = tours
            });
        }

        // Display detail of a trip, including book button to book a ticket (add to cart)
        // Parameter id represent the id of the trip, it is null when client does not provide id or given id cannot be converted to int
        // Return Notfound() when invalid id or no trip with id is found, View(tripDetails) otherwise
        public async Task<IActionResult> Detail(int id)
        {
            var trip = await _tripRepository.Queryable
                .Include(tr => tr.Tour)
                .Include(tr => tr.Itineraries)
                .Include(tr => tr.TripDiscounts)
                .ThenInclude(tr => tr.Discount)
                .Include(tr => tr.Bookings)
                .ThenInclude(bk => bk.CustomerInfos)
                .FirstOrDefaultAsync(tr => tr.ID == id);

            if (trip == null || !trip.IsOpen)
            {
                return NotFound();
            }

            return View(trip);
        }
    }
}
