using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GolfHole : MonoBehaviour
{
    private void Reset()
    {
        Collider colliderRef = GetComponent<Collider>();
        if (colliderRef != null)
            colliderRef.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        BabyProjectile baby = other.GetComponent<BabyProjectile>();
        if (baby == null)
            return;

        baby.MarkScored();

        BaolfGameManager manager = FindFirstObjectByType<BaolfGameManager>();
        if (manager != null)
            manager.NotifyHoleScored(baby, this);
    }
}
