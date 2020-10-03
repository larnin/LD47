using UnityEngine;
using System.Collections;

public class PlayerCommandsHuman : PlayerCommandsBase
{
    const string horizontalButton = "Horizontal";
    const string jumpButton = "Jump";
    const string kickButton = "Kick";
    const string ActionButton = "Action";

    bool m_jumped = false;
    bool m_kicked = false;
    bool m_action = false;
    float m_direction = 0;

    void Update()
    {
        m_direction = Input.GetAxisRaw(horizontalButton);

        if (!m_jumped)
            m_jumped = Input.GetButtonDown(jumpButton);
        if (!m_kicked)
            m_kicked = Input.GetButtonDown(kickButton);
        if (!m_action)
            m_action = Input.GetButtonDown(ActionButton);
    }

    public override void GetCommands(GetCommandsData data)
    {
        data.moveDir = m_direction;
        data.jumpPressed = m_jumped;
        data.kickPressed = m_kicked;
        data.actionPressed = m_action;

        m_jumped = false;
        m_kicked = false;
        m_action = false;
    }
}
