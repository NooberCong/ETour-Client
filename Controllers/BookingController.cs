using Client.Models;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Authorize]
    public class BookingController : BaseController
    {
        private readonly IETourLogger _eTourLogger;
        private readonly IZaloPayService _zaloPayService;
        private readonly ITripRepository _tripRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;

        private readonly IUnitOfWork _unitOfWork;
        // Display list of the tickets user had added to cart but not yet paid for
        // Return View(bookingList)
        public BookingController(IUnitOfWork unitOfWork, IBookingRepository bookingrepository, ICustomerRepository customerRepository, ITripRepository tripRepository, IETourLogger eTourLogger, IZaloPayService zaloPayService)
        {
            _bookingRepository = bookingrepository;
            _customerRepository = customerRepository;
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
            _eTourLogger = eTourLogger;
            _zaloPayService = zaloPayService;
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

            if (trip == null || !trip.IsOpen)
            {
                return NotFound();
            }

            return View(new BookingFormModel
            {
                Trip = trip,
                Customer = customer,
                Total = trip.GetSalePriceFor(CustomerInfo.CustomerAgeGroup.Adult)
            });
        }

        // This action is called when user submit booking form to add new booking to cart
        // Parameter tripId represents the id of the trip user want to book, it is null if not provided or cannot be converted to int
        // Parameter model represent the model bound values that supports form validation
        // Return NotFound() if tripId is null or no trip with tripId is found, View(bookingModel) otherwise
        [HttpPost]
        public async Task<IActionResult> New(Booking booking, string[] customerNames, CustomerInfo.CustomerSex[] customerSexes, DateTime[] customerDobs)
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

            if (!ModelState.IsValid)
            {
                return View(new BookingFormModel
                {
                    Customer = customer,
                    Booking = booking,
                    CustomerNames = customerNames,
                    CustomerSexes = customerSexes,
                    CustomerDobs = customerDobs,
                    CustomerAgeGroups = customerDobs.Select(dob => CustomerInfo.AgeGroupFor(dob)).ToArray(),
                    Trip = trip,
                    Total = customerDobs.Select(dob => trip.GetSalePriceFor(CustomerInfo.AgeGroupFor(dob))).Sum(),
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

                booking.Total += trip.GetSalePriceFor(ageGroup);
            }
            booking.Status = Booking.BookingStatus.AwaitingDeposit;

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
                Booking.BookingStatus.AwaitingDeposit => View("Views/Booking/Deposit.cshtml", booking),
                Booking.BookingStatus.Processing => throw new NotImplementedException(),
                Booking.BookingStatus.AwaitingPayment => throw new NotImplementedException(),
                Booking.BookingStatus.Completed => throw new NotImplementedException(),
                Booking.BookingStatus.Canceled => throw new NotImplementedException(),
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
            string paymentUrl = "";
            string viewName = "";

            switch (provider)
            {
                case Booking.BookingPaymentProvider.Zalo_Pay:
                    paymentUrl = await _zaloPayService.CreateOrderAsync(booking, (long)booking.GetDeposit(), $"Toure: Deposit for tour {booking.Trip.Tour.Title}");
                    viewName = "ZaloPay";
                    break;
                case Booking.BookingPaymentProvider.MoMo:
                    break;
                case Booking.BookingPaymentProvider.Google_Pay:
                    break;
                default:
                    break;
            }

            Console.WriteLine($"PaymentUrl: {paymentUrl}");

            QRCodeData _qrCodeData = new QRCodeGenerator().CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return View(viewName, new QRPaymentModel
            {
                QRImageSource = string.Format($"data:image/png;base64,{Convert.ToBase64String(BitmapToBytesCode(qrCodeImage))}"),
                TotalAmount = booking.GetDeposit()
            });
        }

        [NonAction]
        private static byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
