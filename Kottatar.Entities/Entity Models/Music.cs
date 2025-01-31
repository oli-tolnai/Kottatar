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
        public Music(string title, string sheetMusicFile)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            SheetMusicFile = sheetMusicFile;
        }

        [StringLength(50)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(200)]
        public string SheetMusicFile { get; set; }

        [NotMapped]
        public virtual ICollection<Instrument>? Instruments { get; set; }

    }
}
