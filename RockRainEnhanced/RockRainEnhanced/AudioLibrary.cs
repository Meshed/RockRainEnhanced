using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace RockRainEnhanced
{
    public class AudioLibrary
    {
        private SoundEffect explosion;
        private SoundEffect newMeteor;
        private SoundEffect menuBack;
        private SoundEffect menuSelect;
        private SoundEffect menuScroll;
        private SoundEffect powerGet;
        private SoundEffect powerShow;
        private Song backMusic;
        private Song startMusic;

        public SoundEffect Explosion
        {
            get { return explosion; }
        }
        public SoundEffect NewMeteor
        {
            get { return newMeteor; }
        }
        public SoundEffect MenuBack
        {
            get { return menuBack; }
        }
        public SoundEffect MenuSelect
        {
            get { return menuSelect; }
        }
        public SoundEffect MenuScroll
        {
            get { return menuScroll; }
        }
        public SoundEffect PowerGet
        {
            get { return powerGet; }
        }
        public SoundEffect PowerShow
        {
            get { return powerShow; }
        }
        public Song BackMusic
        {
            get { return backMusic; }
        }
        public Song StartMusic
        {
            get { return startMusic; }
        }

        public void LoadContent(ContentManager content)
        {
            explosion = content.Load<SoundEffect>("explosion");
            newMeteor = content.Load<SoundEffect>("newmeteor");
            backMusic = content.Load<Song>("backmusic");
            startMusic = content.Load<Song>("startmusic");
            menuBack = content.Load<SoundEffect>("menu_back");
            menuSelect = content.Load<SoundEffect>("menu_select3");
            menuScroll = content.Load<SoundEffect>("menu_scroll");
            powerShow = content.Load<SoundEffect>("powershow");
            powerGet = content.Load<SoundEffect>("powerget");
        }
    }
}
