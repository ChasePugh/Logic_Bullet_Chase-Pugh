using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPushForceBehavior : MonoBehaviour
{
    GameManager gm;
    sPlayer s_Player;
    GameObject target;


    public float bulletSpeed;
    public float accelRate;
    public float pushForce;
    public float maxPushForce;

    [Space]
    Vector3 startPosition;
    Vector3 curPos;
    public Vector3 heightMarker;

    [Space]
    public Collider accelrator;



    // Start is called before the first frame update
    public void Start()
    {
        gm = GameManager.gm;
        s_Player = gm.player;

        startPosition = gameObject.transform.position;
        curPos = startPosition;
        //StartCoroutine(MovetoMaxHeight());
    }



    //While Player is in the colider it will push the player upward
    private void OnTriggerStay(Collider other)
    {
         target = other.gameObject;

        if (target.layer == LayerMask.NameToLayer("Player"))
        {
            Rigidbody rbPlayer = target.GetComponent<Rigidbody>();

            //target.GetComponent<Rigidbody>().velocity += new Vector3(0, pushForce, 0);
            rbPlayer.velocity += transform.up * pushForce;

            // cap Y
            if (rbPlayer.velocity.y > maxPushForce)
            {
                // this will set the player's y velocity to the max, effectively capping it
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, maxPushForce, rbPlayer.velocity.z);
            }

            //// cap positive X
            //if (rbPlayer.velocity.x > maxPushForce)
            //{
            //    rbPlayer.velocity = new Vector3(maxPushForce, rbPlayer.velocity.y, rbPlayer.velocity.z);
            //}

            //// cap negative X
            //if (rbPlayer.velocity.x < maxPushForce)
            //{
            //    rbPlayer.velocity = new Vector3(-maxPushForce, rbPlayer.velocity.y, rbPlayer.velocity.z);
            //}

        }
        if(target.layer == LayerMask.NameToLayer("Terrain"))
        {
            DestroyBullet();
        }
    }
   

    // works just stops pushing bool up 
    IEnumerator MovetoMaxHeight()
    {
        float step = bulletSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, heightMarker, step);

        if (Vector3.Distance(transform.position, heightMarker) < 0.001f)
        {
            DestroyBullet();
        }

        yield return new WaitForEndOfFrame();
    }

   
    //current movemnt on the force taken from the pellet bullet script
    private void FixedUpdate()
    {
        StartCoroutine(MovetoMaxHeight());
        
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    

    public void Update()
    {
      
    }

}
