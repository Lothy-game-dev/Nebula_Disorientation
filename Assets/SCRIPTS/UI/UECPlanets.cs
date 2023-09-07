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
    #endregion
    #region NormalVariables
    private List<GameObject> OrbitPlaces;
    private GameObject NextPlace;
    private bool alreadySetVeloc;
    private float initScale;
    private float initCameraSize;
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
            SelfRotate();
        } else
        {
            rb.velocity = new Vector2(0,0);
            alreadySetVeloc = false;
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
        rb.velocity = (NextPlace.transform.position - transform.position) * 0.05f / (NextPlace.transform.position - transform.position).magnitude;
    }

    private void SelfRotate()
    {
        transform.Rotate(new Vector3(0, 0, 0.1f));
        Text.transform.Rotate(new Vector3(0, 0, -0.1f));
    }
    #endregion
    #region Mouse check
    private void OnMouseEnter()
    {
        transform.localScale = new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f);
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector2(initScale, initScale);
    }
    private void OnMouseDown()
    {
        Controller.isPlanetMoving = false;
        rb.velocity = new Vector2(0,0);
        StartCoroutine(ZoomOutWhenClick());
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
        FindObjectOfType<MainMenuCameraController>().ChangeToScene(MoveToScene);
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingScene(1f);
        MainCamera.orthographicSize = initCameraSize;
        Controller.isPlanetMoving = true;
    }
    #endregion
}
