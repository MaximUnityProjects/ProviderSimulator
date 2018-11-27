using UnityEngine;

[AddComponentMenu("Camera-Control/CameraZoom")]

public class CameraZoom : MonoBehaviour {

    [SerializeField] int speedzoom = 4, maxzoom = 27, maxzoomY = 27, maxzoomX = 40, minzoom = 6;
    private float minX, maxX, minY, maxY, zoomSize, initialOrthographicSize, zoomSensitivity = 10.0f;
    private new Camera camera;
    private bool drag, zoom;
    private Vector3 initialTouchPosition, initialCameraPosition, initialTouch0Position, initialTouch1Position, initialMidPointScreen;

    void Start() {
        Input.simulateMouseWithTouches = true;
        camera = GetComponent<Camera>();
        zoomSize = camera.orthographicSize;
    }

    void Update() {
        if (Game.Pause) goto SKIP;

        // Scroll
        if (Input.GetMouseButton(0) && Input.touchCount < 2) {
            zoom = false;
            Vector3 touch0 = Input.mousePosition;

            if (!drag) {
                initialTouchPosition = touch0;
                initialCameraPosition = transform.position;
                drag = true;
            }
            else {
                Vector2 delta = camera.ScreenToWorldPoint(touch0) - camera.ScreenToWorldPoint(initialTouchPosition);
                Vector3 newPos = initialCameraPosition;
                newPos.x -= delta.x;
                newPos.y -= delta.y;
                transform.position = newPos;
            }
        }
        else drag = false;


        // Zoom
        var scrollWhell = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWhell != 0) {
            zoomSize -= scrollWhell * zoomSensitivity;
        }
        else if (Input.touchCount == 2) {
            drag = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (!zoom) {
                initialTouch0Position = touch0.position;
                initialTouch1Position = touch1.position;
                initialCameraPosition = transform.position;
                initialOrthographicSize = camera.orthographicSize;
                initialMidPointScreen = (touch0.position + touch1.position) / 2;

                zoom = true;
            }
            else {
                transform.position = initialCameraPosition;
                camera.orthographicSize = initialOrthographicSize;

                float scaleFactor = GetScaleFactor(touch0.position,
                                                   touch1.position,
                                                   initialTouch0Position,
                                                   initialTouch1Position);

                Vector2 currentMidPoint = (touch0.position + touch1.position) / 2;
                Vector3 initialPointWorldBeforeZoom = camera.ScreenToWorldPoint(initialMidPointScreen);

                zoomSize = initialOrthographicSize / scaleFactor;
                zoomSize = Mathf.Clamp(zoomSize, minzoom, maxzoom);
                camera.orthographicSize = zoomSize;

                Vector3 initialPointWorldAfterZoom = camera.ScreenToWorldPoint(initialMidPointScreen);
                Vector2 initialPointDelta = initialPointWorldBeforeZoom - initialPointWorldAfterZoom;

                Vector2 oldAndNewPointDelta =
                    camera.ScreenToWorldPoint(currentMidPoint) -
                    camera.ScreenToWorldPoint(initialMidPointScreen);

                Vector3 newPos = initialCameraPosition;
                newPos.x -= oldAndNewPointDelta.x - initialPointDelta.x;
                newPos.y -= oldAndNewPointDelta.y - initialPointDelta.y;

                transform.position = newPos;
            }
        }
        else {
            zoom = false;
        }

        SKIP:

        // Card limit
        CalcMinMax();
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), -50f);
    }


    void LateUpdate() {
        zoomSize = Mathf.Clamp(zoomSize, minzoom, maxzoom);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomSize, Time.deltaTime * speedzoom);
    }


    void CalcMinMax() {
        maxX = (maxzoomX - camera.orthographicSize) * 4 / 3;
        minX = -maxX;

        maxY = maxzoomY - camera.orthographicSize;
        minY = -maxY;
    }

    public static float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2) {
        float distance = Vector2.SqrMagnitude(position1 - position2);
        float oldDistance = Vector2.SqrMagnitude(oldPosition1 - oldPosition2);

        if (oldDistance == 0 || distance == 0) {
            return 1.0f;
        }

        return Mathf.Sqrt(distance / oldDistance);
    }
}