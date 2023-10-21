using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuEffect : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    #endregion
    #region NormalVariables
    public string Effect;
    private bool isHighlighted;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {

    }

    public void Highlight()
    {
        if (!isHighlighted)
        {
            isHighlighted = true;
            transform.localScale *= 1.03f;
            Color c = transform.GetChild(0).GetComponent<Image>().color;
            c.a *= 3;
            transform.GetChild(0).GetComponent<Image>().color = c;
            Color c1 = transform.GetChild(1).GetComponent<Image>().color;
            c1.a *= 3;
            transform.GetChild(1).GetComponent<Image>().color = c1;
        }
       
    }

    public void UnHighlight()
    {
        if (isHighlighted)
        {
            isHighlighted = false;
            transform.localScale /= 1.03f;
            Color c = transform.GetChild(0).GetComponent<Image>().color;
            c.a /= 3;
            transform.GetChild(0).GetComponent<Image>().color = c;
            Color c1 = transform.GetChild(1).GetComponent<Image>().color;
            c1.a /= 3;
            transform.GetChild(1).GetComponent<Image>().color = c1;
        }
    }
    #endregion
}
