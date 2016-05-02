using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Abp.Extensions
{
    public static class ObjectExtensions
    {
        public static T Dereference<T>(this object obj)
        {
            var formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            object oVal;
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                oVal = formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }

            var result = (T)Convert.ChangeType(oVal, obj.GetType());

            return result;
        }
    }
}
