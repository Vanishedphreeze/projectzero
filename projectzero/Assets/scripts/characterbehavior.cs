using System.Collections;
using System.Collections.Generic;
using Globalspace;
using UnityEngine;

// A boxman is initiallized to be an enemy
// before change this into a player check out all tags of it

// punch and basedamage are all regarded as damage. this is related with inv judges

public class characterbehavior : MonoBehaviour {

    // character statics
    [SerializeField] private float jumpForce = 5500f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float fastfallSpeed = -70f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float chargeDecSpdRate = 0.5f;
    [SerializeField] private bool airControl = true;
    [SerializeField] private float damageKnockBackSpd = 3f;
    [SerializeField] private float punchBaseKnockBackSpd = 5f;
    [SerializeField] private float punchKnockBackRatio = 10f;
    [SerializeField] private float footstoolJumpHeightRatio = 1.2f;


    [SerializeField] private float damageInvcibileTime = 2f;
    [SerializeField] private float maxChargeTime = 3f;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    [SerializeField] private float punchBaseKnockBackTime = 0.3f;
    [SerializeField] private float punchKnockBackTimeRatio = 0.2f;
    [SerializeField] private float spawnRecoveryTime = 0.2f;
    [SerializeField] private float dashRecoveryTime = 0.2f;
    [SerializeField] private float punchRecoveryTime = 0.2f;
    [SerializeField] private float footstooledRecoveryTime = 0.05f;
    [SerializeField] private float invincibleFlashTime = 0.05f;
    [SerializeField] private float deadAnimPlayTime = 1.1f;


    [SerializeField] private float visibleZPosition = 0f;
    [SerializeField] private float hiddenZPosition = 17f;
    
    public float health = 100f;
    public float damagePerHit = 10f;

    //make sure that damageInvcibileTime > recoveryTime


    // temporary variable
    [HideInInspector] public bool facingright = true;
    [HideInInspector] public bool grounded = false;
    [HideInInspector] public bool inBaseDamageRecovery = false;
    [HideInInspector] public bool inFastfall = false;
    [HideInInspector] public bool inSpawn = false;
    [HideInInspector] public bool inDash = false;
    [HideInInspector] public bool inFootstooled = false;
    [HideInInspector] public bool inCharge = false;
    [HideInInspector] public bool inPunch = false;              // true if punching
    [HideInInspector] public bool ingetPunchedRecovery = false; // true if has been punched
    [HideInInspector] public bool inDeadAnimPlay = false;
    [HideInInspector] public bool damageinv = false;            // true if the charater is in invincible frames caused by hitboxs
    [HideInInspector] public bool moveInv = false;              // true if the charater is in invincible frames caused by moves
    [HideInInspector] public bool controllable = true;
    [HideInInspector] public bool footstooledOnSth = false;
    private float lastDashTime = -100f;
    private float lastSpawnTime = -100f;
    private float lastFlashTime = -100f;
    private float lastFootstooledTime = -100f;
    private float lastChargeTime = -100f;
    private float lastPunchTime = -100f;
    private float damageInvStartTime = -100f;
    private float deadAnimStartTime = -100f;
    private float getpunchedRcoStartTime = -100f;
    private float baseDamagedRcoStartTime = -100f;
    private float curGetPunchedRecoveryTime = 0f;
    private float curGetPunchedSpd = 0f;
    private Rigidbody2D m_Rigidbody2D;
    private Animator m_Anim;
    private camerafollowing m_Cam;








