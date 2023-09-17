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
    public Texture2D MainCursor;
    public Texture2D ClickCursor;
    public Texture2D UnallowedCursor;
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
        if (IsMouseInsideWindow())
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            string check = "Normal";
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    /*if (hit.collider.gameObject.GetComponent<CursorUnallowed>()!=null)
                    {
                        check = "Unallowed";
                    } else */
                    check = "Click";
                    break;
                }
            }
            if ("Click".Equals(check))
            {
                Cursor.SetCursor(ClickCursor, Vector2.zero, CursorMode.ForceSoftware);
            } else if ("Normal".Equals(check))
            {
                Cursor.SetCursor(MainCursor, Vector2.zero, CursorMode.ForceSoftware);
            } /*else if ("Unallowed".Equals(check))
            {
                Cursor.SetCursor(UnallowedCursor, Vector2.zero, CursorMode.ForceSoftware);
            }*/
        }
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
