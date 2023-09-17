using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceShopLoading : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject LoadingCenter;
    #endregion
    #region NormalVariables
    public float LoadTime;
    private float SpinTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        SpinTimer = 0f;
        if (LoadTime==0f)
        {
            LoadTime = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        // Spin 36 degree x 10 time per second -> 360 degree
        if (SpinTimer<=0f)
        {
            SpinTimer = 0.1f;
            transform.RotateAround(LoadingCenter.transform.position, new Vector3(0, 0, 1f), -36f);
        }
        SpinTimer -= Time.fixedDeltaTime;
    }
    #endregion
    #region Spin
    // Group all function that serve the same algorithm
    #endregion
}
