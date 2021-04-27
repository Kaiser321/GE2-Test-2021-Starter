using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public GameObject ball;
    public GameObject player;

    public List<AudioClip> barks;
    public AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<StateMachine>().ChangeState(new FollowPlayer());
    }

    public void Bark()
    {
        audio.PlayOneShot(barks[Random.Range(0, barks.Count)]);
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
    Vector3 ballPos;
    GameObject head;

    public override void Enter()
    {
        player = owner.GetComponent<Dog>().player;
        owner.GetComponent<Dog>().ball = player.GetComponent<FPSController>().ball;
        head = owner.transform.Find("Head").gameObject;
        ballPos = player.GetComponent<FPSController>().ball.transform.position;

        owner.GetComponent<Dog>().Bark();

        owner.GetComponent<Arrive>().targetPosition = ballPos;
        owner.GetComponent<Arrive>().enabled = true;
    }
    public override void Think()
    {
        if (Vector3.Distance(head.transform.position, ballPos) <= 1)
        {
            owner.GetComponent<StateMachine>().ChangeState(new AttachBall());
        }
        else
        {
            ballPos = player.GetComponent<FPSController>().ball.transform.position;
            ballPos.y = 0.25f;
            owner.GetComponent<Arrive>().targetPosition = ballPos;
        }
    }
    public override void Exit()
    {
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

        owner.GetComponent<Dog>().Bark();

        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().AddForce(owner.transform.forward * 100);
        ball.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<FPSController>().ifThrownBall = false;
        owner.GetComponent<StateMachine>().ChangeState(new FollowPlayer());
    }
}