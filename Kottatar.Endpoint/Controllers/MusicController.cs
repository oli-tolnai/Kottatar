using Kottatar.Data;
using Kottatar.Entities.Dtos.Music;
using Kottatar.Entities.Entity_Models;
using Kottatar.Logic.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Kottatar.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        MusicLogic logic;

        public MusicController(MusicLogic logic)
        {
            this.logic = logic;
        }

        [HttpPost]
        public void AddMusic(MusicCreateDto dto)
        {
            logic.AddMusic(dto);
        }

        [HttpGet]
        public IEnumerable<MusicShortViewDto> GetAllMusic()
        {
            return logic.GetAllMusic();
        }
    }
}
