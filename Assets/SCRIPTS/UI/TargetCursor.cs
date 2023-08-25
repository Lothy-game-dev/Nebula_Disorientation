using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCursor : MonoBehaviour
{
    #region ComponentVariables

    #endregion
    #region InitializeVariables
    public Camera cam;
    public GameObject AimLeft;
    public GameObject AimRight;
    public float GlowTime;
    public GameController GameController;
    #endregion
    #region NormalVariables
    private Vector3 MousePos;
    private bool LeftGlowing;
    private bool RightGlowing;
    private bool LeftNotDown;
    private bool RightNotDown;
    private float LeftUpTimer;
    private float LeftDownTimer;
    private float RightUpTimer;
    private float RightDownTimer;
    private float InitScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        LeftGlowing = false;
        RightGlowing = false;
        InitScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Change size if cam is close
        if (GameController.IsClose)
        {
            transform.localScale = new Vector3(InitScale / 2, InitScale / 2, InitScale / 2);
        } else
        {
            transform.localScale = new Vector3(InitScale, InitScale, InitScale);
        }
        // Change position of target to the same position as cursor
        MousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        MousePos.z = 0;
        transform.position = MousePos;
        // Check Mouse Input Left Right To Glow Up the right part of cursor
        // Case Left Button Press
        if (Input.GetMouseButtonDown(0))
        {
            LeftGlowing = true;
        }
        // Case Left Button Hold
        if (Input.GetMouseButton(0))
        {
            LeftGlowing = true;
            LeftNotDown = true;
        }
        // Case Left Button Exit
        if (Input.GetMouseButtonUp(0))
        {
            if (LeftNotDown)
            {
                LeftNotDown = false;
            }
        } 
        // Glowing Up for Left mouse
        if (LeftGlowing && LeftUpTimer<=0f)
        {
            Color c = AimLeft.GetComponent<SpriteRenderer>().color;
            if (c.a<1)
            {
                GlowUpCursor(AimLeft);
                LeftUpTimer = GlowTime / 20f;
            } else if (!LeftNotDown)
            {
                LeftGlowing = false;
            }
        }
        // Glowing Down for Left mouse
        if (!LeftGlowing && LeftDownTimer<=0f)
        {
            Color c = AimLeft.GetComponent<SpriteRenderer>().color;
            if (c.a > 0)
            {
                GlowDownCursor(AimLeft);
                LeftDownTimer = GlowTime / 20f;
            }
        }
        // Case Right Button Press
        if (Input.GetMouseButtonDown(1))
        {
            RightGlowing = true;
            RightNotDown = true;
        }
        // Case Right Button Exit
        if (Input.GetMouseButtonUp(1))
        {
            if (RightNotDown)
            {
                RightNotDown = false;
            }
        }
        // Glowing Up for Right mouse
        if (RightGlowing && RightUpTimer <= 0f)
        {
            Color c = AimRight.GetComponent<SpriteRenderer>().color;
            if (c.a < 1)
            {
                GlowUpCursor(AimRight);
                RightUpTimer = GlowTime / 20f;
            }
            else if (!RightNotDown)
            {
                RightGlowing = false;
            }
        }
        // Glowing Down for Right mouse
        if (!RightGlowing && RightDownTimer <= 0f)
        {
            Color c = AimRight.GetComponent<SpriteRenderer>().color;
            if (c.a > 0)
            {
                GlowDownCursor(AimRight);
                RightDownTimer = GlowTime / 20f;
            }
        }
        // Timer reset to 0 if > 0
        if (LeftUpTimer > 0f)
        {
            LeftUpTimer -= Time.deltaTime;
        }
        if (LeftDownTimer > 0f)
        {
            LeftDownTimer -= Time.deltaTime;
        }
        if (RightUpTimer > 0f)
        {
            RightUpTimer -= Time.deltaTime;
        }
        if (RightDownTimer > 0f)
        {
            RightDownTimer -= Time.deltaTime;
        }
    }
    #endregion
    #region Glow Cursor
    // Glow Up/Down Cursor by increase transparent of the object
    void GlowUpCursor(GameObject cursor)
    {
        Color c = cursor.GetComponent<SpriteRenderer>().color;
        c.a += 0.1f;
        cursor.GetComponent<SpriteRenderer>().color = c;
    }
    // Glow Down Cursor when not press
    void GlowDownCursor(GameObject cursor)
    {
        Color c = cursor.GetComponent<SpriteRenderer>().color;
        c.a -= 0.1f;
        cursor.GetComponent<SpriteRenderer>().color = c;
    }
    #endregion
}
