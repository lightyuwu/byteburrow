using ByteBurrow.Core;

namespace ByteBurrow;

public class SavableClass
{
    public byte[] Save()
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {

                SaveCreator.Save(this, writer);

                writer.Flush();
                return stream.ToArray();
            }
        }
    }
}