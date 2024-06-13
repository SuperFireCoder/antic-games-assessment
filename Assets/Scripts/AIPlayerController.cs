// 1. Hill Climbing Algorithm

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject[] balls;
    public int maxIterations = 500;
    public float gridResolution = 0.5f;
    public float stepSize = 0.5f;
    public float gradientThreshold = 0.01f;
    public float decelerationRate = 0.5f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        balls = GameObject.FindGameObjectsWithTag("Ball");
        
    }

    // Update is called once per frame
    public void StartMove()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move() {
        Vector3 bestPosition = FindBestPosition();
        Vector3 direction = bestPosition - transform.position;
        Vector3 velocity = direction.normalized * Mathf.Sqrt(direction.magnitude);

        while(velocity.magnitude >= 0.3f) {
            velocity -= velocity.normalized * decelerationRate * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            yield return null;
        }

        transform.position = bestPosition;
        gameManager.turn = GameManager.Turn.Idle;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach(Collider collider in colliders) {
            if(collider.gameObject.tag == "Ball") {
                Color targetColor = collider.gameObject.GetComponent<MeshRenderer>().material.color;
                if(targetColor == GetComponent<MeshRenderer>().material.color)
                    gameManager.UpdateValue(GameManager.UpdateType.EnemyScore, 1);
                else 
                    gameManager.UpdateValue(GameManager.UpdateType.EnemyScore, -1);
                Destroy(collider.gameObject);
            }
        }
        yield return null;
    }

    Vector3 FindBestPosition() {
        Vector3 bestPosition = Vector3.zero;
        int bestScore = int.MinValue;
        for (float x = -9.5f; x <= 9.5f; x += gridResolution)
        {
            for (float z = -9.5f; z <= 9.5f; z += gridResolution)
            {
                Vector3 testPosition = new Vector3(x, transform.position.y, z);
                int score = CalculateScore(testPosition);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = testPosition;
                }
            }
        }

        for (int i = 0; i < maxIterations; i++)
        {
            Vector3 gradient = CalculateGradient(bestPosition);
            if (gradient.magnitude < gradientThreshold)
            {
                break; // Converged to a local optimum
            }

            bestPosition += stepSize * gradient;
            int score = CalculateScore(bestPosition);
            if (score > bestScore)
            {
                bestScore = score;
            }
            else
            {
                break; // Stop if no improvement
            }
        }

        return bestPosition;
    }

    int CalculateScore(Vector3 position) {
        int score = 0;
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f);
        foreach(Collider collider in colliders) {
            if(collider.gameObject.tag == "Ball") {
                Color targetColor = collider.gameObject.GetComponent<MeshRenderer>().material.color;
                if(targetColor == GetComponent<MeshRenderer>().material.color)
                    score ++;
                else 
                    score --;
            }
        }
        return score;
    }

    Vector3 CalculateGradient(Vector3 position) {
        float delta = 0.1f; // Small delta for numerical gradient calculation
        float baseScore = CalculateScore(position);

        Vector3 gradient = Vector3.zero;
        Vector3 offsetX = new Vector3(delta, 0, 0);
        Vector3 offsetZ = new Vector3(0, 0, delta);

        float scoreX = CalculateScore(position + offsetX);
        float scoreZ = CalculateScore(position + offsetZ);

        gradient.x = (scoreX - baseScore) / delta;
        gradient.z = (scoreZ - baseScore) / delta;

        return gradient.normalized;
    }
}
*/

