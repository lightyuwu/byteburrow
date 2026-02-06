using System.IO;
using ByteBurrow.Core;

namespace ByteBurrow
{
    /// <summary>
    /// Base class for nested or reusable saveable objects within a <see cref="SaveData"/> system.
    /// <br/>
    /// Provides a simple way to serialize an object to a byte array that can be stored inside a parent save file.
    /// </summary>
    /// <remarks>
    /// Use this class for any object that should be saved as part of a larger SaveData object.
    /// The actual serialization is handled by <see cref="SaveCreator"/> using reflection on fields
    /// marked with the <see cref="SaveField"/> attribute.
    /// </remarks>
    public class SaveableClass
    {
        /// <summary>
        /// Serializes this object to a byte array.
        /// <br/>
        /// Only fields marked with <see cref="SaveField"/> will be included.
        /// </summary>
        /// <returns>A byte array containing the serialized save data of this object.</returns>
        public byte[] Save()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // Delegate all reflection-based serialization to SaveCreator
                    SaveCreator.Save(this, writer);

                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }
    }
}