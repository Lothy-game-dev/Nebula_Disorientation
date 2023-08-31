using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arsenal : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private AccessDatabase AccDB;

    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject GameController;
    public GameObject Item;
    public List<Image> WeaponList;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private GameObject CloneItem;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        AccDB = GameController.GetComponent<AccessDatabase>();
        for (int i = 0; i < AccDB.ArsenalWeaponCount(); i++)
        {
            CloneItem = Instantiate(Item, Item.transform.position, Quaternion.identity);
            CloneItem.transform.SetParent(gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
