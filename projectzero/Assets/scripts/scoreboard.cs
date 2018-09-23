using System.Collections;
using System.Collections.Generic;
using Globalspace;
using UnityEngine;
using UnityEngine.UI;

public class scoreboard : MonoBehaviour {
    private Text scoretext; 

	// Use this for initialization
	void Awake () {
        scoretext = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        scoretext.text = "Score: " + playerstatics.money.ToString();
	}
}
