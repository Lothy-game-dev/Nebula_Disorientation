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
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int[] WarshipID;
    public Vector2[] WarshipPosition;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector2[6];
        // Initialize variables
        /*pos.x = Random.Range(700f, 2100f);
        pos.y = Random.Range(-3500f, 3500f);*/
        pos[0] = new Vector2(-2000f, 200f);
        pos[1] = new Vector2(-3000f, 1000f);
        pos[2] = new Vector2(-3000f, 1500f);
        pos[3] = new Vector2(-3000f, 2000f);
        pos[4] = new Vector2(-3000f, 2500f);
        pos[5] = new Vector2(-3000f, 3000f);
        SpawnWS(pos);
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
    public void SpawnAllyWarships()
    {
        for (int j = 0; j < WarshipID.Length; j++)
        {
            SpawnWS(WarshipID[j], WarshipPosition[j]);
        }
    }
    private void SpawnWS(int WarshipID, Vector2 Position)
    {
        Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetWSById(WarshipID);
        for (int i = 0; i < WarshipModel.transform.childCount; i++)
        {
            if (WarshipModel.transform.GetChild(i).name.Replace("_", "").ToLower() == data["WarshipName"].ToString().Replace("-", "").ToLower())
            {
                GameObject game = Instantiate(WSTemplate, new Vector3(Position.x, Position.y, WarshipModel.transform.GetChild(i).position.z), Quaternion.identity);
                game.GetComponent<SpriteRenderer>().sprite = WarshipModel.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                game.transform.localScale = WarshipModel.transform.GetChild(i).localScale;
                game.transform.GetChild(3).localScale = new Vector3(
                    1/WarshipModel.transform.GetChild(i).localScale.x * 130,1/WarshipModel.transform.GetChild(i).localScale.y * 130, 1/WarshipModel.transform.GetChild(i).localScale.z);
                game.name = data["WarshipName"].ToString();
                game.AddComponent<PolygonCollider2D>();
                game.GetComponent<WSShared>().InitData(data, WarshipModel.transform.GetChild(i).gameObject);              
            }
        }
    }

    public void SpawnImmobileWS(int WarshipID, Vector2 Position)
    {
        Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetWSById(WarshipID);
        for (int i = 0; i < WarshipModel.transform.childCount; i++)
        {
            if (WarshipModel.transform.GetChild(i).name.Replace("_", "").ToLower() == data["WarshipName"].ToString().Replace("-", "").ToLower())
            {
                GameObject game = Instantiate(WSTemplate, new Vector3(Position.x, Position.y, WarshipModel.transform.GetChild(i).position.z), Quaternion.identity);
                game.GetComponent<SpriteRenderer>().sprite = WarshipModel.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                game.transform.localScale = WarshipModel.transform.GetChild(i).localScale;
                game.transform.GetChild(3).localScale = new Vector3(
                    1 / WarshipModel.transform.GetChild(i).localScale.x * 130, 1 / WarshipModel.transform.GetChild(i).localScale.y * 130, 1 / WarshipModel.transform.GetChild(i).localScale.z);
                game.name = data["WarshipName"].ToString();
                game.AddComponent<PolygonCollider2D>();
                game.GetComponent<WSShared>().InitData(data, WarshipModel.transform.GetChild(i).gameObject);
                game.GetComponent<WSMovement>().enabled = false;
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
