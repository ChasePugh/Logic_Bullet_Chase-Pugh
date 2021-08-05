using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sHydrant : MonoBehaviour
{
    GameManager gm;
    sLogic logic;

    public GameObject water;
    public Transform waterSpawnPoint;

    [Space]
    public Transform maxHeightMarker;

    sPushForceBehavior s_Push;
    [Space]
    [Header("Water Parameters")]
    public float waterSpeed; //The speed of each water shot
    public float maxPushForce; // power of the water (going off of bool's velocoty)
    public float waterPressure; //Used in this coroutine to shoot faster(and check for solidity more often)
    public float pushForce;

    [Space]
    [Header("Particles")]
    public GameObject particleJet;
    public GameObject particleDroplets;
    bool particleSystemActivated;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gm;
        logic = GetComponent<sLogic>();
        maxHeightMarker.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(CoroutineCheckForSolidity());

        particleJet.GetComponent<ParticleSystem>().Stop();
        particleDroplets.GetComponent<ParticleSystem>().Stop();
    }

    //Used to spawn the water
    public void WaterShot()
    {
        GameObject obj = Instantiate(water, waterSpawnPoint.position, waterSpawnPoint.rotation);
        s_Push = obj.GetComponent<sPushForceBehavior>();
        s_Push.heightMarker = maxHeightMarker.position; // marker for height of the water 
        s_Push.bulletSpeed = waterSpeed; //speed the water will move 
        s_Push.maxPushForce = maxPushForce; //used to limit bool's velocity 
        s_Push.pushForce = pushForce;
    }

    IEnumerator CoroutineCheckForSolidity()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(waterPressure);

            //Check the solidity state
            if (logic.currentLogic[(int)eLogics.solidity] == false)
            {
                WaterShot();
            }
            
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(logic.currentLogic[(int)eLogics.solidity] == true)
        {
            if(particleSystemActivated == true)
            {
                particleJet.GetComponent<ParticleSystem>().Stop();
                particleDroplets.GetComponent<ParticleSystem>().Stop();
            }

            particleSystemActivated = false;
        }
        else
        {
            if (particleSystemActivated == false)
            {
                particleJet.GetComponent<ParticleSystem>().Play();
                particleDroplets.GetComponent<ParticleSystem>().Play();
            }

            particleSystemActivated = true;
        }
    }
}
