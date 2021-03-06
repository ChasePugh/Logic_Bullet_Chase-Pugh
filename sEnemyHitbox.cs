using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]


/*public class TargetsHit
{
    public string name; //Will name element
    public GameObject hitTarget;
    public bool canBeHit;
    public float hitTimer;
    public float hittimeLimit;


    public TargetsHit(string _name, GameObject _hitTarget)
    {
        name = _name;
    }

    
}*/

public class sEnemyHitbox : MonoBehaviour
{
    GameManager gm;
    public GameObject enemyWhoSpawnedThisHitbox;
    sLogic enemyLogic;
    public bool isProjectile;
    public bool getsDestroyedWhenColliding;
    public bool isTouchHitbox;

    [Range(0.0f, 3.0f)]
    public int damageAgainstPlayer = 1;
    [Range(0.0f, 50.0f)]
    public int damageAgainstEnemies = 1;
    public float knockbackMultipier = 1;

    [Space]
    [Header("Targets Hit")]
    public List<GameObject> listTargetsHit;
    public float hitboxClearTimer;
    public float maxHitboxClearTime;

   


    private void Awake()
    {
        if (isProjectile == false)
        {
            enemyLogic = enemyWhoSpawnedThisHitbox.GetComponent<sLogic>();
        }         
    }

    void Start()
    {
        gm = GameManager.gm;
        if (isTouchHitbox)
        {
            hitboxClearTimer = maxHitboxClearTime;
        }
    }

    public void AssignOriginEnemy(GameObject enemy)
    {
        enemyWhoSpawnedThisHitbox = enemy;

        if (isProjectile)
        {
            enemyLogic = enemyWhoSpawnedThisHitbox.GetComponent<sLogic>();
        }
    }

    void HurtPlayer()
    {   
        if (gm.player.stateKnockback == false)
        {
            //print("enemy hitbox hit player");
            gm.player.playerEnergy.UpdateBars(-damageAgainstPlayer);
            gm.player.TakeKnockback(gameObject);

            // commenting this out for now until touch hitboxes are made compatible with the targetsHit List
            AddTargetHit(gm.player.gameObject);

            if (isProjectile)
            {
                if (getsDestroyedWhenColliding)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
            else
            {
                if (getsDestroyedWhenColliding)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void DestroyHitbox()
    {
        Destroy(gameObject);
    }

   public void AddTargetHit(GameObject _target)
    {
        listTargetsHit.Add(_target);
    }

    public void ResetList()
    {
        listTargetsHit.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject target = other.gameObject;
        
        if (target.layer == LayerMask.NameToLayer("Player"))
        {
            for (int i = 0; i < listTargetsHit.Count; i++)
            {
                if (listTargetsHit[i] == target)
                {
                    Debug.Log("This target is on the list");
                    // commenting this out for now until touch hitboxes are made compatible with the targetsHit List
                    return;
                }
            }

            if (isProjectile == false)
            {
                // enemy should not damage the player when Friendly
                if (enemyLogic.currentLogic[(int)eLogics.friendly] == false)
                {
                    HurtPlayer();
                }
            }
            else
            {
                HurtPlayer();
            }
        }

        if (target.layer == LayerMask.NameToLayer("Enemy") || target.layer == LayerMask.NameToLayer("EnemyWalkable"))
        {
            // stop this hitbox from interacting with its own user
            if(target != enemyWhoSpawnedThisHitbox)
            {
                sLogic _targetEnemyLogic = target.GetComponent<sLogic>();
                if (_targetEnemyLogic == null)
                {
                    print("<color=red>Target does not have sEnemyHealth script!</color>");
                }
                else
                {
                    // check if the target and this enemy do NOT have the same Friendly states
                    if ((_targetEnemyLogic.currentLogic[(int)eLogics.friendly] == true && enemyLogic.currentLogic[(int)eLogics.friendly] == false) ||
                        (_targetEnemyLogic.currentLogic[(int)eLogics.friendly] == false && enemyLogic.currentLogic[(int)eLogics.friendly] == true))
                    {
                        //Debug.Log("the target and this enemy do NOT have the same Friendly states");

                        sEnemyHealth _targetEnemyHealth = target.GetComponent<sEnemyHealth>();
                        if (_targetEnemyHealth == null)
                        {
                            print("<color=red>Target does not have sEnemyHealth script!</color>");
                        }
                        else
                        {
                            for (int i = 0; i < listTargetsHit.Count; i++)
                            {
                                if (listTargetsHit[i] == target)
                                {
                                    Debug.Log("This target is on the list");
                                    return;
                                }

                                else
                                {
                                 
                                }
                            }
                            // only do damage and knockback if enemy isn't in knockback state
                            if (_targetEnemyHealth.stateKnockback == false)
                            {
                                if (isProjectile)
                                {
                                    _targetEnemyHealth.TakeDamage(damageAgainstEnemies, eDamageTypes.enemyBullet, _targetEnemyHealth.transform.position);
                                    _targetEnemyHealth.TakeKnockback(this.gameObject, knockbackMultipier);
                                    AddTargetHit(target);
                                }
                                else
                                {
                                    _targetEnemyHealth.TakeDamage(damageAgainstEnemies, eDamageTypes.enemyMelee, _targetEnemyHealth.transform.position);
                                    _targetEnemyHealth.TakeKnockback(this.gameObject, knockbackMultipier);
                                    AddTargetHit(target);
                                }


                                Debug.Log("enemy took damage and knockback from another enemy");

                                if (isProjectile)
                                {
                                    if (getsDestroyedWhenColliding)
                                    {
                                        Destroy(transform.parent.gameObject);
                                    }
                                }
                                else
                                {
                                    if (getsDestroyedWhenColliding)
                                    {
                                        Destroy(gameObject);
                                    }
                                }
                            
                            }        
                        }
                    }
                }
            }
        }
        
    }
    public void Update()
    {
        if(isTouchHitbox == true)
        {
            if(hitboxClearTimer > 0)
            {
                hitboxClearTimer -= Time.deltaTime;
            }
            else
            {
                ResetList();
                hitboxClearTimer = maxHitboxClearTime;
            }
        }

    }
}
