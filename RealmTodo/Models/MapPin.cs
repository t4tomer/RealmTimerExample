using MongoDB.Bson;
using Realms;
using RealmTodo.Services;
///////
namespace RealmTodo.Models
{
    public partial class MapPin : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }



        //new code 
        [MapTo("mapname")]
        [Required]
        public string Mapname { get; set; }

        [MapTo("labelpin")]
        [Required]

        public string Labelpin { get; set; }

        [MapTo("address")]
        [Required]

        public string Address { get; set; }

        [MapTo("latitude")]
        [Required]

        public string Latitude { get; set; }

        [MapTo("longitude")]
        [Required]

        public string Longitude { get; set; }



        //new code 
        [MapTo("isComplete")]
        public bool IsComplete { get; set; }

        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;
    }
}

