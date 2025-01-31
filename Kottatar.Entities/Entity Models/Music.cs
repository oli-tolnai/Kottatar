using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Entities.Entity_Models
{
    public class Music
    {
        public Music(string title, string sheetMusic)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            this.sheetMusic = sheetMusic;
        }

        [StringLength(50)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(200)]
        public string sheetMusicFile { get; set; }

        [NotMapped]
        public virtual ICollection<Instrument>? Instruments { get; set; }

    }
}
