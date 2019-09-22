﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BreakoutAgent : Agent
{
    public Ball ball;
    public Paddle paddle;
    public BrickManager brickManager;

    private Rigidbody ballRb;

    public override void InitializeAgent()
    {
        ballRb = ball.GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        AddVectorObs(ball.GetState());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = (int)vectorAction[0];
        switch (action)
        {
            case 0: paddle.MovePaddle(0.0f); break;
            case 1: paddle.MovePaddle(-1.0f); break;
            case 2: paddle.MovePaddle(1.0f); break;
        }

        float ballReward = ball.GetReward();
        float brickManagerReward = brickManager.GetReward();
        AddReward(ballReward + brickManagerReward);
        //if (reward > 0 || reward < 0) print(reward);
        if (brickManager.GetDone())
        {
            Done();
        }
    }

    public override void AgentReset()
    {
        paddle.Reset();
        brickManager.Reset();
        ball.Reset(1);
        ball.Respawn();
    }
}
