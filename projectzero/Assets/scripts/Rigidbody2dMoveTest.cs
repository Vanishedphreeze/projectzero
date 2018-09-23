using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2dMoveTest : MonoBehaviour {

    public float movespd = 1f;
    private Rigidbody2D m_Rigidbody2D;

    // Use this for initialization
    void Start () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        Vector2 force = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //m_Rigidbody2D.AddForce(force*movespd);
        m_Rigidbody2D.velocity = force * movespd;
    }
}
