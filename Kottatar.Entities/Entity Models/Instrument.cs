using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kottatar.Entities;

namespace Kottatar.Entities
{
    public class Instrument : IIdEntity
    {
        public Instrument(string musicId, string type, string auidoFile)
        {
            Id = Guid.NewGuid().ToString();
            MusicId = musicId;
            Type = type;
            AuidoFile = auidoFile;
        }

        [StringLength(50)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [StringLength(50)]
        public string MusicId { get; set; }

        [NotMapped]
        public virtual Music? Music { get; set; }

        [StringLength(150)]
        public string Type { get; set; }

        [StringLength(200)]
        public string AuidoFile { get; set; }


    }
}
