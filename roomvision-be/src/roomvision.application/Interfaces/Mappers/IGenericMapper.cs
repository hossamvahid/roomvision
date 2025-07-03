using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roomvision.application.Interfaces.Mappers
{
    public interface IGenericMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source);
    }
}
