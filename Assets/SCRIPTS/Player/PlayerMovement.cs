using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region ComponentVariables
    private Rigidbody2D rb;
    private PlayerFighter pf;
    #endregion
    #region InitializeVariables
    public float RotateSpeed;
    public float MovingSpeed;
    public float DashingTime;
    public float DashSpeedRate;
    public GameObject LeftWeapon;
    public GameObject RightWeapon;
    public GameObject backFire;
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public Slider AESlider;
    public GameObject AEText;
    public GameObject FireEffect;
    public GameObject FreezeEffect;
    public GameObject HeadObject;
    public LayerMask HazardMask;
    public GameplayInteriorController ControllerMain;
    public SpaceZoneHazardEnvironment HazEnv;
    public GameObject Indicator;
    #endregion
    #region NormalVariables
    public GameObject PlayerIcon;
    public float CurrentRotateAngle;
    private string CurrentKeyRotate;
    private string CurrentKeyMove;
    public int RotateDirection;
    public float CurrentSpeed;
    private int SpeedUp;
    private bool Dashing;
    private float DashingTimer;
    private bool Movable;
    private float BackFireInitScale;
    private Vector2 speedVector;
    private float LimitSpeedScale;
    private float AEEnergy;
    public float ExteriorROTSpeed;
    public float LaserBeamSlowScale;
    public float AEIncreaseScale;
    public LOTWEffect LOTWEffect;
    public float AccelEngineSpeedUpTimer;
    public float AccelEngineSpeedUpScale;
    public float AccelEngineRoTUpScale;
    private bool PressDash;
    private float DelayAERechargeTimer;
    public bool autoPilot;
    private float DelayCheckLimit;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        rb = GetComponent<Rigidbody2D>();
        pf = GetComponent<PlayerFighter>();
        Movable = true;
        BackFireInitScale = backFire.transform.localScale.x;
        AESlider.maxValue = 100f;
        ExteriorROTSpeed = 1;
        AEEnergy = 100f;
        LaserBeamSlowScale = 1;
        AEIncreaseScale = 1f;
        AccelEngineSpeedUpScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Indicator.transform.position = transform.position + new Vector3(-1.8f, 79.5f, 0f);
        if (DelayAERechargeTimer <= 0f)
        {
            if (AEEnergy < 100f)
            {
                AEEnergy += 5f * Time.deltaTime * AEIncreaseScale;
            }
            else
            {
                AEEnergy = 100f;
            }
        } else
        {
            DelayAERechargeTimer -= Time.deltaTime;
        }
        
        if (pf.isFrozen || pf.isSFBFreeze || ControllerMain.IsInLoading)
        {
            Movable = false;
            speedVector = new Vector2(0, 0);
            CurrentSpeed = 0f;
        } else
        {
            Movable = true;
        }
        // Rotate Left Right
        if (!autoPilot) DetectADButton();
        // Dash using Spacebar
        // Timer for Dashing
        if (DashingTimer>0f)
        {
            DashingTimer -= Time.deltaTime;
        }
        else
        {
            Dashing = false;
            if (PressDash)
            {
                PressDash = false;
                AccelEngineSpeedUpTimer = 5f;
                AccelEngineSpeedUpScale = 1.5f;
                AccelEngineRoTUpScale = 2f;
            }
        }
        // Dashing conditions
        if (Input.GetKeyDown(KeyCode.Space) && DashingTimer <= 0f && Movable && CurrentSpeed >= MovingSpeed && AEEnergy>=100f)
        {
            DashingTimer = DashingTime;
            DelayAERechargeTimer = 10f + DashingTime;
            Dashing = true;
            PressDash = true;
            AEEnergy = 0f;
            if (!transform.GetChild(9).gameObject.activeSelf)
            {
                transform.GetChild(9).gameObject.SetActive(true);
            }
            Dash();
        }
        // Moving Front and Back
        DetectWSButton();
        if (!Dashing && Movable) PlayerMoving();
        pf.CalculateVelocity(speedVector);
        if (!autoPilot) CheckLimit();
        else CheckLimitDone();
        ShowAE();
        if (HazEnv.HazardID == 2 || HazEnv.HazardID == 5 || HazEnv.HazardID == 6)
        {
            CheckForHazard();
        }
        if (AccelEngineSpeedUpTimer > 0f)
        {
            AccelEngineSpeedUpTimer -= Time.deltaTime;
            if (!transform.GetChild(9).gameObject.activeSelf)
            {
                transform.GetChild(9).gameObject.SetActive(true);
            }
        } else
        {
            AccelEngineSpeedUpScale = 1f;
            AccelEngineRoTUpScale = 1f;
            if (transform.GetChild(9).gameObject.activeSelf && !Dashing)
            {
                transform.GetChild(9).gameObject.SetActive(false);
            }
        }
    }
    private void FixedUpdate()
    {
        if (!Dashing && Movable) PlayerRotate();
    }
    #endregion
    #region Moving Functions
    // Detect Player's Input Cases on A and D
    void DetectADButton()
    {
        // Case: Hold A and no touch D => A
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            CurrentKeyRotate = "A";
            RotateDirection = -1;
        }
        // Case: Hold D and no touch A => D
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            CurrentKeyRotate = "D";
            RotateDirection = 1;
        }
        // Case: Hold A and press D after => D
        else if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && CurrentKeyRotate.Equals("A"))
        {
            CurrentKeyRotate = "D";
            RotateDirection = 1;
        }
        // Case: Hold D and press A after => A
        else if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.A) && CurrentKeyRotate.Equals("D"))
        {
            CurrentKeyRotate = "A";
            RotateDirection = -1;
        }
        // Case: Hold A and D but A is pressed after => A
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) && CurrentKeyRotate.Equals("A"))
        {
            CurrentKeyRotate = "A";
            RotateDirection = -1;
        }
        // Case: Hold A and D but D is pressed after => D
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) && CurrentKeyRotate.Equals("D"))
        {
            CurrentKeyRotate = "D";
            RotateDirection = 1;
        }
        // Case: Not pressing or holding any key
        else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            RotateDirection = 0;
        }
    }
    // Rotate Player and Icon on Minimap With Degree
    void PlayerRotate()
    {
        float RotateScale = 2;
        transform.Rotate(new Vector3(0,0, -RotateScale * RotateDirection * RotateSpeed * pf.SlowedMoveSpdScale * ExteriorROTSpeed * AccelEngineRoTUpScale));
        CurrentRotateAngle += RotateScale * RotateDirection * RotateSpeed * pf.SlowedMoveSpdScale * ExteriorROTSpeed * AccelEngineRoTUpScale;
        if (CurrentRotateAngle < 0)
        {
            CurrentRotateAngle += 360;
        }
        CurrentRotateAngle %= 360;
        if (PlayerIcon!=null)
        {
            PlayerIcon.transform.Rotate(new Vector3(0, 0, -RotateScale * RotateDirection * RotateSpeed * pf.SlowedMoveSpdScale * ExteriorROTSpeed * AccelEngineRoTUpScale));
        }
        // Fire and Freeze eff not rotate
        FireEffect.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * pf.SlowedMoveSpdScale * ExteriorROTSpeed * AccelEngineRoTUpScale));
        FreezeEffect.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * pf.SlowedMoveSpdScale * ExteriorROTSpeed * AccelEngineRoTUpScale));
        Indicator.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed * pf.SlowedMoveSpdScale * ExteriorROTSpeed * AccelEngineRoTUpScale));
    }
    // Detect Player's Input Cases for W and S
    void DetectWSButton()
    {
        // Case hold W and no touch S => W
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            SpeedUp = 1;
            CurrentKeyMove = "W";
        }
        // Case hold S and no touch W => S
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            SpeedUp = -1;
            CurrentKeyMove = "S";
        }
        // Case hold S and press W => W
        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.W) && CurrentKeyMove.Equals("S"))
        {
            SpeedUp = 1;
            CurrentKeyMove = "W";
        }
        // Case hold W and press S => S
        else if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.S) && CurrentKeyMove.Equals("W"))
        {
            SpeedUp = -1;
            CurrentKeyMove = "S";
        }
        // Case hold S and W but press W after => W
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) && CurrentKeyMove.Equals("W"))
        {
            SpeedUp = 1;
            CurrentKeyMove = "W";
        }
        // Case hold S and W but press S after => S
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) && CurrentKeyMove.Equals("S"))
        {
            SpeedUp = -1;
            CurrentKeyMove = "S";
        }
        // Case not pressing or holding any key
        else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            SpeedUp = 0;
        }
    }
    // Move Player Forward Backward
    void PlayerMoving()
    {
        Vector2 movementVector = CalculateMovement();
        if (CurrentSpeed>0)
        {
            GetComponent<PlayerFighter>().PlayMovingSound(CurrentSpeed * pf.SlowedMoveSpdScale / MovingSpeed);
            if (!backFire.activeSelf)
            {
                backFire.SetActive(true);
            }
            backFire.transform.localScale =
                new Vector3(CurrentSpeed * pf.SlowedMoveSpdScale / MovingSpeed * BackFireInitScale,
                CurrentSpeed * pf.SlowedMoveSpdScale / MovingSpeed * BackFireInitScale,
                backFire.transform.localScale.z);
        } else
        {
            GetComponent<PlayerFighter>().StopSound();
            backFire.SetActive(false);
        }
        AccelerateSpeed();
        speedVector = movementVector * CurrentSpeed * pf.SlowedMoveSpdScale * LimitSpeedScale * LaserBeamSlowScale * AccelEngineSpeedUpScale;
    }
    // Accelerate Players
    void AccelerateSpeed()
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
    Vector2 CalculateMovement()
    {
        float x;
        float y;
        float z = transform.eulerAngles.z;
        float degree;
        if (z == 180) return new Vector2(0, -1);
        else if (z == 0 || z==360) return new Vector2(0, 1);
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
    // Dash Player
    void Dash()
    {
        Vector2 movementVector = CalculateMovement();
        GetComponent<PlayerFighter>().PlayDashSound();
        speedVector = movementVector * MovingSpeed * DashSpeedRate * LimitSpeedScale;
    }
    #endregion
    #region Check Limit
    private void CheckLimit()
    {
        float LimitTopY = TopBorder.transform.position.y;
        float LimitBottomY = BottomBorder.transform.position.y;
        float LimitLeftX = LeftBorder.transform.position.x;
        float LimitRightX = RightBorder.transform.position.x;
        LimitSpeedScale = 1;
        int check = 0;
        bool AorD = false;
        if (transform.position.x >= LimitRightX && CurrentRotateAngle > 0 && CurrentRotateAngle < 180)
        {
            check++;
            autoPilot = true;
            if (CurrentRotateAngle > 90)
            {
                AorD = false;
            } else
            {
                AorD = true;
            }
        }
        if (transform.position.x <= LimitLeftX && CurrentRotateAngle > 180 && CurrentRotateAngle < 360)
        {
            check++;
            autoPilot = true;
            if (CurrentRotateAngle > 270)
            {
                AorD = false;
            }
            else
            {
                AorD = true;
            }
        }
        if (transform.position.y >= LimitTopY && (CurrentRotateAngle > 270 || CurrentRotateAngle < 90))
        {
            check++;
            autoPilot = true;
            if (CurrentRotateAngle > 0 && CurrentRotateAngle < 90)
            {
                AorD = false;
            }
            else if (CurrentRotateAngle < 360 && CurrentRotateAngle > 270)
            {
                AorD = true;
            }
        }
        if (transform.position.y <= LimitBottomY && CurrentRotateAngle > 90 && CurrentRotateAngle < 270)
        {
            check++;
            autoPilot = true;
            if (CurrentRotateAngle > 180)
            {
                AorD = false;
            }
            else
            {
                AorD = true;
            }
        }
        if (check >= 1)
        {
            AutoPilot(AorD, check);
        } else
        {
            autoPilot = false;
        }
    }

    private void CheckLimitDone()
    {
        float LimitTopY = TopBorder.transform.position.y;
        float LimitBottomY = BottomBorder.transform.position.y;
        float LimitLeftX = LeftBorder.transform.position.x;
        float LimitRightX = RightBorder.transform.position.x;
        LimitSpeedScale = 1;
        int check = 0;
        if (transform.position.x >= LimitRightX)
        {
            check++;
            if (transform.position.x - (180 / (120 * RotateSpeed)) * CurrentSpeed >= LimitRightX)
            {
                if (CurrentRotateAngle > 260 && CurrentRotateAngle < 280)
                RotateDirection = 0;
                else if (CurrentRotateAngle > 350 || CurrentRotateAngle < 10)
                    RotateDirection = -1;
                else if (CurrentRotateAngle > 170 && CurrentRotateAngle < 190)
                {
                    RotateDirection = 1;
                }
            }
        }
        if (transform.position.x <= LimitLeftX)
        {
            check++; 
            if (transform.position.x - (180 / (120 * RotateSpeed)) * CurrentSpeed <= LimitLeftX)
            {
                if (CurrentRotateAngle > 80 && CurrentRotateAngle < 100)
                RotateDirection = 0;
                else if (CurrentRotateAngle > 350 || CurrentRotateAngle < 10)
                    RotateDirection = 1;
                else if (CurrentRotateAngle > 170 && CurrentRotateAngle < 190)
                {
                    RotateDirection = -1;
                }
            }
        }
        if (transform.position.y >= LimitTopY)
        {
            check++;
            if (transform.position.y - (180 / (120 * RotateSpeed)) * CurrentSpeed >= LimitTopY )
            {
                if (CurrentRotateAngle > 170 && CurrentRotateAngle < 190)
                RotateDirection = 0;
                else if (CurrentRotateAngle > 80 && CurrentRotateAngle < 100)
                    RotateDirection = 1;
                else if (CurrentRotateAngle > 260 && CurrentRotateAngle < 280)
                {
                    RotateDirection = -1;
                }
            }
        }
        if (transform.position.y <= LimitBottomY)
        {
            check++;
            if (transform.position.y - (180 / (120 * RotateSpeed)) * CurrentSpeed <= LimitBottomY)
            {
                if ((CurrentRotateAngle > 350 || CurrentRotateAngle < 10))
                RotateDirection = 0;
                else if (CurrentRotateAngle > 80 && CurrentRotateAngle < 100)
                    RotateDirection = -1;
                else if (CurrentRotateAngle > 260 && CurrentRotateAngle < 280)
                {
                    RotateDirection = 1;
                }
            }
        }
        if (check == 0)
        {
            autoPilot = false;
        }
    }

    private void AutoPilot(bool AorD, int check)
    {
        autoPilot = true;
        CurrentSpeed = MovingSpeed;
        if (AorD)
        {
            RotateDirection = -1;
        } else
        {
            RotateDirection = 1;
        }
    }

    IEnumerator TeleportBack(Vector2 Position)
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(Position.x, Position.y, transform.position.z);
        CurrentSpeed = 0f;
    }
    #endregion
    #region Show AE Energy
    private void ShowAE()
    {
        AESlider.value = AEEnergy;
        AEText.GetComponent<TextMeshPro>().text = (int)AEEnergy + "%";
    }
    #endregion
    #region Check Hazard
    private void CheckForHazard()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, (HeadObject.transform.position - transform.position).magnitude, HazardMask);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                if (col.name.Contains("SR"))
                {
                    if (pf != null)
                    {
                        pf.ReceiveDamage(pf.MaxHP * 10/100, col.gameObject);
                        pf.ReceiveForce(transform.position - col.transform.position, 3000f, 0.5f, "Asteroid");
                        if (col.GetComponent<SpaceZoneAsteroid>() != null)
                        {
                            col.GetComponent<SpaceZoneAsteroid>().FighterHit(transform.position, MovingSpeed);
                        }
                    }
                    CurrentSpeed = 0;
                } else if (col.name.Contains("RS"))
                {
                    if (pf != null && !pf.alreadyHitByComet)
                    {
                        pf.HitByCometDelay = 5f;
                        pf.alreadyHitByComet = true;
                        pf.ReceiveDamage(pf.MaxHP * 50 / 100, col.gameObject);
                        pf.ReceiveForce(transform.position - col.transform.position, 10000f, 0.5f, "Rogue Star");
                    }
                    CurrentSpeed = 0;
                }
            }
        }
    }
    #endregion
}
