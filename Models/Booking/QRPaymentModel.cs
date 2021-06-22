namespace Client.Models
{
    public class QRPaymentModel
    {
        public int BookingID { get; set; }
        public string QRImageSource { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
