using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager
{
    public class ServiceManager : IServiceManager
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceManager(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string keyword)
        {
            return await _serviceRepository.SearchServicesAsync(keyword);
        }
    }
}
