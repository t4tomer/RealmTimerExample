using MongoDB.Bson;
using Realms;
using RealmTodo.Services;

namespace RealmTodo.Models
{
    public partial class Dog : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        public string OwnerId { get; set; }  // Store the user's ID


        [MapTo("name")]
        public string Name { get; set; }

        [MapTo("age")]
        public int Age { get; set; }

        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;

    }
}