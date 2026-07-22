using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenRS.Gui.Helpers
{
    public sealed class FramerateCounter
    {
        private static volatile FramerateCounter instance;
        private static readonly Lock syncRoot = new();

        private static float OneSecond => 1.0f;

        private readonly Queue<float> sampleBuffer;

        public static FramerateCounter Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new FramerateCounter();
                    }
                }

                return instance;
            }
        }

        public long TotalFrames { get; private set; }

        public float TotalSeconds { get; private set; }

        public float AverageFramesPerSecond { get; private set; }

        public float CurrentFramesPerSecond { get; private set; }

        public static int MaximumSamples => 100;

        private FramerateCounter()
        {
            sampleBuffer = new Queue<float>();
        }

        public void Update(float deltaTime)
        {
            CurrentFramesPerSecond = OneSecond / deltaTime;

            sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (sampleBuffer.Count > MaximumSamples)
            {
                sampleBuffer.Dequeue();
                AverageFramesPerSecond = sampleBuffer.Average(sample => sample);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames += 1;
            TotalSeconds += deltaTime;
        }
    }
}
