using FlightPoc.Exceptions;
using FlightPoc.Models;
using FlightPoc.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using FlightPoc.API.Application.Interfaces;
using FlightPoc.API.Application.Commands;

namespace FlightPoc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckInPassenger([FromBody] CheckInRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CheckInPassengerCommand
            {
                FlightId = request.FlightId,
                PassengerName = request.PassengerName,
                PassengerUniqueId = request.PassengerUniqueId,
                BaggageWeights = request.BaggageWeights
            };

            try
            {
                await _checkInService.CheckIn(command);
                return Ok(new { message = $"Passenger '{request.PassengerName}' successfully checked in." });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
