using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace RealmTodo.Models
{
    public sealed class ObjectSingleton
    {
        private static readonly Lazy<ObjectSingleton> _instance = new(() => new ObjectSingleton());

        private object _currentType;

        // Private constructor to prevent instantiation
        private ObjectSingleton()
        {
            // Default type is MapPin
            _currentType = new MapPin();
            Console.WriteLine("Default object type is MapPin.");
        }

        // Public property to get the singleton instance
        public static ObjectSingleton Instance => _instance.Value;

        // Method to set type to MapPin
        public void SetMapPinType()
        {
            _currentType = new MapPin();
            Console.WriteLine("Object type set to MapPin.");
        }

        // Method to set type to Item
        public void SetItemType()
        {
            _currentType = new Item();
            Console.WriteLine("Object type set to Item.");
        }


        // Method to set type to Item
        public void SetDogType()
        {
            _currentType = new Dog();
            Console.WriteLine("Object type set to Dog.");
        }



        // Method to get the current type
        public Type GetCurrentType()
        {
            return _currentType.GetType();
        }
    }


}

