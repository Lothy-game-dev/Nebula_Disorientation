using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAllyWarship : MonoBehaviour
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
    public int[] WarshipID;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public Vector2[] pos;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector2[2];
        // Initialize variables
        /*pos.x = Random.Range(700f, 2100f);
        pos.y = Random.Range(-3500f, 3500f);*/
        pos[0] = new Vector2(-2000f, 200f);
        pos[1] = new Vector2(-3000f, 200f);
        SpawnWS(pos);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Spawn Warship
    // Group all function that serve the same algorithm
    public void SpawnWS(Vector2[] randomPos)
    {
        for (int j = 0; j < WarshipID.Length; j++)
        {
            Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetWSById(WarshipID[j]);
            for (int i = 0; i < WarshipModel.transform.childCount; i++)
            {
                if (WarshipModel.transform.GetChild(i).name.Replace("_", "").ToLower() == data["WarshipName"].ToString().Replace("-", "").ToLower())
                {
                    GameObject game = Instantiate(WSTemplate, new Vector3(randomPos[j].x, randomPos[j].y, WarshipModel.transform.GetChild(i).position.z), Quaternion.identity);
                    game.GetComponent<SpriteRenderer>().sprite = WarshipModel.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    game.transform.localScale = WarshipModel.transform.GetChild(i).localScale;
                    game.name = data["WarshipName"].ToString();
                    game.AddComponent<PolygonCollider2D>();
                    game.GetComponent<WSShared>().InitData(data, WarshipModel.transform.GetChild(i).gameObject);              
                }
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
