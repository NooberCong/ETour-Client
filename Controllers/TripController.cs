using Client.Models;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Services;
using Core.Value_Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class TripController : BaseController
    {
        private static readonly int _pageSize = 10;

        private readonly ITripRepository _tripRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly TripFilterService _tripFilterService;
        private readonly TripRecommendationService _recommendationService;

        public TripController(ITripRepository tripRepository, ITourRepository tourRepository, ICustomerRepository customerRepository, ITourReviewRepository tourReviewRepository, TripFilterService tripFilterService, TripRecommendationService recommendationService)
        {
            _tripRepository = tripRepository;
            _tourRepository = tourRepository;
            _tourReviewRepository = tourReviewRepository;
            _customerRepository = customerRepository;
            _tripFilterService = tripFilterService;
            _recommendationService = recommendationService;
        }

        // Display list of trips
        // Parameter filterParams contains info about trip filters (price, starting time, end time, tour)
        // Parameter pageNumber specify which set of tours to display according to pageSize
        // Returns View(tourList)

        public IActionResult Index(TripFilterParams filterParams, int pageNumber = 1)
        {
            var trips = _tripRepository.Queryable
                .Where(tr => tr.IsOpen)
                .Include(tr => tr.Tour)
                .Include(tr => tr.TripDiscounts)
                .ThenInclude(td => td.Discount)
                .Include(tr => tr.Bookings)

                .AsEnumerable();

            var filteredTrips = _tripFilterService.ApplyFilter(trips, filterParams);

            var tours = _tourRepository.Queryable.Where(tour => tour.IsOpen).AsEnumerable();


            return View(new TripListModel
            {
                Trips = PaginatedList<Trip>.Create(filteredTrips.AsQueryable(), pageNumber, _pageSize),
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
                .FirstOrDefaultAsync(tr => tr.ID == id);

            if (trip == null || !trip.IsOpen)
            {
                return NotFound();
            }

            var customer = await _customerRepository.Queryable
                .Include(cus => cus.Bookings)
                .ThenInclude(bk => bk.Trip)
                .FirstOrDefaultAsync(cus => cus.ID == UserID);

            var recommendCandidates = _tripRepository.Queryable
                .Where(tr => tr.IsOpen)
                .Include(tr => tr.Tour)
                .Include(tr => tr.TripDiscounts)
                .ThenInclude(td => td.Discount)
                .Include(tr => tr.Bookings);

            var reviews = _tourReviewRepository.GetReviewsForTour(trip.Tour);

            return View(new TripDetailModel
            {
                Trip = trip,
                IsTourFollowed = await CheckFollowing(trip.TourID),
                Reviews = reviews,
                Recommendations = _recommendationService.GetRecommendations(recommendCandidates, trip),
            });
        }

        private async Task<bool> CheckFollowing(int tourID)
        {
            // User is not logged in
            if (UserID == null)
            {
                return false;
            }

            var customer = await _customerRepository.Queryable
                .Include(cus => cus.TourFollowings)
                .FirstOrDefaultAsync(cus => cus.ID == UserID);

            return customer.TourFollowings.Any(tf => tf.TourID == tourID);
        }
    }
}
