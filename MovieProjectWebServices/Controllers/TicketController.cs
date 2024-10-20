﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetTickets")]
        public async Task<IActionResult> Get()
        {
            try
            {
                (bool result, string message, ICollection<TicketModel> tickets) = await _TicketRepository.GetAll();
                if (result)
                {
                    return Ok(tickets);
                }

                else
                {
                    return Problem("No Ticket");
                }
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("GetTicketWithScheduleID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                (bool result, string message, List<TicketDTO> tickets) = await _TicketRepository.GetAllWithScheduleID(id);
                if (result)
                {
                    return Ok(tickets);
                }

                else
                {
                    return Problem("No Tickets");
                }
            }
            catch (Exception ex) { return Problem(ex.Message); }
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

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            (bool result, string message, var theme) = await _TicketRepository.GetWithId(id);
            if (result)
            {
                if (theme != null)
                {
                    await _TicketRepository.Delete(id);
                    return Ok();
                }

                else return Problem("ticket does not exist in db");
            }

            else return Problem(message);
        }
    }
}
