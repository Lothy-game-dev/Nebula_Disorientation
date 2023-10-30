using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBrieflyMoving : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject Background;
    public GameObject mainCamera;
    #endregion
    #region NormalVariables
    private float BGMovingSpeed;
    private Vector2 BGveloc;
    private float HalfCamHeight;
    private float HalfCamWidth;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        BGMovingSpeed = 0.25f;
        HalfCamHeight = mainCamera.GetComponent<Camera>().orthographicSize;
        HalfCamWidth = HalfCamHeight * mainCamera.GetComponent<Camera>().aspect;
        BGveloc = new Vector2((Random.Range(0, 2) - 0.5f) * 2 * BGMovingSpeed, (Random.Range(0, 2) - 0.5f) * 2 * BGMovingSpeed);
        Background.GetComponent<Rigidbody2D>().velocity = BGveloc;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DefineBackgroundVelocity();
        if (gameObject.GetComponent<BackgroundBrieflyMoving>().enabled)
        {
            Background.GetComponent<Rigidbody2D>().velocity = BGveloc;
        }
    }
    #endregion
    #region Background moving
    private void DefineBackgroundVelocity()
    {
        float x = 0;
        float y = 0;
        // If top border inside camera -> reset moving speed to the way out from the camera
        // Other cases use the same algo
        if (TopBorder.transform.position.y <= mainCamera.transform.position.y + HalfCamHeight)
        {
            y = Random.Range(0, BGMovingSpeed);
        }
        else if (BottomBorder.transform.position.y >= mainCamera.transform.position.y - HalfCamHeight)
        {
            y = Random.Range(-BGMovingSpeed, 0);
        }
        if (LeftBorder.transform.position.x >= mainCamera.transform.position.x - HalfCamWidth)
        {
            x = Random.Range(-BGMovingSpeed, 0);
        }
        else if (RightBorder.transform.position.x <= mainCamera.transform.position.x + HalfCamWidth)
        {
            x = Random.Range(0, BGMovingSpeed);
        }
        // If both x and y =0 -> all cases passed -> no change
        if (x != 0 && y == 0)
        {
            BGveloc = new Vector2(x, BGveloc.y);
        }
        else if (x == 0 && y != 0)
        {
            BGveloc = new Vector2(BGveloc.x, y);
        }
        else if (x != 0 && y != 0)
        {
            BGveloc = new Vector2(x, y);
        }
    }

    private void OnDisable()
    {
        Background.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }
    #endregion
}
