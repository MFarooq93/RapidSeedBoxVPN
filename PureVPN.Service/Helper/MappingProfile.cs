using Atom.Core.Models;
using AutoMapper;
using PureVPN.Entity.Models;

namespace PureVPN.Service.Helper
{
    public static class MappingProfile
    {
        /// <summary>
        /// Maps one object to another object depending upon the Name and Type of the Properties
        /// </summary>
        /// <returns></returns>
        public static IMapper InitializeAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Country, CountryModel>();
                cfg.CreateMap<Protocol, ProtocolModel>();
                cfg.CreateMap<CountryModel, Country>();
                cfg.CreateMap<City, CityModel>();
                cfg.CreateMap<CityModel, City>();
            });
            return config.CreateMapper();
        }
    }
}
