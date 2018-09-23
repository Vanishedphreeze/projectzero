using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Globalspace;

public class lifebar : MonoBehaviour {

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(1, playerstatics.health / 100, 1);
        //print(playerstatics.health);
	}
}
