//using Microsoft.AspNetCore.Mvc;
//using BusinessLogic.Services;
//using DataAccess.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace RestApi.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class FlightController : ControllerBase
//    {
//        private readonly IFlightService _flightService;

//        public FlightController(IFlightService flightService)
//        {
//            _flightService = flightService;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
//        {
//            var flights = await _flightService.GetAllFlightsAsync();
//            return Ok(flights);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Flight>> GetFlight(int id)
//        {
//            var flight = await _flightService.GetFlightByIdAsync(id);
//            if (flight == null)
//                return NotFound();
//            return Ok(flight);
//        }

//        [HttpGet("number/{flightNumber}")]
//        public async Task<ActionResult<Flight>> GetFlightByNumber(string flightNumber)
//        {
//            var flight = await _flightService.GetFlightByNumberAsync(flightNumber);
//            if (flight == null)
//                return NotFound();
//            return Ok(flight);
//        }

//        [HttpPost]
//        public async Task<ActionResult<Flight>> CreateFlight(Flight flight)
//        {
//            await _flightService.AddFlightAsync(flight);
//            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
//        }

//        [HttpPut("{id}/status")]
//        public async Task<IActionResult> UpdateFlightStatus(int id, [FromBody] FlightStatus status)
//        {
//            try
//            {
//                await _flightService.UpdateFlightStatusAsync(id, status);
//                return NoContent();
//            }
//            catch (KeyNotFoundException)
//            {
//                return NotFound();
//            }
//        }
//    }
//}