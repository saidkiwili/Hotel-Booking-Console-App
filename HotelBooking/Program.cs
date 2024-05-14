using HotelBooking.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using static HotelBooking.Models.ModelHelper;

namespace HotelBookingPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var dbContext = new AppDbContext())
                {
                    if (!dbContext.Database.CanConnect())
                    {
                        dbContext.Database.EnsureCreated();
                    }

                    dbContext.Database.Migrate();
                }

                ShowWelcomePage();

                User loggedInUser = null;
                while (loggedInUser == null)
                {
                    Console.Write("Enter your choice: ");
                    Console.Write("\n");
                    Console.Write("\n");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            loggedInUser = Login();
                            break;
                        case "2":
                            Register();
                            ShowWelcomePage();
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }

                if (loggedInUser.Role == "User")
                {
                   // UserDashboard(loggedInUser);

                    ShowUserDashboard(loggedInUser);
                    ShowUserMenu(loggedInUser);
                }
                else if (loggedInUser.Role == "Administrator")
                {
                    ShowAdminDashboard();
                    ShowAdminMenu(loggedInUser);
                }
              
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

            }

        }
       
        static void ShowWelcomePage()
        {
            Console.WriteLine("Welcome to the Hotel Booking Platform");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine();
        }

        #region Authentication
        static User Login()
        {
            bool isLoggedIn = false;
            var user = new User();
            while (!isLoggedIn)
            {
                Console.Write("Enter username: ");
                string username = Console.ReadLine();

                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                using (var dbContext = new AppDbContext())
                {
                     user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                    if (user != null)
                    {
                        isLoggedIn = true;
                       
                    }
                    else
                    {
                        Console.WriteLine("Invalid username or password.");
                       
                    }

                   
                }
               
            }
            Console.Write("\n");
            Console.Write("\n");
            return user ?? new User();
        }

        static void Register()
        {
            Console.WriteLine("Registration");
            Console.WriteLine("------------");

            Console.Write("Enter first name: ");
            string firstname = Console.ReadLine();

            Console.Write("Enter last name: ");
            string lastname = Console.ReadLine();

            Console.Write("Enter phone number: ");
            string phone = Console.ReadLine();

            string email = "";
            int i = 0;

            while (string.IsNullOrWhiteSpace(email))
            {
                if(i == 0)
                {
                    Console.Write("Enter email: ");
                    email = Console.ReadLine();
                }
                else 
                {
                    Console.Write("Email is required! ");
                    Console.Write("Enter email: ");
                    email = Console.ReadLine();
                }
                
                i++;
            }


            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();


            using (var dbContext = new AppDbContext())
            {
                if (dbContext.Users.Any(u => u.Username == username))
                {
                    Console.WriteLine("Username already exists. Please choose another username.");
                    return;
                }

                dbContext.Users.Add(new User
                {
                    Username = username ,
                    Password = password,
                    Email = email ,
                    FirstName = firstname ?? "",
                    LastName = lastname ?? "",
                    PhoneNumber = phone ?? "",
                    Role = "User"
                });

                dbContext.SaveChanges();
            }

            Console.WriteLine("Registration successful!");
            Console.Write("\n");
            Console.Write("\n");
        }
        #endregion

        #region Dashboard and menu
        static void ShowUserDashboard(User user)
        {
            Console.WriteLine($"Welcome, {user.Username} (User)");
            Console.WriteLine("--------------------------------------");

            using (var dbContext = new AppDbContext())
            {
                
                int bookingCount = dbContext.Bookings.Where(b=>b.Username == user.Username).Count();

                Console.WriteLine("Dashboard");
                Console.WriteLine("------------------");
                Console.WriteLine($"Bookings: {bookingCount}");
                Console.WriteLine();
            }

        }

        static void ShowAdminDashboard()
        {
            Console.WriteLine("Welcome, Admin");
            Console.WriteLine("--------------------------------------");
            using (var dbContext = new AppDbContext())
            {
                int roomCount = dbContext.Rooms.Count();
                int roomTypeCount = dbContext.RoomTypes.Count();
                int bookingCount = dbContext.Bookings.Count();
                int userCount = dbContext.Users.Count();

                Console.WriteLine("Dashboard");
                Console.WriteLine("------------------");
                Console.WriteLine($"Rooms: {roomCount}");
                Console.WriteLine($"Room Types: {roomTypeCount}");
                Console.WriteLine($"Bookings: {bookingCount}");
                Console.WriteLine($"Users: {userCount}");
                Console.WriteLine();
            }
        }

        static void ShowUserMenu(User loggedInUser)
        {
            Console.WriteLine("Menu (User)");
            Console.WriteLine("--------------------------------------");
           
            Console.WriteLine("1. Bookings");
            Console.WriteLine("2. Profile");
            Console.WriteLine("0. Exit");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Bookings");
                    BookingManagement(loggedInUser);
                    break;
                case "2":
                    Console.WriteLine("PROFILE");
                    ShowUserProfile(loggedInUser);
                    break;
                case "0":
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            Console.Write("\n");
            Console.Write("\n");
        }

        static void ShowAdminMenu(User loggedInUser)
        {
            Console.WriteLine("Menu (Admin)");
            Console.WriteLine("--------------------------------------");
         
            Console.WriteLine("1. Rooms");
            Console.WriteLine("2. Room Types");
            Console.WriteLine("3. Bookings");
            Console.WriteLine("4. User Management");
            Console.WriteLine("5. Profile");
            Console.WriteLine("0. Exit");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("ROOMS MANAGEMENT");
                    RoomManagement(loggedInUser);
                    break;
                case "2":
                    Console.WriteLine("ROOM TYPES MANAGEMENT");
                    RoomTypeManagement(loggedInUser);
                    break;
                case "3":
                    Console.WriteLine("BOOKINGS MANAGEMENT");
                    BookingManagement(loggedInUser);
                    break;
                case "4":
                    Console.WriteLine("USER MANAGEMENT");
                    UserManagement(loggedInUser);
                    break;
                case "5":
                    Console.WriteLine("PROFILE");
                    ShowUserProfile(loggedInUser);

                    break;
                case "0":
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            Console.Write("\n");
            Console.Write("\n");
        }
        #endregion
      
        
        #region user profile
        static void ShowUserProfile(User user)
        {
            Console.WriteLine("User Profile");
            Console.WriteLine("------------------");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Role: {user.Role}");
            Console.WriteLine();

            Console.WriteLine("1. Change Password");
            Console.WriteLine("0. Back to Menu");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ChangePassword(user);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            Console.Write("\n");
            Console.Write("\n");
        }

        static void ChangePassword(User user)
        {
            Console.Write("Enter current password: ");
            string currentPassword = Console.ReadLine();

            // Check if the current password matches the user's password
            if (currentPassword != user.Password)
            {
                Console.WriteLine("Incorrect current password. Password change failed.");
                ChangePassword(user);
            }

            Console.Write("Enter new password: ");
            string newPassword = Console.ReadLine();

            // Update the user's password in the database
            using (var dbContext = new AppDbContext())
            {
                var userToUpdate = dbContext.Users.FirstOrDefault(u => u.Username == user.Username);
                if (userToUpdate != null)
                {
                    userToUpdate.Password = newPassword;
                    dbContext.SaveChanges();
                    Console.WriteLine("Password changed successfully!");
                    ShowUserProfile(user);
                }
                else
                {
                    Console.WriteLine("User not found. Password change failed.");
                }
            }
            Console.Write("\n");
            Console.Write("\n");
        }
        #endregion

        #region room type 
        static void RoomTypeManagement(User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                while (true)
                {
                    Console.WriteLine("Room Types Management");
                    Console.WriteLine("----------------------");

                    var roomTypes = dbContext.RoomTypes.ToList();
                    Console.WriteLine("Room Types");
                    Console.WriteLine("-------------------------------------------------------------");

                    Console.WriteLine("Type                 Capacity                      Description");
                    Console.WriteLine("-------------------------------------------------------------");

                    foreach (var roomType in roomTypes)
                    {
                        Console.WriteLine($"{roomType.Type,-20}  {roomType.Capacity,-10}  {roomType.Description,30}");
                    }

                    Console.WriteLine("-------------------------------------------------------------");
                    Console.Write("\n");
                    Console.Write("\n");
                    
                    Console.WriteLine("1. Add New Room Type");
                    Console.WriteLine("2. Delete a Room Type");
                    Console.WriteLine("3. Edit a Room Type");
                    Console.WriteLine("0. Back to Menu");
                    Console.WriteLine();

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddRoomType(dbContext);
                            break;
                        case "2":
                            DeleteRoomType(dbContext);
                            break;
                        case "3":
                            EditRoomType(dbContext);
                            break;
                        case "0":
                            Console.WriteLine("Returning to Menu...");
                            ShowAdminMenu(loggedInUser);
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.Write("\n");
                    Console.Write("\n");
                }
            }
        }
        static void AddRoomType(AppDbContext dbContext)
        {
            Console.WriteLine("Add New Room Type");
            Console.Write("Enter room type: ");
            string newType = Console.ReadLine();

            if (dbContext.RoomTypes.Any(rt => rt.Type == newType))
            {
                Console.WriteLine($"Room type '{newType}' already exists. Please enter a unique room type.");
                return;
            }

            Console.Write("Enter description: ");
            string newDescription = Console.ReadLine();

            Console.Write("Enter capacity: ");
            int newCapacity;
            while (!int.TryParse(Console.ReadLine(), out newCapacity))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                Console.Write("Enter capacity: ");
            }

            var newRoomType = new RoomType
            {
                Type = newType,
                Description = newDescription,
                Capacity = newCapacity
            };

            dbContext.RoomTypes.Add(newRoomType);
            dbContext.SaveChanges();

            Console.WriteLine("New room type added successfully!");
            Console.Write("\n");
            Console.Write("\n");
        }

        static void EditRoomType(AppDbContext dbContext)
        {
            Console.WriteLine("Edit a Room Type");
            Console.Write("Enter the room type to edit: ");
            string typeToEdit = Console.ReadLine();

            var roomTypeToEdit = dbContext.RoomTypes.FirstOrDefault(rt => rt.Type == typeToEdit);
            if (roomTypeToEdit != null)
            {
                Console.Write("Enter new description (leave blank to keep current): ");
                string newDescription = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newDescription))
                {
                    roomTypeToEdit.Description = newDescription;
                }

                Console.Write("Enter new capacity (leave blank to keep current): ");
                string newCapacityInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newCapacityInput) && int.TryParse(newCapacityInput, out int newCapacity))
                {
                    roomTypeToEdit.Capacity = newCapacity;
                }

                dbContext.SaveChanges();

                Console.WriteLine("Room type updated successfully!");
            }
            else
            {
                Console.WriteLine("Room type not found.");
            }
            Console.Write("\n");
            Console.Write("\n");
        }


        static void DeleteRoomType(AppDbContext dbContext)
        {
            Console.WriteLine("Delete a Room Type");
            Console.Write("Enter the room type to delete: ");
            string typeToDelete = Console.ReadLine();

            
            var roomTypeToDelete = dbContext.RoomTypes.FirstOrDefault(rt => rt.Type == typeToDelete);
            if (roomTypeToDelete != null)
            {
                
                dbContext.RoomTypes.Remove(roomTypeToDelete);
                dbContext.SaveChanges();

                Console.WriteLine("Room type deleted successfully!");
            }
            else
            {
                Console.WriteLine("Room type not found.");
            }
            Console.Write("\n");
            Console.Write("\n");
        }

        

        #endregion

        #region ROOMS
        static void RoomManagement(User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                while (true)
                {
                    Console.WriteLine("Rooms Management");
                    Console.WriteLine("----------------------");

                    var rooms = dbContext.Rooms.ToList();
                    Console.WriteLine("Rooms");
                    Console.WriteLine("----------------------------------------------------------------------------------------");

                    Console.WriteLine("Room Number    Type                 Price Per Night      Max Occupancy   Availability");
                    Console.WriteLine("----------------------------------------------------------------------------------------");

                    foreach (var room in rooms)
                    {
                        Console.WriteLine($"{room.RoomNumber,-14} {room.Type,-20} {room.PricePerNight,-20} {room.MaximumOccupancy,-15} {room.IsAvailable}");
                    }

                    Console.WriteLine("----------------------------------------------------------------------------------------");
                    Console.Write("\n");
                    Console.Write("\n");
                    Console.WriteLine("1. Add New Room");
                    Console.WriteLine("2. Delete a Room");
                    Console.WriteLine("3. Edit a Room");
                    Console.WriteLine("0. Back to Menu");
                    Console.WriteLine();

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddRoom(dbContext);
                            break;
                        case "2":
                            DeleteRoom(dbContext);
                            break;
                        case "3":
                            EditRoom(dbContext);
                            break;
                        case "0":
                            Console.WriteLine("Returning to Menu...");
                            ShowAdminMenu(loggedInUser);
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.Write("\n");
                    Console.Write("\n");
                }
            }
        }
        static void AddRoom(AppDbContext dbContext)
        {
            Console.WriteLine("Add New Room");
            Console.Write("Enter room number: ");
            int roomNumber;
            while (!int.TryParse(Console.ReadLine(), out roomNumber) || dbContext.Rooms.Any(r => r.RoomNumber == roomNumber))
            {
                if (dbContext.Rooms.Any(r => r.RoomNumber == roomNumber))
                {
                    Console.WriteLine("Room number already exists. Please enter a unique room number.");
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
                Console.Write("Enter room number: ");
            }

            var existingRoomTypes = dbContext.RoomTypes.ToList();

            Console.WriteLine("Existing Room Types:");
            foreach (var roomType in existingRoomTypes)
            {
                Console.WriteLine($"- {roomType.Type} (Capacity: {roomType.Capacity})");
            }

            string roomTypeInput;
            RoomType selectedRoomType = null;
            while (selectedRoomType == null)
            {
                Console.Write("Enter room type: ");
                roomTypeInput = Console.ReadLine();
                selectedRoomType = existingRoomTypes.FirstOrDefault(rt => rt.Type == roomTypeInput);
                if (selectedRoomType == null)
                {
                    Console.WriteLine("Invalid room type. Please select from the existing room types.");
                }
            }

            Console.Write("Enter price per night: ");
            decimal pricePerNight;
            while (!decimal.TryParse(Console.ReadLine(), out pricePerNight))
            {
                Console.WriteLine("Invalid input. Please enter a valid decimal.");
                Console.Write("Enter price per night: ");
            }

            Console.Write("Enter maximum occupancy: ");
            int maxOccupancy;
            while (!int.TryParse(Console.ReadLine(), out maxOccupancy) || maxOccupancy > selectedRoomType.Capacity)
            {
                Console.WriteLine($"Invalid input. Maximum occupancy cannot exceed {selectedRoomType.Capacity}.");
                Console.Write("Enter maximum occupancy: ");
            }

            var newRoom = new Room
            {
                RoomNumber = roomNumber,
                Type = selectedRoomType.Type,
                PricePerNight = pricePerNight,
                MaximumOccupancy = maxOccupancy,
                IsAvailable = true 
            };

            dbContext.Rooms.Add(newRoom);
            dbContext.SaveChanges();

            Console.WriteLine("New room added successfully!");
        }


        static void DeleteRoom(AppDbContext dbContext)
        {
            Console.WriteLine("Delete a Room");
            Console.Write("Enter the room number to delete: ");
            int roomNumber;
            while (!int.TryParse(Console.ReadLine(), out roomNumber))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                Console.Write("Enter the room number to delete: ");
            }

            var roomToDelete = dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (roomToDelete != null)
            {
                dbContext.Rooms.Remove(roomToDelete);
                dbContext.SaveChanges();

                Console.WriteLine("Room deleted successfully!");
            }
            else
            {
                Console.WriteLine("Room not found.");
            }
        }

        static void EditRoom(AppDbContext dbContext)
        {
            Console.WriteLine("Edit a Room");
            Console.Write("Enter the room number to edit: ");
            int roomNumber;
            while (!int.TryParse(Console.ReadLine(), out roomNumber))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                Console.Write("Enter the room number to edit: ");
            }

            
            var roomToEdit = dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (roomToEdit != null)
            {
                var existingRoomTypes = dbContext.RoomTypes.ToList();

                
                Console.WriteLine("Existing Room Types:");
                foreach (var roomType in existingRoomTypes)
                {
                    Console.WriteLine($"- {roomType.Type} (Capacity: {roomType.Capacity})");
                }

                string roomTypeInput;
                RoomType selectedRoomType = null;
                while (selectedRoomType == null)
                {
                    Console.Write("Enter room type (leave blank to keep current): ");
                    roomTypeInput = Console.ReadLine();

                    if (!string.IsNullOrEmpty(roomTypeInput))
                    {
                        selectedRoomType = existingRoomTypes.FirstOrDefault(rt => rt.Type == roomTypeInput);
                        if (selectedRoomType == null)
                        {
                            Console.WriteLine("Invalid room type. Please select from the existing room types.");
                        }
                        roomToEdit.Type = selectedRoomType.Type;

                    }
                    else
                    {
                        selectedRoomType = existingRoomTypes.FirstOrDefault(rt => rt.Type == roomToEdit.Type);

                    }
                   
                    
                }

                Console.Write("Enter new price per night (leave blank to keep current): ");
                string newPricePerNightInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPricePerNightInput) && decimal.TryParse(newPricePerNightInput, out decimal newPricePerNight))
                {
                    roomToEdit.PricePerNight = newPricePerNight;
                }

                Console.Write("Enter maximum occupancy (leave blank to keep current): ");
                var occurrenceInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(occurrenceInput))
                {
                    int maxOccupancy;
                    while (!int.TryParse(occurrenceInput, out maxOccupancy) || maxOccupancy > selectedRoomType.Capacity)
                    {
                        Console.WriteLine($"Invalid input. Maximum occupancy cannot exceed {selectedRoomType.Capacity}.");
                        Console.Write("Enter maximum occupancy: ");
                    }
                    roomToEdit.MaximumOccupancy = maxOccupancy;
                }
                
                dbContext.SaveChanges();

                Console.WriteLine("Room updated successfully!");
            }
            else
            {
                Console.WriteLine("Room not found.");
            }
        }


        #endregion

        #region Bookings
        static void AddBooking(AppDbContext dbContext, User loggedInUser)
        {
            Console.WriteLine("Add New Booking");

            var availableRooms = dbContext.Rooms.Where(r => r.IsAvailable).ToList();
            if (!availableRooms.Any())
            {
                Console.WriteLine("No available rooms to book.");
                return;
            }

            Console.WriteLine("Available Rooms:");
            foreach (var room in availableRooms)
            {
                Console.WriteLine($"- Room Number: {room.RoomNumber}, Type: {room.Type}, Price per Night: {room.PricePerNight}");
            }

            Console.Write("Enter room number to book: ");
            int roomNumber;
            while (!int.TryParse(Console.ReadLine(), out roomNumber) || !availableRooms.Any(r => r.RoomNumber == roomNumber))
            {
                Console.WriteLine("Invalid room number. Please enter a valid room number from the list.");
                Console.Write("Enter room number to book: ");
            }

            string username;

            if (loggedInUser.Role == "user")
            {
                username = loggedInUser.Username;
                Console.WriteLine($"Booking will be made for user: {username}");
            }
            else
            {
                Console.Write("Enter username: ");
                username = Console.ReadLine();

                if (!dbContext.Users.Any(u => u.Username == username))
                {
                    Console.WriteLine("Username does not exist. Please choose a different username.");
                    return;
                }
            }

            Console.Write("Enter check-in date (dd-MM-yyyy): ");
            DateTime checkInDate;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkInDate))
            {
                Console.WriteLine("Invalid date format. Please enter the date in the format dd-MM-yyyy.");
                Console.Write("Enter check-in date (dd-MM-yyyy): ");
            }

            Console.Write("Enter check-out date (dd-MM-yyyy): ");
            DateTime checkOutDate;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkOutDate) || checkOutDate <= checkInDate)
            {
                Console.WriteLine("Invalid date. Check-out date must be after check-in date.");
                Console.Write("Enter check-out date (dd-MM-yyyy): ");
            }

            var newBooking = new Booking
            {
                RoomNumber = roomNumber,
                Username = username,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate
            };

            var bookedRoom = dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (bookedRoom != null)
            {
                bookedRoom.IsAvailable = false;
            }

            dbContext.Bookings.Add(newBooking);
            dbContext.SaveChanges();

            Console.WriteLine("Booking added successfully!");
            BookingManagement(loggedInUser);
        }


        static void BookingManagement( User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                while (true)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("\n");
                    Console.WriteLine("-------------------");

                    var bookings = loggedInUser.Role == "Administrator" ? dbContext.Bookings.ToList() : dbContext.Bookings.Where(b => b.Username == loggedInUser.Username).ToList();
                    if (bookings.Any())
                    {
                        Console.WriteLine("Existing Bookings:");
                        foreach (var booking in bookings)
                        {
                            Console.WriteLine($"- Booking ID: {booking.BookingId}, Room Number: {booking.RoomNumber}, " +
                                              $"Username: {booking.Username}, Check-in: {booking.CheckInDate}, Check-out: {booking.CheckOutDate}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No bookings found.");
                    }

                    Console.WriteLine();
                    Console.WriteLine("1. Add New Booking");
                    Console.WriteLine("2. Edit Booking");
                    Console.WriteLine("3. Delete Booking");
                    Console.WriteLine("0. Back to Menu");
                    Console.WriteLine();

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddBooking(dbContext, loggedInUser);
                            break;
                        case "2":
                            EditBooking(dbContext, loggedInUser);
                            break;
                        case "3":
                            DeleteBooking(dbContext, loggedInUser);
                            break;
                        case "0":
                            Console.WriteLine("Returning to Menu...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.WriteLine();
                }

            }
        }
        static void DeleteBooking(AppDbContext dbContext, User loggedInUser)
        {
            if (loggedInUser.Role != "administrator")
            {
                Console.WriteLine("Only administrators can delete bookings.");
                return;
            }

            Console.WriteLine("Delete Booking");
            Console.Write("Enter booking ID to delete: ");
            int bookingId;
            while (!int.TryParse(Console.ReadLine(), out bookingId))
            {
                Console.WriteLine("Invalid input. Please enter a valid booking ID.");
                Console.Write("Enter booking ID to delete: ");
            }

            var bookingToDelete = dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (bookingToDelete != null)
            {
                var bookedRoom = dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == bookingToDelete.RoomNumber);
                if (bookedRoom != null)
                {
                    bookedRoom.IsAvailable = true;
                }

                dbContext.Bookings.Remove(bookingToDelete);
                dbContext.SaveChanges();

                Console.WriteLine("Booking deleted successfully!");
                BookingManagement(loggedInUser);
            }
            else
            {
                Console.WriteLine("Booking not found.");
            }
        }

        static void EditBooking(AppDbContext dbContext, User loggedInUser)
        {
            Console.WriteLine("Edit Booking");

            if (loggedInUser.Role == "user")
            {
                Console.WriteLine($"Editing bookings for user: {loggedInUser.Username}");
            }

            Console.Write("Enter booking ID to edit: ");
            int bookingId;
            while (!int.TryParse(Console.ReadLine(), out bookingId))
            {
                Console.WriteLine("Invalid input. Please enter a valid booking ID.");
                Console.Write("Enter booking ID to edit: ");
            }

            var bookingToEdit = dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (bookingToEdit != null)
            {
                if (loggedInUser.Role == "user" && bookingToEdit.Username != loggedInUser.Username)
                {
                    Console.WriteLine("You can only edit your own bookings.");
                    return;
                }

                Console.Write("Enter new room number (leave blank to keep current): ");
                string roomNumberInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(roomNumberInput) && int.TryParse(roomNumberInput, out int newRoomNumber))
                {
                    bookingToEdit.RoomNumber = newRoomNumber;
                }

                Console.Write("Enter new check-in date (leave blank to keep current): ");
                string checkInDateInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(checkInDateInput) && DateTime.TryParseExact(checkInDateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newCheckInDate))
                {
                    bookingToEdit.CheckInDate = newCheckInDate;
                }

                Console.Write("Enter new check-out date (leave blank to keep current): ");
                string checkOutDateInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(checkOutDateInput) && DateTime.TryParseExact(checkOutDateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newCheckOutDate))
                {
                    bookingToEdit.CheckOutDate = newCheckOutDate;
                }

                dbContext.SaveChanges();

                Console.WriteLine("Booking updated successfully!");
                BookingManagement(loggedInUser);
            }
            else
            {
                Console.WriteLine("Booking not found.");
            }
        }

        #endregion

        #region
        static void UserManagement(User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                while (true)
                {
                    Console.WriteLine("User Management");
                    Console.WriteLine("----------------");

                    var users = dbContext.Users.ToList();
                    Console.WriteLine("Users");
                    Console.WriteLine("-------------------------------------------------------------");
                    Console.WriteLine("Username             Role                   Name");
                    Console.WriteLine("-------------------------------------------------------------");
                    foreach (var user in users)
                    {
                        if (string.IsNullOrEmpty(user.Username))
                            continue;
                        Console.WriteLine($"{user.Username,-20}  {user.Role,-20}  {user.FirstName} {user.LastName}");
                    }
                    Console.WriteLine("-------------------------------------------------------------");

                    Console.WriteLine("1. Add User");
                    Console.WriteLine("2. Edit User");
                    Console.WriteLine("3. Delete User");
                    Console.WriteLine("0. Back to Menu");
                    Console.WriteLine();

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddUser(dbContext, loggedInUser);
                            break;
                        case "2":
                            EditUser(dbContext, loggedInUser);
                            break;
                        case "3":
                            DeleteUser(dbContext, loggedInUser);
                            break;
                        case "0":
                            Console.WriteLine("Returning to Menu...");
                            ShowAdminMenu(loggedInUser);
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.WriteLine();
                }
            }
        }
        static void AddUser(AppDbContext dbContext, User loggedInUser)
        {
            if (loggedInUser.Role != "Administrator")
            {
                Console.WriteLine("Only administrators can add users.");
                return;
            }

            Console.WriteLine("Add New User");

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            if (dbContext.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Please choose a different username.");
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            Console.Write("Enter first name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter email: ");
            string email = Console.ReadLine();

            Console.Write("Enter phone number: ");
            string phoneNumber = Console.ReadLine();

            Console.Write("Enter role (admin/user): ");
            string role = Console.ReadLine();

            var newUser = new User
            {
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Role = role
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            Console.WriteLine("User added successfully!");
        }
        static void EditUser(AppDbContext dbContext, User loggedInUser)
        {
            if (loggedInUser.Role != "administrator")
            {
                Console.WriteLine("Only administrators can edit users.");
                return;
            }

            Console.WriteLine("Edit User");

            Console.Write("Enter username of the user to edit: ");
            string usernameToEdit = Console.ReadLine();

            var userToEdit = dbContext.Users.FirstOrDefault(u => u.Username == usernameToEdit);
            if (userToEdit != null)
            {
                Console.Write("Enter new password (leave blank to keep current): ");
                string newPassword = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    userToEdit.Password = newPassword;
                }

                Console.Write("Enter new first name (leave blank to keep current): ");
                string newFirstName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newFirstName))
                {
                    userToEdit.FirstName = newFirstName;
                }

                Console.Write("Enter new last name (leave blank to keep current): ");
                string newLastName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newLastName))
                {
                    userToEdit.LastName = newLastName;
                }

                Console.Write("Enter new email (leave blank to keep current): ");
                string newEmail = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newEmail))
                {
                    userToEdit.Email = newEmail;
                }

                Console.Write("Enter new phone number (leave blank to keep current): ");
                string newPhoneNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPhoneNumber))
                {
                    userToEdit.PhoneNumber = newPhoneNumber;
                }

                Console.Write("Enter new role (admin/user) (leave blank to keep current): ");
                string newRole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newRole))
                {
                    userToEdit.Role = newRole;
                }

                dbContext.SaveChanges();

                Console.WriteLine("User updated successfully!");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }
        static void DeleteUser(AppDbContext dbContext, User loggedInUser)
        {
            if (loggedInUser.Role != "administrator")
            {
                Console.WriteLine("Only administrators can delete users.");
                return;
            }

            Console.WriteLine("Delete User");

            Console.Write("Enter username of the user to delete: ");
            string usernameToDelete = Console.ReadLine();

            var userToDelete = dbContext.Users.FirstOrDefault(u => u.Username == usernameToDelete);
            if (userToDelete != null)
            {
                dbContext.Users.Remove(userToDelete);
                dbContext.SaveChanges();

                Console.WriteLine("User deleted successfully!");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }


        #endregion

        #region User area
        static void UserDashboard(User loggedInUser)
        {
            while (true)
            {
                Console.WriteLine("User Dashboard");
                Console.WriteLine("----------------");
                Console.WriteLine("1. Search Available Rooms");
                Console.WriteLine("2. View My Bookings");
                Console.WriteLine("3. Cancel Booking");
                Console.WriteLine("4. Profile");
                Console.WriteLine("0. Logout");
                Console.WriteLine();

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchAvailableRooms(loggedInUser);
                        break;
                    case "2":
                        ViewMyBookings(loggedInUser);
                        break;
                    case "3":
                        CancelBooking(loggedInUser);
                        break;
                    case "4":
                        ShowUserProfile(loggedInUser);
                        break;
                    case "0":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void SearchAvailableRooms(User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                Console.WriteLine("Search Available Rooms");
                Console.WriteLine("-----------------------");

                Console.Write("Enter check-in date (dd-MM-yyyy): ");
                DateTime checkInDate;
                while (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkInDate))
                {
                    Console.WriteLine("Invalid date format. Please enter the date in the format dd-MM-yyyy.");
                    Console.Write("Enter check-in date (dd-MM-yyyy): ");
                }

                Console.Write("Enter check-out date (dd-MM-yyyy): ");
                DateTime checkOutDate;
                while (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out checkOutDate) || checkOutDate <= checkInDate)
                {
                    Console.WriteLine("Invalid date. Check-out date must be after check-in date.");
                    Console.Write("Enter check-out date (dd-MM-yyyy): ");
                }

                var roomTypes = dbContext.RoomTypes.Select(rt => rt.Type).Distinct().ToList();
                Console.WriteLine("Room Types:");
                foreach (var type in roomTypes)
                {
                    Console.WriteLine($"- {type}");
                }

                Console.Write("Select room type: ");
                string selectedRoomType = Console.ReadLine();

                if (!roomTypes.Contains(selectedRoomType))
                {
                    Console.WriteLine("Invalid room type selected.");
                    return;
                }

                var selectedRoomTypeDetails = dbContext.RoomTypes.FirstOrDefault(rt => rt.Type.ToLower() == selectedRoomType.ToLower());
                int maxOccupancy = selectedRoomTypeDetails?.Capacity ?? 0;

                Console.WriteLine($"Maximum Occupancy for {selectedRoomType}: {maxOccupancy}");

                Console.Write("Enter number of occupants: ");
                int numOccupants;
                while (!int.TryParse(Console.ReadLine(), out numOccupants) || numOccupants <= 0)
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer greater than 0.");
                    Console.Write("Enter number of occupants: ");
                }

                var availableRooms = dbContext.Rooms
                    .Where(r => r.IsAvailable)
                    .Where(r => r.Type.ToLower() == selectedRoomType.ToLower())
                    .Where(r => r.MaximumOccupancy >= numOccupants)
                    .ToList();

                Console.WriteLine("Available Rooms:");
                foreach (var room in availableRooms)
                {
                    Console.WriteLine($"Room Number: {room.RoomNumber}, Type: {room.Type}, Price per Night: {room.PricePerNight}");

                    Console.Write("Book this room? (Y/N): ");
                    string bookOption = Console.ReadLine().Trim().ToUpper();
                    if (bookOption == "Y")
                    {
                        MakeBooking(dbContext, loggedInUser, room, checkInDate, checkOutDate);
                    }
                }
            }
        }
        static void MakeBooking(AppDbContext dbContext, User loggedInUser, Room selectedRoom, DateTime checkInDate, DateTime checkOutDate)
        {
            var newBooking = new Booking
            {
                RoomNumber = selectedRoom.RoomNumber,
                Username = loggedInUser.Username,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate
            };

            selectedRoom.IsAvailable = false;

            dbContext.Bookings.Add(newBooking);
            dbContext.SaveChanges();

            Console.WriteLine("Booking added successfully!");
        }

        static void ViewMyBookings(User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                Console.WriteLine($"Bookings for User: {loggedInUser.Username}");
                Console.WriteLine("-----------------------");

                var userBookings = dbContext.Bookings
                    .Where(b => b.Username == loggedInUser.Username)
                    .ToList();

                if (userBookings.Any())
                {
                    Console.WriteLine("Your Bookings:");
                    foreach (var booking in userBookings)
                    {
                        Console.WriteLine($"Booking ID: {booking.BookingId}, Room Number: {booking.RoomNumber}, Check-in Date: {booking.CheckInDate}, Check-out Date: {booking.CheckOutDate}");
                    }
                }
                else
                {
                    Console.WriteLine("You have no bookings.");
                }
            }
        }

        static void CancelBooking(User loggedInUser)
        {
            using (var dbContext = new AppDbContext())
            {
                Console.WriteLine($"Cancel Booking for User: {loggedInUser.Username}");
                Console.WriteLine("-----------------------");

                Console.Write("Enter booking ID to cancel: ");
                int bookingId;
                while (!int.TryParse(Console.ReadLine(), out bookingId))
                {
                    Console.WriteLine("Invalid input. Please enter a valid booking ID.");
                    Console.Write("Enter booking ID to cancel: ");
                }

                var bookingToDelete = dbContext.Bookings.FirstOrDefault(b => b.BookingId == bookingId && b.Username == loggedInUser.Username);
                if (bookingToDelete != null)
                {
                    var room = dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == bookingToDelete.RoomNumber);
                    if (room != null)
                    {
                        room.IsAvailable = true;
                    }

                    dbContext.Bookings.Remove(bookingToDelete);
                    dbContext.SaveChanges();

                    Console.WriteLine("Booking cancelled successfully.");
                }
                else
                {
                    Console.WriteLine("Booking not found or you are not authorized to cancel this booking.");
                }
            }
        }


        #endregion

    }
}
