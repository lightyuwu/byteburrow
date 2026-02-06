using System.IO;
using ByteBurrow.Core;

namespace ByteBurrow
{
    /// <summary>
    /// The Main class to use when trying to write Save Data.
    /// Please Create your own Class that extends from this one.
    /// Example:
    /// <code>
    /// public class TestSaveable : SaveData
    /// {
    /// [SaveField(0, "1.0.0", "1.0.0")] public string Username = "";
    /// [SaveField(1)] public int Coins = 0;
    /// [SaveField(2, "0.9.0")] public int NeverSaving = 0;
    /// 
    /// protected override string Prefix => "TEST\0SV";
    /// protected override string Version => "1.0.0";
    /// }
    /// </code>
    /// </summary>
    public abstract class SaveData
    {
        // Each subclass provides its own prefix
        /// <summary>
        /// The Save Data file Prefix to ensure the files match
        /// </summary>
        protected abstract string Prefix { get; }
        
        /// <summary>
        /// The Current Version of the Save Data
        /// <br/>
        /// <i>(<b>UPDATE THIS EVERY TIME YOU MAKE A CHANGE</b>)</i>
        /// </summary>
        protected abstract string Version { get; }

        /// <summary>
        /// Save the Current Save Data
        /// </summary>
        /// <returns>A Byte Array containing the entire Save Data</returns>
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

        /// <summary>
        /// Load the Save Data provided into this C# Object
        /// </summary>
        /// <param name="data">The byte array containing the save data</param>
        /// <exception cref="InvalidDataException">Ocurrs, when the Provided Save Data's prefix does not match the prefix of this class</exception>
        public void Load(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);

            // Skip the prefix & get the version
            if (reader.ReadString() != Prefix)
            {
                throw new InvalidDataException("Save Data Provided does not match Save Data Class!");
            }
            var version = reader.ReadString();

            SaveCreator.Load(this, reader, new SaveableVersion(version));
        }

        /// <summary>
        /// Save the C# Object to a file
        /// </summary>
        /// <param name="filename">File to write into</param>
        public void SaveToFile(string filename)
        {
            var bytes = Save();
            File.WriteAllBytes(filename, bytes);
        }

        /// <summary>
        /// Load the C# Object from a File
        /// </summary>
        /// <param name="filename">File to read from</param>
        public void LoadFromFile(string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            Load(bytes);
        }
    }
}