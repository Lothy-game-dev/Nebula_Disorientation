using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutGoButton : MonoBehaviour
{
    #region ComponentVariables
    #endregion
    #region InitializeVariables
    public LoadoutScene Scene;
    public GameObject Outer;
    public GameObject Inner;
    #endregion
    #region NormalVariables
    private float initScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        initScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse Check
    private void OnMouseEnter()
    {
        transform.localScale = new Vector2(initScale * 1.1f, initScale * 1.1f);
    }

    private void OnMouseOver()
    {
        Inner.transform.Rotate(new Vector3(0, 0, 1));
        Outer.transform.Rotate(new Vector3(0, 0, -1));
    }

    private void OnMouseExit()
    {
        transform.localScale = new Vector2(initScale, initScale);
    }

    private void OnMouseDown()
    {
        //Data
        //Transfer
    }
    #endregion
}
