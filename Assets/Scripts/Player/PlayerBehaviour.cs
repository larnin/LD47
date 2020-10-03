using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_maxSpeed = 5;
    [SerializeField] float m_acceleration = 20;
    [SerializeField] float m_aerialAcceleration = 3;
    [SerializeField] float m_jumpPower = 5;
    [SerializeField] float m_jumpMaxDelay = 0.2f;
    [SerializeField] float m_kickPower = 10;
    [SerializeField] float m_kickAngle = 20;
    [SerializeField] float m_kickDelay = 0.2f;
    [SerializeField] Vector2 m_kickOffset = new Vector2(0.2f, 0.1f);
    [SerializeField] LayerMask m_kickMask;
    [SerializeField] float m_groundCheckDistance = 1;
    [SerializeField] float m_groundCheckRadius = 1;
    [SerializeField] LayerMask m_groundMask;
    [SerializeField] float m_actionRadius = 1;
    [SerializeField] LayerMask m_actionMask;

    PlayerCommandsBase m_playerCommands = null;

    Rigidbody2D m_rigidbody = null;

    bool m_grounded = true;
    float m_jumpDelay = 0;

    bool m_faceRight = true;

    void Start()
    {
        m_playerCommands = GetComponent<PlayerCommandsBase>();
        if (m_playerCommands == null)
            Debug.LogError("PlayerBehaviour - Need a player commands component");

        m_rigidbody = GetComponent<Rigidbody2D>();
        if (m_rigidbody == null)
            Debug.Log("PlayerBehaviour - Need a rigidbody 2D");
    }
    
    void FixedUpdate()
    {
        PlayerCommandsBase.GetCommandsData data = new PlayerCommandsBase.GetCommandsData(0);

        CheckGrounded();

        ProcessDirection(data.moveDir);

        ProcessJump(data.jumpPressed);

        if(data.kickPressed)
            Kick();

        if (data.actionPressed)
            Action();
    }

    void CheckGrounded()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, -m_groundCheckDistance, 0), m_groundCheckDistance, m_groundMask);

        if(colliders.Length == 0)
        {
            m_grounded = false;
            transform.SetParent(null);
            return;
        }
        
        //don't change the ground if already grounded
        if (m_grounded)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform == transform.parent)
                    return;
            }
        }

        m_grounded = true;
        transform.parent = colliders[0].transform;
    }

    void ProcessDirection(float movePower)
    {
        var velocity = m_rigidbody.velocity;

        float maxSpeed = m_maxSpeed;
        float acceleration = m_grounded ? m_acceleration : m_aerialAcceleration;

        float targetSpeed = movePower * maxSpeed;

        if(targetSpeed < velocity.x)
        {
            velocity.x -= Time.deltaTime * acceleration;
            if (targetSpeed > velocity.x)
                velocity.x = targetSpeed;
        }
        if(targetSpeed > velocity.x)
        {
            velocity.x += Time.deltaTime * acceleration;
            if (targetSpeed < velocity.x)
                velocity.x = targetSpeed;
        }

        m_rigidbody.velocity = velocity;

        if(Mathf.Abs(velocity.x) > 0.1f)
            m_faceRight = velocity.x > 0;
    }

    void ProcessJump(bool jumped)
    {
        bool jump = false;

        if (jumped)
        {
            if (m_grounded)
                jump = true;
            else m_jumpDelay = m_jumpMaxDelay;
        }
        else if (m_jumpDelay > 0 && m_grounded)
            jump = true;

        if(jump)
        {
            var velocity = m_rigidbody.velocity;
            velocity.y = m_jumpPower;
            m_jumpDelay = 0;
        }

        m_jumpDelay -= Time.deltaTime;
    }

    void Kick()
    {

    }

    void Action()
    {

    }
}
