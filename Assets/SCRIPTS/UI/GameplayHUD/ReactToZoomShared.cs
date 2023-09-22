using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameController controller;
    private float InitScaleX;
    private float InitScaleY;
    private Vector2 StartPos;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        if (FindObjectOfType<GameController>()!=null)
            controller = FindObjectOfType<GameController>();
        InitScaleX = transform.localScale.x;
        InitScaleY = transform.localScale.y;
        StartPos = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (controller!=null)
        {
            ReactWhenZoom();
        }
    }
    #endregion
    #region React To Zoom
    private void ReactWhenZoom()
    {
        if (controller.IsClose)
        {
            // If close -> All range and scale is half
            transform.localScale = new Vector3(InitScaleX / 2, InitScaleY / 2, transform.localScale.z);
            transform.position = new Vector3(StartPos.x/2, StartPos.y/2, transform.position.z);
        }
        else
        {
            // If zoom out -> All range and scale is back to normal
            transform.localScale = new Vector3(InitScaleX, InitScaleY, transform.localScale.z);
            transform.position = new Vector3(StartPos.x, StartPos.y, transform.position.z);
        }
    }
    #endregion
}
