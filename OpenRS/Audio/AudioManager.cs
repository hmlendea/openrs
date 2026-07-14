using Microsoft.Xna.Framework.Audio;

using NuciXNA.DataAccess.Content;

namespace OpenRS.Audio
{
    public sealed class AudioManager
    {
        private static volatile AudioManager instance;
        private static readonly object syncRoot = new();

        public static AudioManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new AudioManager();
                    }
                }

                return instance;
            }
        }

        public void PlaySound(string sound)
        {
            try
            {
                SoundEffect soundEffect = NuciContentManager.Instance.LoadSoundEffect("Sounds/" + sound);
                soundEffect.CreateInstance().Play();
            }
            catch
            {
            }
        }
    }
}
