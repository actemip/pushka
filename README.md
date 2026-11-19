# Cannon Targets — Unity 6

Open this folder in Unity 6. The first editor open generates `Assets/Scenes/Main.unity` automatically. If needed, use **Cannon Targets → Rebuild demo scene**.

## Controls
- **W/A/S/D** move the cannon on the ground.
- **Q/E** rotate it.
- **Up/Down arrows** raise and lower the barrel.
- **Space** fires.

The camera is mounted behind the cannon and follows it. Five red targets move only along the X axis. A target turns white while the predicted trajectory crosses it. Hitting a target destroys both spheres, increments the counter, and spawns one replacement.

The upper-left HUD shows the mass and radius reserved for the next shot, plus shot and hit counts. On a hit, both the cannonball and red target disappear, and the hit count increases once. The yellow trajectory is recomputed from the muzzle each frame. Prediction and projectile use the same midpoint integration and quadratic drag model, including wind. Each shot consumes the displayed mass and radius, then rolls the next pair. Targets spawn with randomized mass, radius and horizontal velocity; at most four remain active.

## Build
Open `Assets/Scenes/Main.unity` and use File → Build Profiles. The scene is also registered in Build Settings by the setup script.
