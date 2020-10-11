using UnityEngine;
using System.Collections;

public abstract class PlayerCommandsBase : MonoBehaviour
{
    protected int m_timelineIndex = 0;

    public class GetCommandsData
    {
        public int frame;
        public FrameStatus status;

        public float moveDir;
        public bool jumpPressed;
        public bool kickPressed;
        public bool actionPressed;

        public GetCommandsData(int _frame, FrameStatus _status)
        {
            frame = _frame;
            status = _status;
            moveDir = 0;
            jumpPressed = false;
            kickPressed = false;
            actionPressed = false;
        }

        public void Reset()
        {
            moveDir = 0;
            jumpPressed = false;
            kickPressed = false;
            actionPressed = false;
        }

        public void Set(GetCommandsData data)
        {
            moveDir = data.moveDir;
            jumpPressed = data.jumpPressed;
            kickPressed = data.kickPressed;
            actionPressed = data.actionPressed;
        }
    }

    public abstract void GetCommands(GetCommandsData data);

    public void SetTimelineIndex(int index)
    {
        m_timelineIndex = index;
    }
}
