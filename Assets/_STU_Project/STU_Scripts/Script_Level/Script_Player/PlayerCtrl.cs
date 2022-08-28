using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.InteropServices;
using System;
using DG.Tweening;

public class PlayerCtrl : MonoBehaviour
{
    // PlayerSelf
    [Header("參考")]
    [SerializeField] private Rigidbody2D rb = null;
    [SerializeField] private SpriteRenderer sprRenderer = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private WallDetector wallDetector = null;
    [SerializeField] private GameObject girlObj;
    [SerializeField] private GameObject boyObj;

    // movement value
    [Header("移動")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 5f;

    [Header("跳躍")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpPower = 20f;

    [Header("翻轉")]
    [SerializeField] private float flipSec = 2f;
    [SerializeField] private AnimationCurve flipCurve;

    [Header("攻擊_踩")]
    [SerializeField] private float damagePerUnit = 5;
    [SerializeField] private float flipBouns = 1.2f;

    [Header("攝影機跟隨")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float maxDistance = 2;
    [SerializeField] private float followSensitive = 1;
    private float followPosX = 0;
    private float followPosY = 0;

    [Header("地板檢測")]
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [SerializeField] private float envGravity = -40f;

    private float input;
    private bool isFaceRight = false;
    private bool isDie;
    private float wallCheckOriX;

    [HideInInspector]public bool canCtrl = true;

    void Start()
    {
        isDie = false;
        cameraTarget.parent = null;

        if (Physics2D.gravity.y != envGravity)
        {
            Physics2D.gravity = new Vector2(0f, envGravity);
        }
        if (GameManager.instance.life != 10)
        {
            anim.SetTrigger("Reborn");
            //transform.position = GameManager.instance.respawnPosition;
        }

        // Record current level
        GameManager.instance.levelName = SceneManager.GetActiveScene().name;
        GameManager.instance.Save();
    }
    private void OnDestroy()
    {
        // GameManager.instance.respawnPosition = transform.position;
    }

    void Update()
    {
        onFloor = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (onFloor == true)
            UpdateY();
        else
            CameraFollowYFix();

        if (isDie || canCtrl == false)
            return;

        MovementX();
        JumpmentY();

        if (Input.GetKeyDown(KeyCode.C))
            AntiGravity();
    }

    private void CameraFollowYFix()
    {
        float maxY = 3.5f;
        if (isGirl == true)
        {
            if (transform.position.y > followPosY + maxY)
                followPosY = transform.position.y - maxY;
            else if (transform.position.y < followPosY)
                followPosY = transform.position.y;
        }
        else
        {
            if (transform.position.y < followPosY - maxY)
                followPosY = transform.position.y + maxY;
            else if (transform.position.y > followPosY)
                followPosY = transform.position.y;
        }
    }

    #region 移動
    private float sp = 0f;
    private float mixSpeed = 3f;

    private void MovementX()
    {
        input = Input.GetAxisRaw("Horizontal"); // Get keyboard AD value -1 ~ 1
        sp = Mathf.Lerp(sp, Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed, Time.deltaTime * mixSpeed);

        if (input != 0)
            followPosX = Mathf.Lerp(followPosX, input * maxDistance, Time.deltaTime * followSensitive);
        cameraTarget.transform.position = new Vector3(transform.position.x + followPosX, followPosY, 0);

        if (input > 0 && isFaceRight == false)
            FlipSprite();
        else if (input < 0 && isFaceRight == true)
            FlipSprite();

        if (Mathf.Abs(input) > 0.1f)
        {
            anim.SetBool("IsRun", true);
        }
        else
        {
            anim.SetBool("IsRun", false);
        }

        wallDetector.SetDirection(sprRenderer.flipX);

        float wallStop = 1;
        //如果有碰到牆就停止往同方向施加Velocity
        if (wallDetector.isTouching)
            wallStop = sprRenderer.flipX ? Mathf.Clamp(input, -1, 0) : Mathf.Clamp(input, 0, 1);

        Vector2 moveNormal = new Vector2(wallStop * input * sp * antiGravity, rb.velocity.y);
        rb.velocity = moveNormal;
    }

    private void FlipSprite()
    {
        transform.localScale = new Vector3(isFaceRight ? 1 : -1, 1, 1);
        isFaceRight = !isFaceRight;
    }
#endregion

    #region 跳躍
    private int jumps;
    /// <summary> On floor </summary>
    private bool _onFloor = false;
    bool onFloor
    {
        // 讀取的時候回傳_onFloor
        get { return _onFloor; }
        // 寫入的時候寫入_onFloor
        // 順便修改動畫的onFloor
        set { 
            _onFloor = value; 
            anim.SetBool("OnFloor", value); }
    }

    public void JumpmentY()
    {
        bool isJump = Input.GetKeyDown(KeyCode.Space);
        if (isJump)
        {
            if (onFloor)
            {
                jumps = maxJumps;
                JumpG();
                jumps -= 1;
            }
            else
            {
                if (jumps > 0)
                {
                    JumpG();
                    jumps -= 1;
                }
                else
                    return;
            }
        }

        anim.SetFloat("Y", rb.velocity.y); // Send Y into the animation
    }
    public void JumpG()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        SoundManager.Instance.Play(Sound.Jump);
    }
    #endregion

    #region 翻轉
    private int antiGravity = 1;
    private bool isGirl = true;
    private bool isAntiPlayer = default;
    public void AntiGravity()
    {
        if (onFloor) return;

        canCtrl = false;

        ChangeSex();
        DoFlip();
    }

    private void ChangeSex()
    {
        isGirl = !isGirl;
        if (isGirl == true)
        {
            girlObj.SetActive(true);
            boyObj.SetActive(false);
            anim = girlObj.GetComponent<Animator>();
        }
        else
        {
            girlObj.SetActive(false);
            boyObj.SetActive(true);
            anim = boyObj.GetComponent<Animator>();
        }
    }

    private void DoFlip()
    {
        rb.simulated = false;
        antiGravity *= -1;
        isAntiPlayer = !isAntiPlayer;
        Physics2D.gravity *= antiGravity;
        GameObject.FindGameObjectWithTag("PlayerCM").transform.SetParent(transform);
        transform.DORotate(new Vector3(0, 0, 180), flipSec, RotateMode.WorldAxisAdd).SetEase(flipCurve).OnComplete(() =>
        {
            rb.simulated = true;
            GameObject.FindGameObjectWithTag("PlayerCM").transform.SetParent(null);
            canCtrl = true;
        });
    }
    public void AntiGravityByProps(bool isInvert)
    {
        if (isInvert)
        {
            // Negative
            Physics2D.gravity = new Vector2(0, Mathf.Abs(Physics2D.gravity.y));
            antiGravity = -1;
            transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
            //isAntiPlayer = true;
        }
        else
        {
            // Positive
            Physics2D.gravity = new Vector2(0, Mathf.Abs(Physics2D.gravity.y) * -1f);
            antiGravity = 1;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //isAntiPlayer = false;
        }
    }
    #endregion

    /// <summary> 與任何東西碰撞 </summary>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Damage2")
        {
            GameManager.instance.hp -= 2f;
            EvaluationForm.touchTrapCount++;
            anim.SetTrigger("Hit");
        }
        if (other.transform.tag == "Damage999")
        {
            GameManager.instance.hp -= 999f;
            anim.SetTrigger("Hit");
        }

        if (GameManager.instance.hp <= 0f)
        {
            isDie = true;
            // Die Sound
            SoundManager.Instance.Play(Sound.Die, 2f);
            anim.SetTrigger("Die");
            // Pause
            // Time.timeScale = 0f;
            
            Invoke("Regeneration",1f);
        }
    }

    void Regeneration()
    {
        GameManager.instance.life -= 1;
        GameManager.instance.hp = 1f;        
        GameManager.instance.LoadGame();
    }
    private void UpdateY()
    {
        float offset = 0.7f;
        if (isGirl == true)
            followPosY = transform.position.y + offset;
        else
            followPosY = transform.position.y - offset;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}