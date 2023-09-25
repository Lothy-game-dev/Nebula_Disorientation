using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Icons
    public GameObject NormalEnemyIcon;
    public GameObject EliteEnemyIcon;
    public GameObject BossEnemyIcon;
    public GameObject PlayerIcon;
    public GameObject BlackholeIcon;
    // Render related
    public GameObject Player;
    public float RenderRange;
    public LayerMask RenderLayer;
    public GameObject RadiusRange;
    public GameController GameController;
    // Zoom out and close related
    public GameObject ZoomOutPosition;
    public GameObject ClosePosition;
    #endregion
    #region NormalVariables
    private float MiniMapRange;
    public float RenderRate;
    private List<GameObject> icons;
    private float InitRange;
    private float InitScale;
    private float IconScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Calculate Init Minimap Radius
        MiniMapRange = RadiusRange.transform.position.y - transform.position.y;
        // Set Init Range and Scale For Zoom Out/Close
        InitRange = MiniMapRange;
        InitScale = transform.localScale.x;
        // Set Render Rate for Distance from real world to minimap
        RenderRate = MiniMapRange / RenderRange;
        icons = new List<GameObject>();
        // Create Player Icon
        CreatePlayerIcon();
    }

    // Update is called once per frame
    void Update()
    {
        // React When Player Zoom out/ close
        ReactWhenZoom();
        // Render All Objects With Renderable and Enemy Layers that have Collider Within Render Range Declared
        RenderAllWithCollider(RenderRange);
    }
    #endregion
    #region Render Minimap
    // Render All Objects With Renderable and Enemy Layers that have Collider Within Render Range Declared
    public void RenderAllWithCollider(float range)
    {
        // Get All Objects With Renderable and Enemy Layers that have Collider Within Render Range Declared
        Collider2D[] cols = Physics2D.OverlapCircleAll(Player.transform.position, range, RenderLayer);
        foreach (var col in cols)
        {
            // Check if Normal/Elite/Boss Enemy to create corresponding icon
            if (col.tag == "NormalEnemy")
            {
                // Check exist -> if exist: Only move icon's position
                // If not exist -> instantiate object to create new one and save to the list
                GameObject iconCheck = IconExist(col.name);
                if (iconCheck != null)
                {
                    iconCheck.transform.position = CalculateIconPosition(col.transform.position);
                } 
                else
                {
                    GameObject icon = Instantiate(NormalEnemyIcon, CalculateIconPosition(col.transform.position), Quaternion.identity);
                    // Set Icon child of Minimap
                    icon.transform.SetParent(transform);
                    // Set scale of icon based on minimap scale
                    icon.transform.localScale =
                    new Vector3(icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale,
                        icon.transform.localScale.x * transform.localScale.x / InitScale); 
                    icon.SetActive(true);
                    // Set Icon name = object's name + " Icon"
                    icon.name = col.name + " Icon";
                    icons.Add(icon);
                }
            } 
            // Same to Normal Enemy
            else if (col.tag == "EliteEnemy")
            {
                GameObject iconCheck = IconExist(col.name);
                if (iconCheck != null)
                {
                    iconCheck.transform.position = CalculateIconPosition(col.transform.position);
                }
                else
                {
                    GameObject icon = Instantiate(EliteEnemyIcon, CalculateIconPosition(col.transform.position), Quaternion.identity);
                    icon.transform.SetParent(transform);
                    icon.transform.localScale =
                    new Vector3(icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale); 
                    icon.SetActive(true);
                    icon.name = col.name + " Icon";
                    icons.Add(icon);
                }
            }
            // Same to Normal Enemy
            else if (col.tag == "BossEnemy")
            {
                GameObject iconCheck = IconExist(col.name);
                if (iconCheck != null)
                {
                    iconCheck.transform.position = CalculateIconPosition(col.transform.position);
                }
                else
                {
                    GameObject icon = Instantiate(BossEnemyIcon, CalculateIconPosition(col.transform.position), Quaternion.identity);
                    icon.transform.SetParent(transform);
                    icon.transform.localScale = 
                        new Vector3(icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x / InitScale);
                    icon.SetActive(true);
                    icon.name = col.name + " Icon";
                    icons.Add(icon);
                }
            }
            // Blackhole
            else if (col.tag == "Blackhole")
            {
                GameObject iconCheck = IconExist(col.name);
                if (iconCheck != null)
                {
                    iconCheck.transform.position = CalculateIconPosition(col.transform.position);
                }
                else
                {
                    GameObject icon = Instantiate(BlackholeIcon, CalculateIconPosition(col.transform.position), Quaternion.identity);
                    icon.transform.SetParent(transform);
                    float BHRange = col.GetComponent<BlackHole>().radius;
                    float iconBHRange = Mathf.Abs(icon.transform.position.y - icon.transform.GetChild(0).transform.position.y);
                    icon.transform.localScale =
                        new Vector3(icon.transform.localScale.x * IconScale * transform.localScale.x * BHRange * RenderRate / iconBHRange / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x * BHRange * RenderRate / iconBHRange / InitScale,
                        icon.transform.localScale.x * IconScale * transform.localScale.x * BHRange * RenderRate / iconBHRange / InitScale);
                    icon.SetActive(true);
                    icon.name = col.name + " Icon";
                    icons.Add(icon);
                }
            }
            // Create Allies' Icons: WIP
        }
        // Check if object still renderable to choose whether to keep or delete the icon
        CheckIconStillAvailable();
    }
    // Calculate Icon Position On Minimap Relative to the real map
    public Vector2 CalculateIconPosition(Vector2 RealPosition)
    {
        // Calculate
        Vector2 iconPos = (RealPosition - new Vector2(Player.transform.position.x, Player.transform.position.y)) * RenderRate;
        // Return position based on minimap's center position
        return new Vector2(transform.position.x , transform.position.y) + iconPos;
    }
    // Create Player Icon At the center of the minimap
    public void CreatePlayerIcon()
    {
        GameObject icon = Instantiate(PlayerIcon, transform.position, Quaternion.identity);
        // Set Player Icon Child of Minimap
        icon.transform.SetParent(transform);
        icon.SetActive(true);
        // Set PlayerMovement's PlayerIcon
        Player.GetComponent<PlayerMovement>().PlayerIcon = icon;
    }
    // Check if icon exists. Return the icon object if yes, and null if no
    private GameObject IconExist(string name)
    {
        foreach (var icon in icons)
        {
            if (icon.name.Equals(name + " Icon"))
            {
                return icon.gameObject;
            }
        }
        return null;
    }
    // Check object available to render icon
    private void CheckIconStillAvailable()
    {
        int i = 0;
        // While loop > For loop because remove object
        while (i < icons.Count)
        {
            // Substring part: Remove the " Icon" from the name to get the real object's name
            // If the object can not be find = object does not exists or inactive 
            // -> Delete the icon
            if (GameObject.Find(icons[i].name.Substring(0, icons[i].name.Length-5))==null)
            {
                GameObject icon = icons[i];
                icons.RemoveAt(i);
                Destroy(icon);
            } 
            // If the icon's position get out of the minimap -> Delte the icon
            else
            {
                float distance = new Vector2(transform.position.x - icons[i].transform.position.x, transform.position.y - icons[i].transform.position.y).magnitude;
                if (distance > MiniMapRange)
                {
                    GameObject icon = icons[i];
                    icons.RemoveAt(i);
                    Destroy(icon);
                }
                else
                {
                    // Increase interation if all case passed
                    i++;
                }
            }
        }
    }
    #endregion
    #region React When Zoom Out/Close
    // React to zoom
    private void ReactWhenZoom()
    {
        if (GameController.IsClose)
        {
            // If close -> All range and scale is half
            MiniMapRange = InitRange / 2;
            RenderRate = MiniMapRange / RenderRange;
            IconScale = 1 / 2f;
            transform.position = new Vector3(ClosePosition.transform.position.x, ClosePosition.transform.position.y, transform.position.z);
        }
        else
        {
            // If zoom out -> All range and scale is back to normal
            MiniMapRange = InitRange;
            RenderRate = MiniMapRange / RenderRange;
            IconScale = 1;
            transform.position = new Vector3(ZoomOutPosition.transform.position.x, ZoomOutPosition.transform.position.y, transform.position.z);
        }
    }
    #endregion
}
