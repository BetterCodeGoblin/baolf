# Baolf

Baby golf in Unity. Launch babies from a catapult into golf holes.

## Current state

This repo is a **code-first Unity scaffold** for the core playable loop:
- aim the catapult
- launch the baby projectile
- detect hole scoring
- reset for the next shot

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

1. A launcher object with `CatapultLauncher`
2. A baby projectile prefab with:
   - `Rigidbody`
   - `Collider`
   - `BabyProjectile`
3. A hole trigger with `GolfHole`
4. An empty spawn transform with `ShotSpawnPoint`
5. An empty game controller with `BaolfGameManager`
6. Optional camera rig with `FollowBabyCamera`

Then connect references in the inspector:
- `BaolfGameManager.launcher`
- `BaolfGameManager.spawnPoint`
- `BaolfGameManager.babyProjectilePrefab`
- `CatapultLauncher.launchOrigin`

## Default loop

- Hold launch input to charge
- Release to fire
- Baby reports landing / stop state back to manager
- Hole trigger reports score
- Manager resets for the next shot

## Notes

- Built to work with Unity physics first
- Tone is intentionally ridiculous, but the code path is clean and reusable
- Start simple, then add chaos
