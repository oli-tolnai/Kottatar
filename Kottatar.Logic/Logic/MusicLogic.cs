using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kottatar.Data;
using Kottatar.Entities.Dtos.Music;
using Kottatar.Entities.Entity_Models;



namespace Kottatar.Logic.Logic
{
    public class MusicLogic
    {
        Repository<Music> repo;

        public MusicLogic(Repository<Music> repo)
        {
            this.repo = repo;
        }

        public void AddMusic(MusicCreateDto dto)
        {
            Music m = new Music(dto.Title, dto.SheetMusicFile);
            if (repo.GetAll().FirstOrDefault(m => m.Title == dto.Title) == null)
            {
                repo.Create(m);
            }
            else
            {
                throw new System.Exception("Music with this title already exists");
            }
        }

        public IEnumerable<MusicShortViewDto> GetAllMusic()
        {
            return repo.GetAll().Select(x => 
                new MusicShortViewDto()
                {
                    Id = x.Id,
                    Title = x.Title
                });
        }   

        public void DeleteMusic(string id)
        {
            repo.DeleteById(id);
        }
    }
}
