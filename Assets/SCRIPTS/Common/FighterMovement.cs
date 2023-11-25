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
    public GameObject ShieldBarSlider;
    public GameObject HeadObject;
    public SpaceZoneHazardEnvironment HazardEnvi;
    public GameplayInteriorController ControllerMain;
    public bool IsNotMoving;
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
    private float CheckMovingDelay;
    public string LimitString;
    private float ChosenRandom;
    private bool inAttackRange;
    public bool PreventReachLimit;
    public float PreventReachLimitTimer;
    private int InAttackRangeCount;
    private float LimitDelay;
    public LayerMask HazardMask;
    private float StartMovingDelay;
    private float xRandom;
    private float yRandom;
    private bool NoEscorting;
    private float delayCheckLimit;
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
        NoEscorting = true;
        ExteriorROTSpeed = 1;
        LaserBeamSlowScale = 1;
        UpMove();
        LimitString = "";
        if (als!=null)
        {
            if (als.Escort)
                StartMovingDelay = 2f;
            else if (als.Defend)
            {
                StartMovingDelay = 0f;
                xRandom = Random.Range(1500, 2000f);
            }
            else
                StartMovingDelay = 4f;
        }
        if (es!=null)
        {
            if (es.Escort)
                StartMovingDelay = 3f;
            else if (es.isBomb)
                StartMovingDelay = Random.Range(3f, 6f);
            else
                StartMovingDelay = 4f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (fs.isFrozen || (ControllerMain.IsInLoading))
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

        if (PreventReachLimitTimer <= 0f)
        {
            if (PreventReachLimit)
            {
                PreventReachLimit = false;
            }
        }
        else
        {
            PreventReachLimit = true;
            PreventReachLimitTimer -= Time.deltaTime;
        }
        if (!ControllerMain.IsInLoading)
        {
            StartMovingDelay -= Time.deltaTime;
        }
        if (StartMovingDelay<=0f)
        {
            if (LimitString == "")
            {
                if (LimitDelay <= 0f)
                {
                    if ((es!=null && es.isBomb))
                    {
                        if (CheckMovingDelay <= 0f)
                        {
                            CheckMovingDelay = Random.Range(0.25f, 0.5f);
                            CheckOnMoving();
                        }
                        else
                        {
                            CheckMovingDelay -= Time.deltaTime;
                        }
                    } 
                    else
                    if (!PreventReachLimit)
                    {
                        if (als!=null && als.IsEscorting)
                        {

                        }
                        else
                        {
                            PreventReachingLimit();
                        }
                        if (!PreventReachLimit)
                        {
                            if (CheckMovingDelay <= 0f)
                            {
                                CheckMovingDelay = Random.Range(0.25f, 0.5f);
                                CheckOnMoving();
                            }
                            else
                            {
                                CheckMovingDelay -= Time.deltaTime;
                            }
                        }
                    }
                }
                else
                {
                    LimitDelay -= Time.deltaTime;
                }
            }
            else
            {
                GetAwayFromLimit();
            }
            if (HazardEnvi.HazardID == 2 || HazardEnvi.HazardID == 5 || HazardEnvi.HazardID == 6)
            {
                CheckForHazard();
            }
        }
        if (delayCheckLimit>0f)
        {
            delayCheckLimit -= Time.deltaTime;
        } else
        {
            CheckLimit();
        }
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
        SpeedUp = IsNotMoving ? 0 :1;
    }

    public void DownMove()
    {
        SpeedUp = IsNotMoving ? 0 : -1;
    }

    public void NoUpDownMove()
    {
        SpeedUp = 0;
    }

    public void LeftMove(int scale = 1)
    {
        RotateDirection = -1 * scale;
    }

    public void RightMove(int scale = 1)
    {
        RotateDirection = 1 * scale;
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
        if (CurrentRotateAngle < 0) CurrentRotateAngle += 360;
        CurrentRotateAngle %= 360;
        // Fire and Freeze eff not rotate
        FireEffect.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        FreezeEffect.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        HealthBarSlider.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        ShieldBarSlider.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * fs.SlowedMoveSpdScale * ExteriorROTSpeed));
        
    }

    private void FighterForceRotate(float angle)
    {
        transform.Rotate(new Vector3(0, 0, -angle));
        CurrentRotateAngle += angle;
        if (CurrentRotateAngle < 0) CurrentRotateAngle += 360;
        CurrentRotateAngle %= 360;
        // Fire and Freeze eff not rotate
        FireEffect.transform.Rotate(new Vector3(0, 0, angle));
        FreezeEffect.transform.Rotate(new Vector3(0, 0, angle));
        HealthBarSlider.transform.Rotate(new Vector3(0, 0, angle));
        ShieldBarSlider.transform.Rotate(new Vector3(0, 0, angle));
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
        
        // Reach Limit
        if (transform.position.x >= (LimitRightX + 50))
        {
            LimitString = "Right";
        }
        else if (transform.position.x <= (LimitLeftX - 50))
        {
            LimitString = "Left";
        }
        else if (transform.position.y >= (LimitTopY + 50))
        {
            LimitString = "Top";
        }
        else if (transform.position.y <= (LimitBottomY - 50))
        {
            LimitString = "Bottom";
        }
    }

    private void PreventReachingLimit()
    {
        float LimitTopY = TopBorder.transform.position.y;
        float LimitBottomY = BottomBorder.transform.position.y;
        float LimitLeftX = LeftBorder.transform.position.x;
        float LimitRightX = RightBorder.transform.position.x;
        // Prevent Reach Limit
        int PreventX = 0;
        int PreventY = 0;
        float Distance = 0;
        if (transform.position.x >= (LimitRightX - 1200) && CurrentRotateAngle % 360 > 0 && CurrentRotateAngle % 360 < 180)
        {
            PreventX = -1;
            Distance = Mathf.Abs(LimitRightX - transform.position.x);
        }
        if (transform.position.x <= (LimitLeftX + 1200) && CurrentRotateAngle % 360 > 180 && CurrentRotateAngle % 360 < 360)
        {
            PreventX = 1;
            Distance = Mathf.Abs(LimitLeftX - transform.position.x);
        }
        if (transform.position.y >= (LimitTopY - 1200) && (CurrentRotateAngle % 360 > 270 || CurrentRotateAngle % 360 < 90))
        {
            PreventY = -1;
            if (Mathf.Abs(LimitTopY - transform.position.y) < Distance)
            {
                Distance = Mathf.Abs(LimitTopY - transform.position.y);
            }
        }
        if (transform.position.y <= (LimitBottomY + 1200) && CurrentRotateAngle % 360 > 90 && CurrentRotateAngle % 360 < 270)
        {
            PreventY = 1;
            if (Mathf.Abs(LimitTopY - transform.position.y) < Distance)
            {
                Distance = Mathf.Abs(LimitBottomY - transform.position.y);
            }
        }
        if (PreventX != 0 || PreventY != 0)
        {
            PreventReachLimitTimer = 180 / (RotateSpeed * 120f) + Random.Range(0, 0.25f);
            PreventReachLimit = true;
            if (CurrentSpeed * PreventReachLimitTimer > Distance)
            {
                if (CurrentSpeed / MovingSpeed < 0.5f)
                {
                    UpMove();
                }
                else if (CurrentSpeed / MovingSpeed >= 1f)
                {
                    DownMove();
                }
            } else
            {
                UpMove();
            }
            if (PreventY == 1)
            {
                if (PreventX == 1)
                {
                    LeftMove();
                }
                else if (PreventX == -1)
                {
                    RightMove();
                }
                else
                {
                    if (CurrentRotateAngle < 180)
                    {
                        RightMove();
                    }
                    else if (CurrentRotateAngle == 180)
                    {
                        int n = Random.Range(0, 2);
                        if (n == 0)
                        {
                            RightMove();
                        }
                        else
                        {
                            LeftMove();
                        }
                    }
                    else
                    {
                        LeftMove();
                    }
                }
            }
            else if (PreventY == -1)
            {
                if (PreventX == 1)
                {
                    RightMove();
                }
                else if (PreventX == -1)
                {
                    LeftMove();
                }
                else
                {
                    if (CurrentRotateAngle < 180)
                    {
                        RightMove();
                    }
                    else if (CurrentRotateAngle == 180)
                    {
                        int n = Random.Range(0, 2);
                        if (n == 0)
                        {
                            RightMove();
                        }
                        else
                        {
                            LeftMove();
                        }
                    }
                    else
                    {
                        LeftMove();
                    }
                }
            }
            else
            {
                if (PreventX == 1)
                {
                    if (CurrentRotateAngle > 270 || CurrentRotateAngle < 90)
                    {
                        RightMove();
                    }
                    else if (CurrentRotateAngle == 270 || CurrentRotateAngle == 90)
                    {
                        int n = Random.Range(0, 2);
                        if (n==0)
                        {
                            RightMove();
                        } else
                        {
                            LeftMove();
                        }
                    } else
                    {
                        LeftMove();
                    }
                }
                else if (PreventX == -1)
                {
                    if (CurrentRotateAngle > 270 || CurrentRotateAngle < 90)
                    {
                        LeftMove();
                    }
                    else if (CurrentRotateAngle == 270 || CurrentRotateAngle == 90)
                    {
                        int n = Random.Range(0, 2);
                        if (n == 0)
                        {
                            RightMove();
                        }
                        else
                        {
                            LeftMove();
                        }
                    }
                    else
                    {
                        RightMove();
                    }
                }
            }
        }
    }

    private void GetAwayFromLimit()
    {
        if (LimitString == "Left")
        {
            delayCheckLimit = 1f;
            FighterForceRotate((180- CurrentRotateAngle) * 2);
            LimitString = "";
            PreventReachLimitTimer = 0f;
            
        } 
        else if (LimitString == "Right")
        {
            delayCheckLimit = 1f;
            FighterForceRotate((0 - CurrentRotateAngle) * 2);
            LimitString = "";
            PreventReachLimitTimer = 0f;
        }
        else if (LimitString == "Top")
        {
            delayCheckLimit = 1f;
            FighterForceRotate((270 - CurrentRotateAngle) * 2);
            LimitString = "";
            PreventReachLimitTimer = 0f;
        }
        else if (LimitString == "Bottom")
        {
            delayCheckLimit = 1f;
            FighterForceRotate((90 - CurrentRotateAngle) * 2);
            LimitString = "";
            PreventReachLimitTimer = 0f;
        }
    }

    private void CheckOnMoving()
    {
        // Allies
        if (als != null)
        {
            // SSTP
            if (als.IsEscorting)
            {
                GameObject go = new();
                go.transform.position = als.EscortTargetPosition;
                int k = CheckIsUpOrDownMovement(go, HeadObject, gameObject, true);
                Destroy(go);
                UpMove();
                if (k == -1)
                {
                    LeftMove();
                }
                else if (k == 0)
                {
                    NoLeftRightMove();
                }
                else if (k == 1)
                {
                    RightMove();
                }
            }
            // Other allies
            else
            {
                // Escort Map
                if (als.Escort && als.EscortObject != null)
                {
                    if ((als.transform.position - als.EscortObject.transform.position).magnitude <= 500f)
                    {
                        NoEscorting = true;
                    }
                    else if ((als.transform.position - als.EscortObject.transform.position).magnitude >= 4000f)
                    {
                        NoEscorting = false;
                    }
                }
                // No Escort
                else if (!als.Escort || als.EscortObject == null)
                {
                    NoEscorting = true;
                }
                // Check based on cases
                if (NoEscorting)
                {
                    if (als.LeftTarget != null && als.RightTarget != null)
                    {
                        als.NearestTarget = null;
                        if ((als.LeftTarget.transform.position - transform.position).magnitude <= als.TargetRange)
                        {
                            if (!inAttackRange)
                            {
                                int k = CheckIsUpOrDownMovement(als.LeftTarget, HeadObject, gameObject);
                                if (k == -1)
                                {
                                    if (CurrentSpeed / MovingSpeed < 0.5f)
                                    {
                                        UpMove();
                                    }
                                    else if (CurrentSpeed / MovingSpeed >= 1f)
                                    {
                                        DownMove();
                                    }
                                    LeftMove();
                                }
                                else if (k == 0)
                                {
                                    UpMove();
                                    NoLeftRightMove();
                                }
                                else if (k == 1)
                                {
                                    if (CurrentSpeed / MovingSpeed < 0.5f)
                                    {
                                        UpMove();
                                    }
                                    else if (CurrentSpeed / MovingSpeed >= 1f)
                                    {
                                        DownMove();
                                    }
                                    RightMove();
                                }
                                InAttackRangeCount = 1;
                            }
                            else
                            {
                                int k = CheckIsUpOrDownMovement(als.LeftTarget, HeadObject, gameObject);
                                if (CurrentSpeed / MovingSpeed < 0.5f)
                                {
                                    UpMove();
                                }
                                else if (CurrentSpeed / MovingSpeed >= 1f)
                                {
                                    DownMove();
                                }
                                if (k == -1)
                                {
                                    LeftMove();
                                }
                                else if (k == 0)
                                {
                                    NoLeftRightMove();
                                }
                                else if (k == 1)
                                {
                                    RightMove();
                                }
                                InAttackRangeCount++;
                            }
                            inAttackRange = true;
                        }
                        else
                        {
                            if (inAttackRange)
                            {
                                UpMove();
                                int k = CheckIsUpOrDownMovement(als.LeftTarget, HeadObject, gameObject);
                                if (k == -1)
                                {
                                    LeftMove();
                                }
                                else if (k == 0)
                                {
                                    NoLeftRightMove();
                                }
                                else if (k == 1)
                                {
                                    RightMove();
                                }
                            }
                            else
                            {
                                UpMove();
                                int k = 0;
                                if (als.ForceTargetGO != null)
                                {
                                    k = CheckIsUpOrDownMovement(als.ForceTargetGO, HeadObject, gameObject);
                                }
                                else
                                {
                                    als.TargetNearestTarget();
                                    k = CheckIsUpOrDownMovement(als.NearestTarget, HeadObject, gameObject);
                                }
                                if (k == -1)
                                {
                                    LeftMove();
                                }
                                else if (k == 0)
                                {
                                    NoLeftRightMove();
                                }
                                else if (k == 1)
                                {
                                    RightMove();
                                }
                            }
                            inAttackRange = false;
                        }
                    }
                    else
                    {
                        int k = 0;
                        als.TargetNearestTarget();
                        k = CheckIsUpOrDownMovement(als.NearestTarget, HeadObject, gameObject);
                        UpMove();
                        if (k == -1)
                        {
                            LeftMove();
                        }
                        else if (k == 0)
                        {
                            NoLeftRightMove();
                        }
                        else if (k == 1)
                        {
                            RightMove();
                        }
                    }
                }
                else
                {
                    int k = CheckIsUpOrDownMovement(als.EscortObject, HeadObject, gameObject, true);
                    if (CurrentSpeed / MovingSpeed < 0.5f)
                    {
                        UpMove();
                    }
                    else if (CurrentSpeed / MovingSpeed >= 1f)
                    {
                        DownMove();
                    }
                    if (k == -1)
                    {
                        LeftMove();
                    }
                    else if (k == 0)
                    {
                        NoLeftRightMove();
                    }
                    else if (k == 1)
                    {
                        RightMove();
                    }
                }
            }
        } else if (es != null)
        {
            if (es.isBomb)
            {
                if (es.ForceTargetGO!=null)
                {
                    int k = CheckIsUpOrDownMovement(es.ForceTargetGO, HeadObject, gameObject, true);
                    UpMove();
                    if (k == -1)
                    {
                        LeftMove();
                    }
                    else if (k == 0)
                    {
                        NoLeftRightMove();
                    }
                    else if (k == 1)
                    {
                        RightMove();
                    }
                }
                
            } else
            {
                if (es.LeftTarget != null && es.RightTarget != null)
                {
                    es.NearestTarget = null;
                    if ((es.LeftTarget.transform.position - transform.position).magnitude <= es.TargetRange)
                    {
                        if (!inAttackRange)
                        {
                            int k = CheckIsUpOrDownMovement(es.LeftTarget, HeadObject, gameObject);
                            if (k == -1)
                            {
                                if (CurrentSpeed / MovingSpeed < 0.5f)
                                {
                                    UpMove();
                                }
                                else if (CurrentSpeed / MovingSpeed >=1f)
                                {
                                    DownMove();
                                }
                                LeftMove();
                            }
                            else if (k == 0)
                            {
                                UpMove();
                                NoLeftRightMove();
                            }
                            else if (k == 1)
                            {
                                if (CurrentSpeed / MovingSpeed < 0.5f)
                                {
                                    UpMove();
                                }
                                else if (CurrentSpeed / MovingSpeed >= 1f)
                                {
                                    DownMove();
                                }
                                RightMove();
                            }
                            InAttackRangeCount = 1;
                        }
                        else
                        {
                            int k = CheckIsUpOrDownMovement(es.LeftTarget, HeadObject, gameObject);
                            if (CurrentSpeed / MovingSpeed < 0.5f)
                            {
                                UpMove();
                            }
                            else if (CurrentSpeed / MovingSpeed >= 1f)
                            {
                                DownMove();
                            }
                            if (k == -1)
                            {
                                LeftMove();
                            }
                            else if (k == 0)
                            {
                                NoLeftRightMove();
                            }
                            else if (k == 1)
                            {
                                RightMove();
                            }
                            InAttackRangeCount++;
                        }
                        inAttackRange = true;
                    }
                    else
                    {
                        if (inAttackRange)
                        {
                            UpMove();
                            int k = CheckIsUpOrDownMovement(es.LeftTarget, HeadObject, gameObject);
                            if (k == -1)
                            {
                                LeftMove();
                            }
                            else if (k == 0)
                            {
                                NoLeftRightMove();
                            }
                            else if (k == 1)
                            {
                                RightMove();
                            }
                        }
                        else
                        {
                            UpMove();
                            int k = 0;
                            if (es.ForceTargetGO != null)
                            {
                                k = CheckIsUpOrDownMovement(es.ForceTargetGO, HeadObject, gameObject);
                            }
                            else
                            {
                                es.TargetNearestTarget();
                                k = CheckIsUpOrDownMovement(es.NearestTarget, HeadObject, gameObject);
                            }
                            if (k == -1)
                            {
                                LeftMove();
                            }
                            else if (k == 0)
                            {
                                NoLeftRightMove();
                            }
                            else if (k == 1)
                            {
                                RightMove();
                            }
                        }
                        inAttackRange = false;
                    }
                }
                else
                {
                    int k = 0;
                    es.TargetNearestTarget();
                    k = CheckIsUpOrDownMovement(es.NearestTarget, HeadObject, gameObject);
                    UpMove();
                    if (k == -1)
                    {
                        LeftMove();
                    }
                    else if (k == 0)
                    {
                        NoLeftRightMove();
                    }
                    else if (k == 1)
                    {
                        RightMove();
                    }
                }
            }
        }
    }

    private int CheckIsUpOrDownMovement(GameObject Aim, GameObject ShootingPosition, GameObject RotatePoint, bool LowAngle = false)
    {
        int DirMov = 0;
        Vector2 HeadToTarget = Aim.transform.position - ShootingPosition.transform.position;
        Vector2 MovingVector = ShootingPosition.transform.position - RotatePoint.transform.position;
        float angle = Vector2.Angle(HeadToTarget, MovingVector);
        float DistanceNew = Mathf.Cos(angle * Mathf.Deg2Rad) * HeadToTarget.magnitude;
        Vector2 TempPos = new Vector2(RotatePoint.transform.position.x, RotatePoint.transform.position.y) + MovingVector / MovingVector.magnitude * (MovingVector.magnitude + DistanceNew);
        Vector2 CheckPos = new Vector2(Aim.transform.position.x, Aim.transform.position.y) + (TempPos - new Vector2(Aim.transform.position.x, Aim.transform.position.y)) * 2;
        float compareAngle = LowAngle ? 5 : 45;
        if (ShootingPosition.transform.position.x == RotatePoint.transform.position.x)
        {
            if (ShootingPosition.transform.position.y > RotatePoint.transform.position.y)
            {
                if (angle < compareAngle)
                {
                    if (Aim.transform.position.y > ShootingPosition.transform.position.y)
                        DirMov = 0;
                    else
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                } else
                {
                    if (Aim.transform.position.x < ShootingPosition.transform.position.x)
                    {
                        DirMov = -1;
                    }
                    else if (Aim.transform.position.x == ShootingPosition.transform.position.x)
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                    else if (Aim.transform.position.x > ShootingPosition.transform.position.x)
                    {
                        DirMov = 1;
                    }
                }
            }
            else
            {
                if (angle < compareAngle)
                {
                    if (Aim.transform.position.y < ShootingPosition.transform.position.y)
                        DirMov = 0;
                    else
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                }
                else
                {
                    if (Aim.transform.position.x < ShootingPosition.transform.position.x)
                    {
                        DirMov = 1;
                    }
                    else if (Aim.transform.position.x == ShootingPosition.transform.position.x)
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                    else if(Aim.transform.position.x > ShootingPosition.transform.position.x)
                    {
                        DirMov = -1;
                    }
                }
            }
        }
        else if (ShootingPosition.transform.position.y == RotatePoint.transform.position.y)
        {
            if (ShootingPosition.transform.position.x > RotatePoint.transform.position.x)
            {
                if (angle < compareAngle)
                {
                    if (Aim.transform.position.x > ShootingPosition.transform.position.x)
                        DirMov = 0;
                    else
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                }
                else
                {
                    if (Aim.transform.position.y > ShootingPosition.transform.position.y)
                    {
                        DirMov = -1;
                    }
                    else if (Aim.transform.position.y == ShootingPosition.transform.position.y)
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                    else if (Aim.transform.position.y < ShootingPosition.transform.position.y)
                    {
                        DirMov = 1;
                    }
                }
            }
            else
            {
                if (angle < compareAngle)
                {
                    if (Aim.transform.position.x < ShootingPosition.transform.position.x)
                        DirMov = 0;
                    else
                    {
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
                    }
                }
                else
                {
                    if (Aim.transform.position.y > ShootingPosition.transform.position.y)
                    {
                        DirMov = 1;
                    }
                    else if (Aim.transform.position.y == ShootingPosition.transform.position.y)
                    {
                        DirMov = Random.Range(0,1f) < 0.5f ? -1 : 1;
                    }
                    else if (Aim.transform.position.y < ShootingPosition.transform.position.y)
                    {
                        DirMov = -1;
                    }
                }
            }
        }
        else if (ShootingPosition.transform.position.x > RotatePoint.transform.position.x)
        {
            if (angle < compareAngle)
            {
                DirMov = 0;
            } else
            {
                if (CheckPos.y < Aim.transform.position.y)
                {
                    DirMov = -1;
                }
                else
                {
                    DirMov = 1;
                }
            }
        }
        else
        {
            if (angle < compareAngle)
            {
                DirMov = 0;
            }
            else
            {
                if (CheckPos.y < Aim.transform.position.y)
                {
                    DirMov = 1;
                }
                else
                {
                    DirMov = -1;
                }
            }
        }
        return DirMov;
    }
    #endregion
    #region Check Hazard Environment
    private void CheckForHazard()
    {

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, (HeadObject.transform.position - transform.position).magnitude, HazardMask);
            if (cols.Length>0)
            {
                foreach (var col in cols)
                {
                    if (col.name.Contains("SR"))
                    {
                        if (als!=null)
                        {
                            als.ReceiveDamage(als.MaxHP * 10 /100, col.gameObject);
                            als.ReceiveForce(transform.position - col.transform.position, 3000f, 0.5f, "Asteroid");
                            if (col.GetComponent<SpaceZoneAsteroid>()!=null)
                            {
                                col.GetComponent<SpaceZoneAsteroid>().FighterHit(transform.position, MovingSpeed);
                            }
                        } else if (es!=null)
                        {
                            es.ReceiveDamage(es.MaxHP * 10 / 100, col.gameObject);
                            es.ReceiveForce(transform.position - col.transform.position, 3000f, 0.5f, "Asteroid");
                            if (col.GetComponent<SpaceZoneAsteroid>() != null)
                            {
                                col.GetComponent<SpaceZoneAsteroid>().FighterHit(transform.position, MovingSpeed);
                            }
                        }
                        CurrentSpeed = 0;
                    }
                    else if (col.name.Contains("RS"))
                    {
                        if (als != null && !als.alreadyHitByComet)
                        {
                            als.HitByCometDelay = 5f;
                            als.alreadyHitByComet = true;
                            als.ReceiveDamage(als.MaxHP * 50 / 100, col.gameObject);
                            als.ReceiveForce(transform.position - col.transform.position, 10000f, 1f, "Rogue Star");
                        }
                        else if (es != null && !es.alreadyHitByComet)
                        {
                            es.HitByCometDelay = 5f;
                            es.alreadyHitByComet = true;
                            es.ReceiveDamage(es.MaxHP * 50 / 100, col.gameObject);
                            es.ReceiveForce(transform.position - col.transform.position, 10000f, 1f, "Rogue Star");
                        }
                        CurrentSpeed = 0;
                    }
                }
            }
    }
    #endregion
}
