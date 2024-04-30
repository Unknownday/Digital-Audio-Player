using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player_Loader.Logic.Models
{
    public class ConfigModel
    {
        public int LastSongIndex { get; private set; }
        public int LastPlaylistIndex { get; private set; }
        public int LastVolumeAmount { get; private set; }
        public string DefaultPath { get; private set; }
        public string BackgroundImagePath { get; private set; }
        public string Theme { get; private set; }
        public bool IsAutoSwitching { get; private set; }
        
        public ConfigModel() { }

        public ConfigModel(int lastSongIndex, int lastPlaylistIndex, int lastVolumeAmount, string defaultPath, string backgroundImagePath, string theme, bool isAutoSwitching) 
        { 
            LastPlaylistIndex = lastSongIndex;
            LastSongIndex = lastPlaylistIndex;
            LastVolumeAmount = lastVolumeAmount;
            DefaultPath = defaultPath;
            BackgroundImagePath = backgroundImagePath;
            Theme = theme;
            IsAutoSwitching = isAutoSwitching;
        }
    }
}
