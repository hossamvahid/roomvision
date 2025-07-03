using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using roomvision.domain.Entities;
using roomvision.application.Interfaces.Mappers;
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
                cfg.CreateMap<Account, AccountEntity>();
                cfg.CreateMap<AccountEntity, Account>();

                cfg.CreateMap<PersonFace, PersonFaceEntity>();
                cfg.CreateMap<PersonFaceEntity, PersonFace>();
            });

            mapper = config.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}
