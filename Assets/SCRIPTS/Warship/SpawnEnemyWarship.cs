using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyWarship : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject WarshipModel;
    public GameObject WSTemplate;

    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int[] WarshipID;
    public Vector2[] WarshipPosition;
    public string[] WarshipClass;
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
    #region Spawn Warship
    // Group all function that serve the same algorithm
    public void SpawnEnemyWarships()
    {
        for (int i=0; i< WarshipID.Length;i++)
        {
            SpawnWS(WarshipID[i], WarshipPosition[i], WarshipClass[i]);
        }
    }
    private void SpawnWS(int WarshipID, Vector2 randomPos, string Class)
    {
        Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetWSById(WarshipID);
        for (int i = 0; i < WarshipModel.transform.childCount; i++)
        {
            if (WarshipModel.transform.GetChild(i).name.Replace("_", "").ToLower() == data["WarshipName"].ToString().Replace("-","").ToLower())
            {
                GameObject game = Instantiate(WSTemplate, new Vector3(randomPos.x, randomPos.y, WarshipModel.transform.GetChild(i).position.z), Quaternion.identity);
                game.GetComponent<SpriteRenderer>().sprite = WarshipModel.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                game.transform.localScale = WarshipModel.transform.GetChild(i).localScale;
                if (Class == "A")
                {
                    game.transform.GetChild(5).gameObject.SetActive(true);
                }
                else if (Class == "B")
                {
                    game.transform.GetChild(6).gameObject.SetActive(true);
                }
                else if (Class == "C")
                {
                    game.transform.GetChild(7).gameObject.SetActive(true);
                }
                game.transform.GetChild(4).localScale = new Vector3(
                        1 / WarshipModel.transform.GetChild(i).localScale.x * 130, 1 / WarshipModel.transform.GetChild(i).localScale.y * 130, 1 / WarshipModel.transform.GetChild(i).localScale.z);
                game.name = data["WarshipName"].ToString();
                game.AddComponent<PolygonCollider2D>();
                game.GetComponent<WSShared>().InitData(data, WarshipModel.transform.GetChild(i).gameObject);
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
