using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    #endregion
    #region NormalVariables
    public GameObject bar;
    public GameObject background;
    public bool detectMouse;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion
    #region Check Mouse
    private void OnMouseDown()
    {
        if (detectMouse)
        {
            for (int i = 1; i < transform.parent.parent.childCount; i++)
            {
                if (!transform.parent.parent.GetChild(i).GetChild(0).name.Replace("(Clone)","").Replace(" ","")
                    .Equals(transform.parent.name.Replace("(Clone)", "").Replace(" ", "")))
                {
                    transform.parent.parent.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
                }
            }
            GetComponent<Image>().color = Color.green;
            if (bar.GetComponent<LoadOutBar>() != null)
            {
                bar.GetComponent<LoadOutBar>().CurrentItem = transform.parent.gameObject;
            }
        }
    }
    #endregion
}
