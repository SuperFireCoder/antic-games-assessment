using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{    
    private GameManager gameManager;
    private PlayerController playerController;
    private Vector3 targetPosition;
    private Transform hintTransform;
    public LineRenderer lineRenderer;
    public float arrowSpeed = 5f;
    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        playerController = FindObjectOfType<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
        hintTransform = GameObject.Find("Hint").transform;
        lineRenderer.positionCount = 2;
        SetLine();
    }

    public void SetLine() {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.parent.position);
        lineRenderer.SetPosition(1, transform.parent.position);
    }

    void Update()
    {
        if(playerController.isClicked) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Map"))
            {
                targetPosition = hit.point;
                targetPosition.y += 0.5f;
            }
            Vector3 direction = transform.position - transform.parent.position;
            transform.position = Vector3.Lerp(transform.position, targetPosition, arrowSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(direction);
            
            Vector3 normal = direction.normalized;
            float length = direction.magnitude * direction.magnitude;
            
            ray = new Ray(transform.parent.position, normal);
            if(Physics.Raycast(ray, out hit, length) && hit.collider.CompareTag("Wall")) {
                float distance = Vector3.Distance(ray.origin, hit.point);
                length = Mathf.Min(length, distance - 0.5f);
            }

            hintTransform.position = transform.parent.position + normal * length;
            lineRenderer.SetPosition(1, transform.position);
            lineRenderer.materials[0].SetVector("_EndPoint", transform.position);

            if (Input.GetMouseButtonUp(0)) {
                gameManager.UpdateValue(GameManager.UpdateType.Move, 1);
                gameManager.turn = GameManager.Turn.Player;
                playerController.gameObject.GetComponent<MeshRenderer>().material.SetInteger("_IsIdle", 0);
                playerController.isClicked = false;
                playerController.SetVelocity(direction);
                transform.localPosition = Vector3.zero;
                lineRenderer.enabled = false;
            }
        }
    }
}
