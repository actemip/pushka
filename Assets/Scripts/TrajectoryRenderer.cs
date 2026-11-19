using UnityEngine;

namespace CannonTargets
{
    public class TrajectoryRenderer : MonoBehaviour
    {
        public Transform Muzzle;
        public LineRenderer Line;
        public float WindX = 0.25f;
        [Range(0.02f, 0.15f)] public float Step = 0.05f;
        public int MaxSteps = 240;
        private float mass = 4f, radius = 0.15f, speed = 35f;
        private TargetBehaviour[] targets = System.Array.Empty<TargetBehaviour>();
        private float targetRefreshTimer;

        public void SetPreview(float m, float r, float launchSpeed)
        { mass = m; radius = r; speed = launchSpeed; }

        private void LateUpdate()
        {
            if (!Muzzle || !Line) return;
            Vector3[] points = Ballistics.Predict(Muzzle.position, Muzzle.forward * speed,
                mass, radius, new Vector3(WindX, 0f, 0f), Step, MaxSteps, 0f);
            Line.positionCount = points.Length;
            Line.SetPositions(points);

            targetRefreshTimer -= Time.deltaTime;
            if (targetRefreshTimer <= 0f)
            {
                targets = FindObjectsOfType<TargetBehaviour>();
                targetRefreshTimer = 0.2f;
            }

            foreach (TargetBehaviour target in targets)
            {
                if (!target) continue;
                float radiusWithLine = target.Radius + Line.startWidth * 0.5f;
                bool intersects = false;
                for (int i = 0; i < points.Length - 1; i++)
                {
                    Vector3 segment = points[i + 1] - points[i];
                    float lengthSquared = segment.sqrMagnitude;
                    float t = lengthSquared > 0f
                        ? Mathf.Clamp01(Vector3.Dot(target.transform.position - points[i], segment) / lengthSquared)
                        : 0f;
                    Vector3 closest = points[i] + segment * t;
                    if ((target.transform.position - closest).sqrMagnitude <= radiusWithLine * radiusWithLine)
                    {
                        intersects = true;
                        break;
                    }
                }
                target.SetTrajectoryHighlighted(intersects);
            }
        }
    }
}
