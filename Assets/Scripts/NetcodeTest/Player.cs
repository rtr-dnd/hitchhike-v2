using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class Player : NetworkBehaviour
{
    [SerializeField] float m_moveSpeed = 1;

    private Rigidbody m_rigidBody;
    private Vector2 m_moveInput = Vector2.zero;


    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsOwner)
        {
            SetMoveInputServerRpc(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }

        if (IsServer)
        {
            ServerUpdate();
        }
    }

    [ServerRpc]
    private void SetMoveInputServerRpc(float x, float y)
    {
        m_moveInput = new Vector2(x, y);
    }

    private void ServerUpdate()
    {
        var velocity = Vector3.zero;
        velocity.x = m_moveSpeed * m_moveInput.normalized.x;
        velocity.z = m_moveSpeed * m_moveInput.normalized.y;
        m_rigidBody.AddForce(velocity);
    }
}
