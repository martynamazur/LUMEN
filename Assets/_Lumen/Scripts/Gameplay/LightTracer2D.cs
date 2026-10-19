using System.Collections.Generic;
using Lumen.Core;
using UnityEngine;
namespace Lumen.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class LightTracer2D : MonoBehaviour
    {
        [SerializeField] private LightReceiver receiver;
        [SerializeField] private int maxReflections = 8;
        [SerializeField] private float maxDistance = 30f;
        private LineRenderer line;
        private int mask;
        private readonly List<Vector3> points = new List<Vector3>();
        private void Awake() { line = GetComponent<LineRenderer>(); mask = LayerMask.GetMask("Ground", "Optics"); }
        private void LateUpdate()
        {
            if (AppRoot.Instance.Session.State != SessionState.Playing) return;
            points.Clear(); var origin = (Vector2)transform.position; var direction = Vector2.down; points.Add(origin); bool lit = false;
            for (int i = 0; i <= maxReflections; i++)
            {
                var hit = Physics2D.Raycast(origin, direction, maxDistance, mask);
                if (hit.collider == null) { points.Add(origin + direction * maxDistance); break; }
                points.Add(hit.point);
                var target = hit.collider.GetComponent<LightReceiver>();
                if (target != null) { lit = target == receiver; break; }
                var mirror = hit.collider.GetComponent<MirrorObject>();
                if (mirror == null || i == maxReflections) break;
                direction = Vector2.Reflect(direction, mirror.SurfaceNormal).normalized;
                // Leave the finite mirror collider to avoid tracing repeatedly from inside it.
                origin = hit.point + direction * .18f;
            }
            line.positionCount = points.Count; line.SetPositions(points.ToArray()); receiver.SetLit(lit);
        }
    }
}
