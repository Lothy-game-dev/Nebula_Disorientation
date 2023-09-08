using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject Background;
    public GameObject mainCamera;
    public GameObject Cash;
    public GameObject TimelessShard;
    public GameObject FuelCell;
    public GameObject FuelEnergy;
    #endregion
    #region NormalVariables
    public bool isPlanetMoving;
    private float BGMovingSpeed;
    private Vector2 BGveloc;
    private float HalfCamHeight;
    private float HalfCamWidth;
    private Dictionary<string, object> ListData;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    private void OnEnable()
    {
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingScene(1f);
    }
    void Start()
    {
        // Initialize variables
        isPlanetMoving = true;
        BGMovingSpeed = 0.25f;
        HalfCamHeight = mainCamera.GetComponent<Camera>().orthographicSize;
        HalfCamWidth = HalfCamHeight * mainCamera.GetComponent<Camera>().aspect;
        BGveloc = new Vector2((Random.Range(0,2)-0.5f)*2*BGMovingSpeed, (Random.Range(0, 2) - 0.5f) * 2 * BGMovingSpeed);
        Background.GetComponent<Rigidbody2D>().velocity = BGveloc;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DefineBackgroundVelocity();
        Background.GetComponent<Rigidbody2D>().velocity = BGveloc;
    }
    #endregion
    #region Background moving
    private void DefineBackgroundVelocity()
    {
        float x = 0;
        float y = 0;
        if (TopBorder.transform.position.y <= mainCamera.transform.position.y + HalfCamHeight)
        {
            y = Random.Range(0, BGMovingSpeed);
        } else if (BottomBorder.transform.position.y >= mainCamera.transform.position.y - HalfCamHeight)
        {
            y = Random.Range(-BGMovingSpeed, 0);
        }
        if (LeftBorder.transform.position.x >= mainCamera.transform.position.x - HalfCamWidth)
        {
            x = Random.Range(-BGMovingSpeed, 0);
        }
        else if (RightBorder.transform.position.x <= mainCamera.transform.position.x + HalfCamWidth)
        {
            x = Random.Range(0, BGMovingSpeed);
        }
        if (x != 0 && y == 0)
        {
            BGveloc = new Vector2(x, BGveloc.y);
        }
        else if (x == 0 && y != 0)
        {
            BGveloc = new Vector2(BGveloc.x, y);
        }
        else if (x != 0 && y != 0)
        {
            BGveloc = new Vector2(x, y);
        }
    }
    #endregion
    #region Data
    public void SetDataToView(Dictionary<string,object> datas)
    {
        ListData = datas;
    }
    #endregion
}
