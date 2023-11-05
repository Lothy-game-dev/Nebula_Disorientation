using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySpaceStationSpawn : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject SpaceStationModel;
    public GameObject SpaceStationTemplate;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int[] SpaceStationID;
    public Vector2[] SpaceStationPosition;
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
    #region Spawn SpaceStation
    // Group all function that serve the same algorithm
    public void SpawnAllySpaceStation()
    {
        for (int i=0; i < SpaceStationID.Length; i++)
        {
            SpawnSpaceStation(SpaceStationID[i], SpaceStationPosition[i]);
        }
    }
    private void SpawnSpaceStation(int SpaceStationID, Vector2 randomPos)
    {
        Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetSpaceStationById(SpaceStationID);
        //Debug.Log(SpaceStationModel.transform.childCount);
        for (int i = 0; i < SpaceStationModel.transform.childCount; i++)
        {
            if (SpaceStationModel.transform.GetChild(i).name.ToLower() == data["SpaceStationName"].ToString().ToLower())
            {
                GameObject game = Instantiate(SpaceStationTemplate, new Vector3(randomPos.x, randomPos.y, SpaceStationModel.transform.GetChild(i).position.z), Quaternion.identity);
                game.GetComponent<SpriteRenderer>().sprite = SpaceStationModel.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                game.transform.localScale = SpaceStationModel.transform.GetChild(i).localScale;
                game.name = data["SpaceStationName"].ToString();
                game.AddComponent<PolygonCollider2D>();
                game.GetComponent<SpaceStationShared>().InitData(SpaceStationModel.transform.GetChild(i).gameObject, data);
            }
        }
    }
    #endregion
}
