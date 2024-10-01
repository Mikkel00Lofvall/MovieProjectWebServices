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

        [HttpGet("GetTicketWithScheduleID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool result, string message, List<TicketModel> tickets) = await _TicketRepository.GetAllWithScheduleID(id);
            if (result == false) return BadRequest(message);

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
        public async Task<IActionResult> Create([FromBody] TicketDTO ticket)
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
