using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.DTO;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {

        private readonly ScheduleRepository _scheduleRepository;
        private readonly TicketRepository _TicketRepository;

        public TicketController(ScheduleRepository scheduleRepo, TicketRepository ticketRepo)
        {
            _TicketRepository = ticketRepo;
            _scheduleRepository = scheduleRepo;
        }

        [HttpGet("GetTicketWithScheduleID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool result, string message, List<TicketDTO> tickets) = await _TicketRepository.GetAllWithScheduleID(id);
            if (result == false) return Problem(message);

            if (tickets.Count() > 0)
            {
                return Ok(tickets);
            }

            else
            {
                return Ok("No Tickets For This Schedule");
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TicketCreateDTO ticket)
        {
            if (ticket != null)
            {
                (bool result, string message, ScheduleModel schedue) = await _scheduleRepository.GetWithId(ticket.ScheduleID);;

                if (schedue != null)
                {
                    TicketModel ticketModel = new TicketModel()
                    {
                        SeatID = ticket.SeatID,
                        ScheduleID = ticket.ScheduleID,
                        DateID = ticket.DateID,
                        Email = ticket.Email,
                        PhoneNumber = ticket.PhoneNumber,
                    };

                    (result, message) = await _TicketRepository.Create(ticketModel);

                    if (result)
                    {
                        return Ok();
                    }

                    else return Problem(message);
                }

                else return Problem("No Movie Or Hall With Those IDs");
            }

            else return Problem("No Ticket Data");
        } 
    }
}
