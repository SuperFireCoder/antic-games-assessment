using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    public bool isPlay = true;
    private Transform hintTransform;
    public bool isClicked = false;
    public float decelerationRate = 0.5f;
    public Vector3 velocity = Vector3.zero;
    
    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        hintTransform = GameObject.Find("Hint").transform;
    }

    public void SetVelocity(Vector3 newVelocity) {
        velocity = newVelocity;
    }

    private void Update()
    {
        if(isPlay) {
            velocity -= velocity.normalized * decelerationRate * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            if(transform.position.x >= 9.2f || transform.position.x <= -9.2f || transform.position.z >= 9.2f || transform.position.z <= -9.2f )
                velocity = Vector3.zero;
            if(velocity.magnitude <= 0.3f && gameManager.turn == GameManager.Turn.Player) {
                if(gameManager.isStarted) {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
                    foreach(Collider collider in colliders) {
                        if(collider.gameObject.tag == "Ball") {
                            Color targetColor = collider.gameObject.GetComponent<MeshRenderer>().material.color;
                            if(targetColor == GetComponent<MeshRenderer>().material.color)
                                gameManager.UpdateValue(GameManager.UpdateType.PlayerScore, 1);
                            else 
                                gameManager.UpdateValue(GameManager.UpdateType.PlayerScore, -1);
                            Destroy(collider.gameObject);
                        }
                    }
                    GetComponent<MeshRenderer>().material.SetInteger("_IsIdle", 1);
                    gameManager.EnemyMove();
                }
            }

            if (Input.GetMouseButtonDown(0) && gameManager.turn == GameManager.Turn.Idle) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                        if(!isClicked) {
                            isClicked = true;
                            ArrowController arrowController = FindObjectOfType<ArrowController>();
                            arrowController.SetLine();
                            hintTransform.position = transform.position;

                        } else {
                            isClicked = false;
                        }
                    }
                }
            }
        }
    }
}
