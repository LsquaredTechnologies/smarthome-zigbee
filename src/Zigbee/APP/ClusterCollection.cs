﻿using System;
using System.Collections.Generic;

namespace Lsquared.SmartHome.Zigbee.APP
{
    internal sealed class ClusterCollection : IClusterCollection
    {
        public ZCL.Cluster this[ushort clusterID] =>
           _entries.TryGetValue(clusterID, out var cluster)
               ? cluster
               : throw new IndexOutOfRangeException();

        public bool Contains(ushort clusterID) =>
            _entries.ContainsKey(clusterID);

        public TCluster? Get<TCluster>() where TCluster : ZCL.Cluster
        {
            foreach (var entry in _entries.Values)
                if (entry is TCluster cluster)
                    return cluster;
            return null;
        }

        internal void Add(ushort clusterID, ZCL.Cluster cluster) =>
           _entries.Add(clusterID, cluster);

        private readonly Dictionary<ushort, ZCL.Cluster> _entries = new();
    }
}
