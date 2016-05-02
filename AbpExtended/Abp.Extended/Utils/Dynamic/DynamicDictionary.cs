using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;

namespace Abp.Utils.Dynamic
{
    public class DynamicDictionary : DynamicObject, ISerializable
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            var name = binder.Name;
            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return _dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            _dictionary[binder.Name] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kvp in _dictionary)
            {
                info.AddValue(kvp.Key, kvp.Value);
            }
        }
    }
}
