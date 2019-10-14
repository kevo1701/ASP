using CommonFiles.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureInterface
{
    public interface IDefaultService<DTOType> where DTOType:DTO
    {
        void Insert(DTOType dto);

        void Delete(int id);

        void Edit(DTOType dto);

        DTOType Get(int id);

        IEnumerable<DTOType> GetAll();
    }
}
