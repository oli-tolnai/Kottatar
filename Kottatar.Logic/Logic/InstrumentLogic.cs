using Kottatar.Data;
using Kottatar.Entities.Dtos.Instrument;
using Kottatar.Entities.Entity_Models;
using Kottatar.Logic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Logic.Logic
{
    public class InstrumentLogic
    {
        Repository<Instrument> repo;
        DtoProvider dtoProvider;

        public InstrumentLogic(Repository<Instrument> repo, DtoProvider dtoProvider)
        {
            this.repo = repo;
            this.dtoProvider = dtoProvider;
        }

        public void AddInstrument(InstrumentCreateDto dto)
        {
            var model = dtoProvider.Mapper.Map<Instrument>(dto);
            if (repo.GetAll().FirstOrDefault(i => i.Type == dto.Type) == null)
            {
                repo.Create(model);
            }
            else
            {
                throw new System.Exception("Instrument with this type already exists");
            }
        }
    }
}
