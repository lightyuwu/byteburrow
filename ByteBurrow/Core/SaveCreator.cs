using System.Reflection;
#if UNITY
using UnityEngine;
#else
using System.Numerics;
#endif

namespace ByteBurrow.Core {
    /// <summary>
    /// The Internal Class to handle Saving and Loading of Data.
    /// <br/>
    /// Usually you'd want to extend your own class from the <see cref="SaveData"/> class.
    /// </summary>
    public static class SaveCreator {

        /// <summary>
        /// <b>ByteBurrow INTERNAL</b>
        /// Storage of a C# Field and the <see cref="SaveField"/> reference
        /// </summary>
        private class SortedField
        {
            internal readonly FieldInfo Field;
            internal readonly SaveField? Attr;

            internal SortedField(FieldInfo field, SaveField? sf)
            {
                Field = field;
                Attr = sf;
            }
        }
        
        /// <summary>
        /// <b>ByteBurrow INTERNAL</b>
        /// Get all SaveFields of an object inside an IOrderedEnumerable
        /// </summary>
        /// <param name="obj">The object to search</param>
        /// <returns>Sorted Fields, Ordered.</returns>
        private static IOrderedEnumerable<SortedField> _GetSortedFields(object obj)
        {
            var objType = obj.GetType();

            var fields = objType.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            // Sort out fields that don't have the attribute and order by ID
            var sortedFields = fields
                .Select(field => new SortedField(field, field.GetCustomAttribute<SaveField>()) )
                .Where(x => x.Attr != null) // Never Null
                .OrderBy(x => x.Attr!.Id);

            return sortedFields;
        }
        
        /// <summary>
        /// Save a C# Object with the BinaryWriter being utilized.
        /// </summary>
        /// <param name="obj">The Object to get the SaveFields from</param>
        /// <param name="writer">The BinaryWriter where the results will be written to</param>
        /// <exception cref="InvalidDataException">Occurs when a fields class is not a SaveableClass</exception>
        public static void Save(object obj, BinaryWriter writer) {
            var sortedFields = _GetSortedFields(obj);
            
            foreach(var entry in sortedFields)
            {
                var field = entry.Field;
                var id = entry.Attr!.Id;

                var val = field.GetValue(obj); // Get the value of the field in our instance
                
                switch (val)
                {
                    case int i: writer.Write(i); break;
                    case uint ui: writer.Write(ui); break;
                    case float f: writer.Write(f); break;
                    case long l: writer.Write(l); break;
                    case ulong ul: writer.Write(ul); break;
                    case short s: writer.Write(s); break;
                    case ushort us: writer.Write(us); break;
                    case bool b: writer.Write(b); break;
                    case byte by: writer.Write(by); break;
                    case char c: writer.Write(c); break;
                    case char[] ca: 
                        writer.Write(ca.Length);
                        writer.Write(ca);
                        break;
                    case double d: writer.Write(d); break;
                    case sbyte sb: writer.Write(sb); break;
                    case byte[] ba:
                        writer.Write(ba.Length);
                        writer.Write(ba);
                        break;
                    case string str:
                        writer.Write(str); // BinaryWriter already does length prefixing
                        break;
                    case SaveableClass sc: writer.Write(sc.Save()); break;
                    #if UNITY
                    case Quaternion qa:
                        writer.Write(qa.x);
                        writer.Write(qa.y);
                        writer.Write(qa.z);
                        writer.Write(qa.w);
                        break;

                    case Vector3 v3:
                        writer.Write(v3.x);
                        writer.Write(v3.y);
                        writer.Write(v3.z);
                        break;
                    
                    case Vector2 v2:
                        writer.Write(v2.x);
                        writer.Write(v2.y);
                        break;
                    #else
                    case Quaternion qa:
                        writer.Write(qa.X);
                        writer.Write(qa.Y);
                        writer.Write(qa.Z);
                        writer.Write(qa.W);
                        break;
                    
                    case Vector4 v4:
                        writer.Write(v4.X);
                        writer.Write(v4.Y);
                        writer.Write(v4.Z);
                        writer.Write(v4.W);
                        break;
                    
                    case Vector3 v3:
                        writer.Write(v3.X);
                        writer.Write(v3.Y);
                        writer.Write(v3.Z);
                        break;
                    
                    case Vector2 v2:
                        writer.Write(v2.X);
                        writer.Write(v2.Y);
                        break;
                    #endif
                    
                    default:
                        throw new InvalidDataException($"Unsupported field type: {field.FieldType.FullName}");
                        break;
                }
            }
            
        }

