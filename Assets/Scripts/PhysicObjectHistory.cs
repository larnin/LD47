using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class PhysicObjectHistory : MonoBehaviour
{
    enum AnimDataType
    {
        Float,
        Int,
        Bool,
    }
    [StructLayout(LayoutKind.Explicit)]
    struct AnimData
    {
        [FieldOffset(0)] public float floatValue;
        [FieldOffset(0)] public int intValue;
        [FieldOffset(0)] public bool boolValue;
    }

    class OneAnimData
    {
        public AnimDataType type;
        public AnimData oldValue;
        public AnimData newValue;
        public string name;

        public OneAnimData(string _name, float _oldValue, float _newValue)
        {
            name = _name;
            type = AnimDataType.Float;
            oldValue.floatValue = _oldValue;
            newValue.floatValue = _newValue;
        }

        public OneAnimData(string _name, int _oldValue, int _newValue)
        {
            name = _name;
            type = AnimDataType.Int;
            oldValue.intValue = _oldValue;
            newValue.intValue = _newValue;
        }

        public OneAnimData(string _name, bool _oldValue, bool _newValue)
        {
            name = _name;
            type = AnimDataType.Bool;
            oldValue.boolValue = _oldValue;
            newValue.boolValue = _newValue;
        }
    }

    class HistoryFrame
    {
        public List<OneAnimData> data = new List<OneAnimData>();

        public Vector2 pos = Vector2.zero;
        public float rot = 0;
        public Vector2 velocity = Vector2.zero;
        public float angularVelocity = 0;
    }

    List<HistoryFrame> m_history = new List<HistoryFrame>();
    int m_startFrame = 0;

    SubscriberList m_subscriberList = new SubscriberList();

    Rigidbody2D m_rigidbody = null;
    Animator m_animator = null;

    HistoryFrame m_currentFrame = new HistoryFrame();

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_subscriberList.Add(new Event<ProcessFrameEvent>.Subscriber(OnFrame));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnFrame(ProcessFrameEvent e)
    {
        switch(e.status)
        {
            case FrameStatus.Playing:
            case FrameStatus.FastForward:
                OnForwardFrame(e);
                break;
            case FrameStatus.FastBackward:
                OnBackwardFrame(e);
                break;
            default:
                break;
        }
    }

    void OnForwardFrame(ProcessFrameEvent e)
    {
        m_currentFrame.pos = transform.position;
        m_currentFrame.rot = transform.rotation.eulerAngles.z;

    }

    void OnBackwardFrame(ProcessFrameEvent e)
    {

    }

    void SetFloat(string name, float value)
    {
        if (m_animator == null)
            return;

        float oldValue = m_animator.GetFloat(name);

        m_currentFrame.data.Add(new OneAnimData(name, oldValue, value));

        m_animator.SetFloat(name, value);
    }

    void SetInt(string name, int value)
    {
        if (m_animator == null)
            return;

        int oldValue = m_animator.GetInteger(name);

        m_currentFrame.data.Add(new OneAnimData(name, oldValue, value));

        m_animator.SetInteger(name, value);
    }

    void SetBool(string name, bool value)
    {
        if (m_animator == null)
            return;

        bool oldValue = m_animator.GetBool(name);

        m_currentFrame.data.Add(new OneAnimData(name, oldValue, value));

        m_animator.SetBool(name, value);
    }
}
