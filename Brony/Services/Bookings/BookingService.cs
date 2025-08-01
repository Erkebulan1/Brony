using Brony.Constants;
using Brony.Domain;
using Brony.Extensions;
using Brony.Helpers;
using Brony.Models.Bookings;
using Brony.Services.Stadiums;
using Brony.Services.Users;
using Brony.Services.Stadiums;

namespace Brony.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly UserService userService;
    private readonly StadiumService stadiumService;

    public BookingService(UserService userService, StadiumService stadiumService)
    {
        this.userService = userService;
        this.stadiumService = stadiumService;
    }

    public void Book(BookingCreateModel createModel)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        // check user

        var existUser = userService.Get(createModel.UserId);

        // check stadium
        var existStadium = stadiumService.Get(createModel.StadiumId);

        // stadium working hours
        TimeSpan startTimeOfStadium = TimeSpan.Parse(existStadium.StartWorkingTime);
        TimeSpan endTimeOfStadium = TimeSpan.Parse(existStadium.EndWorkingTime);

        //booking start and end time
        TimeSpan startTimeOfMatch = createModel.StartTime.TimeOfDay;
        TimeSpan endTimeOfMatch = createModel.EndTime.TimeOfDay;


        if (startTimeOfStadium > startTimeOfMatch || endTimeOfStadium < endTimeOfMatch)
        {
            throw new Exception($"Stadium does not work in this time period! | " +
                                $"Working time: {existStadium.StartWorkingTime} - {existStadium.EndWorkingTime}");
        }

        // check time which is not booked
        foreach (var item in convertedBookings)
        {
            if (item.StadiumId == createModel.StadiumId)
            {
                if (item.StartTime < createModel.EndTime && item.EndTime > createModel.StartTime)
                {
                    throw new Exception("This stadium is already booked in this time");
                }
            }
        }

        convertedBookings.Where(item =>
            item.StadiumId == createModel.StadiumId &&
            (item.StartTime < createModel.EndTime && item.EndTime > createModel.StartTime));

        double numberOfMatchHours = (endTimeOfMatch - startTimeOfMatch).TotalHours;
        decimal totalPrice = (decimal)numberOfMatchHours * existStadium.Price;

        convertedBookings.Add(
            new Booking
            {
                UserId = createModel.UserId,
                StadiumId = createModel.StadiumId,
                StartTime = createModel.StartTime,
                EndTime = createModel.EndTime,
                Price = totalPrice,
            });

        File.WriteAllLines(PathHolder.BookingsFilePath, convertedBookings.ConvertToString());
    }

    public void Cancel(int bookingId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        var existBooking = convertedBookings.Find(x => x.Id == bookingId);

        if (existBooking == null)
        {
            throw new Exception("Booking was not found");
        }

        convertedBookings.Remove(existBooking);

        File.WriteAllLines(PathHolder.BookingsFilePath, convertedBookings.ConvertToString());
    }

    public void ChangeDateTime(int bookingId, DateTime startTime, DateTime endTime)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        var existBooking = convertedBookings.Find(x => x.Id == bookingId);

        if (existBooking == null)
        {
            throw new Exception("Booking was not found");
        }

        var existStadium = stadiumService.Get(existBooking.StadiumId);

        // stadium working hours
        TimeSpan startTimeOfStadium = TimeSpan.Parse(existStadium.StartWorkingTime);
        TimeSpan endTimeOfStadium = TimeSpan.Parse(existStadium.EndWorkingTime);

        //booking start and end time
        TimeSpan startTimeOfMatch = startTime.TimeOfDay;
        TimeSpan endTimeOfMatch = endTime.TimeOfDay;


        if (startTimeOfStadium > startTimeOfMatch || endTimeOfStadium < endTimeOfMatch)
        {
            throw new Exception($"Stadium does not work in this time period! | " +
                                $"Working time: {existStadium.StartWorkingTime} - {existStadium.EndWorkingTime}");
        }

        // check time which is not booked
        foreach (var booking in convertedBookings)
        {
            if (booking.StadiumId == existBooking.StadiumId)
            {
                if (booking.StartTime < endTime && booking.EndTime > startTime)
                {
                    throw new Exception("This stadium is already booked in this time");
                }
            }
        }

        existBooking.StartTime = startTime;
        existBooking.EndTime = endTime;

        File.WriteAllLines(PathHolder.BookingsFilePath, convertedBookings.ConvertToString());
    }

    public BookingViewModel Get(int id)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        var existBooking = convertedBookings.Find(x => x.Id == id);

        if (existBooking == null)
        {
            throw new Exception("Booking is not found");
        };

        return existBooking.ToBookingViewModel();
    }

    public List<Booking> GetAll()
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        return convertedBookings;
    }

    public List<BookingViewModel> GetAllByUserId(int userId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        var result = new List<BookingViewModel>();

        foreach (var booking in convertedBookings)
        {
            if (booking.UserId == userId)
            {
                result.Add(booking.ToBookingViewModel());
            }
        }

        return result;
    }

    public List<BookingViewModel> GetAllByStadiumId(int stadiumId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingsFilePath);

        List<Booking> convertedBookings = text.ToBooking();

        var result = new List<BookingViewModel>();

        foreach (var item in convertedBookings)
        {
            if (item.StadiumId == stadiumId)
            {
                result.Add(item.ToBookingViewModel());
            }
        }

        return result;
    }

}


