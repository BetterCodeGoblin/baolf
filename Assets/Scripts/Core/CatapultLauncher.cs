using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CatapultLauncher : MonoBehaviour
{
    [Header("References")]
    public Transform launchOrigin;

    [Header("Launch Tuning")]
    [SerializeField] private float minLaunchForce = 8f;
    [SerializeField] private float maxLaunchForce = 28f;
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private Vector3 launchDirection = new Vector3(0f, 0.45f, 1f);

    [Header("Input")]
    [SerializeField] private KeyCode launchKey = KeyCode.Space;

    [Header("Aim Preview")]
    [SerializeField] private int previewSteps = 24;
    [SerializeField] private float previewTimeStep = 0.075f;

    private BabyProjectile loadedProjectile;
    private float currentCharge;
    private bool isCharging;
    private BaolfGameManager gameManager;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        gameManager = GetComponentInParent<BaolfGameManager>();
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (loadedProjectile == null || gameManager == null || gameManager.ShotInProgress)
        {
            isCharging = false;
            currentCharge = 0f;
            HidePreview();
            return;
        }

        if (Input.GetKeyDown(launchKey))
        {
            isCharging = true;
            currentCharge = 0f;
        }

        if (isCharging && Input.GetKey(launchKey))
        {
            currentCharge += Time.deltaTime;
            UpdatePreview();
        }

        if (!isCharging)
            UpdatePreview(0f);

        if (isCharging && Input.GetKeyUp(launchKey))
        {
            Fire();
        }
    }

    public void SetLoadedProjectile(BabyProjectile projectile)
    {
        loadedProjectile = projectile;
        isCharging = false;
        currentCharge = 0f;

        if (loadedProjectile != null && launchOrigin != null)
        {
            loadedProjectile.transform.position = launchOrigin.position;
            loadedProjectile.transform.rotation = launchOrigin.rotation;
        }

        UpdatePreview(0f);
    }

    private void Fire()
    {
        if (loadedProjectile == null)
            return;

        isCharging = false;
        float launchForce = GetCurrentLaunchForce();
        Vector3 velocity = GetLaunchVelocity(launchForce);

        loadedProjectile.Launch(velocity);
        gameManager?.RegisterShotFired();
        loadedProjectile = null;
        currentCharge = 0f;
        HidePreview();
    }

    private float GetCurrentLaunchForce()
    {
        float charge01 = Mathf.Clamp01(currentCharge / maxChargeTime);
        return Mathf.Lerp(minLaunchForce, maxLaunchForce, charge01);
    }

    private Vector3 GetLaunchVelocity(float launchForce)
    {
        Transform origin = launchOrigin != null ? launchOrigin : transform;
        return origin.TransformDirection(launchDirection.normalized) * launchForce;
    }

    private void UpdatePreview()
    {
        UpdatePreview(Mathf.Clamp01(currentCharge / maxChargeTime));
    }

    private void UpdatePreview(float normalizedCharge)
    {
        if (lineRenderer == null || loadedProjectile == null)
            return;

        lineRenderer.enabled = true;
        lineRenderer.positionCount = previewSteps;

        float launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, normalizedCharge);
        Vector3 start = launchOrigin != null ? launchOrigin.position : transform.position;
        Vector3 velocity = GetLaunchVelocity(launchForce);

        for (int i = 0; i < previewSteps; i++)
        {
            float t = i * previewTimeStep;
            Vector3 point = start + velocity * t + 0.5f * Physics.gravity * t * t;
            lineRenderer.SetPosition(i, point);
        }
    }

    private void HidePreview()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}
