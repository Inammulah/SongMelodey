using Microsoft.AspNetCore.Mvc;
using SongMelodey.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SongMelodey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly SongsMedleyMakerAndDjContext _db;
        private readonly IWebHostEnvironment _env;

        public SongController(SongsMedleyMakerAndDjContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetAllSongs()
        {
            var songs = _db.Songs.ToList();
            return Ok(songs);
        }
        [HttpGet("{id}")]
        public IActionResult GetSong(int id)
        {
           var song = _db.Songs.Find(id);
            if(song == null)
                return NotFound();
            return Ok(song);
        }
        [HttpPost("add")]
        public IActionResult AddSong([FromForm] SongCreateDto dto)
        {
            if (dto.File == null)
            {
                return BadRequest("File is required");
            }
            var folder = Path.Combine(_env.WebRootPath, "songs");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);

            }
            var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(folder, fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                dto.File.CopyTo(stream);
            }
           var song = new Song
            {
                ThemeId = dto.ThemeId,
                SongTitle = dto.SongTitle,
                ArtistName = dto.ArtistName,
                DurationSec = dto.DurationSec,
                FilePath = "/songs/" + fileName
            };
           
            _db.Songs.Add(song);
            _db.SaveChanges();
            return Ok(song);
        }

     
        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
            {
           var song =  _db.Songs.Find(id);
            if(song == null)
                return NotFound();

            _db.Songs.Remove(song);
            _db.SaveChanges();

            return Ok("Deleted");

        }
    }

}






//[Route("api/[controller]")]
//[ApiController]
//public class SongsController : ControllerBase

//{
//    private readonly SongsMedleyMakerAndDjContext context;

//    public SongsController(SongsMedleyMakerAndDjContext context)
//    {
//        this.context = context;
//    }
//    // GET: api/<ValuesController>

//    [HttpGet("GetAllSong")]
//    public IActionResult GetAllSong()
//    {
//        var songs = context.Songs.ToList();
//        if (songs == null || songs.Count == 0)
//        {
//            return NotFound("song does not exists");
//        }
//        return Ok(songs);

//    }
//    // GET api/<ValuesController>/5
//    [HttpGet("{id}")]
//    public string Get(int id)
//    {
//        return "value";
//    }

//    // POST api/<ValuesController>
//    [HttpPost]
//    public void AddSong([FromBody] string value)
//    {


//    }


//    // PUT api/<ValuesController>/5
//    [HttpPut("{id}")]
//    public void Put(int id, [FromBody] string value)
//    {
//    }

//    // DELETE api/<ValuesController>/5
//    [HttpDelete("{id}")]
//    public void Delete(int id)
//    {
//    }
//}