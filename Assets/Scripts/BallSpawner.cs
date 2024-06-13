using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    private int colorCount = 10;
    private int ballCount = 400;
    private float planeSize = 10f;
    private float minSize = 0.3f, maxSize = 0.7f;
    private Color[] colorArray = new Color[20];
    public GameObject ballPrefab;
    public GameObject player;
    public GameObject enemy;
    public int sameColorCnt;
    
    public void SetColorCount(int count) {
        colorCount = count;
    }

    public void StartGame() {
        int i;
        for(i = 0; i < colorCount; i++)
            colorArray[i] = new Color(Random.Range(0f, 256f) / 256f, Random.Range(0f, 256f) / 256f, Random.Range(0f, 256f) / 256f);
        player.GetComponent<MeshRenderer>().material.SetColor("_Color", colorArray[0]);
        enemy.GetComponent<MeshRenderer>().material.SetColor("_Color", colorArray[1]);
        sameColorCnt = 0;
        for(i = 0; i < ballCount; i++)
        {
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 randomPosition = new Vector3(Random.Range(-planeSize, planeSize), randomSize / 2.0f - 0.5f, Random.Range(-planeSize, planeSize));

            if(!IntersectsExistingBalls(randomPosition, randomSize))
            {
                GameObject ball = Instantiate(ballPrefab, randomPosition, Quaternion.identity);
                ball.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
                ball.transform.parent = transform;
                int colorIndex = Random.Range(0, colorCount * 4);
                if(colorIndex < colorCount) {
                    ball.GetComponent<MeshRenderer>().material.color = colorArray[0];
                    sameColorCnt ++;
                } else if(colorIndex < 2 * colorCount && colorIndex >= colorCount) {
                    ball.GetComponent<MeshRenderer>().material.color = colorArray[1];
                }
                else
                    ball.GetComponent<MeshRenderer>().material.color = colorArray[(colorIndex / 4 + 2) % (colorCount - 2)];
            }
        }
    }

    private bool IntersectsExistingBalls(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        return colliders.Length > 1 || colliders.Length == 1 && !colliders[0].CompareTag("Map");
    }
}
