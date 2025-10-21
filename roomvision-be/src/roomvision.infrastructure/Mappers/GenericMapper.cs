using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Mappers
{
    public class GenericMapper : IGenericMapper
    {
        private readonly IMapper mapper;

        public GenericMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoomDbModel,Room>();
                cfg.CreateMap<Room, RoomDbModel>();
                
                
                cfg.CreateMap<AccountDbModel,Account>();
                cfg.CreateMap<Account,AccountDbModel>();
            });

            mapper = config.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}
