namespace Cloudsume.Cassandra.Models
{
    using global::Cassandra;

    public sealed class PhotoProperty : DataProperty<PhotoProperty, Photo?>
    {
        public static readonly UdtMap Mapping = CreateMapping("photoprop");
    }
}
