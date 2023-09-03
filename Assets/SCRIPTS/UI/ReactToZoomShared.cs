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
    private float InitScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        if (FindObjectOfType<GameController>()!=null)
            controller = FindObjectOfType<GameController>();
        InitScale = transform.localScale.x;
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
            transform.localScale = new Vector3(InitScale / 2, InitScale / 2, InitScale / 2);
            transform.position = new Vector3(ClosePosition.transform.position.x, ClosePosition.transform.position.y, transform.position.z);
        }
        else
        {
            // If zoom out -> All range and scale is back to normal
            transform.localScale = new Vector3(InitScale, InitScale, InitScale);
            transform.position = new Vector3(ZoomOutPosition.transform.position.x, ZoomOutPosition.transform.position.y, transform.position.z);
        }
    }
    #endregion
}
