using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject m_playerPrefab = null;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_pos = Vector2.zero;
    Vector2 m_velocity = Vector2.zero;
    float m_rot = 0;
    float m_angularVelocity = 0;

    GameObject m_player = null;

    private void Awake()
    {
        m_subscriberList.Add(new Event<FrameStatusChangedEvent>.Subscriber(OnStatusChange));
        m_subscriberList.Add(new Event<ProcessFrameEvent>.Subscriber(OnFrame));
        m_subscriberList.Subscribe();

        m_pos = transform.position;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnStatusChange(FrameStatusChangedEvent e)
    {
        if(e.status == FrameStatus.Playing && m_player == null)
        {
            m_player = Instantiate(m_playerPrefab);
            m_player.transform.position = m_pos;
            m_player.transform.rotation = Quaternion.Euler(0, 0, m_rot);

            var rigidbody = m_player.GetComponent<Rigidbody2D>();
            rigidbody.velocity = m_velocity;
            rigidbody.angularVelocity = m_angularVelocity;

            var commands = m_player.GetComponent<PlayerCommandsBase>();
            int timelineIndex = 0;
            var manager = TimelineManager.instance;
            if(manager != null)
            {
                timelineIndex = manager.GetTimelineNb();
                if (timelineIndex > 0)
                    timelineIndex--;
            }
            commands.SetTimelineIndex(timelineIndex);
        }
        if((e.status == FrameStatus.FastBackward || e.status == FrameStatus.FastForward) && m_player != null)
        {
            CopyPlayerInfos();
            Destroy(m_player);
            var manager = TimelineManager.instance;
            if (manager != null)
            {
                var timelineNb = manager.GetTimelineNb();
                manager.SetTimeline(timelineNb);
            }
        }
    }

    void OnFrame(ProcessFrameEvent e)
    {
        CopyPlayerInfos();
    }

    void CopyPlayerInfos()
    {
        if (m_player == null)
            return;

        m_pos = m_player.transform.position;
        m_rot = m_player.transform.rotation.eulerAngles.z;

        var rigidbody = m_player.GetComponent<Rigidbody2D>();
        m_velocity = rigidbody.velocity;
        m_angularVelocity = rigidbody.angularVelocity;
    }
}
