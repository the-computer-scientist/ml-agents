using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public float speed;
    public Paddle paddle;
    public BrickManager brickManager;
    public Text livesCount;
    public Text scoreCount;

    private Rigidbody rb;
    private int lives;
    private int score;
    private int scoreTotal;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Reset(0);
    }

    public void Respawn()
    {
        transform.position = Vector3.zero;
        Vector2 startV = Random.insideUnitCircle.normalized * speed;
        rb.velocity = new Vector3(startV.x, 0, startV.y);
        livesCount.text = lives.ToString();
        if(lives-- <= 0)
        {
            brickManager.Done(-1 * brickManager.GetActiveBrickCount());
        }
    }

    public void Reset(int lives)
    {
        this.lives = lives;
        score = 0;
        scoreTotal = 0;
        scoreCount.text = scoreTotal.ToString();
    }

    public List<float> GetState()
    {
        Vector3 ballPos = rb.transform.position;
        Vector3 ballVel = rb.velocity;
        Vector3 paddlePos = paddle.transform.position;
        float paddleVel = paddle.Direction * paddle.speed;
        List<float> state = new List<float> {
            ballPos.x,// / 5.0f,
            ballPos.z,// / 5.0f,
            ballVel.x,// / speed,
            ballVel.z,// / speed,
            paddlePos.x,// / 5.0f,
            paddleVel// / paddle.speed
        };
        state.AddRange(brickManager.GetBricksStatus());
        return state;
    }

    public int GetReward()
    {
        int reward = score;
        score = 0;
        return reward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        Vector3 velocity = rb.velocity;
        if (other.CompareTag("pit"))
        {
            Respawn();
        }
        else if (other.CompareTag("brick"))
        {
            score++;
            scoreTotal++;
            scoreCount.text = scoreTotal.ToString();
            collision.gameObject.SetActive(false);
            if(brickManager.GetActiveBrickCount() == 0)
            {
                brickManager.Done(0);
            }
        }
        else if (other.CompareTag("paddle"))
        {
            velocity.z = Mathf.Abs(velocity.z);
            velocity.x += 0.01f * paddle.speed * paddle.Direction;
        }
        else if (other.CompareTag("top"))
        {
            Transform tfm = other.transform;
            float direction = Mathf.Abs(velocity.x) > 0.0f ? Mathf.Sign(velocity.x) : -Mathf.Sign(rb.position.x);
            velocity.x = Mathf.Max(1f, Mathf.Abs(velocity.x)) * direction;
            velocity.z = Mathf.Abs(velocity.z) * -Mathf.Sign(tfm.position.z);
        }
        else if (other.CompareTag("side"))
        {
            Transform tfm = other.transform;
            velocity.z = Mathf.Max(1f, Mathf.Abs(velocity.z)) * Mathf.Sign(velocity.z);
            velocity.x = Mathf.Abs(velocity.x) * -Mathf.Sign(tfm.position.x);
        }
        rb.velocity = velocity;
    }
}
