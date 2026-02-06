
namespace ByteBurrow
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SaveField : Attribute
    {
        public int Id { get; }
        public string FromVersion { get; }
        public string ToVersion { get; }
        public SaveField(int id, string fromVersion, string toVersion) 
        {
            Id = id;
            FromVersion = fromVersion;
            ToVersion = toVersion;
        }
    }
}
