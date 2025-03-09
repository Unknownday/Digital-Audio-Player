using System.Collections.Generic;

namespace Musical_Player.Models
{
    public class PlaylistModel
    {
        public string Name { get; set; }
        public List<SongModel> Songs { get; set; }  
    }
}
