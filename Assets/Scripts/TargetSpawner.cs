using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CannonTargets
{
    public class TargetSpawner : MonoBehaviour
    {
        public int MaxTargets = 5;
        public Vector2 Distance = new Vector2(30f, 45f);
        public Vector2 Height = new Vector2(6f, 11f);
        public Vector2 MassRange = new Vector2(1f, 5f);
        public Vector2 RadiusRange = new Vector2(0.4f, 1f);
        public Vector2 SpeedRange = new Vector2(2f, 5f);
        public float WallHeight = 20f;
        public float WallThickness = 1f;
        private Transform cannon;
        private float innerMinX = -59f;
        private float innerMaxX = 59f;
        private readonly HashSet<TargetBehaviour> activeTargets = new HashSet<TargetBehaviour>();

        private void Start()
        {
            MaxTargets = 5;
            var controller = FindObjectOfType<CannonController>();
            if (controller) cannon = controller.transform;
            CreateSideWalls();
            for (int i = 0; i < MaxTargets; i++) Spawn();
        }

        private void CreateSideWalls()
        {
            GameObject ground = GameObject.Find("Ground");
            Bounds groundBounds = ground ? ground.GetComponent<Collider>().bounds
                : new Bounds(new Vector3(0f, -0.5f, 25f), new Vector3(120f, 1f, 100f));
            float leftCenter = groundBounds.min.x + WallThickness * 0.5f;
            float rightCenter = groundBounds.max.x - WallThickness * 0.5f;
            innerMinX = groundBounds.min.x + WallThickness;
            innerMaxX = groundBounds.max.x - WallThickness;
            float wallY = groundBounds.max.y + WallHeight * 0.5f;
            CreateWall("Purple Wall Left", new Vector3(leftCenter, wallY, groundBounds.center.z),
                new Vector3(WallThickness, WallHeight, groundBounds.size.z));
            CreateWall("Purple Wall Right", new Vector3(rightCenter, wallY, groundBounds.center.z),
                new Vector3(WallThickness, WallHeight, groundBounds.size.z));
        }

        private void CreateWall(string wallName, Vector3 position, Vector3 scale)
        {
            if (GameObject.Find(wallName)) return;
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = wallName;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material.color = new Color(0.58f, 0.16f, 0.82f);
            wall.AddComponent<TargetWall>();
        }

        private void Spawn()
        {
            Vector3 origin = cannon ? cannon.position : Vector3.zero;
            Vector3 direction = cannon ? cannon.forward : Vector3.forward;
            Vector3 position = origin + direction * Random.Range(Distance.x, Distance.y)
                + Vector3.up * Random.Range(Height.x, Height.y);
            float radius = Random.Range(RadiusRange.x, RadiusRange.y);
            float minX = innerMinX + radius + 0.05f;
            float maxX = innerMaxX - radius - 0.05f;
            position.x = Mathf.Clamp(position.x + Random.Range(-12f, 12f), minX, maxX);

            var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            target.name = "Flying Target";
            target.transform.position = position;
            target.GetComponent<Renderer>().material.color = new Color(0.95f, 0.28f, 0.12f);
            target.AddComponent<Rigidbody>();
            var behaviour = target.AddComponent<TargetBehaviour>();
            float velocityX = Random.Range(SpeedRange.x, SpeedRange.y) * (Random.value < 0.5f ? -1f : 1f);
            behaviour.Initialize(Random.Range(MassRange.x, MassRange.y), radius, Vector3.right * velocityX);
            behaviour.HitCallback = HandleTargetHit;
            activeTargets.Add(behaviour);
        }

        private void HandleTargetHit(TargetBehaviour target)
        {
            activeTargets.Remove(target);
            StartCoroutine(RefillAfterTargetIsGone());
        }

        private IEnumerator RefillAfterTargetIsGone()
        {
            yield return null;
            if (activeTargets.Count < MaxTargets) Spawn();
        }
    }
}
