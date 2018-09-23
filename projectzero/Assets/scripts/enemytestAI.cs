using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemytestAI : MonoBehaviour {

    private characterbehavior m_Character;

    private void Awake () {
        m_Character = GetComponent<characterbehavior>();
    }

    private void FixedUpdate () {
        //m_Character.act(h, jump, fastfall, dash, charge, punch);
        m_Character.act(0f, false, false, false, true, true);
    }
}
