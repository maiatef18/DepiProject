using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository
{
    public interface IServiceRepository
    {
        // We'll add methods for GetById, Filter, etc., later
        Task<IEnumerable<Service>> SearchServicesAsync(string keyword);
    }
}
