using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneAsteroid : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject[] SmallerComponents;
    public Vector2[] SmallerComponentPos;
    public GameObject Explosion;
    public LayerMask BlackholeLayer;
    #endregion
    #region NormalVariables
    private bool isBHPulled;
    private List<Vector2> PulledVector;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        AudioSource aus = gameObject.AddComponent<AudioSource>();
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 2000;
        aus.minDistance = 1000;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        SoundController sc = gameObject.AddComponent<SoundController>();
        sc.SoundType = "SFX";
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        CheckInsideBlackhole();
        CalculateVelocity();
    }
    #endregion
    #region FighterHit
    // Group all function that serve the same algorithm
    public void FighterHit(Vector2 ForcePos, float Power)
    {
        Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("OtherExplo", gameObject);
        GetComponent<Collider2D>().enabled = false;
        if (SmallerComponents.Length > 0)
        {
            for (int i=0; i< SmallerComponents.Length;i++)
            {
                GameObject Component = Instantiate(SmallerComponents[i], new Vector2(transform.position.x + SmallerComponentPos[i].x,transform.position.y + SmallerComponentPos[i].y), Quaternion.identity);
                Component.SetActive(true);
                Component.GetComponent<SpaceZoneAsteroid>().SplitCreate(new Vector2(Component.transform.position.x - ForcePos.x,Component.transform.position.y - ForcePos.y), Power/6f);
            }
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 2f);
        } else
        {
            GameObject go = Instantiate(Explosion, transform.position, Quaternion.identity);
            go.SetActive(true);
            Destroy(go, 0.3f);
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 2f);
        }
    }

    public void SplitCreate(Vector2 Force, float Power)
    {
        StartCoroutine(WaitEnableCollider(Force, Power));
    }

    public void CheckInsideBlackhole()
    {
        isBHPulled = false;
        PulledVector = new List<Vector2>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f, BlackholeLayer);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                BlackHole bh = col.GetComponent<BlackHole>();
                if (bh != null)
                {
                    isBHPulled = true;
                    PulledVector.Add(bh.CalculatePullingVector(gameObject));
                }
            }
        }
    }

    public void CalculateVelocity()
    {
        Vector2 veloc = new Vector2(0, 0);
        if (isBHPulled)
        {
            foreach (Vector2 v in PulledVector)
            {
                veloc += v;
            }
        }
        GetComponent<Rigidbody2D>().velocity = veloc;
    }

    private IEnumerator WaitEnableCollider(Vector2 Force, float Power)
    {
        for (int i=0; i<10;i++)
        {
            GetComponent<Rigidbody2D>().AddForce(Force / Force.magnitude * (Power/10f), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }
    #endregion
}
