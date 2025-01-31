using Kottatar.Data;
using Kottatar.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Kottatar.Endpoint.Controllers
{
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
        public void AddMusic(Music music)
        {
            repo.Create(music);
        }
    }
}