// 2. Genetic Algorithm

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject[] balls;
    public int maxIterations = 200;
    public float gridResolution = 0.5f;
    public float stepSize = 0.5f;
    public float gradientThreshold = 0.01f;
    public float decelerationRate = 0.5f;
    private List<Vector3> population;
    private float fitnessThreshold = 0.95f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        balls = GameObject.FindGameObjectsWithTag("Ball");
        // Initialize the population of candidate positions
        InitializePopulation();
    }

    // Update is called once per frame
    public void StartMove()
    {
        GetComponent<MeshRenderer>().material.SetInteger("_IsIdle", 0);
        StartCoroutine(Move());
    }

    IEnumerator Move() {
        while (true)
        {
            // Evaluate the fitness of each candidate position in the population
            EvaluatePopulation();

            // Select the best candidate as the new position
            Vector3 bestPosition = SelectBestCandidate();
            float bestFitness = CalculateFitness(bestPosition);

            // If the best fitness is above the threshold, move to the best position and exit the loop
            if (bestFitness >= fitnessThreshold)
            {
                Vector3 direction = bestPosition - transform.position;
                Vector3 velocity = direction.normalized * Mathf.Sqrt(direction.magnitude);

                while (velocity.magnitude >= 0.3f)
                {
                    velocity -= velocity.normalized * decelerationRate * Time.deltaTime;
                    transform.position += velocity * Time.deltaTime;
                    yield return null;
                }

                transform.position = bestPosition;
                gameManager.turn = GameManager.Turn.Idle;
                GetComponent<MeshRenderer>().material.SetInteger("_IsIdle", 1);
                Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
                foreach(Collider collider in colliders) {
                    if(collider.gameObject.tag == "Ball") {
                        Color targetColor = collider.gameObject.GetComponent<MeshRenderer>().material.color;
                        if(targetColor == GetComponent<MeshRenderer>().material.color)
                            gameManager.UpdateValue(GameManager.UpdateType.EnemyScore, 1);
                        else 
                            gameManager.UpdateValue(GameManager.UpdateType.EnemyScore, -1);
                        Destroy(collider.gameObject);
                    }
                }
                yield break;
            }

            // Otherwise, breed a new population and continue the search
            BreedPopulation();
            yield return null;
        }
    }


    int CalculateScore(Vector3 position) {
        int score = 0;
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f);
        foreach(Collider collider in colliders) {
            if(collider.gameObject.tag == "Ball") {
                Color targetColor = collider.gameObject.GetComponent<MeshRenderer>().material.color;
                if(targetColor == GetComponent<MeshRenderer>().material.color)
                    score ++;
                else 
                    score --;
            }
        }
        return score;
    }

    void InitializePopulation()
    {
        population = new List<Vector3>();
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-9.5f, 9.5f);
            float z = Random.Range(-9.5f, 9.5f);
            population.Add(new Vector3(x, transform.position.y, z));
        }
    }

    void EvaluatePopulation()
    {
        foreach (Vector3 position in population)
        {
            CalculateFitness(position);
        }
    }

    float CalculateFitness(Vector3 position)
    {
        int score = CalculateScore(position);
        return (float)score / balls.Length;
    }

    Vector3 SelectBestCandidate()
    {
        float bestFitness = 0f;
        Vector3 bestPosition = Vector3.zero;
        foreach (Vector3 position in population)
        {
            float fitness = CalculateFitness(position);
            if (fitness > bestFitness)
            {
                bestFitness = fitness;
                bestPosition = position;
            }
        }
        return bestPosition;
    }

    void BreedPopulation()
    {
        List<Vector3> newPopulation = new List<Vector3>();
        for (int i = 0; i < population.Count; i++)
        {
            Vector3 parent1 = SelectParent();
            Vector3 parent2 = SelectParent();
            Vector3 child = CrossoverPositions(parent1, parent2);
            child = MutatePosition(child);
            newPopulation.Add(child);
        }
        population = newPopulation;
    }

    Vector3 SelectParent()
    {
        int index = Random.Range(0, population.Count);
        return population[index];
    }

    Vector3 CrossoverPositions(Vector3 parent1, Vector3 parent2)
    {
        float x = Random.Range(parent1.x, parent2.x);
        float z = Random.Range(parent1.z, parent2.z);
        return new Vector3(x, transform.position.y, z);
    }

    Vector3 MutatePosition(Vector3 position)
    {
        float mutationRate = 0.1f;
        float x = position.x + Random.Range(-mutationRate, mutationRate) * gridResolution;
        float z = position.z + Random.Range(-mutationRate, mutationRate) * gridResolution;
        return new Vector3(x, transform.position.y, z);
    }
}
