using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Models
{
    public class ModelHelper
    {
        public class RoomType
        {
            [Key]
            public string Type { get; set; }

            public string? Description { get; set; }

            [Required]
            public int Capacity { get; set; }
        }
        public class Room
        {
            [Key]
            public int RoomNumber { get; set; }

            [Required]
            public string Type { get; set; }

            [Required]
            public decimal PricePerNight { get; set; }

            [Required]
            public int MaximumOccupancy { get; set; }

            [Required]
            public bool IsAvailable { get; set; }
        }

        public class Booking
        {
            [Key]
            public int BookingId { get; set; }

            [Required]
            public int RoomNumber { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            public DateTime CheckInDate { get; set; }

            [Required]
            public DateTime CheckOutDate { get; set; }
        }

        public class User
        {
            [Key]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

            public string? FirstName { get; set; }

            public string? LastName { get; set; }
            [Required]
            public string Email { get; set; }

            public string? PhoneNumber { get; set; }
            [Required]
            public string Role { get; set; }
        }

        public enum UserRole
        {
            Administrator,
            User
        }
     
    }
}
