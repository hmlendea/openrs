using NuciLog.Core;

namespace OpenRS.Settings
{
    public sealed class GameLogInfoKey(string name) : LogInfoKey(name)
    {
        public static LogInfoKey AnimationFrameCount => new GameLogInfoKey(nameof(AnimationFrameCount));

        public static LogInfoKey CoordinateX => new GameLogInfoKey(nameof(CoordinateX));

        public static LogInfoKey FileName => new GameLogInfoKey(nameof(FileName));

        public static LogInfoKey FilePath => new GameLogInfoKey(nameof(FilePath));

        public static LogInfoKey LoadProgress => new GameLogInfoKey(nameof(LoadProgress));

        public static LogInfoKey LoginResponseCode => new GameLogInfoKey(nameof(LoginResponseCode));

        public static LogInfoKey ObjectCount => new GameLogInfoKey(nameof(ObjectCount));

        public static LogInfoKey ObjectIndex => new GameLogInfoKey(nameof(ObjectIndex));

        public static LogInfoKey PacketId => new GameLogInfoKey(nameof(PacketId));

        public static LogInfoKey PacketLength => new GameLogInfoKey(nameof(PacketLength));

        public static LogInfoKey ScrollAmount => new GameLogInfoKey(nameof(ScrollAmount));

        public static LogInfoKey ScrollBegin => new GameLogInfoKey(nameof(ScrollBegin));

        public static LogInfoKey SessionId => new GameLogInfoKey(nameof(SessionId));
    }
}
