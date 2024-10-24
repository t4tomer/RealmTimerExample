using MongoDB.Bson;
using Realms;
using RealmTodo.Services;

namespace RealmTodo.Models
{
    public partial class UserRecord : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }

        [MapTo("summary")]
        [Required]
        public string Summary { get; set; }

        [MapTo("mapName")]
        public string MapName { get; set; }

        [MapTo("trackTime")]
        public string TrackTime { get; set; }


        [MapTo("uploadDate")]
        public string UploadDate { get; set; }

        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;
    }
}
