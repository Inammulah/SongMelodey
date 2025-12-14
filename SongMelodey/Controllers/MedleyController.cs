using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongMelodey.Services;

namespace SongMelodey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedleyController : ControllerBase
    {
        private readonly IMedleyService _service;

        public MedleyController(IMedleyService service)
        {
            _service = service;
        }

        [HttpPost("{medleyId}/generate")]
        public async Task<IActionResult> Generate(int medleyId, CancellationToken ct)
        {
            var result = await _service.GenerateMedleyAsync(medleyId, ct);

            if (!result.Success)
                return StatusCode(500, result);

            return Ok(result);
        }
    }
}

