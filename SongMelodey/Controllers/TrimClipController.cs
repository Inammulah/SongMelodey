using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongMelodey.Models;
using SongMelodey.Services;

namespace SongMelodey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrimClipController : ControllerBase
    {
        private readonly SongsMedleyMakerAndDjContext _db;
        private readonly ITrimService _trimService;

        public TrimClipController(SongsMedleyMakerAndDjContext db, ITrimService trimService)
        {
            _db = db;
            _trimService = trimService;
        }

        /// <summary>
        /// Create TrimClip by trimming an existing Song on server
        /// </summary>
        [HttpPost("create-from-song")]
        public async Task<IActionResult> CreateFromSong([FromBody] CreateTrimFromSongDto dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest("Payload required.");
            if (dto.EndMs <= dto.StartMs) return BadRequest("EndMs must be greater than StartMs.");

            var song = await _db.Songs.FindAsync(dto.SongId);
            if (song == null) return NotFound("Song not found.");

            var res = await _trimService.TrimFromSongAsync(dto.SongId, dto.StartMs, dto.EndMs, ct);
            if (!res.Success) return StatusCode(500, new { message = "Trim failed", error = res.Error });


            // save DB record
            var clip = new TrimClip
            {
                SongId = dto.SongId,
                StartMs = dto.StartMs,
                EndMs = dto.EndMs,
                ClipLengthMs = (int)(res.ClipLengthMs),
                FilePath = res.FilePath
            };

            _db.TrimClips.Add(clip);
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                clip.TrimClipId,
                clip.SongId,
                clip.StartMs,
                clip.EndMs,
                clip.ClipLengthMs,
                clip.FilePath
            });

        }

        /// <summary>
        /// Upload a client trimmed clip file directly (optional)
        /// </summary>
        [HttpPost("upload-clip")]
        public async Task<IActionResult> UploadClip([FromForm] UploadTrimClipDto dto, CancellationToken ct)
        {
            if (dto?.File == null) return BadRequest("File required.");
            var res = await _trimService.SaveUploadedClipAsync(dto.File, ct);
            if (!res.Success) return StatusCode(500, new { message = "Save failed", error = res.Error });

            // optional: create a DB trim clip record without SongId (or require SongId)
            var clip = new TrimClip
            {
                SongId = 0, // or require SongId in form
                StartMs = 0,
                EndMs = 0,
                ClipLengthMs = (int)res.ClipLengthMs,
                FilePath = res.FilePath
            };
            _db.TrimClips.Add(clip);
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                clip.TrimClipId,
                clip.FilePath
            });
        }

        /// <summary>
        /// Get all trim clips for a song
        /// </summary>
        [HttpGet("by-song/{songId}")]
        public IActionResult GetBySong(int songId)
        {
            var clips = _db.TrimClips.Where(t => t.SongId == songId).Select(t => new {
                t.TrimClipId,
                t.SongId,
                t.StartMs,
                t.EndMs,
                t.ClipLengthMs,
                t.FilePath
            }).ToList();
            return Ok(clips);
        }
    }

}
