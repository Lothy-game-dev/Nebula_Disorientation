using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOutDetailStatus : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Health;
    public GameObject Speed;
    public GameObject RotationSpeed;
    public GameObject DmgMod;
    public GameObject AoEMod;
    public GameObject PowerCDMod;
    public GameObject AoF;
    public GameObject AoFImage;
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
    }
    #endregion
    #region Set Data
    public void SetData(string name, Dictionary<string, object> data)
    {
        transform.GetChild(0).GetComponent<TextMeshPro>().text = name;
        Health.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["HP"];
        Speed.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["SPD"];
        RotationSpeed.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["ROT"];
        AoF.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["AOF"];
        DmgMod.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["DM"];
        AoEMod.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["AM"];
        PowerCDMod.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (string)data["PM"];
        string AoFDemo = (string)data["AOFDemo"];
        if ("45".Equals(AoFDemo))
        {

        }
    }
    #endregion
}
