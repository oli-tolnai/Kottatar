using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Entities.Dtos.Instrument
{
    public class InstrumentCreateDto
    {
        public required string MusicId { get; set; } = "";

        [MinLength(5)]
        [MaxLength(100)]
        public required string Type { get; set; } = "";

        [MinLength(5)]
        [MaxLength(200)]
        public required string AuidoFile { get; set; } = "";
    }
}
