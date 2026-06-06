using Microsoft.AspNetCore.Mvc;

using EventRegisterationAPI.Models;
using EventRegisterationAPI.Service;

namespace EventRegistrationAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly EventService _service;

        public EventsController(EventService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult CreateEvent(CreateEventRequest request)
        {
            var result = _service.CreateEvent(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("register")]
        public IActionResult RegisterUser(RegisterUserRequest request)
        {
            var result = _service.RegisterUser(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet]
        public IActionResult GetEvents([FromQuery] bool upcomingOnly = false, [FromQuery] bool sortByDate = false)
        {
            var result = _service.GetEvents(upcomingOnly, sortByDate);
            return Ok(result);
        }
        [HttpPost("cancel")]
        public IActionResult CancelRegistration(CancelRegistrationRequest request)
        {
            var result = _service.CancelRegistration(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}