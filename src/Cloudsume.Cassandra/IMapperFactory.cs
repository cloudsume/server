namespace Cloudsume.Cassandra;

using global::Cassandra.Mapping;

public interface IMapperFactory
{
    IMapper CreateMapper();
}
