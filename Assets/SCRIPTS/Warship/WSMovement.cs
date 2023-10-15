using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMovement : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private AudioSource aus;
    private WSShared wss;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public GameObject HeadObject;
    public GameObject HPSlider;
    public GameplayInteriorController ControllerMain;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float RotateSpeed;
    public float CurrentRotateAngle;
    public float CurrentSpeed;
    public float MovingSpeed;
    public Vector2 speedVector;
    public int SpeedUp;
    public int RotateDirection;
    public float LimitSpeedScale;
    public List<GameObject> BackFires;
    public List<Vector2> BackFireInitScale;
    public List<Vector2> BackFireInitPos;
    public AudioClip Sound;
    private List<Vector3> DistanceList;
    private string LimitString;
    private float ChosenRandom;
    private float LimitDelay;
    private float PreventReachLimitTimer;
    private bool PreventReachLimit;
    private bool inAttackRange;
    private int InAttackRangeCount;
    private float CheckMovingDelay;
    private bool Moveable;
    private float WaitTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        CurrentRotateAngle = 0;
        if (BackFires.Count > 0)
        {
            for (int i = 0; i < BackFires.Count; i++)
            {
                BackFireInitScale.Add(BackFires[i].transform.localScale);
            }
        }
        aus = GetComponent<AudioSource>();
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 2000;
        aus.minDistance = 1000;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        DistanceList = new List<Vector3>();
        for (int i = 0; i < BackFires.Count; i++)
        {
            BackFireInitPos.Add(BackFires[i].transform.localPosition);
            DistanceList.Add(-gameObject.transform.position + BackFires[i].transform.position);
        }

        wss = GetComponent<WSShared>();
        LimitString = "";
        WaitTimer = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if ((ControllerMain.IsInLoading))
        {
            Moveable = false;
            speedVector = new Vector2(0, 0);
            CurrentSpeed = 0f;
        }
        else
        {
            Moveable = true;
        }
        AccelerateSpeed();
        if (Moveable) FighterMoving();
        GetComponent<Rigidbody2D>().velocity = speedVector;
        CheckLimit();
        if (PreventReachLimitTimer <= 0f)
        {
            if (PreventReachLimit)
            {
                PreventReachLimit = false;
                NoLeftRightMove();
            }
        }
        else
        {
            PreventReachLimit = true;
            PreventReachLimitTimer -= Time.deltaTime;
        }
        if (Moveable)
        {
            WaitTimer -= Time.deltaTime;
        }
        if (WaitTimer <= 0f)
        {
            if (LimitString == "")
            {
                if (LimitDelay <= 0f)
                {
                    if (!PreventReachLimit)
                    {
                        PreventReachingLimit();
                        if (CheckMovingDelay <= 0f)
                        {
                            CheckMovingDelay = Random.Range(0.5f, 1f);
                            CheckOnMoving();
                        }
                        else
                        {
                            CheckMovingDelay -= Time.deltaTime;
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
        }
    }
    private void FixedUpdate()
    {
        FighterRotate();
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
        transform.Rotate(new Vector3(0, 0, -RotateScale * RotateDirection * RotateSpeed));
        CurrentRotateAngle += RotateScale * RotateDirection * RotateSpeed;
        HPSlider.transform.Rotate(new Vector3(0, 0, RotateScale * RotateDirection * RotateSpeed));


    }
    private void FighterMoving()
    {
        Vector2 movementVector = CalculateMovement();

        for (int i = 0; i < BackFires.Count; i++)
        {
            if (CurrentSpeed > 0)
            {
                PlayMovingSound(CurrentSpeed * 1 / MovingSpeed);
                if (!BackFires[i].activeSelf)
                {
                    BackFires[i].SetActive(true);
                }
                BackFires[i].transform.localScale =
                    new Vector3(CurrentSpeed * BackFireInitScale[i].x,
                    CurrentSpeed * BackFireInitScale[i].y,
                    BackFires[i].transform.localScale.z);
                Vector3 distance = (BackFires[i].transform.position - transform.position);
                distance = distance / distance.magnitude * DistanceList[i].magnitude;
                BackFires[i].transform.position = transform.position + distance * (0.8f + 0.2f * (CurrentSpeed) / MovingSpeed);

            }
            else
            {
                StopSound();
                /*BackFires[i].transform.position = BackFireInitPos[i];*/
                BackFires[i].SetActive(false);
            }
        }
       
        AccelerateSpeed();
        speedVector = movementVector * CurrentSpeed * LimitSpeedScale;
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
        float LimitTopY = TopBorder.transform.position.y - 450;
        float LimitBottomY = BottomBorder.transform.position.y + 450;
        float LimitLeftX = LeftBorder.transform.position.x + 450;
        float LimitRightX = RightBorder.transform.position.x - 450;
        LimitSpeedScale = 1f;
        if (transform.position.x >= (LimitRightX + 50))
        {
            StartCoroutine(TeleportBack(new Vector2(LimitRightX, transform.position.y)));
            NoUpDownMove();
            LimitString = "Right";
        }
        else if (transform.position.x <= (LimitLeftX - 50))
        {
            StartCoroutine(TeleportBack(new Vector2(LimitLeftX, transform.position.y)));
            NoUpDownMove();
            LimitString = "Left";
        }
        else if (transform.position.y >= (LimitTopY + 50))
        {
            StartCoroutine(TeleportBack(new Vector2(transform.position.x, LimitTopY)));
            NoUpDownMove();
            LimitString = "Top";
        }
        else if (transform.position.y <= (LimitBottomY - 50))
        {
            StartCoroutine(TeleportBack(new Vector2(transform.position.x, LimitBottomY)));
            NoUpDownMove();
            LimitString = "Bottom";
        }
    }

    private IEnumerator TeleportBack(Vector2 Position)
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(Position.x, Position.y, transform.position.z);
        CurrentSpeed = 0f;
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
        if (transform.position.x >= (LimitRightX - MovingSpeed * 2))
        {
            PreventX = -1;
        }
        if (transform.position.x <= (LimitLeftX + MovingSpeed * 2))
        {
            PreventX = 1;
        }
        if (transform.position.y >= (LimitTopY - MovingSpeed * 2))
        {
            PreventY = -1;
        }
        if (transform.position.y <= (LimitBottomY + MovingSpeed * 2))
        {
            PreventY = 1;
        }
        if (PreventX != 0 || PreventY != 0)
        {
            PreventReachLimitTimer = Random.Range(2f, 3f);
            PreventReachLimit = true;
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
        if (ChosenRandom == -200f)
        {
            ChosenRandom = Random.Range(-100f, 100f);
        }
        if (LimitString == "Left")
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(transform.position.x + 1000f, transform.position.y, transform.position.z);
            int k = CheckIsUpOrDownMovement(go, HeadObject, gameObject);
            Destroy(go);
            if (k == 1)
            {
                RightMove();
            }
            else if (k == -1)
            {
                LeftMove();
            }
            else if (k == 0)
            {
                UpMove();
                NoLeftRightMove();
                LimitString = "";
                ChosenRandom = -200f;
                LimitDelay = 3f;
            }
        }
        else if (LimitString == "Right")
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(transform.position.x - 1000f, transform.position.y, transform.position.z);
            int k = CheckIsUpOrDownMovement(go, HeadObject, gameObject);
            Destroy(go);
            if (k == 1)
            {
                RightMove();
            }
            else if (k == -1)
            {
                LeftMove();
            }
            else if (k == 0)
            {
                UpMove();
                NoLeftRightMove();
                LimitString = "";
                ChosenRandom = -200f;
                LimitDelay = 3f;
            }
        }
        else if (LimitString == "Top")
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(transform.position.x, transform.position.y - 1000f, transform.position.z);
            int k = CheckIsUpOrDownMovement(go, HeadObject, gameObject);
            Destroy(go);
            if (k == 1)
            {
                RightMove();
            }
            else if (k == -1)
            {
                LeftMove();
            }
            else if (k == 0)
            {
                UpMove();
                NoLeftRightMove();
                LimitString = "";
                ChosenRandom = -200f;
                LimitDelay = 3f;
            }
        }
        else if (LimitString == "Bottom")
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(transform.position.x, transform.position.y + 1000f, transform.position.z);
            int k = CheckIsUpOrDownMovement(go, HeadObject, gameObject);
            Destroy(go);
            if (k == 1)
            {
                RightMove();
            }
            else if (k == -1)
            {
                LeftMove();
            }
            else if (k == 0)
            {
                UpMove();
                NoLeftRightMove();
                LimitString = "";
                ChosenRandom = -200f;
                LimitDelay = 3f;
            }
        }
    }

    private void CheckOnMoving()
    {
        if (wss != null)
        {
            if (wss.MainTarget[wss.MainWps[0]] != null)
            {
                Debug.Log("khang" + wss.MainTarget[wss.MainWps[0]]);
                if ((wss.MainTarget[wss.MainWps[0]].transform.position - transform.position).magnitude <= wss.TargetRange / 2)
                {
                    if (!inAttackRange)
                    {
                        int k = CheckIsUpOrDownMovement(wss.MainTarget[wss.MainWps[0]], HeadObject, gameObject);
                        if (k == -1)
                        {
                            DownMove();
                            LeftMove();
                        }
                        else if (k == 0)
                        {
                            UpMove();
                            NoLeftRightMove();
                        }
                        else if (k == 1)
                        {
                            DownMove();
                            RightMove();
                        }
                        InAttackRangeCount = 1;
                    }
                    else
                    {
                        int test = InAttackRangeCount / 4;
                        if (test > 5)
                        {
                            foreach (var weapon in wss.MainWps)
                            {
                                wss.MainTarget[wss.MainWps[0]] = wss.MainWeaponTargetEnemy(weapon);
                            }                           
                            return;
                        }
                        int k = CheckIsUpOrDownMovement(wss.MainTarget[wss.MainWps[0]], HeadObject, gameObject);
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
                        int k = CheckIsUpOrDownMovement(wss.MainTarget[wss.MainWps[0]], HeadObject, gameObject);
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
                        int k = CheckIsUpOrDownMovement(wss.MainTarget[wss.MainWps[0]], HeadObject, gameObject);
                        if (k == -1)
                        {
                            DownMove();
                            LeftMove();
                        }
                        else if (k == 0)
                        {
                            UpMove();
                            NoLeftRightMove();
                        }
                        else if (k == 1)
                        {
                            DownMove();
                            RightMove();
                        }
                    }
                    inAttackRange = false;
                }
            }
            else
            {
                NoLeftRightMove();
                UpMove();
            }
        }       
    }

    private int CheckIsUpOrDownMovement(GameObject Aim, GameObject ShootingPosition, GameObject RotatePoint)
    {
        int DirMov = 0;
        Vector2 HeadToTarget = Aim.transform.position - ShootingPosition.transform.position;
        Vector2 MovingVector = ShootingPosition.transform.position - RotatePoint.transform.position;
        float angle = Vector2.Angle(HeadToTarget, MovingVector);
        float DistanceNew = Mathf.Cos(angle * Mathf.Deg2Rad) * HeadToTarget.magnitude;
        Vector2 TempPos = new Vector2(RotatePoint.transform.position.x, RotatePoint.transform.position.y) + MovingVector / MovingVector.magnitude * (MovingVector.magnitude + DistanceNew);
        Vector2 CheckPos = new Vector2(Aim.transform.position.x, Aim.transform.position.y) + (TempPos - new Vector2(Aim.transform.position.x, Aim.transform.position.y)) * 2;
        float compareAngle = 30;
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
                }
                else
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
                    else if (Aim.transform.position.x > ShootingPosition.transform.position.x)
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
                        DirMov = Random.Range(0, 1f) < 0.5f ? -1 : 1;
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
            }
            else
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
    #region
    public void PlayMovingSound(float volume)
    {
        if (aus.clip != Sound)
        {
            aus.clip = Sound;
            aus.loop = true;
            aus.Play();
        }
        aus.volume = volume;
    }

    public void StopSound()
    {
        aus.clip = null;
    }
    #endregion
}
