using System.Collections.Generic;
using System.IO;

namespace OpenRS.Net.Client
{
    internal static class SignedByteStreamReader
    {
        internal static sbyte[] ReadRemaining(BinaryReader stream)
        {
            List<sbyte> result = [];

            try
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    result.Add(stream.ReadSByte());
                }
            }
            catch (IOException) { }

            return [.. result];
        }
    }
}