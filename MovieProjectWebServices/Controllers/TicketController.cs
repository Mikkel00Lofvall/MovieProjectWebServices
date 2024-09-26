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

        private readonly CinemaHallRepository _HallRepository;
        private readonly MovieRepository _MovieRepository;
        private readonly TicketRepository _TicketRepository;

        public TicketController(CinemaHallRepository hallRepository, MovieRepository movieRepository, TicketRepository ticketRepository)
        {
            _HallRepository = hallRepository;
            _MovieRepository = movieRepository;
            _TicketRepository = ticketRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TicketDTO ticket)
        {
            if (ticket == null)
            {
                (bool result, string message, MovieModel movie) = await _MovieRepository.GetWithId(ticket.MovieID);
                (result, message, CinemaHallModel hall) = await _HallRepository.GetWithId(ticket.MovieID);

                if (movie != null && hall != null)
                {
                    TicketModel ticketModel = new TicketModel()
                    {
                        MovieID = ticket.MovieID,
                        HallID = ticket.HallID,
                        SeatID = ticket.SeatID,
                    };

                    (result, message) = await _TicketRepository.Create(ticketModel);

                    if (result)
                    {
                        return Ok();
                    }

                    else return BadRequest(message);
                }

                else return BadRequest("No Movie Or Hall With Those IDs");
            }

            else return BadRequest("No Ticket Data");
        }
    }
}
