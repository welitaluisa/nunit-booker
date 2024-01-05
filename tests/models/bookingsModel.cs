using System.Formats.Asn1;

namespace Models;

public class BookingsModel
{
    public String firstname;
    public String lastname;
    public int totalprice;
    public Boolean depositpaid;
    public BookingDates bookingdates;
    public String additionalneeds;
}

public class BookingDates 
{
    public DateTime  checkin;
    public DateTime  checkout;

    // Construtor
    public BookingDates(DateTime  checkin, DateTime  checkout)
    {
        this.checkin = checkin;
        this.checkout = checkout;
    }
}