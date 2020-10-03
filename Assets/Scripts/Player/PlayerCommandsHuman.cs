using UnityEngine;
using System.Collections;

public class PlayerCommandsHuman : PlayerCommandsBase
{
    const string horizontalButton = "Horizontal";
    const string jumpButton = "Jump";
    const string kickButton = "Kick";

    bool m_jumped = false;
    bool m_kicked = false;
    float m_direction = 0;

    void Update()
    {
        m_direction = Input.GetAxisRaw(horizontalButton);

        if (!m_jumped)
            m_jumped = Input.GetButtonDown(jumpButton);
        if (!m_kicked)
            m_kicked = Input.GetButtonDown(kickButton);
    }

    public override void GetCommands(GetCommandsData data)
    {
        data.moveDir = m_direction;
        data.jumpPressed = m_jumped;
        data.kickPressed = m_kicked;

        m_jumped = false;
        m_kicked = false;
    }
}
