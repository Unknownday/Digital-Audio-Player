using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musical_Player.Models
{
    /// <summary>
    /// Model of the song
    /// </summary>
    public class SongModel
    {
        /// <summary>
        /// Id of the song
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the song
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path to the song
        /// </summary>
        public string Path { get; set; }
    }
}
