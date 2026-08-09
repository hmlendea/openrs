using System;
using System.Collections.Generic;

namespace OpenRS.GameLogic.GameManagers
{
    internal sealed class EntityLookup<T>
        where T : class
    {
        private readonly Dictionary<int, T> byV1Id;
        private readonly Dictionary<string, T> byId;

        internal EntityLookup(
            IEnumerable<T> entities,
            Func<T, int> v1IdSelector)
        {
            byV1Id = [];
            byId = [];

            foreach (T entity in entities)
            {
                byV1Id[v1IdSelector(entity)] = entity;
            }
        }

        internal EntityLookup(
            IEnumerable<T> entities,
            Func<T, int> v1IdSelector,
            Func<T, string> idSelector)
        {
            byV1Id = [];
            byId = [];

            foreach (T entity in entities)
            {
                byV1Id[v1IdSelector(entity)] = entity;
                byId[idSelector(entity)] = entity;
            }
        }

        internal int Count => byV1Id.Count;

        internal IEnumerable<T> Values => byV1Id.Values;

        internal T GetByV1Id(int v1Id)
        {
            if (!byV1Id.TryGetValue(v1Id, out T entity))
            {
                return null;
            }

            return entity;
        }

        internal T GetById(string id)
        {
            if (!byId.TryGetValue(id, out T entity))
            {
                return null;
            }

            return entity;
        }
    }
}
