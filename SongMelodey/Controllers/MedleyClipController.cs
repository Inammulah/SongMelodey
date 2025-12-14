using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongMelodey.Models;
using SongMelodey.Services;
using SongMelodey.Dto;

namespace SongMelodey.Controllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        public class MedleyClipController : ControllerBase
        {
            private readonly SongsMedleyMakerAndDjContext _db;

            public MedleyClipController(SongsMedleyMakerAndDjContext db)
            {
                _db = db;
            }

            // 1️⃣ Add TrimClip to Medley
            [HttpPost("add")]
            public async Task<IActionResult> AddClip(AddMedleyClipDto dto)
            {
                var maxSeq = _db.MedleyClips
                    .Where(m => m.MedleyId == dto.MedleyId)
                    .Select(m => (int?)m.SequenceNumber)
                    .Max() ?? 0;

                var medleyClip = new MedleyClip
                {
                    MedleyId = dto.MedleyId,
                    TrimClipId = dto.TrimClipId,
                    SequenceNumber = maxSeq + 1
                };

                _db.MedleyClips.Add(medleyClip);
                await _db.SaveChangesAsync();

                return Ok(medleyClip);
            }

            // 2️⃣ Reorder Medley Clips
            [HttpPut("reorder")]
            public async Task<IActionResult> Reorder(ReorderMedleyClipDto dto)
            {
                var clip = await _db.MedleyClips.FindAsync(dto.MedleyClipId);
                if (clip == null) return NotFound();

                var clips = _db.MedleyClips
                    .Where(m => m.MedleyId == clip.MedleyId)
                    .OrderBy(m => m.SequenceNumber)
                    .ToList();

                clips.Remove(clip);
                clips.Insert(dto.NewSequenceNumber - 1, clip);

                for (int i = 0; i < clips.Count; i++)
                    clips[i].SequenceNumber = i + 1;

                await _db.SaveChangesAsync();
                return Ok("Reordered successfully");
            }

            // 3️⃣ Remove Clip from Medley
            [HttpDelete("{medleyClipId}")]
            public async Task<IActionResult> Remove(int medleyClipId)
            {
                var clip = await _db.MedleyClips.FindAsync(medleyClipId);
                if (clip == null) return NotFound();

                _db.MedleyClips.Remove(clip);
                await _db.SaveChangesAsync();

                return Ok("Removed");
            }

            // 4️⃣ Add or Update Transition
            [HttpPost("transition")]
            public async Task<IActionResult> AddTransition(AddTransitionDto dto)
            {
                var existing = _db.TransitionClips
                    .FirstOrDefault(t => t.MedleyClipId == dto.MedleyClipId);

                if (existing != null)
                {
                    existing.TransitionName = dto.TransitionName;
                    existing.FilePath = dto.FilePath;
                    existing.DurationMs = dto.DurationMs;
                }
                else
                {
                    _db.TransitionClips.Add(new TransitionClip
                    {
                        MedleyClipId = dto.MedleyClipId,
                        TransitionName = dto.TransitionName,
                        FilePath = dto.FilePath,
                        DurationMs = dto.DurationMs
                    });
                }

                await _db.SaveChangesAsync();
                return Ok("Transition saved");
            }

            // 5️⃣ Get Medley Timeline
            [HttpGet("timeline/{medleyId}")]
            public IActionResult GetTimeline(int medleyId)
            {
                var timeline = _db.MedleyClips
                    .Where(m => m.MedleyId == medleyId)
                    .OrderBy(m => m.SequenceNumber)
                    .Select(m => new
                    {
                        m.MedleyClipId,
                        m.SequenceNumber,
                        TrimClipPath = m.TrimClip.FilePath,
                        Transition = m.TransitionClip == null ? null : new
                        {
                            m.TransitionClip.TransitionName,
                            m.TransitionClip.FilePath,
                            m.TransitionClip.DurationMs
                        }
                    }).ToList();

                return Ok(timeline);
            }
        }
    }

