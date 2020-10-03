using UnityEngine;
using System.Collections;

public abstract class PlayerCommandsBase : MonoBehaviour
{
    public class GetCommandsData
    {
        public int frame;

        public float moveDir;
        public bool jumpPressed;
        public bool kickPressed;
        public bool actionPressed;

        public GetCommandsData(int _frame)
        {
            frame = _frame;
            moveDir = 0;
            jumpPressed = false;
            kickPressed = false;
            actionPressed = false;
        }
    }

    public abstract void GetCommands(GetCommandsData data);
}
