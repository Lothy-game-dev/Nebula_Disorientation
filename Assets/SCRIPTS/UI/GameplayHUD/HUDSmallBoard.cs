using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSmallBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject LeftLight;
    public GameObject RightLight;
    public GameObject TopLight;
    public GameObject BottomLight;
    #endregion
    #region NormalVariables
    private GameObject CreatePos;
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
    #region Set Position
    // Group all function that serve the same algorithm
    public void SetPosition(Vector2 Pos, string LeftRighTopBottom)
    {
        if ("left".Equals(LeftRighTopBottom.ToLower()))
        {
            LeftLight.SetActive(true);
            CreatePos = LeftLight.transform.GetChild(0).gameObject;
        } 
        else if ("right".Equals(LeftRighTopBottom.ToLower()))
        {
            RightLight.SetActive(true);
            CreatePos = RightLight.transform.GetChild(0).gameObject;
        }
        else if ("top".Equals(LeftRighTopBottom.ToLower()))
        {
            TopLight.SetActive(true);
            CreatePos = TopLight.transform.GetChild(0).gameObject;
        }
        else if ("bottom".Equals(LeftRighTopBottom.ToLower()))
        {
            BottomLight.SetActive(true);
            CreatePos = BottomLight.transform.GetChild(0).gameObject;
        }
        Vector2 estiPos = Pos - new Vector2(CreatePos.transform.position.x, CreatePos.transform.position.y)
            + new Vector2(transform.position.x, transform.position.y);
        transform.position = new Vector3(estiPos.x, estiPos.y, transform.position.z);
        gameObject.SetActive(true);
    }
    #endregion
}
