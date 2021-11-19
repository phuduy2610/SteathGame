using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public float speed;
    public Transform pathHolder;
    public float waitTime = 0.3f;
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
    }
    // Start is called before the first frame update
    void Start()
    {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i=0;i<waypoints.Length;i++){
            waypoints[i] = new Vector3(pathHolder.GetChild(i).position.x,transform.position.y,pathHolder.GetChild(i).position.z) ;
        }
        StartCoroutine(followPath(waypoints));
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator followPath(Vector3[] path)
    {
        transform.position = path[0];
        
        int index = 1;
        while(true){
            transform.position = Vector3.MoveTowards(transform.position,path[index],speed*Time.deltaTime);
            if(transform.position == path[index]){
                index++;
                index = index % path.Length;
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
        
    }



}