    private void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Anim = GetComponent<Animator>();
        m_Cam = Camera.main.GetComponent<camerafollowing>();
        //m_Anim.SetTrigger("toSpawn");
    }






    private void Update () {
        // invincible flash
        if (damageinv) {
            if (Time.time > lastFlashTime + invincibleFlashTime * 2) {
                lastFlashTime = Time.time;
                Vector3 tempPos = new Vector3 (transform.position.x, transform.position.y, visibleZPosition);
                transform.position = tempPos;
            }
            else if (Time.time > lastFlashTime + invincibleFlashTime) {
                Vector3 tempPos = new Vector3 (transform.position.x, transform.position.y, hiddenZPosition);
                transform.position = tempPos;
            }
        }
    }






    private void FixedUpdate() {

        // ground judge
        grounded = false;
        Vector2 colliderBoxSize = new Vector2(Mathf.Abs(transform.lossyScale.x * 2f), Mathf.Abs(transform.lossyScale.y * 2f));
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, colliderBoxSize, 0f, LayerMask.GetMask("ground"));
        for (int i = 0; i < colliders.Length; i++) {
            float colliderTopPos = colliders[i].transform.position.y + colliders[i].transform.lossyScale.y;
            float characterBottomPos = transform.position.y - transform.lossyScale.y;
            if (colliders[i].gameObject != gameObject && characterBottomPos >= colliderTopPos) grounded = true;
        }



        // hit && damage judge
        colliders = Physics2D.OverlapBoxAll(transform.position, colliderBoxSize, 0f, LayerMask.GetMask("hitboxes"));
        for (int i = 0; i < colliders.Length; i++) {
            // process order:
            // abyssfall -> instadead -> footstool judge -> punch judge -> basedmg judge


            // abyssfall judge
            if (colliders[i].gameObject.tag == "AbyssHitbox") {
                playerstatics.health = 0;
                Destroy(gameObject);
                m_Cam.starshaking();
            }

            // footstool judge
            else if ((gameObject.tag == "Player" && colliders[i].gameObject.tag == "EnemyFastfallHitbox") ||
                (gameObject.tag == "Enemy" && colliders[i].gameObject.tag == "PlayerFastfallHitbox")) {

                characterbehavior tempobject = colliders[i].GetComponentInParent<characterbehavior>();
                if (tempobject != null && !damageinv && !moveInv) {
                    // footstool to differernt character should behaves variably. 
                    // now footstool kills enemy instantly but no effect on player
                    // now character in footstool anim has moveinv
                    tempobject.footstooledOnSth = true;
                    inFootstooled = true;
                    m_Anim.SetTrigger("Footstooled");
                    lastFootstooledTime = Time.time;
                }
            }

            // punch judge
            else if ((gameObject.tag == "Player" && colliders[i].gameObject.tag == "EnemyPunchHitbox") ||
                     (gameObject.tag == "Enemy" && colliders[i].gameObject.tag == "PlayerPunchHitbox")) {

                characterbehavior tempobject = colliders[i].GetComponentInParent<characterbehavior>();
                if (tempobject != null && !damageinv && !moveInv) {
                    if ((facingright && tempobject.facingright) || (!facingright && !tempobject.facingright)) turnaround();
                    // face to the character who's punching it
                    damageinv = ingetPunchedRecovery = true;
                    damageInvStartTime = getpunchedRcoStartTime = Time.time;
                    curGetPunchedSpd = tempobject.getpunchspd();
                    curGetPunchedRecoveryTime = tempobject.getpunchrcotime();
                    m_Anim.SetTrigger("Damaged");
                }
                break;
            }

            // base damage judge
            else if ((gameObject.tag == "Player" && colliders[i].gameObject.tag == "EnemyBaseHitbox") ||
                     (gameObject.tag == "Enemy" && colliders[i].gameObject.tag == "PlayerBaseHitbox")) {

                if (!damageinv && !moveInv) {
                    // enemy do have invincible frames now 
                    if (health > damagePerHit) {
                        health -= damagePerHit;
                        damageinv = inBaseDamageRecovery = true;
                        damageInvStartTime = baseDamagedRcoStartTime = Time.time;
                        m_Anim.SetTrigger("Damaged");
                    }
                    else {
                        health = 0;
                        deadAnimStartTime = Time.time;
                        inDeadAnimPlay = true;
                        m_Anim.SetTrigger("Normaldie");
                    }
                }
                break;
            }
        }

        if (inDeadAnimPlay) characterdeadprocess();
        else if (inDash) dashprocess();
        else if (inFastfall || inSpawn) fastfallandspawnprocess();
        else if (inPunch) punchprocess();
        else if (inBaseDamageRecovery) basedamagercoprocess();
        else if (ingetPunchedRecovery) getpunchedrcoprocess();
        else if (inFootstooled) getfootstooledprocess();


        // get out from damageinv if time
        if (Time.time > damageInvStartTime + damageInvcibileTime && damageinv) {
            damageinv = false;
            Vector3 tempPos = new Vector3 (transform.position.x, transform.position.y, 0f); // original z position is 0
            transform.position = tempPos;
        }
    }






    public void act(float move, bool jump, bool fastfall, bool dash, bool charge, bool punch) {
        // act is called every fixedframe in player/AI controller.
        // it is not called if there is no controller.

        // process moves and controls
        if (controllable) {
            if (fastfall && !inCharge) {
                inFastfall = true;
                m_Anim.SetTrigger("Fastfall");
            }

            if (dash  && !inCharge) {
                lastDashTime = Time.time;
                inDash = true;
                m_Anim.SetTrigger("Dash");
            }

            if (charge) {
                lastChargeTime = Time.time;
                inCharge = true;
                m_Anim.SetTrigger("Punchcharge");
            }

            if (inCharge && punch) {
                lastPunchTime = Time.time;
                inCharge = false;
                inPunch = true;
                m_Anim.SetTrigger("Punch");
            }

            if (grounded || airControl) {
                if (inCharge) m_Rigidbody2D.velocity = new Vector2(move * maxSpeed * chargeDecSpdRate, m_Rigidbody2D.velocity.y);
                else m_Rigidbody2D.velocity = new Vector2(move * maxSpeed, m_Rigidbody2D.velocity.y);
                if (move > 0 && !facingright) turnaround();
                else if (move < 0 && facingright) turnaround();
            }

            if (grounded && jump) {
                grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            }
        }
    }






    private void turnaround() {
        facingright = !facingright;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1f;
        transform.localScale = tempScale;
    }






    /*
     * move process functions 
     * e.g.
     * 

    private void dashprocess() {
        // move attribute

        // move actions

        // exit condition
    }

     * 
     * put "attribute" before the first call of the function is more optimized
     * 
     * */


    private void dashprocess() {
        // move attribute
        moveInv = true;
        controllable = false;

        // move actions
        if (facingright) m_Rigidbody2D.velocity = new Vector2(1f * dashSpeed, 0f);
        else m_Rigidbody2D.velocity = new Vector2(-1f * dashSpeed, 0f);

        // exit condition
        if (Time.time > lastDashTime + dashRecoveryTime) {
            inDash = false;
            moveInv = false;
            controllable = true;
            m_Anim.SetTrigger("Recovered");
        }
    }






    private void fastfallandspawnprocess() { // this include both fastfall and spawn
        // move attribute
        moveInv = true;
        controllable = false;

        // move actions
        // it could causes some bugs if a footstooled character cannot destory instantly 
        if (!footstooledOnSth) m_Rigidbody2D.velocity = new Vector2(0f, fastfallSpeed);
        else {
            if (grounded) {
                //m_Rigidbody2D.velocity = Vector2.zero;
                grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce * footstoolJumpHeightRatio));
            }
        }

        // fastfall-spawn transform condition
        if (inFastfall && grounded) {
            m_Anim.SetTrigger("toSpawn");
            lastSpawnTime = Time.time;
            inSpawn = true;
            inFastfall = false;
        }

        // exit condition
        if (Time.time > lastSpawnTime + spawnRecoveryTime && inSpawn) {
            inSpawn = false;
            moveInv = false;
            controllable = true;
            footstooledOnSth = false;
            m_Anim.SetTrigger("Recovered");
        }
    }






    private void punchprocess() {
        // move attribute
        moveInv = true;
        controllable = false;

        // exit condition
        if (Time.time > lastPunchTime + punchRecoveryTime) {
            inPunch = false;
            moveInv = false;
            controllable = true;
            m_Anim.SetTrigger("Recovered");
        }
    }






    private void getpunchedrcoprocess () {
        // move attribute
        inCharge = false; // interrupt charge
        controllable = false;
        // damage inv process has written before

        // move actions
        if (facingright) m_Rigidbody2D.velocity = new Vector2(-1f * curGetPunchedSpd, m_Rigidbody2D.velocity.y);
        else m_Rigidbody2D.velocity = new Vector2(1f * curGetPunchedSpd, m_Rigidbody2D.velocity.y);

        // exit condition
        if (Time.time > getpunchedRcoStartTime + curGetPunchedRecoveryTime) {
            //print(curGetPunchedSpd);
            ingetPunchedRecovery = false;
            controllable = true;
            m_Anim.SetTrigger("Recovered");
        }
    }






    private void basedamagercoprocess () {
        // move attribute
        inCharge = false; // interrupt charge
        controllable = false;

        // move actions
        if (facingright) m_Rigidbody2D.velocity = new Vector2(-1f * damageKnockBackSpd, m_Rigidbody2D.velocity.y);
        else m_Rigidbody2D.velocity = new Vector2(1f * damageKnockBackSpd, m_Rigidbody2D.velocity.y);

        // exit condition
        if (Time.time > baseDamagedRcoStartTime + damageRecoveryTime) {
            inBaseDamageRecovery = false;
            controllable = true;
            m_Anim.SetTrigger("Recovered");
        }
    }






    private void getfootstooledprocess () {
        // move attribute
        inCharge = false; // interrupt charge
        controllable = false;
        moveInv = true;

        // exit condition
        if (Time.time > lastFootstooledTime + footstooledRecoveryTime) {
            inFootstooled = false;
            controllable = true;
            moveInv = false;
            //m_Anim.SetTrigger("Recovered");

            m_Anim.SetTrigger("Footstooldie");
            inDeadAnimPlay = true;
            deadAnimStartTime = Time.time;
            // footstool is designed to kill the character instantly
            // footstool to differernt character should behaves variably. 
        }
    }






    private void characterdeadprocess() {
        // move attribute
        inCharge = false;
        moveInv = true;
        controllable = false;
        m_Rigidbody2D.isKinematic = true;
        m_Rigidbody2D.velocity = Vector2.zero;

        // exit condition
        if (Time.time > deadAnimStartTime + deadAnimPlayTime) {
            Destroy(gameObject);
        }
    }






    public float getpunchspd() {

        // BKB = 5;
        // KB = 10*(t) + 5;            (0 <= t <= maxchargetime)
        //      10*(maxt) + 5;         (t > maxchargetime)

        if (Time.time - lastChargeTime > maxChargeTime) return punchKnockBackRatio * maxChargeTime + punchBaseKnockBackSpd;
        else return punchKnockBackRatio * (Time.time - lastChargeTime) + punchBaseKnockBackSpd;
    }






    public float getpunchrcotime() {
        // Brco = 0.3;
        // rco = 0.2*(t) + 0.3;          (0 <= t <= maxchargetime)
        //       0.2*(maxt) + 0.3;       (t > maxchargetime)

        if (Time.time - lastChargeTime > maxChargeTime) return punchKnockBackTimeRatio * maxChargeTime + punchBaseKnockBackTime;
        else return punchKnockBackTimeRatio * (Time.time - lastChargeTime) + punchBaseKnockBackTime;
    }






}
