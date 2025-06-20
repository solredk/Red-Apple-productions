using UnityEngine;
using System.Collections;

public class MovingBackground : MonoBehaviour
{
    [Header("moving specs")]
    public Transform[] points;

    public float speed = 1.0f;
    public float rotationSpeed = 5.0f;

    private float moveDuration;

    private int currentTargetIndex = 0;


    void Start()
    {
        if (points.Length > 0)
        {
            moveDuration = Vector3.Distance(transform.position, points[0].position) / speed;
            StartCoroutine(MoveThroughPoints());
        }
    }


    IEnumerator MoveThroughPoints()
    {
        //making sure it keeps looping
        while (true)
        {
            //getting all the values for the movement
            Vector3 startPos = transform.position;
            Vector3 targetPos = points[currentTargetIndex].position;
            float elapsedTime = 0f;


            while (elapsedTime < moveDuration)
            {
                // moving the object
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);


                Vector3 direction = targetPos - transform.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            transform.position = targetPos;

            currentTargetIndex++;

            if (currentTargetIndex >= points.Length)
            {
                currentTargetIndex = 0;
            }

            moveDuration = Vector3.Distance(transform.position, points[currentTargetIndex].position) / speed;
        }
    }
}