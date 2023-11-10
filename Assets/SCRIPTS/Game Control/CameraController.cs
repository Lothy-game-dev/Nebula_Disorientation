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
    public bool isClose;
    private float CameraHeight;
    private float CameraWidth;
    private float TopYPosition;
    private float BottomYPosition;
    private float LeftXPosition;
    private float RightXPosition;
    private float PosX;
    private float PosY;
    private float zoomTimer;
    private bool isPausing;
    private GameplayInteriorController InteriorController;
    private Vector2 FollowPos;
    private float LimitRange;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        cam = GetComponent<Camera>();
        InteriorController = GetComponent<GameplayInteriorController>();
        isClose = true;
        GameController.GetComponent<GameController>().IsClose = true;
        CameraTracking = true;
        cam.orthographicSize = CloseHeight / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        // Change to close or zoom-out when press left shift
        zoomTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && zoomTimer<=0f && Time.timeScale != 0 && !InteriorController.IsInLoading)
        {
            if (!isClose)
            {
                isClose = true;
                GameController.GetComponent<GameController>().IsClose = true;
                cam.gameObject.transform.localScale = new Vector3(1f, 1f, 1);
                cam.orthographicSize = CloseHeight / 2;
                zoomTimer = 0.5f;
            } else
            {
                isClose = false;
                GameController.GetComponent<GameController>().IsClose = false;
                cam.gameObject.transform.localScale = new Vector3(2f, 2f, 1);
                cam.orthographicSize = ZoomOutHeight / 2;
                zoomTimer = 0.5f;
            }
/*            StartCoroutine(Zoom());*/
        }
        // Calculate Height And Width
        CameraHeight = 2 * cam.orthographicSize;
        CameraWidth = CameraHeight * cam.aspect;
        if (FollowObject!=null && FollowObject.GetComponent<FighterShared>()!=null)
        {
            float LeftRange = FollowObject.GetComponent<FighterShared>().LeftWeapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaximumDistance == 0 ?
               FollowObject.GetComponent<FighterShared>().LeftWeapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaxEffectiveDistance : 
               FollowObject.GetComponent<FighterShared>().LeftWeapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaximumDistance;
            float RightRange = FollowObject.GetComponent<FighterShared>().RightWeapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaximumDistance == 0 ?
               FollowObject.GetComponent<FighterShared>().RightWeapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaxEffectiveDistance :
               FollowObject.GetComponent<FighterShared>().RightWeapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaximumDistance;
            LimitRange = LeftRange > RightRange ? LeftRange : RightRange;
        }
        
    

    }
    private void LateUpdate()
    {
        if (!InteriorController.isEnding && !InteriorController.isPausing)
            SetCameraPosition();
    }
    #endregion
    #region Camera Position
    private void SetCameraPosition()
    {
        GetPosition();
        // Follow Player And Limit In Boundary
        // Calculate Top Bottom Left Right position of the Camera
        TopYPosition = FollowPos.y + CameraHeight / 2;
        BottomYPosition = FollowPos.y - CameraHeight / 2;
        LeftXPosition = FollowPos.x - CameraWidth / 2;
        RightXPosition = FollowPos.x + CameraWidth / 2;
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
            PosY = FollowPos.y;
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
            PosX = FollowPos.x;
        }
        // Set Position of Camera
        transform.position = new Vector3(PosX, PosY, transform.position.z);
    }

    private void GetPosition()
    {
        if (FollowObject!=null && FollowObject.GetComponent<PlayerFighter>()!=null && FollowObject.GetComponent<PlayerFighter>().Aim != null)
        {
            Vector2 TestPos = new Vector2((FollowObject.transform.position.x + FollowObject.GetComponent<PlayerFighter>().Aim.transform.position.x) / 2,
            (FollowObject.transform.position.y + FollowObject.GetComponent<PlayerFighter>().Aim.transform.position.y) / 2);
            LimitRange = isClose ? 50 : 200;
            if ((TestPos - new Vector2(FollowObject.transform.position.x,FollowObject.transform.position.y)).magnitude > LimitRange)
            {
                TestPos = new Vector2(FollowObject.transform.position.x, FollowObject.transform.position.y)
                    + (TestPos - new Vector2(FollowObject.transform.position.x, FollowObject.transform.position.y)) * LimitRange / (TestPos - new Vector2(FollowObject.transform.position.x, FollowObject.transform.position.y)).magnitude;
            }
            FollowPos = TestPos;
        } else
        {
            FollowPos = FollowObject.transform.position;
        }
    }
    #endregion
    #region Main Gameplay Popup Screen
    public bool PauseGame()
    {
        if (!InteriorController.isEnding)
        {
            if (!isPausing)
            {
                Time.timeScale = 0;
                isPausing = true;
                InteriorController.PauseMenuOn();
                Debug.Log("Pause");
                FollowObject.GetComponent<PlayerFighter>().isPausing = true;
            } else
            {
                Time.timeScale = 1;
                isPausing = false;
                InteriorController.PauseMenuOff();
                Debug.Log("Continue");
                FollowObject.GetComponent<PlayerFighter>().isPausing = false;
            }
        }
        return isPausing;
    }
    #endregion
}
