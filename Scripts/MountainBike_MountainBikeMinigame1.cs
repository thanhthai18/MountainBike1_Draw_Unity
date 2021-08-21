using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainBike_MountainBikeMinigame1 : MonoBehaviour
{
    public static MountainBike_MountainBikeMinigame1 instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance);
    }

    [SerializeField] private float speed;
    public float jumpPower;
    public float fallPower;
    private Rigidbody2D rb;
    private bool isBegin;
    public bool isCanJump;
    public bool jump;
    public bool isPass;
    public bool isCheck;
    public TimeManager timeManager;

    private void Start()
    {
        isBegin = false;
        jump = false;
        isPass = false;
        isCheck = true;
        speed = 5f;
        jumpPower = 8f;
        fallPower = 2.5f;
        rb = GetComponent<Rigidbody2D>();
        Invoke(nameof(Begin), 2f);
    }

    void Begin()
    {
        isBegin = true;
    }

    void Jump()
    {
        //rb.velocity = Vector2.zero;
        jump = false;
        isCanJump = false;
        rb.angularVelocity = 0;
        Flip();
        rb.velocity = Vector2.up * jumpPower;

    }

    void Fall()
    {
        rb.velocity += Vector2.up * Physics.gravity.y * fallPower * Time.deltaTime;
        rb.AddForce(Vector2.down);
    }

    void Flip()
    {
        if (GameController_MountainBikeMinigame1.instance.stage == 9 || GameController_MountainBikeMinigame1.instance.stage == 10)
        {
            transform.DORotateQuaternion(Quaternion.Euler(0, 0, 350), 0.5f).SetEase(Ease.InBack).OnComplete(() => { transform.DORotateQuaternion(Quaternion.Euler(0, 0, transform.eulerAngles.z + 700), 0.8f).SetEase(Ease.OutBack); });
            OnFlip1();
            transform.DOMoveX(transform.position.x + 4, 0.6f).SetEase(Ease.Linear).OnComplete(OnFlip2);
        }
        else
        {
            transform.DORotateQuaternion(Quaternion.Euler(0, 0, 350), 1f);
            OnFlip1();
            transform.DOMoveX(transform.position.x + 3, 0.6f).SetEase(Ease.Linear).OnComplete(OnFlip2);
        }
    }

    void OnFlip1()
    {
        speed = 0;
    }
    void OnFlip2()
    {
        speed = 5f;
    }

    void FallLose()
    {
        transform.DORotate(new Vector3(0, 0, 180), 2f);
    }

    void OnCheck()
    {
        isCheck = true;
    }

    private void Update()
    {
        if (isBegin)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            if (jump && isCanJump)
            {
                Jump();
            }

            if (rb.velocity.y < 0)
            {
                Fall();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBegin)
        {
            rb.freezeRotation = true;
            rb.freezeRotation = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isBegin)
        {
            isCanJump = true;

            if (collision.gameObject.CompareTag("Path"))
            {
                speed = 9f;
                fallPower = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isBegin)
        {
            if (collision.gameObject.CompareTag("Path"))
            {
                speed = 5f;
                fallPower = 2.5f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !GameController_MountainBikeMinigame1.instance.isLock && isCheck)
        {
            if (GameController_MountainBikeMinigame1.instance.isFirst)
            {
                GameController_MountainBikeMinigame1.instance.tutorial.SetActive(true);
                GameController_MountainBikeMinigame1.instance.tutorial.transform.DOMoveX(1.54f, 0.05f).SetLoops(-1);
                speed = 0;
            }
            isCheck = false;
            Invoke(nameof(OnCheck), 1);
            GameController_MountainBikeMinigame1.instance.AddStage();
            GameController_MountainBikeMinigame1.instance.isLock = true;
            timeManager.DoSlowmotion();
            isPass = false;
        }

        if (collision.gameObject.CompareTag("Trash") && !isPass)
        {
            GameController_MountainBikeMinigame1.instance.LoseGame();
            timeManager.isSlow = false;
            rb.gravityScale *= 2;
        }
        //if (collision.gameObject.CompareTag("DestroySpawn") && !isPass)
        //{
        //    GameController_MountainBikeMinigame1.instance.LoseGame();
        //    timeManager.isSlow = false;
        //    isBegin = false;
        //    Invoke(nameof(FallLose), 1);
        //}

        if (collision.gameObject.CompareTag("Finish"))
        {
            GameController_MountainBikeMinigame1.instance.WinGame();
        }
    }

}
