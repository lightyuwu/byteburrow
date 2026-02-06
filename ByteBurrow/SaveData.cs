using System.IO;
using ByteBurrow.Core;

namespace ByteBurrow
{
    public abstract class SaveData
    {
        // Each subclass provides its own prefix
        protected abstract string Prefix { get; }
        protected abstract string Version { get; }

        public byte[] Save()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            // Write prefix first
            writer.Write(Prefix);
            writer.Write(Version);

            SaveCreator.Save(this, writer);
            writer.Flush();
            return ms.ToArray();
        }

        public void Load(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            // Skip the prefix & get the version
            reader.ReadString();
            
            var version = reader.ReadString();

            SaveCreator.Load(this, reader, new SaveableVersion(version));
        }

        public void SaveToFile(string filename)
        {
            var bytes = Save();
            File.WriteAllBytes(filename, bytes);
        }

        public void LoadFromFile(string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            Load(bytes);
        }
    }
}