using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Globalspace;

public class pickups : MonoBehaviour {
    [SerializeField] private int faceValue = 100;
    [SerializeField] private float pickupAnimPlayTime = 0.55f;
    [HideInInspector] public bool inAnimPlay = false;
    private float pickupAnimStartTime = -100f;
    private Animator m_anim;
    private characterbehavior m_player;

	// Use this for initialization
	void Awake () {
        m_anim = GetComponent<Animator>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<characterbehavior>();
	}

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (CompareTag("Money")) playerstatics.money += faceValue;
            else if (CompareTag("Health")) {
                if (m_player.health + faceValue > 100f) m_player.health = 100f;
                else m_player.health += faceValue;
            }
        }
        inAnimPlay = true;
        pickupAnimStartTime = Time.time;
        m_anim.SetTrigger("Play");
    }

    // Update is called once per frame
    void Update () {
		if (Time.time > pickupAnimStartTime + pickupAnimPlayTime && inAnimPlay) {
            Destroy(gameObject);
        }
	}
}
