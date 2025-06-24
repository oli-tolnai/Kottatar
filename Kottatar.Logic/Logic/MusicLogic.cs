using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kottatar.Data;
using Kottatar.Entities.Dtos.Music;
using Kottatar.Entities.Entity_Models;
using Kottatar.Logic.Helpers;



namespace Kottatar.Logic.Logic
{
    public class MusicLogic
    {
        Repository<Music> repo;
        DtoProvider dtoProvider;

        public MusicLogic(Repository<Music> repo, DtoProvider dtoProvider)
        {
            this.repo = repo;
            this.dtoProvider = dtoProvider;
        }

        public void AddMusic(MusicCreateUpdateDto dto)
        {
            Music m = dtoProvider.Mapper.Map<Music>(dto);

            if (repo.GetAll().FirstOrDefault(m => m.Title == dto.Title) == null)
            {
                repo.Create(m);
            }
            else
            {
                throw new ArgumentException("Music with this title already exists");
            }
        }

        public IEnumerable<MusicShortViewDto> GetAllMusic()
        {
            return repo.GetAll().Select(x =>
                dtoProvider.Mapper.Map<MusicShortViewDto>(x)
            );
        }

        public void DeleteMusic(string id)
        {
            repo.DeleteById(id);
        }

        public void UpdateMusic(string id, MusicCreateUpdateDto dto)
        {
            Music oldm = repo.FindById(id);
            dtoProvider.Mapper.Map(dto, oldm);
            repo.Update(oldm);
        }

        public MusicViewDto GetMusic(string id)
        {
            Music mModel = repo.FindById(id);
            return dtoProvider.Mapper.Map<MusicViewDto>(mModel);
        }
    }
}
