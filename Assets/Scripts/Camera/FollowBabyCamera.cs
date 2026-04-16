using UnityEngine;

public class FollowBabyCamera : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 6f, -8f);
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private bool lookAtTarget = true;

    private BaolfGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<BaolfGameManager>();
    }

    private void LateUpdate()
    {
        if (gameManager == null || gameManager.CurrentBaby == null)
            return;

        Transform target = gameManager.CurrentBaby.transform;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        if (lookAtTarget)
            transform.LookAt(target.position);
    }
}
