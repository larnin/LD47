using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] GameObject m_clonePrefab = null;

    static TimelineManager m_instance = null;
    public static TimelineManager instance { get { return m_instance; } }

    class FrameData
    {
        public PlayerCommandsBase.GetCommandsData commandData = null;
        public Vector2 pos = Vector2.zero;
        public float rot = 0;
        public Vector2 velocity = Vector2.zero;
        public float angularVelocity = 0;
    }

    class TimelineData
    {
        public int firstFrame = 0;
        public List<FrameData> frames = new List<FrameData>();
    }

    SubscriberList m_subscriberList = new SubscriberList();

    List<TimelineData> m_timelines = new List<TimelineData>();
    Dictionary<int, GameObject> m_instancedClones = new Dictionary<int, GameObject>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<ProcessFrameEvent>.Subscriber(OnFrame));
        m_subscriberList.Subscribe();
        m_instance = this;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnFrame(ProcessFrameEvent e)
    {
        switch(e.status)
        {
            case FrameStatus.FastForward:
            case FrameStatus.Playing:
                OnFrameForward(e);
                break;
            case FrameStatus.FastBackward:
                OnFrameBackward(e);
                break;
            case FrameStatus.Pause:
            default:

                break;
        }
    }

    void OnFrameForward(ProcessFrameEvent e)
    {
        //the last line is for the player
        for (int i = 0; i < m_timelines.Count - 1; i++)
        {
            if (GetTimelineFirstFrame(i) <= e.frame && GetTimelineLastFrame(i) > e.frame)
                SpawnClone(i, e.frame);
        }

        List<int> clonesToRemove = new List<int>();
        foreach (var clone in m_instancedClones)
        {
            if (GetTimelineLastFrame(clone.Key) < e.frame)
                clonesToRemove.Add(clone.Key);
        }
        foreach (var i in clonesToRemove)
            m_instancedClones.Remove(i);
    }

    void OnFrameBackward(ProcessFrameEvent e)
    {
        //the last line is for the player
        for (int i = 0; i < m_timelines.Count - 1; i++)
        {
            if (GetTimelineFirstFrame(i) < e.frame && GetTimelineLastFrame(i) >= e.frame)
                SpawnClone(i, e.frame);
        }

        List<int> clonesToRemove = new List<int>();
        foreach (var clone in m_instancedClones)
        {
            if (GetTimelineFirstFrame(clone.Key) > e.frame)
                clonesToRemove.Add(clone.Key);
        }
        foreach (var i in clonesToRemove)
            m_instancedClones.Remove(i);
    }

    void SpawnClone(int timelineIndex, int frameIndex)
    {
        if (m_instancedClones.ContainsKey(timelineIndex))
            return;

        var frame = GetFrame(timelineIndex, frameIndex);
        if (frame == null)
            return;

        var obj = Instantiate(m_clonePrefab);
        obj.transform.position = frame.pos;
        obj.transform.rotation = Quaternion.Euler(0, 0, frame.rot);

        var rigidbody = obj.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            rigidbody.velocity = frame.velocity;
            rigidbody.angularVelocity = frame.angularVelocity;
        }

        var commands = obj.GetComponent<PlayerCommandsBase>();
        commands.SetTimelineIndex(timelineIndex);

        m_instancedClones.Add(timelineIndex, obj);
    }

    public void SetTimeline(int timeline)
    {
        while (m_timelines.Count <= timeline)
            m_timelines.Add(new TimelineData());
    }

    public void SetFrame(int timeline, int frame, PlayerCommandsBase.GetCommandsData commands, Vector2 pos, float rot, Vector2 velocity, float angularVelocity)
    {
        while (m_timelines.Count <= timeline)
            m_timelines.Add(new TimelineData());

        var timelineData = m_timelines[timeline];

        FrameData f = new FrameData();
        f.commandData = commands;
        f.pos = pos;
        f.rot = rot;
        f.velocity = velocity;
        f.angularVelocity = angularVelocity;

        if (timelineData.frames.Count == 0)
        {
            timelineData.frames.Add(f);
            timelineData.firstFrame = frame;
        }
        else if(frame < timelineData.firstFrame)
        {
            for (int i = frame + 1; i < timelineData.firstFrame; i++)
                timelineData.frames.Insert(0, null);
            timelineData.frames.Insert(0, f);
            timelineData.firstFrame = frame;
        }
        else if(frame >= timelineData.firstFrame + timelineData.frames.Count)
        {
            for (int i = timelineData.firstFrame + timelineData.frames.Count + 1; i < frame - 1; i++)
                timelineData.frames.Add(null);
            timelineData.frames.Add(f);
        }
        else timelineData.frames[frame - timelineData.firstFrame] = f;
    }

    public int GetTimelineNb()
    {
        return m_timelines.Count;
    }

    public int GetTimelineFirstFrame(int timeline)
    {
        if (timeline >= m_timelines.Count)
            return 0;
        return m_timelines[timeline].firstFrame;
    }

    public int GetTimelineLastFrame(int timeline)
    {
        if (timeline >= m_timelines.Count)
            return 0;

        if (m_timelines[timeline].frames.Count == 0)
            return m_timelines[timeline].firstFrame;
        return m_timelines[timeline].firstFrame + m_timelines[timeline].frames.Count - 1;
    }

    public int GetTimelineNbFrame(int timeline)
    {
        if (timeline >= m_timelines.Count)
            return 0;
        return m_timelines[timeline].frames.Count;
    }

    FrameData GetFrame(int timeline, int frame)
    {

        if (timeline >= m_timelines.Count)
            return null;

        if (frame < GetTimelineFirstFrame(timeline) || frame > GetTimelineLastFrame(timeline))
            return null;

        return m_timelines[timeline].frames[frame - m_timelines[timeline].firstFrame];
    }

    public PlayerCommandsBase.GetCommandsData GetCommands(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return null;

        return frameData.commandData;
    }

    public Vector2 GetPos(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return Vector2.zero;

        return frameData.pos;
    }

    public float GetRot(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return 0;

        return frameData.rot;
    }

    public Vector2 GetVelocity(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return Vector2.zero;

        return frameData.velocity;
    }

    public float GetAngularVelocity(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return 0;

        return frameData.angularVelocity;
    }
}
