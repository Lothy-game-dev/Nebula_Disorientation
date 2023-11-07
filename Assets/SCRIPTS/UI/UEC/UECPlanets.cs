using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECPlanets : MonoBehaviour
{
    #region ComponentVariables
    private Rigidbody2D rb;
    private Camera MainCamera;
    #endregion
    #region InitializeVariables
    public UECController Controller;
    public GameObject Orbit;
    public GameObject InitNextPlace;
    public GameObject Text;
    public GameObject MoveToScene;
    public string InfoText;
    #endregion
    #region NormalVariables
    private List<GameObject> OrbitPlaces;
    private GameObject NextPlace;
    private bool alreadySetVeloc;
    private float initScale;
    private float initCameraSize;
    private bool alreadyZoom;
    private bool isLocked;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        OrbitPlaces = new List<GameObject>();
        MainCamera = FindObjectOfType<Camera>();
        initCameraSize = MainCamera.orthographicSize;
        for (int i = 0; i < Orbit.transform.childCount; i++)
        {
            OrbitPlaces.Add(Orbit.transform.GetChild(i).gameObject);
        }
        NextPlace = InitNextPlace;
        alreadySetVeloc = false;
        initScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.isPlanetMoving)
        {
            if ((NextPlace.transform.position - transform.position).magnitude > 0.2f)
            {
                if (!alreadySetVeloc)
                {
                    alreadySetVeloc = true;
                    MoveToPlace();
                }
            }
            else
            {
                ConfigureNextPlace();
            }
        } else
        {
            rb.velocity = new Vector2(0,0);
            alreadySetVeloc = false;
        }
    }
    #endregion
    #region Check Ranking
    public void CheckRanking()
    {
        
        string Rank = (string)FindObjectOfType<AccessDatabase>().GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId)["Rank"];
        Debug.Log(Rank);
        if (Rank== "Unranked")
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1 / 4f;
            c.b = 1 / 4f;
            c.g = 1 / 4f;
            GetComponent<SpriteRenderer>().color = c;
            isLocked = true;
        } else
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1;
            c.b = 1;
            c.g = 1;
            GetComponent<SpriteRenderer>().color = c;
            isLocked = false;
        }
    }
    #endregion
    #region Moving
    private void ConfigureNextPlace()
    {
        alreadySetVeloc = false;
        int n = OrbitPlaces.IndexOf(NextPlace);
        if (n != -1)
        {
            if (n == (OrbitPlaces.Count - 1))
            {
                NextPlace = OrbitPlaces[0];
            } else
            {
                NextPlace = OrbitPlaces[n + 1];
            }
        }
    }
    private void MoveToPlace()
    {
        rb.velocity = (NextPlace.transform.position - transform.position) * 0.5f / (NextPlace.transform.position - transform.position).magnitude;
    }

    #endregion
    #region Mouse check
    private void OnMouseEnter()
    {
        if (!isLocked)
        {
            alreadyZoom = false;
            transform.localScale = new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f);
        }
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, InfoText);
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector2(initScale, initScale);
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (!isLocked)
        {
            if (!alreadyZoom)
            {
                alreadyZoom = true;
                Controller.isPlanetMoving = false;
                rb.velocity = new Vector2(0, 0);
                StartCoroutine(ZoomOutWhenClick());
            }
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Controller.transform.position,
                "Buy a Fighter in the Factory first", 5f);
        }
    }
    private IEnumerator ZoomOutWhenClick()
    {
        Vector2 CameraVeloc = (transform.position - MainCamera.transform.position)/40;
        for (int i=0;i<40;i++)
        {
            MainCamera.orthographicSize -= initCameraSize / 50;
            MainCamera.gameObject.transform.Translate(new Vector3(CameraVeloc.x, CameraVeloc.y,0));
            yield return new WaitForSeconds(0.025f);
        }
        FindObjectOfType<MainMenuCameraController>().GenerateBlackFadeClose(1f, 1f);
        yield return new WaitForSeconds(1f);
        FindObjectOfType<UECMainMenuController>().TeleportToScene(Controller.gameObject, MoveToScene);
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingScene(1f); 
        MainCamera.orthographicSize = initCameraSize;
        Controller.isPlanetMoving = true;
    }
    #endregion
}
