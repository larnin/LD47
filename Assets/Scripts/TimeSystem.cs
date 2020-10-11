using UnityEngine;
using System.Collections;

public enum FrameStatus
{
    Playing,
    Pause,
    FastForward,
    FastBackward,
}

public class TimeSystem : MonoBehaviour
{
    const string fastForwardButton = "FastForward";
    const string fastBackwardButton = "FastBackward";

    [SerializeField] float m_maxTime = 10;

    int m_currentFrame = 0;

    FrameStatus m_currentStatus = FrameStatus.Pause;

    void Start()
    {
        m_currentStatus = FrameStatus.Playing;

        Event<FrameStatusChangedEvent>.Broadcast(new FrameStatusChangedEvent(m_currentStatus, FrameStatus.Pause));
    }

    private void Update()
    {
        if (m_currentStatus == FrameStatus.Pause)
            return;

        bool backwardPressed = Input.GetButton(fastBackwardButton);
        bool forwardPressed = Input.GetButton(fastForwardButton);

        var lastStatus = m_currentStatus;
        if (backwardPressed)
            m_currentStatus = FrameStatus.FastBackward;
        else if (forwardPressed)
            m_currentStatus = FrameStatus.FastForward;
        else m_currentStatus = FrameStatus.Playing;
        if (m_currentStatus != lastStatus)
            Event<FrameStatusChangedEvent>.Broadcast(new FrameStatusChangedEvent(m_currentStatus, lastStatus));
    }

    void FixedUpdate()
    {
        int maxFrame = (int)(m_maxTime / Time.deltaTime);

        if (m_currentFrame < 0)
            m_currentFrame = 0;
        if (m_currentFrame > maxFrame)
            m_currentFrame = maxFrame;

        float time = m_currentFrame * Time.deltaTime;

        Event<ProcessFrameEvent>.Broadcast(new ProcessFrameEvent(m_currentFrame, maxFrame, time, m_maxTime, m_currentStatus));

        switch(m_currentStatus)
        {
            case FrameStatus.Pause:
                Time.timeScale = 0;
                break;
            case FrameStatus.Playing:
                m_currentFrame++;
                if (m_currentFrame > maxFrame)
                    m_currentFrame = maxFrame;
                Time.timeScale = 1;
                break;
            case FrameStatus.FastForward:
                Time.timeScale = 2;
                m_currentFrame++;
                if (m_currentFrame > maxFrame)
                    m_currentFrame = maxFrame;
                break;
            case FrameStatus.FastBackward:
                Time.timeScale = 1;
                m_currentFrame--;
                if (m_currentFrame < 0)
                    m_currentFrame = 0;
                Event<ProcessFrameEvent>.Broadcast(new ProcessFrameEvent(m_currentFrame, maxFrame, time, m_maxTime, m_currentStatus));
                m_currentFrame--;
                if (m_currentFrame < 0)
                    m_currentFrame = 0;
                break;
        }
    }
}
