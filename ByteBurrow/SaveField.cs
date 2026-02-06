
namespace ByteBurrow
{
    /// <summary>
    /// Marks a field or property to be included in a SaveData save file.
    /// </summary>
    /// <remarks>
    /// Only fields/properties with this attribute will be serialized by <see cref="ByteBurrow.Core.SaveCreator"/>.
    /// You can optionally specify the version range this field is valid for.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SaveField : Attribute
    {
        /// <summary>
        /// The unique ID of this field in the save data. Determines the serialization order.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The minimum version where this field is valid. SaveCreator will skip this field for older files.
        /// </summary>
        public string FromVersion { get; }

        /// <summary>
        /// The maximum version where this field is valid. SaveCreator will skip this field for newer files.
        /// </summary>
        public string ToVersion { get; }

        /// <summary>
        /// Create a SaveField attribute.
        /// </summary>
        /// <param name="id">Unique ID of the field, used to order fields in the save data.</param>
        /// <param name="fromVersion">Minimum version this field is valid for (inclusive).</param>
        /// <param name="toVersion">Maximum version this field is valid for (inclusive).</param>
        public SaveField(int id, string fromVersion = "0.0.0", string toVersion = "999.999.999")
        {
            Id = id;
            FromVersion = fromVersion;
            ToVersion = toVersion;
        }
    }
}
