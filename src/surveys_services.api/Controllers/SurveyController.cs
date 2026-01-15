using MediatR;
using Microsoft.AspNetCore.Mvc;
using surveys_services.application.Commands.Commands;
using surveys_services.application.DTOs;
using surveys_services.application.Queries;
using surveys_services.application.Queries.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace surveys_services.api.Controllers
{
    [ApiController]
    [Route("api/surveys")]
    public class SurveysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SurveysController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("pendientes/{email}")]
        public async Task<ActionResult<List<PendingSurveyDto>>> GetPendingSurveys(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("El email del usuario no es válido.");
            }

            var query = new GetPendingSurveysByUserQuery(email);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("registerRespuesta")]
        public async Task<IActionResult> RegisterAnswer([FromBody] RegisterAnswerDto dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la petición no puede estar vacío.");

            var command = new RegisterAnswerCommand(dto);

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("respuestasEventoUsuario/{eventId}/{email}")]
        public async Task<ActionResult<SurveyResultByEventDto>> GetSurveyByEventAndUser(Guid eventId,string email)
        {
            var query = new GetUserSurveyAnswersByEventQuery(email, eventId);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound("No se encontró una encuesta asociada a este evento.");
            }

            return Ok(result);
        }

        [HttpGet("detailSurveyQuestion/{id}")]
        public async Task<ActionResult<SurveyAndQuestionDtoResponse>> GetSurveyStructure(Guid id)
        {
            var query = new GetDetailSurveyAndQuestionQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound("La encuesta no existe.");
            }

            return Ok(result);
        }

        [HttpGet("promedioRespuestasEvento/{eventId}")]
        public async Task<ActionResult<PromedioEventSurveyDto>> GetEventSurveyStats(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                return BadRequest("El ID del evento no es válido.");
            }

            var query = new PromedioEncuestaPorEventoQuery(eventId);

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"No se encontró una encuesta asociada al evento {eventId}.");
            }

            return Ok(result);
        }

        [HttpGet("respondidas/{email}")]
        public async Task<ActionResult<List<CompletedSurveyDto>>> GetCompletedSurveys(string email)
        {
            var query = new GetCompletedSurveysByUserQuery(email);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

    }
}