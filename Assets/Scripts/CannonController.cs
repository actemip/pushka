using UnityEngine;
using UnityEngine.InputSystem;

namespace CannonTargets
{
    public class CannonController : MonoBehaviour
    {
        public Transform Muzzle;
        public Transform AimPivot;
        public TrajectoryRenderer Trajectory;
        public float MoveSpeed = 12f;
        public float TurnSpeed = 75f;
        public float ElevationSpeed = 45f;
        public float MinElevation = -5f;
        public float MaxElevation = 50f;
        public float LaunchSpeed = 35f;
        public float MinMass = 2f, MaxMass = 6f, MinRadius = 0.1f, MaxRadius = 0.2f;
        public float NextMass { get; private set; }
        public float NextRadius { get; private set; }
        private float shootCooldown;
        private float elevation = 12f;
        private Camera followCamera;

        private void Start()
        {
            ConfigureAimPivot();
            AttachCamera();
            RollNextRound();
        }

        private void AttachCamera()
        {
            followCamera = Camera.main;
            if (!followCamera) return;
            followCamera.transform.SetParent(transform, false);
            followCamera.transform.localPosition = new Vector3(0f, 8f, -18f);
            followCamera.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
        }

        private void ConfigureAimPivot()
        {
            if (!AimPivot)
            {
                AimPivot = new GameObject("Aim Pivot").transform;
                AimPivot.SetParent(transform, false);

                Transform barrel = transform.Find("Barrel");
                if (barrel) barrel.SetParent(AimPivot, true);
                if (Muzzle && Muzzle.parent != AimPivot) Muzzle.SetParent(AimPivot, true);
            }

            AimPivot.localRotation = Quaternion.Euler(-elevation, 0f, 0f);
        }
        private void Update()
        {
            float x = (Keyboard.current != null && Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current != null && Keyboard.current.aKey.isPressed ? 1f : 0f);
            float z = (Keyboard.current != null && Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current != null && Keyboard.current.sKey.isPressed ? 1f : 0f);
            transform.position += new Vector3(x, 0f, z).normalized * MoveSpeed * Time.deltaTime;
            float yaw = (Keyboard.current != null && Keyboard.current.eKey.isPressed ? 1f : 0f) - (Keyboard.current != null && Keyboard.current.qKey.isPressed ? 1f : 0f);
            transform.Rotate(Vector3.up, yaw * TurnSpeed * Time.deltaTime, Space.World);
            float pitch = (Keyboard.current != null && Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
                        - (Keyboard.current != null && Keyboard.current.downArrowKey.isPressed ? 1f : 0f);
            elevation = Mathf.Clamp(elevation + pitch * ElevationSpeed * Time.deltaTime, MinElevation, MaxElevation);
            if (AimPivot) AimPivot.localRotation = Quaternion.Euler(-elevation, 0f, 0f);
            if (shootCooldown > 0f) shootCooldown -= Time.deltaTime;
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && shootCooldown <= 0f) Fire();
            if (Trajectory) Trajectory.SetPreview(NextMass, NextRadius, LaunchSpeed);
        }

        private void RollNextRound()
        {
            NextMass = Random.Range(MinMass, MaxMass);
            NextRadius = Random.Range(MinRadius, MaxRadius);
            if (Trajectory) Trajectory.SetPreview(NextMass, NextRadius, LaunchSpeed);
        }

        private void Fire()
        {
            if (!Muzzle) return;
            var shell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shell.name = "Cannonball";
            shell.transform.position = Muzzle.position;
            shell.transform.localScale = Vector3.one;
            var renderer = shell.GetComponent<Renderer>();
            renderer.material.color = new Color(0.18f, 0.2f, 0.24f);
            var body = shell.AddComponent<Rigidbody>();
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            var drag = shell.AddComponent<QuadraticDrag>();
            drag.SetPhysicalParams(NextMass, NextRadius);
            drag.Launch(Muzzle.forward * LaunchSpeed);
            GameStats.RegisterShot();
            shootCooldown = 0.25f;
            RollNextRound();
        }
    }
}


