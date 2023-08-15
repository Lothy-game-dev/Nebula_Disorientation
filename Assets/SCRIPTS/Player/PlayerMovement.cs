using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region ComponentVariables
    private Rigidbody2D rb;
    #endregion
    #region InitializeVariables
    public float RotateSpeed;
    public float MovingSpeed;
    public float DashingTime;
    #endregion
    #region NormalVariables
    private string CurrentKeyRotate;
    private string CurrentKeyMove;
    private int RotateDirection;
    private float CurrentSpeed;
    private int SpeedUp;
    private bool Dashing;
    private float DashingTimer;
    private bool Rotatable;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate Left Right
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
        if (Rotatable) PlayerRotate();
        // Dash using Spacebar
        if (DashingTimer>0f)
        {
            DashingTimer -= Time.deltaTime;
        }
        else
        {
            Dashing = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && DashingTimer<=0f)
        {
            Dashing = true;
            DashingTimer = DashingTime;
            Dash();
        }
        if (Dashing)
        {
            Rotatable = false;
        } else
        {
            Rotatable = true;
        }
        // Moving Front and Back
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
        if (!Dashing) PlayerMoving();
    }
    #endregion
    #region Moving Functions
    // Rotate Player With Degree
    void PlayerRotate()
    {
        transform.Rotate(new Vector3(0,0, -RotateDirection * RotateSpeed));
    }
    // Move Player Forward Backward
    void PlayerMoving()
    {
        Vector2 movementVector = CalculateMovement();
        AccelerateSpeed();
        rb.velocity = movementVector * CurrentSpeed;
    }
    // Accelerate Players
    void AccelerateSpeed()
    {
        if (CurrentSpeed < MovingSpeed && SpeedUp == 1)
        {
            CurrentSpeed += MovingSpeed / 300;
        }
        else if (CurrentSpeed > MovingSpeed / 300 && SpeedUp == -1)
        {
            CurrentSpeed -= MovingSpeed / 300;
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
    void Dash()
    {
        Vector2 movementVector = CalculateMovement();
        rb.velocity = movementVector * MovingSpeed * 2f;
    }
    #endregion
}
