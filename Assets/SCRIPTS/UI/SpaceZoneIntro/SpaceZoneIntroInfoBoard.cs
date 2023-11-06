using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceZoneIntroInfoBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Template;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<string> Data;
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
    #region GenerateData
    // Group all function that serve the same algorithm
    public void GenerateData()
    {
        Vector2 InstantiatePosition = Template.transform.position;
        float Width = (Template.transform.GetChild(2).position - Template.transform.GetChild(3).position).magnitude;
        for (int i = 0; i < Data.Count; i++)
        {
            GameObject Item = Instantiate(Template, InstantiatePosition, Quaternion.identity);
            Item.transform.SetParent(transform);
            Item.transform.localScale = Template.transform.localScale;
            Item.transform.GetChild(4).GetComponent<TextMeshPro>().text = Data[i];
            if (Data[i].Contains("power") || Data[i].Contains("weapon"))
            {
                Item.transform.GetChild(4).GetComponent<TextMeshPro>().horizontalAlignment = HorizontalAlignmentOptions.Center;
            }
            Item.transform.GetChild(2).gameObject.SetActive(false);
            Item.transform.GetChild(3).gameObject.SetActive(false);
            if (i==0)
            {
                Item.transform.GetChild(3).gameObject.SetActive(true);
            }
            if (i== Data.Count - 1)
            {
                Item.transform.GetChild(2).gameObject.SetActive(true);
            }
            InstantiatePosition = new Vector2(InstantiatePosition.x, InstantiatePosition.y + Width * 60/100f);
            // Icon
            Item.SetActive(true);
        }
    }
    #endregion
}
