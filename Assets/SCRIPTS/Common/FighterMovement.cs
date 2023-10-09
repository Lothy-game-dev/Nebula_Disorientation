using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMovement : MonoBehaviour
{
    #region ComponentVariables
    private FighterShared fs;
    private EnemyShared es;
    private AlliesShared als;
    #endregion
    #region InitializeVariables
    public GameObject FireEffect;
    public GameObject FreezeEffect;
    public GameObject backFire;
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public GameObject HealthBarSlider;
    #endregion
    #region NormalVariables
    public float RotateSpeed;
    public float CurrentRotateAngle;
    public float CurrentSpeed;
    public float MovingSpeed;
    public Vector2 speedVector;
    public int SpeedUp;
    public int RotateDirection;
    public float BackFireInitScale;
    public float LimitSpeedScale;
    public bool Movable;
    public float LaserBeamSlowScale;
    public float ExteriorROTSpeed;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        fs = GetComponent<FighterShared>();
        als = GetComponent<AlliesShared>();
        es = GetComponent<EnemyShared>();
        BackFireInitScale = backFire.transform.localScale.x;
        Movable = true;
        CurrentRotateAngle = 0;
        ExteriorROTSpeed = 1;
        LaserBeamSlowScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (fs.isFrozen)
        {
            Movable = false;
            speedVector = new Vector2(0, 0);
            CurrentSpeed = 0f;
        }
        else
        {
            Movable = true;
        }
        AccelerateSpeed();
        if (Movable) FighterMoving();
        fs.CalculateVelocity(speedVector);
        CheckLimit();
    }
    private void FixedUpdate()
    {
        if (Movable) FighterRotate();
    }
    #endregion
    #region Moving Calling Fuction
    // funtion call to move
    public void UpMove()
    {
        SpeedUp = 1;
    }

    public void DownMove()
    {
        SpeedUp = -1;
    }

    public void NoUpDownMove()
    {
        SpeedUp = 0;
    }

    public void LeftMove()
    {
        RotateDirection = -1;
    }

    public void RightMove()
    {
        RotateDirection = 1;
    }

    public void NoLeftRightMove()
    {
        RotateDirection = 0;
    }
    #endregion
    #region Movement
    private void FighterRotate()
    {
        float RotateScale = 2;
        transform.Rotate(new Vector3(0, 0, -RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        CurrentRotateAngle += RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed;
        // Fire and Freeze eff not rotate
        FireEffect.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        FreezeEffect.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        HealthBarSlider.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
    }
    private void FighterMoving()
    {
        Vector2 movementVector = CalculateMovement();
        if (CurrentSpeed > 0)
        {
            /*GetComponent<PlayerFighter>().PlayMovingSound(CurrentSpeed * fs.SlowedMoveSpdScale / MovingSpeed);*/
            if (!backFire.activeSelf)
            {
                backFire.SetActive(true);
            }
            backFire.transform.localScale =
                new Vector3(CurrentSpeed * fs.SlowedMoveSpdScale / MovingSpeed * BackFireInitScale,
                CurrentSpeed * fs.SlowedMoveSpdScale / MovingSpeed * BackFireInitScale,
                backFire.transform.localScale.z);
        }
        else
        {
/*            GetComponent<PlayerFighter>().StopSound();*/
            backFire.SetActive(false);
        }
        AccelerateSpeed();
        speedVector = movementVector * CurrentSpeed * fs.SlowedMoveSpdScale * LimitSpeedScale * LaserBeamSlowScale;
    }
    // Accelerate Players
    private void AccelerateSpeed()
    {
        if (CurrentSpeed < MovingSpeed && SpeedUp == 1)
        {
            CurrentSpeed += MovingSpeed * Time.timeScale / 300;
        }
        else if (CurrentSpeed > MovingSpeed / 300 && SpeedUp == -1)
        {
            CurrentSpeed -= MovingSpeed * Time.timeScale / 300;
        }
    }
    // Calculate Vector movement based on rotation
    private Vector2 CalculateMovement()
    {
        float x;
        float y;
        float z = transform.eulerAngles.z;
        float degree;
        if (z == 180) return new Vector2(0, -1);
        else if (z == 0 || z == 360) return new Vector2(0, 1);
        else if (z < 180)
        {
            if (z < 90)
            {
                degree = z * Mathf.Deg2Rad;
                x = -Mathf.Abs(Mathf.Sin(degree));
                y = Mathf.Abs(Mathf.Cos(degree));
                return new Vector2(x, y);
            }
            else if (z > 90)
            {
                degree = (180 - z) * Mathf.Deg2Rad;
                x = -Mathf.Abs(Mathf.Sin(degree));
                y = -Mathf.Abs(Mathf.Cos(degree));
                return new Vector2(x, y);
            }
            else return new Vector2(-1, 0);
        }
        else if (z > 180)
        {
            if (z < 270)
            {
                degree = (z - 180) * Mathf.Deg2Rad;
                x = Mathf.Abs(Mathf.Sin(degree));
                y = -Mathf.Abs(Mathf.Cos(degree));
                return new Vector2(x, y);
            }
            else if (z > 270)
            {
                degree = (360 - z) * Mathf.Deg2Rad;
                x = Mathf.Abs(Mathf.Sin(degree));
                y = Mathf.Abs(Mathf.Cos(degree));
                return new Vector2(x, y);
            }
            else return new Vector2(1, 0);
        }
        else return new Vector2(0, 0);
    }

    private void CheckLimit()
    {
        float LimitTopY = TopBorder.transform.position.y - 100;
        float LimitBottomY = BottomBorder.transform.position.y + 100;
        float LimitLeftX = LeftBorder.transform.position.x + 100;
        float LimitRightX = RightBorder.transform.position.x - 100;
        LimitSpeedScale = 1f;
        bool reachLimit = false;
        if (transform.position.x >= (LimitRightX + 50))
        {
            StartCoroutine(TeleportBack(new Vector2(LimitRightX, transform.position.y)));
            reachLimit = true;
        }
        else if (transform.position.x <= (LimitLeftX - 50))
        {
            StartCoroutine(TeleportBack(new Vector2(LimitLeftX, transform.position.y)));
            reachLimit = true;
        }
        else if (transform.position.y >= (LimitTopY + 50))
        {
            StartCoroutine(TeleportBack(new Vector2(transform.position.x, LimitTopY)));
            reachLimit = true;
        }
        else if (transform.position.y <= (LimitBottomY - 50))
        {
            StartCoroutine(TeleportBack(new Vector2(transform.position.x, LimitBottomY)));
            reachLimit = true;
        }
        if (reachLimit)
        {
            if (GetComponent<AlliesFighterMLAgent>()!=null)
            {
                GetComponent<AlliesFighterMLAgent>().MovingLimitReward();
            }
        }
    }

    private IEnumerator TeleportBack(Vector2 Position)
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(Position.x, Position.y, transform.position.z);
        CurrentSpeed = 0f;
    }

    private void CheckOnMoving()
    {

    }
    #endregion
}
