using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealmTodo.Models
{
    public class ObjectSingleton
    {
        // A private static variable to hold the single instance of the class.
        private static ObjectSingleton _instance;

        // Store the type-specific instance, which can be either a Dog or an Item
        private object _currentObject = new Item();
        // Lock object to ensure thread safety.
        private static readonly object _lock = new object();

        // Private constructor to prevent instantiation from outside.
        private ObjectSingleton()
        {
        }




        // Public static property to provide access to the single instance.
        public static ObjectSingleton Instance
        {
            get
            {
                // Double-checked locking to ensure thread safety and performance.
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ObjectSingleton();
                        }
                    }
                }
                return _instance;
            }
        }



        // Set the type-specific instance based on newObject's type
        public void SetObjectType(object newObject)
        {
            if (newObject is Dog)
            {
                _currentObject = new Dog();
                Console.WriteLine("ObjectSingleton is now of Dog type.");
            }
            else if (newObject is Item)
            {
                _currentObject = new Item();
                Console.WriteLine("ObjectSingleton is now of Item type.");
            }
            else
            {
                throw new ArgumentException("Unsupported object type.");
            }
        }

        // Method to retrieve the current object instance
        public object GetCurrentObjectType()
        {
            return _currentObject;
        }



        public Dog getDogObject()
        {
            return new Dog();
        }
        public Item getItemObject()
        {
            return new Item();
        }

    }

}
