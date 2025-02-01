using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Entities.Dtos.Music
{
    public class MusicCreateDto
    {
        public string Title { get; set; } = "";
        public string SheetMusicFile { get; set; } = "";
    }
}
