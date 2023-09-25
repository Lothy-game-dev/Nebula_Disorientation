using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDShowRange : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public MiniMap Minimap;
    public GameObject RangeShow;
    public float Range;
    public GameObject RangeMiniMap;
    #endregion
    #region NormalVariables
    private float InitRange;
    private float InitRangeMinimap;
    private float InitScale;
    private float InitScaleMinimap;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitRange = Mathf.Abs((RangeShow.transform.position - RangeShow.transform.GetChild(0).transform.position).magnitude);
        InitRangeMinimap = Mathf.Abs((RangeMiniMap.transform.position - RangeMiniMap.transform.GetChild(0).transform.position).magnitude);
        InitScale = RangeShow.transform.localScale.x;
        InitScaleMinimap = RangeMiniMap.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse check
    private void OnMouseEnter()
    {
        Debug.Log(InitRange);
        if (Range>0f)
        {
            RangeShow.transform.localScale = new Vector3(
                RangeShow.transform.localScale.x * Range / InitRange,
                RangeShow.transform.localScale.y * Range / InitRange,
                RangeShow.transform.localScale.z);
            RangeShow.gameObject.SetActive(true);
            RangeMiniMap.transform.localScale = new Vector3(
                RangeMiniMap.transform.localScale.x * Minimap.RenderRate * Range / InitRangeMinimap,
                RangeMiniMap.transform.localScale.y * Minimap.RenderRate * Range / InitRangeMinimap,
                RangeMiniMap.transform.localScale.z);
            RangeMiniMap.gameObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (RangeShow.activeSelf)
        {
            RangeShow.transform.localScale = new Vector3(
                InitScale,
                InitScale,
                RangeShow.transform.localScale.z);
            RangeShow.gameObject.SetActive(false);
            RangeMiniMap.transform.localScale = new Vector3(
                InitScaleMinimap,
                InitScaleMinimap,
                RangeMiniMap.transform.localScale.z);
            RangeMiniMap.gameObject.SetActive(false);
        }
    }
    #endregion
}
