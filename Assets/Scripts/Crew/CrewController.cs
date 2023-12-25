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
    private Vector3 destination;
    private float moveStartTime;
    private float currentMoveTime;
    private float moveDuration;

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
        // currentMoveTime = Time.time - moveStartTime;
        // if (currentMoveTime > moveDuration) currentMoveTime = moveDuration;
        transform.position = Vector3.Lerp(transform.position, destination, currentMoveTime / moveDuration);
        currentMoveTime += Time.deltaTime;
        if (currentMoveTime >= moveDuration) isMoving = false;
    }

    public void moveTo(Vector3 pos, float seconds)
    {
        isMoving = true;
        destination = pos;
        moveStartTime = Time.time;
        moveDuration = seconds * 3f;
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
