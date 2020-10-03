using UnityEngine;
using System.Collections;

public abstract class PlayerCommandsBase : MonoBehaviour
{
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
    }

    public abstract void GetCommands(GetCommandsData data);
}
