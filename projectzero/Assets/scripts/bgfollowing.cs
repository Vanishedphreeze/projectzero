using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgfollowing : MonoBehaviour {

    private Transform playerTransform;

    private void Awake () {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Use this for initialization
    void Start () {
        //transform.position = Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        // the bg should be atteched to camera

        //Vector2 campos = new Vector2 (Camera.main.transform.position.x, Camera.main.transform.position.y);
        //transform.position = campos;
        if (playerTransform != null) {
            Vector3 playerPos = new Vector3 (playerTransform.position.x, playerTransform.position.y, transform.position.z);
            transform.position = playerPos;
        }
    }
}
