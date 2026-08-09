using System.Threading;

using Microsoft.Xna.Framework.Audio;

using NuciXNA.DataAccess.Content;

namespace OpenRS.Audio
{
    public sealed class AudioManager
    {
        private static string SoundsDirectoryPath => "Sounds/";

        private static volatile AudioManager instance;
        private static readonly Lock syncRoot = new();

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

        private AudioManager()
        {
        }

        public void PlaySound(string soundName)
        {
            try
            {
                SoundEffect soundEffect = NuciContentManager.Instance.LoadSoundEffect(
                    $"{SoundsDirectoryPath}{soundName}");

                soundEffect.CreateInstance().Play();
            }
            catch
            {
                // Audio failures are intentionally ignored to avoid disrupting gameplay.
            }
        }
    }
}
