using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMovement : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private AudioSource aus;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject LeftBorder;
    public GameObject RightBorder;
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
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        AccelerateSpeed();
        FighterMoving();
        GetComponent<Rigidbody2D>().velocity = speedVector;
        CheckLimit();
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
        float LimitTopY = TopBorder.transform.position.y - 100;
        float LimitBottomY = BottomBorder.transform.position.y + 100;
        float LimitLeftX = LeftBorder.transform.position.x + 100;
        float LimitRightX = RightBorder.transform.position.x - 100;
        LimitSpeedScale = 1f;
        if (transform.position.x >= (LimitRightX + 50))
        {
            StartCoroutine(TeleportBack(new Vector2(LimitRightX, transform.position.y)));
        }
        else if (transform.position.x <= (LimitLeftX - 50))
        {
            StartCoroutine(TeleportBack(new Vector2(LimitLeftX, transform.position.y)));
        }
        else if (transform.position.y >= (LimitTopY + 50))
        {
            StartCoroutine(TeleportBack(new Vector2(transform.position.x, LimitTopY)));
        }
        else if (transform.position.y <= (LimitBottomY - 50))
        {
            StartCoroutine(TeleportBack(new Vector2(transform.position.x, LimitBottomY)));
        }
    }

    private IEnumerator TeleportBack(Vector2 Position)
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(Position.x, Position.y, transform.position.z);
        CurrentSpeed = 0f;
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
