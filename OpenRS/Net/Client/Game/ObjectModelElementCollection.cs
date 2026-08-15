using System;
using System.Collections.Generic;

namespace OpenRS.Net.Client.Game
{
    internal sealed class ObjectModelElementCollection<TItem>
    {
        private readonly List<TItem> items;

        internal IEnumerable<TItem> Items => items;

        internal ObjectModelElementCollection(
            IEnumerable<TItem> sourceItems,
            string parameterName)
        {
            if (sourceItems is null)
            {
                throw new ArgumentNullException(
                    parameterName,
                    $"The {parameterName} collection cannot be null.");
            }

            items = [.. sourceItems];
        }

        internal void Add(TItem item) => items.Add(item);

        internal TItem Get(int index) => items[index];

        internal TItem Remove(int index)
        {
            TItem item = items[index];
            items.Remove(item);

            return item;
        }
    }
}