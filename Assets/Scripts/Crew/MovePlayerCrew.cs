using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

/// <summary>
/// Currently the crew is moved from node to node to complete each and finish the heist
/// with the camera attached. One of the few scene interacting scripts.
/// </summary>
public class MovePlayerCrew : MonoBehaviour
{
    private bool isMoving = false;
    private Vector3 origin, destination;
    private float moveEndTime;
    private float currentMoveTime;
    void OnEnable()
    {
        Camera mainCam = Camera.main;
        mainCam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, mainCam.transform.position.z);
        mainCam.transform.parent = transform;
    }

    void Update()
    {
        if (!isMoving) return;
        currentMoveTime += Time.deltaTime;
        // if (currentMoveTime > moveEndTime) currentMoveTime = moveEndTime;
        transform.position = Vector3.Lerp(origin, destination, currentMoveTime / moveEndTime);
        if (currentMoveTime >= moveEndTime) isMoving = false;
    }

    public void MoveTo(Vector3 pos, float seconds)
    {
        isMoving = true;
        origin = transform.position;
        destination = pos;
        moveEndTime = seconds;
        currentMoveTime = 0f;
    }
}
