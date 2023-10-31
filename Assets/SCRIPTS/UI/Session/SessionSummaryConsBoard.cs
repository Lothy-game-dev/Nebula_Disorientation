using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SessionSummaryConsBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Content;
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;
    public GameObject Bottom;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
        if(CheckIfMouseOutsideScrollRange())
        {
            if (Input.anyKey)
            {
                gameObject.SetActive(false);
            }
        }
    }
    #endregion
    #region Set Data
    // Group all function that serve the same algorithm
    public void SetData(string Data)
    {
        Content.GetComponent<TextMeshProUGUI>().text = Data;
    }

    private bool CheckIfMouseOutsideScrollRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > Right.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Left.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Top.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Bottom.transform.position.y)
            return true;
        return false;
    }
    #endregion
}
