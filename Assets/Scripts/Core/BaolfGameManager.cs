using UnityEngine;
using System.Collections;

public class BaolfGameManager : MonoBehaviour
{
    [Header("Scene References")]
    public CatapultLauncher launcher;
    public ShotSpawnPoint spawnPoint;
    public BabyProjectile babyProjectilePrefab;

    [Header("Flow")]
    [SerializeField] private float resetDelaySeconds = 2f;

    private BabyProjectile currentBaby;
    private Coroutine resetCoroutine;
    private bool shotInProgress;
    private int strokesThisHole;
    private int totalStrokes;
    private int holesCompleted;

    public int StrokesThisHole => strokesThisHole;
    public int TotalStrokes => totalStrokes;
    public int HolesCompleted => holesCompleted;
    public bool ShotInProgress => shotInProgress;
    public BabyProjectile CurrentBaby => currentBaby;

    private void Start()
    {
        PrepareNextShot();
    }

    public void PrepareNextShot()
    {
        if (spawnPoint == null || babyProjectilePrefab == null || launcher == null)
        {
            Debug.LogWarning("[BaolfGameManager] Missing scene references.");
            return;
        }

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (currentBaby != null)
            Destroy(currentBaby.gameObject);

        currentBaby = Instantiate(babyProjectilePrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        currentBaby.Bind(this);
        launcher.SetLoadedProjectile(currentBaby);
        shotInProgress = false;
    }

    public void RegisterShotFired()
    {
        strokesThisHole++;
        totalStrokes++;
        shotInProgress = true;
    }

    public void NotifyBabySettled(BabyProjectile baby)
    {
        if (baby != currentBaby)
            return;

        shotInProgress = false;
        QueueReset();
    }

    public void NotifyHoleScored(BabyProjectile baby, GolfHole hole)
    {
        if (baby != currentBaby)
            return;

        holesCompleted++;
        shotInProgress = false;
        Debug.Log($"[BaolfGameManager] Hole cleared in {strokesThisHole} strokes.");
        strokesThisHole = 0;
        QueueReset();
    }

    private void QueueReset()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetDelaySeconds);
        resetCoroutine = null;
        PrepareNextShot();
    }
}
