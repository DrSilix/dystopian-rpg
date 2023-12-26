/*
 * Overall control of the crew, handles anything crew related that isn't individualistic.
 * contains a health/cohesion state (general group failure)
 * holds same stats as crew but maintains the best for each among the crew
 * handles the macro state of the game object within the game mechanics (where is the crew in world coords, etc..)
 * provides a generic interface for the individual crew members
 * holds variable amound of crew members
 * holds inventory type things for movement between events
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewController : MonoBehaviour
{
    [SerializeField]
    private int crewHealth;
    [SerializeField]
    private int crewLuck;
    [SerializeField]
    private CrewMemberController crewMember1, crewMember2, crewMember3;

    private bool isMoving = false;
    private Vector3 origin, destination;
    private float moveStartTime, moveEndTime;
    private float currentMoveTime;

    void OnEnable()
    {
        crewHealth = 100;
        crewLuck = 12;
        Camera mainCam = Camera.main;
        mainCam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, mainCam.transform.position.z);
        mainCam.transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;
        currentMoveTime += Time.deltaTime;
        // if (currentMoveTime > moveEndTime) currentMoveTime = moveEndTime;
        transform.position = Vector3.Lerp(origin, destination, currentMoveTime / moveEndTime);
        if (currentMoveTime >= moveEndTime) isMoving = false;
    }

    public void moveTo(Vector3 pos, float seconds)
    {
        isMoving = true;
        origin = transform.position;
        destination = pos;
        moveStartTime = Time.time;
        moveEndTime = seconds;
        currentMoveTime = 0f;
    }

    public void AddCrewMembers(CrewMemberController member1, CrewMemberController member2, CrewMemberController member3)
    {
        crewMember1 = member1;
        crewMember2 = member2;
        crewMember3 = member3;
    }

    public void AddCrewMembers(GameObject member1, GameObject member2, GameObject member3)
    {        
        crewMember1 = member1.GetComponent<CrewMemberController>();
        crewMember2 = member2.GetComponent<CrewMemberController>();
        crewMember3 = member3.GetComponent<CrewMemberController>();
    }

    public int GetLuckRoll() { return Random.Range(0, 20) + crewLuck; }

    public int GetLuck() { return crewLuck; }

    public int TakeDamage(int damage) { return crewHealth -= damage; }

    public int GetHealth() { return crewHealth; }
}
