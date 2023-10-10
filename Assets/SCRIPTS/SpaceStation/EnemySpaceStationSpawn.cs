using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceStationSpawn : MonoBehaviour
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
    public int SpaceStationID;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Vector2 pos;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        /*pos.x = Random.Range(700f, 2100f);
        pos.y = Random.Range(-3500f, 3500f);*/
        pos = new Vector2(-3500f, 466f);
        SpawnSpaceStation(pos);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Spawn SpaceStation
    // Group all function that serve the same algorithm
    public void SpawnSpaceStation(Vector2 randomPos)
    {
        Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetSpaceStationById(SpaceStationID);
        Debug.Log(SpaceStationModel.transform.childCount);
        for (int i = 0; i < SpaceStationModel.transform.childCount; i++)
        {
            if (SpaceStationModel.transform.GetChild(i).name.Replace("_", "").ToLower() == data["SpaceStationName"].ToString().Replace("-", "").ToLower())
            {
                Debug.Log(i);
                GameObject game = Instantiate(SpaceStationTemplate, new Vector3(randomPos.x, randomPos.y, SpaceStationModel.transform.GetChild(i).position.z), Quaternion.identity);
                game.GetComponent<SpriteRenderer>().sprite = SpaceStationModel.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                game.transform.localScale = SpaceStationModel.transform.GetChild(i).localScale;
                game.name = data["SpaceStationName"].ToString();
                game.GetComponent<SpaceStationShared>().InitData(SpaceStationModel.transform.GetChild(i).gameObject, data);
            }
        }
    }
    #endregion
}
