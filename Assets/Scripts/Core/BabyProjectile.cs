using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BabyProjectile : MonoBehaviour
{
    [SerializeField] private float settleVelocityThreshold = 0.35f;
    [SerializeField] private float settleTimeRequired = 1f;

    private Rigidbody rb;
    private BaolfGameManager gameManager;
    private float settleTimer;
    private bool launched;
    private bool settledReported;
    private bool scored;

    public Rigidbody Rigidbody => rb;
    public bool HasLaunched => launched;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Bind(BaolfGameManager manager)
    {
        gameManager = manager;
    }

    public void Launch(Vector3 velocity)
    {
        launched = true;
        settledReported = false;
        scored = false;
        settleTimer = 0f;
        rb.isKinematic = false;
        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        if (!launched || settledReported || scored)
            return;

        if (rb.linearVelocity.magnitude <= settleVelocityThreshold)
        {
            settleTimer += Time.deltaTime;
            if (settleTimer >= settleTimeRequired)
            {
                settledReported = true;
                gameManager?.NotifyBabySettled(this);
            }
        }
        else
        {
            settleTimer = 0f;
        }
    }

    public void MarkScored()
    {
        if (scored)
            return;

        scored = true;
        settledReported = true;
    }
}
