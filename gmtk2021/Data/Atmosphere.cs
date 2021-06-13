using Machina.Engine;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace gmtk2021.Data
{
    public class Atmosphere
    {
        private List<SoundEffectInstance> sounds = new List<SoundEffectInstance>();
        private int index;
        public readonly SoundEffectInstance music;

        public Atmosphere()
        {
            for (int i = 1; i < 4; i++)
            {
                var atmos = MachinaGame.Assets.GetSoundEffectInstance("static_atmos" + i);
                atmos.IsLooped = true;
                this.sounds.Add(atmos);
            }
            this.index = 0;
            PlayNext();

            this.music = MachinaGame.Assets.GetSoundEffectInstance("function_music");
            this.music.IsLooped = true;
            this.music.Volume = 0.6f;
            this.music.Play();
        }

        public void PlayNext()
        {
            this.sounds[this.index % this.sounds.Count].Stop();
            this.index++;
            this.sounds[this.index % this.sounds.Count].Play();
        }
    }
}
