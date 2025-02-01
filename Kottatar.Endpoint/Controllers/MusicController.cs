using Kottatar.Data;
using Kottatar.Entities.Entity_Models;
using Microsoft.AspNetCore.Mvc;

namespace Kottatar.Endpoint.Controllers
{

    public class MusicCreateDto
    {
        public string Title { get; set; }
        public string SheetMusicFile { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        Repository<Music> repo;

        public MusicController(Repository<Music> repo)
        {
            this.repo = repo;
        }

        [HttpPost]
        public void AddMusic(MusicCreateDto dto)
        {
            var m = new Music(dto.Title, dto.SheetMusicFile);
            repo.Create(m);
        }
    }
}
