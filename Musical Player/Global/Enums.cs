using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musical_Player.Global
{
    /// <summary>
    /// Enumerations used in the project
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Player states
        /// </summary>
        public enum PlayerStates
        {
            /// <summary>
            /// Currently playing
            /// </summary>
            InProgress,
            /// <summary>
            /// Paused
            /// </summary>
            OnPause,
            /// <summary>
            /// Stopped/idle
            /// </summary>
            Idle
        }

        /// <summary>
        /// Direction for seeking/rewinding
        /// </summary>
        public enum MoveDirections
        {
            /// <summary>
            /// Forward or upward direction for seeking
            /// </summary>
            Up,
            /// <summary>
            /// Backward or downward direction for seeking
            /// </summary>
            Down
        }        
    }
}
