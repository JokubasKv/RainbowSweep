using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class scr_EnemyAi : src_CharacterStats
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public Transform attackPoint;


    //Patroling
    public bool enemyPatrols;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerInLineOfSight;
    public bool wasChasingPlayer;
    public bool attacksAreBullets;
    public int meeleeAttackDamage;
    public Vector3 meeleeAttackDimensions;


    private Quaternion lookRotation;
    private Vector3 direction;

    public float rotationSpeed = 1;

    public GameObject dropOnDeath;

    public scr_HordeSpawner hordeController;

    bool alreadySearchWalkPoint = false;

    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        GameObject playergo = GameObject.FindGameObjectWithTag("Player");
        if (playergo != null) player = playergo.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInLineOfSight = playerInSightRange ? CheckLineOfSight() : false;

        if (player == null) return;
        if (!playerInLineOfSight && enemyPatrols) Patroling();
        if (playerInLineOfSight && !playerInAttackRange) ChasePlayer();
        if (playerInLineOfSight && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        if (!alreadySearchWalkPoint)
        {
            //Calculate random point in range
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            //Check if point not above cliff
            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;

            alreadySearchWalkPoint = true;
            Invoke("ResetSearchWalkPoint", 1f);
        }
    }
    private void ResetSearchWalkPoint()
    {
        alreadySearchWalkPoint = false;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        SetWalkpoint(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        //find the vector pointing from our position to the target
        direction = (player.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        lookRotation = Quaternion.LookRotation(direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        anim.SetBool("isWalking", false);

        if (attacksAreBullets)
        {
            if (!alreadyAttacked)
            {
                ///Attack code here
                Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
                ///End of attack code

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
        else
        {
            if (!alreadyAttacked)
            {
                ///Attack code here
                Collider[] enemies = Physics.OverlapBox(attackPoint.position, meeleeAttackDimensions, transform.rotation);
                foreach (Collider enemy in enemies)
                {
                    if (enemy.CompareTag("Player"))
                    {
                        enemy.GetComponent<src_CharacterStats>().TakeDamage(meeleeAttackDamage);
                        //Debug.Log(enemy);
                    }
                }
                ///End of attack code

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private bool CheckLineOfSight()
    {
        RaycastHit hit;
        Vector3 direction = player.position - attackPoint.position;
        if (Physics.Raycast(attackPoint.position, direction, out hit,whatIsPlayer))
        {
            Debug.DrawRay(attackPoint.position, direction * hit.distance, Color.yellow);
            Debug.Log(hit.transform.gameObject.name);

            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public void SetWalkpoint(Vector3 walk)
    {
        walkPoint = walk;
        walkPointSet = true;
        anim.SetBool("isWalking", true);
    }

    public override void Die()
    {
        if (dropOnDeath != null) Instantiate(dropOnDeath,transform.position,Quaternion.identity);
        if (hordeController != null) hordeController.EnemyDied();
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPoint.position, meeleeAttackDimensions);
    }
}