using UnityEngine;
using UnityEngine.InputSystem;

public class StickInputVisualizer : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [Header("Visualization Settings")]
    [Tooltip("Radius of the stick circle.")]
    [SerializeField] private float radius = 1f;
    [SerializeField] private int arcSegments = 30;

    [Header("Colors")]
    [SerializeField] private Color circleColor = Color.white;
    [SerializeField] private Color regionColor = new Color(0f, 1f, 0f, 0.3f);  // Semi-transparent green for region arcs.
    [SerializeField] private Color boundaryColor = Color.red;
    [SerializeField] private Color stickPositionColor = Color.blue; // Color for the stick position line

    [Header("Stick Position")]
    [SerializeField] private Vector2 stickPosition; // Normalized position of the stick (from -1 to 1)

    private InputAction _stick;

    private void Start()
    {
        InputActionMap map = controls.FindActionMap("DualShock");

        _stick = map.FindAction("DiagonalFlick");

        _stick.performed += ctx => { stickPosition = ctx.ReadValue<Vector2>(); };
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;

        // Draw the outer circle (representing the analog stick).
        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(center, radius);

        // Calculate the angle from the stick position.
        float degrees = Mathf.Atan2(stickPosition.y, stickPosition.x) * Mathf.Rad2Deg;
        if (degrees < 0f)
            degrees += 360f;

        // Draw the regions based on your input conditions
        DrawRegion(center, radius, 20f, 70f);   // Up-Right
        DrawRegion(center, radius, 110f, 160f);  // Up-Left
        DrawRegion(center, radius, 200f, 250f);  // Down-Left
        DrawRegion(center, radius, 290f, 340f);  // Down-Right

        DrawRegion(center, radius, 0f, 20f);     // Right
        DrawRegion(center, radius, 70f, 110f);   // Up
        DrawRegion(center, radius, 160f, 200f);  // Left
        DrawRegion(center, radius, 250f, 290f);  // Down

        // Draw a line from the center to the stick position.
        Gizmos.color = stickPositionColor;
        Vector3 stickDirection = new Vector3(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad), 0f);
        Gizmos.DrawLine(center, center + stickDirection * radius);
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
