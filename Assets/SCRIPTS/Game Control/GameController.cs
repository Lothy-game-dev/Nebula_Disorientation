using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Camera;
    public GameObject Player;
    public LayerMask PlayerLayer;
    #endregion
    #region NormalVariables
    public bool IsClose;
    private InitializeDatabase db;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        db = GetComponent<InitializeDatabase>();
        //db.DropDatabase();
        db.Initialization();
        InitGame();
        Application.targetFrameRate = 120;
        for (int i = 0; i < 8; i++)
        {
                Physics2D.IgnoreLayerCollision(i, 5, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Initialize Game
    private void InitGame()
    {
        // Set Camera to follow player by default
        Camera.GetComponent<CameraController>().FollowObject = Player;
    } 
    #endregion
}
