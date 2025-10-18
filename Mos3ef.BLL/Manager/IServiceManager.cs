using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager
{
    public interface IServiceManager
    {
        Task<IEnumerable<Service>> SearchServicesAsync(string keyword);
    }
}
