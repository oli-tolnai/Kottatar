using Kottatar.Entities.Dtos.Instrument;
using Kottatar.Entities.Entity_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Entities.Dtos.Music
{
    public class MusicViewDto
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string SheetMusicFile { get; set; } = "";
        public IEnumerable<InstrumentViewDto>? Instruments { get; set; }

        public int InstrumentCount => Instruments?.Count() ?? 0;
    }
}
