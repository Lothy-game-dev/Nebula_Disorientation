using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public Camera camera;
    public GameObject Bottom;
    public GameObject Top;
    #endregion
    #region NormalVariables
    public GameObject Position;
    private float InitScaleX;
    private float DistanceToBottom;
    private float DistanceToTop;
    private float HalfCameraHeight;
    #endregion
    #region Start & Update
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
    #region Set Position
    public void SetPosition(GameObject GO)
    {
        // set position based on camera:
        // If the position from gameobject to the top border is not enough for this board
        // then it will show below
        HalfCameraHeight = camera.orthographicSize;
        Vector2 Position = GO.transform.position;
        transform.SetParent(GO.transform);
        DistanceToBottom = (Bottom.transform.position - transform.position).magnitude;
        DistanceToTop = (Top.transform.position - transform.position).magnitude;
        if (Position.y + Mathf.Abs(DistanceToBottom) + Mathf.Abs(DistanceToTop) > camera.gameObject.transform.position.y + HalfCameraHeight)
        {
            transform.Rotate(new Vector3(0, 0, 180f));
            transform.GetChild(1).Rotate(new Vector3(0, 0, 180f));
            transform.position = new Vector3(Position.x, Position.y - Mathf.Abs(DistanceToBottom),transform.position.z);
        } else
        {
            transform.position = new Vector3(Position.x, Position.y + Mathf.Abs(DistanceToBottom), transform.position.z);
        }
        InitScaleX = transform.localScale.x;
        transform.localScale = new Vector2(InitScaleX/10,transform.localScale.y);
        gameObject.SetActive(true);
        if (Time.timeScale!=0)
        {
            StartCoroutine(StartAnimation());
        } else
        {
            transform.localScale = new Vector2(InitScaleX, transform.localScale.y);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private IEnumerator StartAnimation()
    {
        for (int i=0;i<9;i++)
        {
            transform.localScale = new Vector2(transform.localScale.x + InitScaleX / 10, transform.localScale.y);
            yield return new WaitForSeconds(0.02f);
        }
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void Close()
    {
        if (Time.timeScale != 0)
        {
            StartCoroutine(EndAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator EndAnimation()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            transform.localScale = new Vector2(transform.localScale.x - InitScaleX / 10, transform.localScale.y);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(gameObject);
    }
    #endregion
}