        /// <summary>
        /// Load data from a BinaryReader into a C# Object
        /// </summary>
        /// <param name="obj">The Object to write into</param>
        /// <param name="reader">The BinaryReader to read from</param>
        /// <param name="fileVersion">The version of the save data</param>
        /// <exception cref="InvalidDataException">Occurs when a fields class is not a SaveableClass</exception>
        public static void Load(object obj, BinaryReader reader, SaveableVersion fileVersion)
        {
            var sortedFields = _GetSortedFields(obj);

            foreach (var entry in sortedFields)
            {
                var field = entry.Field;
                var type = field.FieldType;

                // Check if fileVersion is inside the valid versions of the field
                // if not, we skip that field.

                var fromVersion = new SaveableVersion(entry.Attr!.FromVersion);
                var toVersion = new SaveableVersion(entry.Attr.ToVersion);
                
                if(fromVersion < fileVersion || toVersion > fileVersion) continue;
                
                // oof... sadly these ifs are needed...
                if (type == typeof(int))
                    field.SetValue(obj, reader.ReadInt32());
                else if (type == typeof(uint))
                    field.SetValue(obj, reader.ReadUInt32());
                else if (type == typeof(float))
                    field.SetValue(obj, reader.ReadSingle());
                else if (type == typeof(long))
                    field.SetValue(obj, reader.ReadInt64());
                else if (type == typeof(ulong))
                    field.SetValue(obj, reader.ReadUInt64());
                else if (type == typeof(short))
                    field.SetValue(obj, reader.ReadInt16());
                else if (type == typeof(ushort))
                    field.SetValue(obj, reader.ReadUInt16());
                else if (type == typeof(bool))
                    field.SetValue(obj, reader.ReadBoolean());
                else if (type == typeof(byte))
                    field.SetValue(obj, reader.ReadByte());
                else if (type == typeof(char))
                    field.SetValue(obj, reader.ReadChar());
                else if (type == typeof(char[]))
                {
                    var calen = reader.ReadInt32();
                    var cabytes = reader.ReadBytes(calen * 2); // 2 bytes per char
                    var chars = new char[calen];
                    Buffer.BlockCopy(cabytes, 0, chars, 0, cabytes.Length);
                    field.SetValue(obj, chars);
                }
                else if (type == typeof(double))
                    field.SetValue(obj, reader.ReadDouble());
                else if (type == typeof(sbyte))
                    field.SetValue(obj, reader.ReadSByte());
                else if (type == typeof(byte[]))
                {
                    var len = reader.ReadInt32();
                    var bytes = reader.ReadBytes(len);
                    field.SetValue(obj, bytes);
                }
                else if (type == typeof(string))
                {
                    field.SetValue(obj, reader.ReadString());
                }
                else if (typeof(SaveableClass).IsAssignableFrom(type))
                {
                    // recursively create an instance of the correct type
                    var nested = (SaveableClass)Activator.CreateInstance(type, nonPublic: true)!;
                    Load(nested, reader, fileVersion); // recurse
                    field.SetValue(obj, nested);
                }
                #if UNITY
                else if (type == typeof(Quaternion))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    var w = reader.ReadSingle();
                    field.SetValue(obj, new Quaternion(x, y, z, w));
                }
                else if (type == typeof(Vector3))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    field.SetValue(obj, new Vector3(x, y, z));
                }
                else if (type == typeof(Vector2))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    field.SetValue(obj, new Vector2(x, y));
                }
                #else
                else if (type == typeof(Quaternion))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    var w = reader.ReadSingle();
                    field.SetValue(obj, new Quaternion(x, y, z, w));
                }
                else if (type == typeof(Vector4))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    var w = reader.ReadSingle();
                    field.SetValue(obj, new Vector4(x, y, z, w));
                }
                else if (type == typeof(Vector3))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    field.SetValue(obj, new Vector3(x, y, z));
                }
                else if (type == typeof(Vector2))
                {
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    field.SetValue(obj, new Vector2(x, y));
                }
                #endif
                else
                {
                    throw new InvalidDataException($"Unsupported field type: {type.FullName}");
                }
            }
        }

    }
}