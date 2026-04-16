# Baolf

Baby golf in Unity. Launch babies from a catapult into golf holes.

## Current state

This repo is a **code-first Unity scaffold** for the core playable loop:
- aim the catapult
- preview the launch arc
- launch the baby projectile
- detect hole scoring
- reset on settle or out-of-bounds
- expose stroke/par hooks for UI

Scenes, prefabs, art, physics tuning, and juice still need to be assembled in Unity.

## Unity target

- Unity 6 (`6000.x`) recommended

## Included script slice

Core scripts live under `Assets/Scripts/`:
- `Core/BaolfGameManager.cs`
- `Core/BabyProjectile.cs`
- `Core/CatapultLauncher.cs`
- `Game/GolfHole.cs`
- `Game/ShotSpawnPoint.cs`
- `Camera/FollowBabyCamera.cs`

This is meant to be one thin playable slice, not full production architecture yet.

## First Unity assembly pass

Create a prototype scene and wire:

1. A game controller object with:
   - `BaolfGameManager`
2. A launcher object with:
   - `CatapultLauncher`
   - `LineRenderer` for aim preview
3. A baby projectile prefab with:
   - `Rigidbody`
   - `Collider`
   - `BabyProjectile`
4. A hole trigger with:
   - `GolfHole`
5. An empty spawn transform with:
   - `ShotSpawnPoint`
6. Optional camera rig with:
   - `FollowBabyCamera`

Then connect references in the inspector:
- `BaolfGameManager.launcher`
- `BaolfGameManager.spawnPoint`
- `BaolfGameManager.babyProjectilePrefab`
- `BaolfGameManager.activeHole`
- `CatapultLauncher.launchOrigin`

## Default loop

- Hold launch input to charge
- View the preview arc while charging
- Release to fire
- Baby reports landing / out-of-bounds back to manager
- Hole trigger reports score and par result
- Manager resets for the next shot

## UI hooks available

`BaolfGameManager` now emits events for:
- stroke count updates
- hole completion
- status text

That gives you a clean place to hook HUD text, scorecards, and announcer nonsense.

## Notes

- Built to work with Unity physics first
- Tone is intentionally ridiculous, but the code path is clean and reusable
- Prototype truth: the fun will come from tuning, feedback, and animation more than architecture
- `GolfHole` still uses `FindFirstObjectByType` for prototype convenience, so that should become a direct reference later if the project grows
