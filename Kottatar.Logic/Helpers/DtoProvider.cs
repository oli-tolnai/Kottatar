using AutoMapper;
using Kottatar.Entities.Dtos.Music;
using Kottatar.Entities.Entity_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Logic.Helpers
{
    public class DtoProvider
    {
        public Mapper Mapper { get; }

        public DtoProvider()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Music, MusicShortViewDto>();
                cfg.CreateMap<Music, MusicViewDto>();
                cfg.CreateMap<MusicCreateUpdateDto, Music>();

            });

            Mapper = new Mapper(config);
        }
    }
}
