using Kottatar.Entities.Dtos.Instrument;
using Kottatar.Logic.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Kottatar.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentController : ControllerBase
    {
        InstrumentLogic logic;

        public InstrumentController(InstrumentLogic logic)
        {
            this.logic = logic;
        }

        [HttpPost]
        public void AddInstrument(InstrumentCreateDto dto)
        {
            logic.AddInstrument(dto);
        }
    }
}
