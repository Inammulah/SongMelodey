using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SongMelodey.Models;

namespace SongMelodey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly SongsMedleyMakerAndDjContext _db;

        public ThemeController(SongsMedleyMakerAndDjContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetThemes()
        {
            return Ok(_db.Themes.ToList());
        }

        [HttpPost]
        public IActionResult AddTheme([FromBody] Theme theme)
        {
            _db.Themes.Add(theme);
            _db.SaveChanges();
            return Ok(theme);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTheme(int id)
        {
            var theme = _db.Themes.Find(id);
            if (theme == null) return NotFound("Theme not found");

            _db.Themes.Remove(theme);
            _db.SaveChanges();

            return Ok("Deleted");
        }
    }

}
