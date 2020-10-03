using UnityEngine;
using System.Collections;

public class TimeUI : MonoBehaviour
{
    GameObject m_pauseIcon;
    GameObject m_playIcon;
    GameObject m_fastForwardIcon;
    GameObject m_fastBackwardIcon;
    Transform m_timerFransform;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<ProcessFrameEvent>.Subscriber(OnFrame));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        m_pauseIcon = transform.Find("IconPause").gameObject;
        m_playIcon = transform.Find("IconPlay").gameObject;
        m_fastForwardIcon = transform.Find("IconFastForward").gameObject;
        m_fastBackwardIcon = transform.Find("IconFastBackward").gameObject;
        m_timerFransform = transform.Find("TimerBack").Find("Timer");
    }

    void OnFrame(ProcessFrameEvent e)
    {
        switch(e.status)
        {
            case FrameStatus.Pause:
                m_pauseIcon.SetActive(true);
                m_playIcon.SetActive(false);
                m_fastForwardIcon.SetActive(false);
                m_fastBackwardIcon.SetActive(false);
                break;
            case FrameStatus.Playing:
                m_pauseIcon.SetActive(false);
                m_playIcon.SetActive(true);
                m_fastForwardIcon.SetActive(false);
                m_fastBackwardIcon.SetActive(false);
                break;
            case FrameStatus.FastForward:
                m_pauseIcon.SetActive(false);
                m_playIcon.SetActive(false);
                m_fastForwardIcon.SetActive(true);
                m_fastBackwardIcon.SetActive(false);
                break;
            case FrameStatus.FastBackward:
                m_pauseIcon.SetActive(false);
                m_playIcon.SetActive(false);
                m_fastForwardIcon.SetActive(false);
                m_fastBackwardIcon.SetActive(true);
                break;
        }

        var scale = m_timerFransform.localScale;
        scale.x = e.time / e.maxTime;
        m_timerFransform.localScale = scale;
    }
}
