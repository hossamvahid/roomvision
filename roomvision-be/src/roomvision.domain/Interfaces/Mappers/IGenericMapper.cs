using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.domain.Interfaces.Mappers
{
    public interface IGenericMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source);
    }
}