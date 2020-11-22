using System.Security.Authentication.ExtendedProtection.Configuration;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class JuggleAgent : Agent
{
    public Rigidbody ball;
    Rigidbody player;
    public float speed = 20f;
    float diff = 0.0f;
    float previousDiff = 0.0f;
    float previousY = 5.0f;
    bool collied = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody == ball)
        {
            collied = true;
        }
    }


    public override void Initialize()
    {
        player = this.GetComponent<Rigidbody>();

    }

    public override void CollectObservations(VectorSensor sensor)//26
    {
        sensor.AddObservation(ball.transform.localPosition);//3d
        sensor.AddObservation(ball.velocity);//3d
        sensor.AddObservation(ball.rotation);//4d
        sensor.AddObservation(ball.angularVelocity);//3d

        sensor.AddObservation(player.transform.localPosition);//3d
        sensor.AddObservation(player.velocity);//3d
        sensor.AddObservation(player.rotation);//4d
        sensor.AddObservation(player.angularVelocity);//3d

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSingle = Vector3.zero;
        controlSingle.x = actions.ContinuousActions[0];
        controlSingle.z = actions.ContinuousActions[1];
        if (player.transform.localPosition.y == 1.0f)
        {
            controlSingle.y = actions.ContinuousActions[2] * 10.0f;
        }

        player.AddForce(controlSingle * speed);

        diff = ball.transform.localPosition.y - previousY;

        if(diff > 0.0f && previousDiff < 0.0f && collied)
        {
            AddReward(1f);
        }

        collied = false;
        previousDiff = diff;
        previousY = ball.transform.localPosition.y;

        if(ball.transform.localPosition.y < 0.5f ||
           Mathf.Abs(player.transform.localPosition.x) > 10f || 
           Mathf.Abs(player.transform.localPosition.z) > 10f)
        {
            EndEpisode();
            AddReward(-1f);
        }
    }

    public override void OnEpisodeBegin()
    {
        ball.transform.localPosition = new Vector3(Random.value * 10f, 10.0f, Random.value * 10f);
        ball.velocity = Vector3.zero;
        ball.rotation = Quaternion.Euler(Vector3.zero);
        ball.angularVelocity = Vector3.zero;

        player.transform.localPosition = new Vector3(5f, 1f, 5f);
        player.velocity = Vector3.zero;
        player.rotation = Quaternion.Euler(Vector3.zero);
        player.angularVelocity = Vector3.zero;

        diff = 0.0f;
        previousDiff = 0.0f;
        previousY = 5.0f;
        collied = false;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetAxis("Jump");
    }
}
