using System.Reflection;
using System.Runtime.CompilerServices;

namespace ByteBurrow.Core {
    public static class SaveCreator {

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
                    case SavableClass sc: writer.Write(sc.Save()); break;
                }
            }
            
        }

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
                else if (typeof(SavableClass).IsAssignableFrom(type))
                {
                    // recursively create an instance of the correct type
                    var nested = (SavableClass)Activator.CreateInstance(type, nonPublic: true)!;
                    Load(nested, reader, fileVersion); // recurse
                    field.SetValue(obj, nested);
                }
                else
                {
                    throw new InvalidDataException($"Unsupported field type: {type.FullName}");
                }
            }
        }

    }
}