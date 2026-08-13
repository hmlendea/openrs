using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    internal sealed class MobInstanceTestDouble : MobInstance
    {
        public bool IsRemoveInvoked { get; private set; }

        public override void Remove() => IsRemoveInvoked = true;
    }
}