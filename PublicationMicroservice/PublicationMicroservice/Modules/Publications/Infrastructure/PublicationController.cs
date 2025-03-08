using Microsoft.AspNetCore.Mvc;
using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;
using PublicationMicroservice.Modules.Publications.Infrastructure.Dtos.Request;
using PublicationMicroservice.Modules.Publications.Infrastructure.Dtos.Response;
using PublicationMicroservice.Modules.Publications.Infrastructure.Services;
using UserMicroservice.Modules.Users.Infrastructure.Extensions;

namespace PublicationMicroservice.Modules.Publications.Infrastructure
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationController : ControllerBase
    {
        private readonly PublicationService _publicationService;
        private readonly TokenValidationService _tokenValidationService;
        private readonly ILogger<PublicationController> _logger;

        public PublicationController(
            PublicationService publicationService,
            ILogger<PublicationController> logger,
            TokenValidationService tokenValidationService)
        {
            _publicationService = publicationService;
            _tokenValidationService = tokenValidationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublication([FromBody] CreatePublicationRequest dto)
        {
            _logger.LogInformation("Received request to create publication");
            try
            {
                var token = HttpContext.GetJwtTokenFromHeader();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("No authorization token provided");
                }

                var validationResult = await _tokenValidationService.ValidateToken(token);

                if (!validationResult.IsValid)
                {
                    return Unauthorized("Invalid token");
                }

                var publication = new Publication(
                    PublicationId.New(),
                    PublicationTitle.FromString(dto.Title),
                    PublicationContent.FromString(dto.Content),
                    validationResult.UserId,
                    CreatedAt.Now(),
                    UpdatedAt.Now()
                );

                await _publicationService.CreatePublication(publication);
                return Created($"/api/publication/{publication.Id.Value}", null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating publication");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetPublicationsByUserId()
        {
            _logger.LogInformation($"Received request to get publications by user");
            try
            {
                var token = HttpContext.GetJwtTokenFromHeader();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("No authorization token provided");
                }

                var validationResult = await _tokenValidationService.ValidateToken(token);

                var publications = await _publicationService.GetPublicationsByUserId(validationResult.UserId);
                return Ok(publications.Select(p => new PublicationResponse
                {
                    Id = p.Id.Value,
                    Title = p.Title.Value,
                    Content = p.Content.Value,
                    UserId = p.UserId,
                    CreatedAt = p.CreatedAt.Value,
                    UpdatedAt = p.UpdatedAt.Value
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting publications by user");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(string id)
        {
            _logger.LogInformation($"Received request to delete publication with id: {id}");
            try
            {
                var token = HttpContext.GetJwtTokenFromHeader();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("No authorization token provided");
                }

                var validationResult = await _tokenValidationService.ValidateToken(token);

                if (!validationResult.IsValid)
                {
                    return Unauthorized("Invalid token");
                }

                var publication = await _publicationService.GetPublicationById(id);

                if (publication == null)
                {
                    return NotFound("Publication not found");
                }

                await _publicationService.DeletePublication(id);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error deleting publication with id: {id}");
                return BadRequest(e.Message);
            }
        }
    }
}