using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineManager : MonoBehaviour
{
    static TimelineManager m_instance = null;
    public static TimelineManager instance { get { return m_instance; } }

    class FrameData
    {
        public PlayerCommandsBase.GetCommandsData commandData = null;
        public Vector2 pos = Vector2.zero;
        public float rot = 0;
        public Vector2 velocity = Vector2.zero;
    }

    class TimelineData
    {
        public int firstFrame = 0;
        public List<FrameData> frames = new List<FrameData>();
    }

    SubscriberList m_subscriberList = new SubscriberList();

    List<TimelineData> m_timelines = new List<TimelineData>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<ProcessFrameEvent>.Subscriber(OnFrame));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnFrame(ProcessFrameEvent e)
    {

    }

    void SetFrame(int timeline, int frame, PlayerCommandsBase.GetCommandsData commands, Vector2 pos, float rot, Vector2 velocity)
    {
        while (m_timelines.Count <= timeline)
            m_timelines.Add(new TimelineData());

        var timelineData = m_timelines[timeline];

        FrameData f = new FrameData();
        f.commandData = commands;
        f.pos = pos;
        f.rot = rot;
        f.velocity = velocity;

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

    int GetTimelineNb()
    {
        return m_timelines.Count;
    }

    int GetTimelineFirstFrame(int timeline)
    {
        if (timeline >= m_timelines.Count)
            return 0;
        return m_timelines[timeline].firstFrame;
    }

    int GetTimelineLastFrame(int timeline)
    {
        if (timeline >= m_timelines.Count)
            return 0;

        if (m_timelines[timeline].frames.Count == 0)
            return m_timelines[timeline].firstFrame;
        return m_timelines[timeline].firstFrame + m_timelines[timeline].frames.Count - 1;
    }

    FrameData GetFrame(int timeline, int frame)
    {

        if (timeline >= m_timelines.Count)
            return null;

        if (frame < GetTimelineFirstFrame(timeline) || frame > GetTimelineLastFrame(timeline))
            return null;

        return m_timelines[timeline].frames[frame - m_timelines[timeline].firstFrame];
    }

    PlayerCommandsBase.GetCommandsData GetCommands(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return null;

        return frameData.commandData;
    }

    Vector2 GetPos(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return Vector2.zero;

        return frameData.pos;
    }

    float GetRot(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return 0;

        return frameData.rot;
    }

    Vector2 GetVelocity(int timeline, int frame)
    {
        var frameData = GetFrame(timeline, frame);
        if (frameData == null)
            return Vector2.zero;

        return frameData.velocity;
    }
}
