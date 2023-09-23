using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unused
public class ReactToZoomShared : MonoBehaviour
{

    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject ZoomOutPosition;
    public GameObject ClosePosition;
    #endregion
    #region NormalVariables
    private CameraController controller;
    private float InitScaleX;
    private float InitScaleY;
    private Vector2 StartPos;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        controller = FindObjectOfType<CameraController>();
        InitScaleX = transform.localScale.x;
        InitScaleY = transform.localScale.y;
        StartPos = new Vector2(transform.localPosition.x, transform.localPosition.y);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Call function and timer only if possible
        /*if (controller != null && controller.ZoomAction)
        {
            ReactWhenZoom();
        }*/
    }
    #endregion
    #region React To Zoom
    private void ReactWhenZoom()
    {
        gameObject.SetActive(false);
        if (controller.isClose)
        {
            // If close -> All range and scale is half
            transform.localScale = new Vector3(InitScaleX / 2, InitScaleY / 2, transform.localScale.z);
            transform.localPosition = new Vector3(StartPos.x/2, StartPos.y/2, 10);
        }
        else
        {
            // If zoom out -> All range and scale is back to normal
            transform.localScale = new Vector3(InitScaleX, InitScaleY, transform.localScale.z);
            transform.localPosition = new Vector3(StartPos.x, StartPos.y, 10);
        }
        gameObject.SetActive(true);
    }
    #endregion
}
