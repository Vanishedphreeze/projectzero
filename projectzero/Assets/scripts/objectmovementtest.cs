using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectmovementtest : MonoBehaviour {

    public float movespd = 1f;

    // Use this for initialization
    private void Start() {
        //m_Object = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update () {
        Vector2 tempPos = new Vector2();
        tempPos = gameObject.transform.position;
        tempPos.x += movespd * Input.GetAxis("Horizontal");
        tempPos.y += movespd * Input.GetAxis("Vertical");
        gameObject.transform.position = tempPos;
    }
}
