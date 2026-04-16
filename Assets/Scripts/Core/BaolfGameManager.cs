using UnityEngine;
using System;
using System.Collections;

public class BaolfGameManager : MonoBehaviour
{
    [Header("Scene References")]
    public CatapultLauncher launcher;
    public ShotSpawnPoint spawnPoint;
    public BabyProjectile babyProjectilePrefab;
    public GolfHole activeHole;

    [Header("Flow")]
    [SerializeField] private float resetDelaySeconds = 2f;
    [SerializeField] private int defaultPar = 3;

    private BabyProjectile currentBaby;
    private Coroutine resetCoroutine;
    private bool shotInProgress;
    private int strokesThisHole;
    private int totalStrokes;
    private int holesCompleted;

    public event Action<int, int> OnStrokeCountChanged;
    public event Action<int, int> OnHoleCompleted;
    public event Action<string> OnStatusMessage;

    public int StrokesThisHole => strokesThisHole;
    public int TotalStrokes => totalStrokes;
    public int HolesCompleted => holesCompleted;
    public bool ShotInProgress => shotInProgress;
    public BabyProjectile CurrentBaby => currentBaby;
    public int CurrentPar => activeHole != null ? activeHole.Par : defaultPar;

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
        OnStatusMessage?.Invoke("Ready for the next deeply irresponsible shot.");
    }

    public void RegisterShotFired()
    {
        strokesThisHole++;
        totalStrokes++;
        shotInProgress = true;
        OnStrokeCountChanged?.Invoke(strokesThisHole, CurrentPar);
        OnStatusMessage?.Invoke($"Shot {strokesThisHole}. Send the baby.");
    }

    public void NotifyBabySettled(BabyProjectile baby)
    {
        if (baby != currentBaby)
            return;

        shotInProgress = false;
        OnStatusMessage?.Invoke("Baby settled. Resetting for the next shot.");
        QueueReset();
    }

    public void NotifyOutOfBounds(BabyProjectile baby)
    {
        if (baby != currentBaby)
            return;

        shotInProgress = false;
        OnStatusMessage?.Invoke("Out of bounds. Baby retrieved. Try again.");
        QueueReset();
    }

    public void NotifyHoleScored(BabyProjectile baby, GolfHole hole)
    {
        if (baby != currentBaby)
            return;

        holesCompleted++;
        shotInProgress = false;

        int scoreVsPar = strokesThisHole - (hole != null ? hole.Par : defaultPar);
        string scoreLabel = scoreVsPar switch
        {
            <= -2 => "Albatross. Disturbing efficiency.",
            -1 => "Birdie. The crowd is concerned.",
            0 => "Par. Clinically absurd.",
            1 => "Bogey. Still counts.",
            _ => $"{scoreVsPar} over par, but technically successful."
        };

        Debug.Log($"[BaolfGameManager] Hole cleared in {strokesThisHole} strokes.");
        OnHoleCompleted?.Invoke(strokesThisHole, hole != null ? hole.Par : defaultPar);
        OnStatusMessage?.Invoke(scoreLabel);

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
