using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region ComponentVariables
    private Camera cam;
    #endregion
    #region InitializeVariables
    // Boundaries
    public GameObject TopBoundary;
    public GameObject BottomBoundary;
    public GameObject LeftBoundary;
    public GameObject RightBoundary;
    public float ZoomOutHeight;
    public float CloseHeight;
    public GameObject GameController;
    #endregion
    #region NormalVariables
    public GameObject FollowObject;
    public bool CameraTracking;
    private bool isClose;
    private float CameraHeight;
    private float CameraWidth;
    private float TopYPosition;
    private float BottomYPosition;
    private float LeftXPosition;
    private float RightXPosition;
    private float PosX;
    private float PosY;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        cam = GetComponent<Camera>();
        isClose = false;
        CameraTracking = true;
        cam.orthographicSize = ZoomOutHeight / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        // Change to close or zoom-out when press left shift
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isClose)
            {
                // 975x675
                cam.orthographicSize = CloseHeight / 2;
                isClose = true;
                GameController.GetComponent<GameController>().IsClose = true;
            } else
            {
                // 1950x1350
                cam.orthographicSize = ZoomOutHeight / 2;
                isClose = false;
                GameController.GetComponent<GameController>().IsClose = false;
            }
        }
        // Calculate Height And Width
        CameraHeight = 2 * cam.orthographicSize;
        CameraWidth = CameraHeight * cam.aspect;
        SetCameraPosition();
    }
    #endregion
    #region Camera Position
    private void SetCameraPosition()
    {
        // Follow Player And Limit In Boundary
        // Calculate Top Bottom Left Right position of the Camera
        TopYPosition = FollowObject.transform.position.y + CameraHeight / 2;
        BottomYPosition = FollowObject.transform.position.y - CameraHeight / 2;
        LeftXPosition = FollowObject.transform.position.x - CameraWidth / 2;
        RightXPosition = FollowObject.transform.position.x + CameraWidth / 2;
        // If those aforementioned position run off the boundary, set it back to the boundary
        if (TopYPosition > TopBoundary.transform.position.y)
        {
            PosY = TopBoundary.transform.position.y - CameraHeight / 2;
        }
        else if (BottomYPosition < BottomBoundary.transform.position.y)
        {
            PosY = BottomBoundary.transform.position.y + CameraHeight / 2;
        }
        else
        {
            PosY = FollowObject.transform.position.y;
        }
        if (LeftXPosition < LeftBoundary.transform.position.x)
        {
            PosX = LeftBoundary.transform.position.x + CameraWidth / 2;
        }
        else if (RightXPosition > RightBoundary.transform.position.x)
        {
            PosX = RightBoundary.transform.position.x - CameraWidth / 2;
        }
        else
        {
            PosX = FollowObject.transform.position.x;
        }
        // Set Position of Camera
        transform.position = new Vector3(PosX, PosY, transform.position.z);
    }
    #endregion
}
