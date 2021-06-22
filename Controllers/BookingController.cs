﻿using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]
    public class BookingController : BaseController
    {
        private readonly IETourLogger _eTourLogger;
        private readonly IZaloPayService _zaloPayService;
        private readonly QRCodeService _QRCodeService;
        private readonly ITripRepository _tripRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;

        private readonly IUnitOfWork _unitOfWork;
        // Display list of the tickets user had added to cart but not yet paid for
        // Return View(bookingList)
        public BookingController(IUnitOfWork unitOfWork, IBookingRepository bookingrepository, ICustomerRepository customerRepository, ITripRepository tripRepository, IETourLogger eTourLogger, IZaloPayService zaloPayService, QRCodeService QRCodeService)
        {
            _bookingRepository = bookingrepository;
            _customerRepository = customerRepository;
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
            _eTourLogger = eTourLogger;
            _zaloPayService = zaloPayService;
            _QRCodeService = QRCodeService;
        }
        public IActionResult Index()
        {
            IEnumerable<Booking> bookingList = _bookingRepository.Queryable.Include(bk => bk.Trip).ThenInclude(tr => tr.Tour);

            return View(bookingList);
        }

        // Display a form for user to enter essential information to add a new booking to cart
        // Parameter tripId represents the id of the trip user want to book, it is null if not provided or cannot be converted to int
        // Return NotFound() if tripId is null or no trip with tripId is found, View(bookingModel) otherwise
        public async Task<IActionResult> New(int tripID)
        {
            var customer = await _customerRepository.FindAsync(UserID);

            var trip = await _tripRepository.Queryable
                .Include(tr => tr.Tour)
                .Include(tr => tr.TripDiscounts)
                .ThenInclude(td => td.Discount)
                .Include(tr => tr.Bookings)
                .FirstOrDefaultAsync(tr => tr.ID == tripID);

            var applicablePoints = new Booking { Total = trip.GetSalePriceFor(CustomerInfo.CustomerAgeGroup.Adult) }.GetApplicablePoints(customer.Points);

            if (trip == null || !trip.IsOpen)
            {
                return NotFound();
            }

            return View(new BookingFormModel
            {
                Trip = trip,
                Customer = customer,
                Total = trip.GetSalePriceFor(CustomerInfo.CustomerAgeGroup.Adult),
                ApplyPoints = false,
                ApplyAmount = applicablePoints
            });
        }

        // This action is called when user submit booking form to add new booking to cart
        // Parameter tripId represents the id of the trip user want to book, it is null if not provided or cannot be converted to int
        // Parameter model represent the model bound values that supports form validation
        // Return NotFound() if tripId is null or no trip with tripId is found, View(bookingModel) otherwise
        [HttpPost]
        public async Task<IActionResult> New(Booking booking, string[] customerNames, CustomerInfo.CustomerSex[] customerSexes, DateTime[] customerDobs, bool applyPoints)
        {
            var trip = await _tripRepository.Queryable
                            .Include(tr => tr.Tour)
                            .Include(tr => tr.TripDiscounts)
                            .ThenInclude(td => td.Discount)
                            .Include(tr => tr.Bookings)
                            .FirstOrDefaultAsync(tr => tr.ID == booking.TripID);
            var customer = await _customerRepository.FindAsync(UserID);

            if (trip == null)
            {
                return NotFound();
            }

            var ageGroups = customerDobs.Select(dob => CustomerInfo.AgeGroupFor(dob)).ToArray();
            var salePrices = trip.GetSalePricesFor(ageGroups);
            booking.Total = salePrices.Sum();
            var applyAmount = 0;

            if (applyPoints)
            {
                applyAmount = await GetApplicablePoints(booking);
                booking.Total -= applyAmount;
            }

            if (booking.TicketCount > trip.Vacancies)
            {
                ModelState.AddModelError(string.Empty, "There was not enough vacancies for your booking request");
            }

            if (customer.Points < applyAmount)
            {
                ModelState.AddModelError(string.Empty, "There ");
            }

            if (!ModelState.IsValid)
            {
                return View(new BookingFormModel
                {
                    Customer = customer,
                    Booking = booking,
                    CustomerNames = customerNames,
                    CustomerSexes = customerSexes,
                    CustomerDobs = customerDobs,
                    CustomerAgeGroups = ageGroups,
                    SalePrices = salePrices,
                    Trip = trip,
                    Total = booking.Total,
                });
            }

            booking.AuthorID = UserID;

            for (int i = 0; i < customerNames.Length; i++)
            {
                var ageGroup = CustomerInfo.AgeGroupFor(customerDobs[i]);

                booking.CustomerInfos.Add(new CustomerInfo
                {
                    Name = customerNames[i],
                    Sex = customerSexes[i],
                    DOB = customerDobs[i],
                    AgeGroup = ageGroup
                });
            }
<<<<<<< HEAD
            booking.Status = Booking.BookingStatus.Awaiting_Deposit;
=======
>>>>>>> master

            booking.Status = Booking.BookingStatus.Awaiting_Deposit;
            booking.PaymentDeadline = DateTime.Now.AddDays(2);
            booking.TicketCount = booking.CustomerInfos.Count;
            booking.PointsApplied = applyAmount;
            booking.SetDeposit(trip.Deposit);

            customer.Consume(applyAmount);
            customer.Reward(trip.RewardPoints);

            await _customerRepository.UpdateAsync(customer);
            await _bookingRepository.AddAsync(booking);
            await _unitOfWork.CommitAsync();

            return RedirectToAction("Detail", new { id = booking.ID });
        }



        public async Task<IActionResult> Detail(int id)
        {
            var booking = await _bookingRepository.Queryable
                .Include(bk => bk.Trip)
                .ThenInclude(tr => tr.Tour)
                .Include(bk => bk.CustomerInfos)
                .Include(bk => bk.Author)
                .FirstOrDefaultAsync(bk => bk.ID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking.Status switch
            {
                Booking.BookingStatus.Awaiting_Deposit => View("Views/Booking/Deposit.cshtml", booking),
<<<<<<< HEAD
                Booking.BookingStatus.Processing => throw new NotImplementedException(),
                Booking.BookingStatus.Awaiting_Payment => throw new NotImplementedException(),
                Booking.BookingStatus.Completed => throw new NotImplementedException(),
                Booking.BookingStatus.Canceled => throw new NotImplementedException(),
=======
                Booking.BookingStatus.Processing => View("Views/Booking/Processing.cshtml", booking),
                Booking.BookingStatus.Awaiting_Payment => View("Views/Booking/Payment.cshtml", booking),
                Booking.BookingStatus.Completed => View("Views/Booking/Completed.cshtml", booking),
                Booking.BookingStatus.Canceled => View("Views/Booking/Canceled.cshtml", booking),
                _ => throw new InvalidOperationException(),
>>>>>>> master
            };
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDepositQRCode(int id, Booking.BookingPaymentProvider provider)
        {
            var booking = await _bookingRepository.Queryable
                            .Include(bk => bk.Trip)
                            .ThenInclude(tr => tr.Tour)
                            .Include(bk => bk.Author)
                            .FirstOrDefaultAsync(bk => bk.ID == id);

            if (booking == null || booking.Status != Booking.BookingStatus.Awaiting_Deposit && booking.Status != Booking.BookingStatus.Awaiting_Payment)
            {
                return NotFound();
            }

            string paymentUrl = "";
            string viewName = "";

            switch (provider)
            {
                case Booking.BookingPaymentProvider.Zalo_Pay:
                    paymentUrl = await _zaloPayService.CreateOrderAsync(booking, (long)booking.Deposit, $"Toure: Deposit for tour {booking.Trip.Tour.Title}");
                    viewName = "ZaloPay";
                    break;
                case Booking.BookingPaymentProvider.MoMo:
                    break;
                case Booking.BookingPaymentProvider.Google_Pay:
                    break;
                default:
                    break;
            }

            
            return View(viewName, new QRPaymentModel
            {
                QRImageSource = _QRCodeService.GenerateBase64(paymentUrl),
                TotalAmount = booking.Deposit.Value,
                BookingID = booking.ID
            });
        }        

        public async Task<IActionResult> CustomerInfos(int adult, int youth, int children, int baby, int tripID, bool applyPoints)
        {

            var trip = await _tripRepository.Queryable
                .Include(tr => tr.TripDiscounts)
                .ThenInclude(td => td.Discount)
                .Include(tr => tr.Bookings)
                .FirstOrDefaultAsync(tr => tr.ID == tripID);

            if (trip == null || adult <= 0 || adult + youth + children + baby > trip.Vacancies)
            {
                return BadRequest();
            }

            List<CustomerInfo.CustomerAgeGroup> ageGroups = new();

            ageGroups.AddRange(Enumerable.Repeat(CustomerInfo.CustomerAgeGroup.Adult, adult));
            ageGroups.AddRange(Enumerable.Repeat(CustomerInfo.CustomerAgeGroup.Youth, youth));
            ageGroups.AddRange(Enumerable.Repeat(CustomerInfo.CustomerAgeGroup.Children, children));
            ageGroups.AddRange(Enumerable.Repeat(CustomerInfo.CustomerAgeGroup.Baby, baby));

            var total = trip.GetSalePricesFor(ageGroups);
            var applicablePoints = await GetApplicablePoints(new Booking { Total = total.Sum() });

            return View(new CustomerInfosModel
            {
<<<<<<< HEAD
                QRImageSource = string.Format($"data:image/png;base64,{Convert.ToBase64String(BitmapToBytesCode(qrCodeImage))}"),
                TotalAmount = booking.Deposit.Value
=======
                AgeGroups = ageGroups,
                SalePrices = total,
                ApplyPoints = applyPoints,
                ApplyAmount = applicablePoints
>>>>>>> master
            });
        }

        public async Task<IActionResult> ZaloPayConfirm(int id)
        {
            var booking = await _bookingRepository.Queryable
                .Include(bk => bk.Trip)
                .FirstOrDefaultAsync(bk => bk.ID == id);

            if (booking == null || booking.Status != Booking.BookingStatus.Awaiting_Deposit && booking.Status != Booking.BookingStatus.Awaiting_Payment)
            {
                return NotFound();
            }

            if (booking.Status == Booking.BookingStatus.Awaiting_Deposit)
            {
                booking.ChangeStatus(Booking.BookingStatus.Processing);
            }
            else
            {
                booking.ChangeStatus(Booking.BookingStatus.Completed);
            }

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.CommitAsync();

            return RedirectToAction("Detail", new { id = id });
        }

        public async Task<JsonResult> ApplyPoints(Booking booking)
        {
            decimal applicablePoints = await GetApplicablePoints(booking);

            return Json(new { ApplyAmount = applicablePoints });
        }

        public async Task<IActionResult> CancelationForm(int id)
        {
            var booking = await _bookingRepository.Queryable
                .Include(bk => bk.Trip)
                .ThenInclude(tr => tr.Tour)
                .FirstOrDefaultAsync(bk => bk.ID == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (!booking.CanCancel(DateTime.Now))
            {
                return BadRequest();
            }

            return View(booking.GetBookingCancelInfo(DateTime.Now));
        }

        public IActionResult History()
        {
            IEnumerable<Booking> bookings = _bookingRepository.Queryable
                .Where(bk => bk.AuthorID == UserID)
                .Include(bk => bk.CustomerInfos)
                .Include(bk => bk.Trip).ThenInclude(t => t.Tour)
                .AsEnumerable();

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingRepository.Queryable
                            .Include(bk => bk.Trip)
                            .FirstOrDefaultAsync(bk => bk.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            var cancelDate = DateTime.Now;

            if (!booking.CanCancel(cancelDate))
            {
                return BadRequest();
            }

            booking.Cancel(cancelDate);

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.CommitAsync();

            return RedirectToAction("Detail", new { id = booking.ID });
        }

        private async Task<int> GetApplicablePoints(Booking booking)
        {
            var customer = await _customerRepository.FindAsync(UserID);
            return booking.GetApplicablePoints(customer.Points);
        }
    }
}
