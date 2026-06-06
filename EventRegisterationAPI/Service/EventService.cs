using EventRegisterationAPI.Models;
using System.Text.Json;

namespace EventRegisterationAPI.Service
{
    public class EventService
    {
        private readonly string _filePath = "Data/data.json";
        private readonly object _lock = new();

        private DataStore LoadData()
        {
            if (!File.Exists(_filePath))
                return new DataStore();

            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new DataStore();

            return JsonSerializer.Deserialize<DataStore>(json) ?? new DataStore();
        }

        private void SaveData(DataStore data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }
        public ApiResponse CreateEvent(CreateEventRequest request)
        {
            lock (_lock)
            {
                var data = LoadData();

                if (data.Events.Any(e => e.Name.ToLower() == request.Name.ToLower()))
                    return new ApiResponse { Success = false, Message = "Event name must be unique" };

                if (request.TotalSeats <= 0)
                    return new ApiResponse { Success = false, Message = "Total seats must be greater than 0" };

                if (request.EventDate <= DateTime.Now)
                    return new ApiResponse { Success = false, Message = "Event date must be in future" };

                var newEvent = new EventModel
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    TotalSeats = request.TotalSeats,
                    EventDate = request.EventDate,
                    Registrations = new List<Registration>()
                };

                data.Events.Add(newEvent);
                SaveData(data);

                return new ApiResponse
                {
                    Success = true,
                    Message = "Event created successfully",
                    Data = newEvent.Id
                };
            }
        }
        public ApiResponse RegisterUser(RegisterUserRequest request)
        {
            lock (_lock)
            {
                var data = LoadData();

                // 1. Find event
                var ev = data.Events.FirstOrDefault(e => e.Id == request.EventId);

                if (ev == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Event not found."
                    };
                }

                // 2. Check if event already passed
                if (ev.EventDate <= DateTime.Now)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Cannot register for past events."
                    };
                }

                // 3. Check duplicate user (active registration only)
                var alreadyRegistered = ev.Registrations.Any(r =>
                    r.UserName.ToLower() == request.UserName.ToLower() &&
                    !r.IsCancelled);

                if (alreadyRegistered)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "User already registered for this event."
                    };
                }

                // 4. Check available seats
                var activeRegistrations = ev.Registrations.Count(r => !r.IsCancelled);

                if (activeRegistrations >= ev.TotalSeats)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "No seats available."
                    };
                }

                // 5. Create registration
                var registration = new Registration
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    RegisteredAt = DateTime.UtcNow,
                    IsCancelled = false
                };

                ev.Registrations.Add(registration);

                SaveData(data);

                return new ApiResponse
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = registration.Id
                };
            }
        }
        public List<EventResponse> GetEvents(bool upcomingOnly = false, bool sortByDate = false)
        {
            var data = LoadData();

            var events = data.Events.AsEnumerable();

            // Filter upcoming events
            if (upcomingOnly)
            {
                events = events.Where(e => e.EventDate > DateTime.Now);
            }

            // Sorting
            if (sortByDate)
            {
                events = events.OrderBy(e => e.EventDate);
            }

            var result = events.Select(e => new EventResponse
            {
                Id = e.Id,
                Name = e.Name,
                TotalSeats = e.TotalSeats,
                EventDate = e.EventDate,
                TotalRegistrations = e.Registrations.Count(r => !r.IsCancelled),
                AvailableSeats = e.TotalSeats - e.Registrations.Count(r => !r.IsCancelled)
            }).ToList();

            return result;
        }
        public ApiResponse CancelRegistration(CancelRegistrationRequest request)
        {
            lock (_lock)
            {
                var data = LoadData();

                // 1. Find event
                var ev = data.Events.FirstOrDefault(e => e.Id == request.EventId);

                if (ev == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Event not found."
                    };
                }

                // 2. Find active registration
                var registration = ev.Registrations.FirstOrDefault(r =>
                    r.UserName.ToLower() == request.UserName.ToLower() &&
                    !r.IsCancelled);

                if (registration == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Registration not found."
                    };
                }

                // 3. Cancel registration (soft delete)
                registration.IsCancelled = true;

                SaveData(data);

                return new ApiResponse
                {
                    Success = true,
                    Message = "Registration cancelled successfully.",
                    Data = registration.Id
                };
            }
        }
    }
}
