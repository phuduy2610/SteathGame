using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;
    Transform player;
    public float speed;
    public Transform pathHolder;
    public float waitTime = 0.5f;
    public float turnSpeed = 90;

    public Light spotLight;
    public float viewDistance;
    float viewAngle;
    Color originalSpotlightColor;
    public LayerMask viewMask;

    public float spotTime;
    float currentTimeOut;
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
    // Start is called before the first frame update
    void Start()
    {
        currentTimeOut = spotTime;
        originalSpotlightColor = spotLight.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotLight.spotAngle;
        print(viewAngle);

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = new Vector3(pathHolder.GetChild(i).position.x, transform.position.y, pathHolder.GetChild(i).position.z);
        }
        StartCoroutine(followPath(waypoints));
    }

    // Update is called once per frame 
    void Update()
    {
        if(player!=null){
            if(CanSeePlayer()){
                spotLight.color = Color.red;
                currentTimeOut -= Time.deltaTime;
                if(currentTimeOut <=0){
                    if(OnGuardHasSpottedPlayer!=null){
                        OnGuardHasSpottedPlayer();
                    }
                }
            }else{
                currentTimeOut = spotTime;
                spotLight.color = originalSpotlightColor;
                
            }
        }
        // if (player != null)
        // {
        //     float distance = (player.transform.position - transform.position).magnitude;
        //     if (distance < viewDistance)
        //     {
        //         Vector3 spotDirection = (player.transform.position - transform.position).normalized;
        //         float maxAngle = transform.eulerAngles.y + viewAngle / 2;
        //         float minAngle = transform.eulerAngles.y - viewAngle / 2;
        //         float playerAngle = Mathf.Atan2(spotDirection.x, spotDirection.z) * Mathf.Rad2Deg;
        //         playerAngle = Clamp0360(playerAngle);
        //         //print(playerAngle);
        //         if (playerAngle >= minAngle && playerAngle <= maxAngle)
        //         {
        //             print("Spot Player");
                    
        //         }
        //     }
        // }
    }


    IEnumerator followPath(Vector3[] path)
    {
        transform.position = path[0];
        int index = 1;
        transform.LookAt(path[index]);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[index], speed * Time.deltaTime);
            if (transform.position == path[index])
            {
                index++;
                index = index % path.Length;
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(Turning(path[index], turnSpeed));

            }
            yield return null;

        }

    }

    IEnumerator Turning(Vector3 destination, float turnSpeed)
    {
        Vector3 direction = (destination - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.005f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;

        }
    }

    // public float Clamp0360(float eulerAngles)
    // {
    //     float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
    //     if (result < 0)
    //     {
    //         result += 360f;
    //     }
    //     return result;
    // }

    bool CanSeePlayer(){
        if(Vector3.Distance(player.position,transform.position)<viewDistance){
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward,dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewAngle/2){
                //Để check xem có gì ở giữa không thì cast 1 Ray giữa player và guard và xem có gì chặn giữa không
                if(!Physics.Linecast(transform.position,player.position,viewMask)){
                    return true;
                }
            }
        }
        return false;
    }

}
