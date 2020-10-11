using UnityEngine;
using System.Collections;

public class PlayerCommandsClone : PlayerCommandsBase
{
    Rigidbody2D m_rigidbody = null;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();    
    }

    public override void GetCommands(GetCommandsData data)
    {
        data.Reset();

        switch(data.status)
        {
            case FrameStatus.Playing:
            case FrameStatus.FastForward:
                ForwardCommands(data);
                break;
            case FrameStatus.FastBackward:
                BackwardCommands(data);
                break;
            default:
                break;
        }
    }

    void ForwardCommands(GetCommandsData data)
    {
        var manager = TimelineManager.instance;
        if (manager == null)
            return;

        int lastFrame = manager.GetTimelineLastFrame(m_timelineIndex);
        if(lastFrame < data.frame)
        {
            Destroy(gameObject);
            return;
        }

        var commands = manager.GetCommands(m_timelineIndex, data.frame);
        if (commands == null)
            return;

        data.Set(commands);

        manager.SetFrame(m_timelineIndex, data.frame, data, transform.position, transform.rotation.eulerAngles.z, m_rigidbody.velocity, m_rigidbody.angularVelocity);
    }

    void BackwardCommands(GetCommandsData data)
    {
        var manager = TimelineManager.instance;
        if (manager == null)
            return;

        int firstFrame = manager.GetTimelineFirstFrame(m_timelineIndex);
        if (firstFrame > data.frame)
        {
            Destroy(gameObject);
            return;
        }

        m_rigidbody.velocity = manager.GetVelocity(m_timelineIndex, data.frame);
        m_rigidbody.angularVelocity = manager.GetAngularVelocity(m_timelineIndex, data.frame);
        transform.position = manager.GetPos(m_timelineIndex, data.frame);
        transform.rotation = Quaternion.Euler(0, 0, manager.GetRot(m_timelineIndex, data.frame));
    }
}
