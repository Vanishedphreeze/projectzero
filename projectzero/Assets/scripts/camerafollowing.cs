using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollowing : MonoBehaviour {

    [SerializeField] private float maxShakingAmplitude = 0.3f;
    [SerializeField] private float shakingTime = 0.5f;
    [SerializeField] private float shakingPeriod = 0.02f;
    [SerializeField] private float shakingDumpRatio = 5f;

    [HideInInspector] public bool inshaking = false;
    private float lastShakeTime = -100f;
    private float shakeStartTime = -100f;
    private Transform playerTransform;
    //private Camera m_Cam;

    private void Awake () {
        //m_Cam = Camera.main;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 stdCameraPos;
        if (playerTransform != null) {
            Vector3 playerPos = new Vector3 (playerTransform.position.x, playerTransform.position.y, transform.position.z);
            //transform.position = playerPos;
            stdCameraPos = playerPos;
        }
        else {
            stdCameraPos = transform.position;
        }

        // fomula: maxa*ex(-dumpratio*cnt)
        if (inshaking) {
            Vector3 tempPos = stdCameraPos;
            if (Time.time > lastShakeTime + shakingPeriod * 2) {
                lastShakeTime = Time.time;
                tempPos.y -= maxShakingAmplitude * Mathf.Exp(-1f * shakingDumpRatio * (Time.time - shakeStartTime));
            }
            else if (Time.time > lastShakeTime + shakingPeriod) {
                tempPos.y += maxShakingAmplitude * Mathf.Exp(-1f * shakingDumpRatio * (Time.time - shakeStartTime));
            }
            transform.position = tempPos;
        }
        else {
            transform.position = stdCameraPos;
        }
        
        // exit shaking if time
        if (Time.time > shakeStartTime + shakingTime && inshaking) {
            inshaking = false;
        }
    }

    public void starshaking () {
        inshaking = true;
        shakeStartTime = Time.time;
    }
}
