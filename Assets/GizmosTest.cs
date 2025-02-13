using UnityEngine;

public class StickInputVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [Tooltip("Radius of the stick circle.")]
    [SerializeField] private float radius = 1f;
    [Tooltip("Tolerance in degrees. If 0, use discrete snapping; if > 0, widen each region by this amount.")]
    [SerializeField] private float tolerance = 0f;
    [SerializeField] private int arcSegments = 30;

    [Header("Colors")]
    [SerializeField] private Color circleColor = Color.white;
    [SerializeField] private Color regionColor = new Color(0f, 1f, 0f, 0.3f);  // Semi-transparent green for region arcs.
    [SerializeField] private Color boundaryColor = Color.red;

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;

        // Draw the outer circle.
        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(center, radius);

        // If tolerance is 0, visualize using discrete snapping.
        if (Mathf.Approximately(tolerance, 0f))
        {
            Gizmos.color = boundaryColor;
            // Draw radial lines at multiples of 45°: 0,45,90,...,315.
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                DrawRadialLine(center, radius, angle);
            }
        }
        else
        {
            // Visualize widened regions for each direction.
            // RS_Right is special because it wraps around.
            DrawRegion(center, radius, 360f - tolerance, 360f);
            DrawRegion(center, radius, 0f, tolerance);

            DrawRegion(center, radius, 45f - tolerance, 45f + tolerance);
            DrawRegion(center, radius, 90f - tolerance, 90f + tolerance);
            DrawRegion(center, radius, 135f - tolerance, 135f + tolerance);
            DrawRegion(center, radius, 180f - tolerance, 180f + tolerance);
            DrawRegion(center, radius, 225f - tolerance, 225f + tolerance);
            DrawRegion(center, radius, 270f - tolerance, 270f + tolerance);
            DrawRegion(center, radius, 315f - tolerance, 315f + tolerance);
        }
    }

    /// <summary>
    /// Draws an arc representing a region from startAngle to endAngle (in degrees).
    /// </summary>
    private void DrawRegion(Vector3 center, float radius, float startAngle, float endAngle)
    {
        // Draw the arc (filled with regionColor).
        Gizmos.color = regionColor;
        float startRad = startAngle * Mathf.Deg2Rad;
        float endRad = endAngle * Mathf.Deg2Rad;
        int segments = arcSegments;
        float angleStep = (endRad - startRad) / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(startRad), Mathf.Sin(startRad), 0f) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float currentRad = startRad + i * angleStep;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(currentRad), Mathf.Sin(currentRad), 0f) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }

        // Draw the boundaries in a different color.
        Gizmos.color = boundaryColor;
        DrawRadialLine(center, radius, startAngle);
        DrawRadialLine(center, radius, endAngle);
    }

    /// <summary>
    /// Draws a radial line from the center outwards at the specified angle (in degrees).
    /// </summary>
    private void DrawRadialLine(Vector3 center, float radius, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 endPoint = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
        Gizmos.DrawLine(center, endPoint);
    }
}
