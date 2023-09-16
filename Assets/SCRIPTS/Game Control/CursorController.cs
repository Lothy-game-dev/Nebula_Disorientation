using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    #region ComponentVariables
    private Camera cam;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    private float TopBorderY;
    private float BottomBorderY;
    private float LeftBorderX;
    private float RightBorderX;
    private float HalfCamHeight;
    private float HalfCamWidth;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Check Border
    private void CalculateBorder()
    {
        HalfCamHeight = cam.orthographicSize;
        HalfCamWidth = cam.orthographicSize * cam.aspect;
        TopBorderY = transform.position.y + HalfCamHeight;
        BottomBorderY = transform.position.y - HalfCamHeight;
        LeftBorderX = transform.position.x - HalfCamWidth;
        RightBorderX = transform.position.x + HalfCamWidth;
    }

    public bool IsMouseInsideWindow()
    {
        CalculateBorder();
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > TopBorderY ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < BottomBorderY ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < LeftBorderX ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x > RightBorderX)
        {
            return false;
        } else
        {
            return true;
        }
    }
    #endregion
}
