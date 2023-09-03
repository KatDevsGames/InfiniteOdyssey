using System;
using System.Collections.Generic;
using System.Linq;

namespace InfiniteOdyssey.Extensions;

[Serializable]
public class RewindGroup : IRewindable
{
    private IRewindable[] m_items;
    private int[] m_rewind;

    public RewindGroup(params IRewindable[] items)
    {
        m_items = items;
        m_rewind = new int[items.Length];
        Snapshot();
    }

    public int Version => m_checkpoints.Count;
    public void RewindTo(int version)
    {
        CheckpointEntry chk;
        if (version == 0)
        {
            if (m_checkpoints.Count == 0) return;
            chk = m_checkpoints.First.Value;
            m_checkpoints.Clear();
            m_checkpoints.AddLast(chk);
            goto done;
        }
        if (version > m_checkpoints.Count) throw new IRewindable.RewindException($"Invalid version {version}. Max was {m_checkpoints.Count}.");
        while (m_checkpoints.Count > version) m_checkpoints.RemoveLast();

        done:
        chk = m_checkpoints.Last.Value;
        m_rewind = chk.rewind.ToArray();
        for (int i = 0; i < m_items.Length; i++)
            m_items[i].RewindTo(m_rewind[i]);
    }

    public int Snapshot()
    {
        //save old values
        m_checkpoints.AddLast(new CheckpointEntry
        {
            rewind = m_rewind.ToArray()
        });

        //write in new values
        for (int i = 0; i < m_items.Length; i++)
        {
            m_rewind[i] = m_items[i].Version;
        }
        return Version;
    }

    private readonly LinkedList<CheckpointEntry> m_checkpoints = new();

    private struct CheckpointEntry
    {
        public int[] rewind;
    }
}