using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECSessionShopIcon : MonoBehaviour
{
    #region ComponentVariables
    private Rigidbody2D rb;
    private Camera MainCamera;
    #endregion
    #region InitializeVariables
    public GameObject Orbit;
    public GameObject InitNextPlace;
    public GameObject Text;
    public GameObject MoveToScene;
    #endregion
    #region NormalVariables
    private List<GameObject> OrbitPlaces;
    private GameObject NextPlace;
    public bool alreadySetVeloc;
    private float initScale;
    private float initCameraSize;
    private bool alreadyZoom;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        OrbitPlaces = new List<GameObject>();
        MainCamera = Camera.main;
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
        /*if (Controller.isPlanetMoving)
        {*/
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
        /* }
         else
         {
             rb.velocity = new Vector2(0, 0);
             alreadySetVeloc = false;
         }*/
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
            }
            else
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
        alreadyZoom = false;
        transform.localScale = new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f);
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector2(initScale, initScale);
    }
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (!alreadyZoom)
        {
            alreadyZoom = true;
            FindObjectOfType<UECSessionScene>().StopMovingIcon = true;
            StartCoroutine(ZoomOutWhenClick());
        }
    }
    private IEnumerator ZoomOutWhenClick()
    {
        Vector2 CameraVeloc = (transform.position - MainCamera.transform.position) / 40;
        for (int i = 0; i < 40; i++)
        {
            MainCamera.orthographicSize -= initCameraSize / 50;
            MainCamera.gameObject.transform.Translate(new Vector3(CameraVeloc.x, CameraVeloc.y, 0));
            yield return new WaitForSeconds(0.025f);
        }
        FindObjectOfType<GameplayExteriorController>().GenerateBlackFadeClose(1f, 2f);
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<GameplayExteriorController>().GenerateBlackFadeOpen(MoveToScene.transform.position, 1f);
        FindObjectOfType<GameplayExteriorController>().ChangeToScene(MoveToScene);
        MainCamera.orthographicSize = initCameraSize;
        FindObjectOfType<UECSessionScene>().StopMovingIcon = false;
        FindObjectOfType<UECSessionScene>().GetComponent<BackgroundBrieflyMoving>().enabled = false;
    }
    #endregion
}
