namespace OpenRS.Net.Client
{
    internal static class FriendListSorter
    {
        internal static void SortByWorldDescending(
            long[] friendHashes,
            int[] friendWorlds,
            int friendCount)
        {
            bool hasSwapped = true;

            while (hasSwapped)
            {
                hasSwapped = false;

                for (int friendIndex = 0;
                    friendIndex < friendCount - 1;
                    friendIndex += 1)
                {
                    if (friendWorlds[friendIndex] <
                        friendWorlds[friendIndex + 1])
                    {
                        int temporaryWorld = friendWorlds[friendIndex];
                        friendWorlds[friendIndex] =
                            friendWorlds[friendIndex + 1];
                        friendWorlds[friendIndex + 1] = temporaryWorld;
                        long temporaryFriend = friendHashes[friendIndex];
                        friendHashes[friendIndex] =
                            friendHashes[friendIndex + 1];
                        friendHashes[friendIndex + 1] = temporaryFriend;
                        hasSwapped = true;
                    }
                }
            }
        }
    }
}