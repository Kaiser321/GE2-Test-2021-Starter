using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public GameObject ball;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<StateMachine>().ChangeState(new FollowPlayer());
    }
}

class FollowPlayer : State
{
    Vector3 stopPos;
    GameObject player;
    public override void Enter()
    {
        player = owner.GetComponent<Dog>().player;
        GetStopPos();
        owner.GetComponent<Arrive>().targetPosition = stopPos;
        owner.GetComponent<Arrive>().enabled = true;
    }
    public override void Think()
    {
        if (player.GetComponent<FPSController>().ifThrownBall)
        {
            owner.GetComponent<StateMachine>().ChangeState(new FetchBall());
        }
        else
        {
            GetStopPos();
            owner.GetComponent<Arrive>().targetPosition = stopPos;
        }
    }

    public override void Exit()
    {
        owner.GetComponent<Arrive>().enabled = false;
    }

    private void GetStopPos()
    {
        Vector3 toOwner = owner.transform.position - player.transform.position;
        toOwner.Normalize();
        stopPos = player.transform.position + toOwner * 10;
        stopPos.y = 0.25f;
    }
}

class FetchBall : State
{
    GameObject player;
    GameObject ball;
    GameObject head;
    public override void Enter()
    {
        player = owner.GetComponent<Dog>().player;
        head = owner.transform.Find("Head").gameObject;
        ball = player.GetComponent<FPSController>().ball;

        owner.GetComponent<Dog>().ball = player.GetComponent<FPSController>().ball;
        owner.GetComponent<Arrive>().targetGameObject = ball;
        owner.GetComponent<Arrive>().enabled = true;
    }
    public override void Think()
    {
        if (Vector3.Distance(head.transform.position, ball.transform.position) <= 2.5f)
        {
            owner.GetComponent<StateMachine>().ChangeState(new AttachBall());
        }
        else
        {
            Vector3 pos = owner.transform.position;
            pos.y = 0.25f;
            owner.transform.position = pos;
        }
    }
    public override void Exit()
    {
        owner.GetComponent<Arrive>().targetGameObject = null;
        owner.GetComponent<Arrive>().enabled = false;
    }

}

class AttachBall : State
{
    GameObject ballAttach;
    GameObject ball;

    public override void Enter()
    {
        ballAttach = owner.transform.Find("Dog/BallAttach").gameObject;
        ball = owner.GetComponent<Dog>().ball;
        ball.GetComponent<Rigidbody>().useGravity = false;
        ball.transform.parent = ballAttach.transform;
        ball.transform.position = ballAttach.transform.position;
        owner.GetComponent<StateMachine>().ChangeState(new ReturnToPlayer());
    }

}

class ReturnToPlayer : State
{
    Vector3 stopPos;
    GameObject player;

    public override void Enter()
    {
        player = owner.GetComponent<Dog>().player;
        GetStopPos();
        owner.GetComponent<Arrive>().targetPosition = stopPos;
        owner.GetComponent<Arrive>().enabled = true;
    }
    public override void Think()
    {
        if (Vector3.Distance(owner.transform.position, stopPos) <= 1)
        {
            owner.GetComponent<StateMachine>().ChangeState(new DropBall());
        }
        else
        {
            GetStopPos();
            owner.GetComponent<Arrive>().targetPosition = stopPos;
        }
    }
    public override void Exit()
    {
        owner.GetComponent<Arrive>().enabled = false;
    }

    private void GetStopPos()
    {
        Vector3 toOwner = owner.transform.position - player.transform.position;
        toOwner.Normalize();
        stopPos = player.transform.position + toOwner * 10;
        stopPos.y = 0.25f;
    }

}

class DropBall : State
{
    GameObject player;
    GameObject ball;
    public override void Enter()
    {
        player = owner.GetComponent<Dog>().player;
        ball = owner.GetComponent<Dog>().ball;

        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().AddForce(owner.transform.forward * 100);
        ball.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<FPSController>().ifThrownBall = false;
        owner.GetComponent<StateMachine>().ChangeState(new FollowPlayer());
    }
}