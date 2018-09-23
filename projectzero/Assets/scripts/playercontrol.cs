using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Globalspace;

public class playercontrol : MonoBehaviour {

    private characterbehavior m_Character;
    private bool jump;
    private bool fastfall;
    private bool dash;
    private bool charge;
    private bool punch;

    // player status temporary placed here
    public int money = 0;

    private void Awake() {
        m_Character = GetComponent<characterbehavior>();
        // edit player's money
        playerstatics.money = 0;
        playerstatics.health = m_Character.health;
    }


    private void Update() {
        // one of the statics here
        playerstatics.health = m_Character.health;

        if (!jump) jump = Input.GetButtonDown("Jump");

        //if (!dash) dash = Input.GetButtonDown("Dash");
        //if (!fastfall) fastfall = Input.GetButtonDown("Punch");

        if (Input.GetButtonDown("Dash")) {
            if (Input.GetAxis("Vertical") < 0 && !m_Character.grounded) {
                if (!fastfall) fastfall = true;
            }
            else {
                if (!dash) dash = true;
            }
        }
        // "dash" is buffered until first call of FixedUpdate

        if (!charge) charge = Input.GetButtonDown("Punch");
        if (!punch) punch = Input.GetButtonUp("Punch");
    }


    private void FixedUpdate() {
        float h = Input.GetAxis("Horizontal");
        m_Character.act(h, jump, fastfall, dash, charge, punch);
        charge = punch = dash = jump = fastfall = false;
    }
}
