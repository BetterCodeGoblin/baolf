using UnityEngine;

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

    private BabyProjectile loadedProjectile;
    private float currentCharge;
    private bool isCharging;
    private BaolfGameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponentInParent<BaolfGameManager>();
    }

    private void Update()
    {
        if (loadedProjectile == null || gameManager == null || gameManager.ShotInProgress)
        {
            isCharging = false;
            currentCharge = 0f;
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
        }

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
    }

    private void Fire()
    {
        if (loadedProjectile == null)
            return;

        isCharging = false;
        float charge01 = Mathf.Clamp01(currentCharge / maxChargeTime);
        float launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, charge01);
        Vector3 velocity = launchOrigin != null
            ? launchOrigin.TransformDirection(launchDirection.normalized) * launchForce
            : transform.TransformDirection(launchDirection.normalized) * launchForce;

        loadedProjectile.Launch(velocity);
        gameManager?.RegisterShotFired();
        loadedProjectile = null;
        currentCharge = 0f;
    }
}
