using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_MoveObjectToPoint_Arc : MonoBehaviour
{
    public Vector3 targetPos;
    // Time to move from sunrise to sunset position, in seconds.
    public float journeyTime = 1.0f;

    // The time at which the animation started.
    private float startTime;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            Die();
        }
        // The center of the arc
        Vector3 center = (startPos + targetPos) * 0.5F;

        // move the center a bit downwards to make the arc vertical
        center += new Vector3(0, 1, 0);

        // Interpolate over the arc relative to center
        Vector3 riseRelCenter = startPos - center;
        Vector3 setRelCenter = targetPos - center;

        // The fraction of the animation that has happened so far is
        // equal to the elapsed time divided by the desired time for
        // the total journey.
        float fracComplete = (Time.time - startTime) / journeyTime;

        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
