using System;
using Cinemachine;
using Invector;
using UnityEngine;

public class vThirdPersonCamera : MonoBehaviour
{
    [Serializable]
    public struct PositionSettings
    {
        public float rightOffset;
        public float defaultDistance;
        public float height;
    }
    
    #region inspector properties    

    public Transform target;
    [Tooltip("Lerp speed between Camera States")]
    public float smoothCameraRotation = 12f;
    [Tooltip("What layer will be culled")]
    public LayerMask cullingLayer = 1 << 0;
    [Tooltip("Debug purposes, lock the camera behind the character for better align the states")]
    public bool lockCamera;
    
    public PositionSettings defaultSettings;
    public PositionSettings aimingSettings;
    
    public float rightOffsetSmooth = 0.1f;
    public float defaultDistanceSmooth = 0.1f;
    public float heightSmooth = 0.1f;

    public bool isAiming;
    public float smoothFollow = 10f;
    public float xMouseSensitivity = 3f;
    public float yMouseSensitivity = 3f;
    public float yMinLimit = -40f;
    public float yMaxLimit = 80f;

    public float dodgeSwayHorizontal = 1.5f;
    public float dodgeSwayVertical = 1.5f;

    #endregion

    #region hide properties    

    [HideInInspector]
    public int indexList, indexLookPoint;
    [HideInInspector]
    public float offSetPlayerPivot;
    [HideInInspector]
    public string currentStateName;
    [HideInInspector]
    public Transform currentTarget;
    [HideInInspector]
    public Vector2 movementSpeed;

    private float rightOffset;
    private float defaultDistance;
    private float height;

    private float rightOffsetCv;
    private float defaultDistanceCv;
    private float heightCv;

    
    private Transform targetLookAt;
    private Vector3 currentTargetPos;
    private Vector3 lookPoint;
    private Vector3 current_cPos;
    private Vector3 desired_cPos;
    private CinemachineVirtualCamera _camera;
    private float distance = 5f;
    public float mouseY = 0f;
    public float mouseX = 0f;
    private float currentHeight;
    private float cullingDistance;
    private float checkHeightRadius = 0.4f;
    private float clipPlaneMargin = 0f;
    private float forward = -1f;
    private float xMinLimit = -360f;
    private float xMaxLimit = 360f;
    private float cullingHeight = 0.2f;
    private float cullingMinDist = 0.1f;

    private Camera _main;

    #endregion

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (target == null)
            return;

        rightOffset = defaultSettings.rightOffset;
        defaultDistance = defaultSettings.defaultDistance;
        height = defaultSettings.height;
        
        _camera = GetComponent<CinemachineVirtualCamera>();
        _main = Camera.main;
        currentTarget = target;
        currentTargetPos = new Vector3(currentTarget.position.x, currentTarget.position.y + offSetPlayerPivot, currentTarget.position.z);

        targetLookAt = new GameObject("targetLookAt").transform;
        targetLookAt.position = currentTarget.position;
        targetLookAt.hideFlags = HideFlags.HideInHierarchy;
        targetLookAt.rotation = currentTarget.rotation;

        mouseY = currentTarget.eulerAngles.x;
        mouseX = currentTarget.eulerAngles.y;

        distance = defaultDistance;
        currentHeight = height;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (target == null || targetLookAt == null) return;

        isAiming = Player.instance.isAiming;
        var settings = isAiming ? aimingSettings : defaultSettings;

        rightOffset = Mathf.SmoothDamp(rightOffset, settings.rightOffset, ref rightOffsetCv, rightOffsetSmooth);
        defaultDistance = Mathf.SmoothDamp(defaultDistance, settings.defaultDistance, ref defaultDistanceCv, defaultDistanceSmooth);
        height = Mathf.SmoothDamp(height, settings.height, ref heightCv, heightSmooth);
        CameraMovement();
    }

    /// <summary>
    /// Set the target for the camera
    /// </summary>
    /// <param name="New cursorObject"></param>
    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget ? newTarget : target;
    }

    public void SetMainTarget(Transform newTarget)
    {
        target = newTarget;
        currentTarget = newTarget;
        mouseY = currentTarget.rotation.eulerAngles.x;
        mouseX = currentTarget.rotation.eulerAngles.y;
        Init();
    }

    /// <summary>    
    /// Convert a point in the screen in a Ray for the world
    /// </summary>
    /// <param name="Point"></param>
    /// <returns></returns>
    public Ray ScreenPointToRay(Vector3 Point)
    {
        return this.GetComponent<Camera>().ScreenPointToRay(Point);
    }

    /// <summary>
    /// Camera Rotation behaviour
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void RotateCamera(float x, float y)
    {
        // free rotation 
        mouseX += x * xMouseSensitivity;
        mouseY -= y * yMouseSensitivity;

        movementSpeed.x = x;
        movementSpeed.y = -y;
        if (!lockCamera)
        {
            mouseY = vExtensions.ClampAngle(mouseY, yMinLimit, yMaxLimit);
            mouseX = vExtensions.ClampAngle(mouseX, xMinLimit, xMaxLimit);
        }
        else
        {
            mouseY = currentTarget.root.localEulerAngles.x;
            mouseX = currentTarget.root.localEulerAngles.y;
        }
    }

    public void SetRotation(Quaternion rot)
    {
        var euler = rot.eulerAngles;
        mouseY = euler.x;
        mouseX = euler.y;
    }

    /// <summary>
    /// Camera behaviour
    /// </summary>    
    void CameraMovement()
    {
        if (currentTarget == null)
            return;

        distance = Mathf.Lerp(distance, defaultDistance, smoothFollow * Time.deltaTime);
        cullingDistance = Mathf.Lerp(cullingDistance, distance, Time.deltaTime);
        var camDir = (forward * targetLookAt.forward) + (rightOffset * targetLookAt.right);

        camDir = camDir.normalized;

        var targetPos = new Vector3(currentTarget.position.x, currentTarget.position.y + offSetPlayerPivot, currentTarget.position.z);
        currentTargetPos = targetPos;
        desired_cPos = targetPos + new Vector3(0, height, 0);
        current_cPos = currentTargetPos + new Vector3(0, currentHeight, 0);
        RaycastHit hitInfo;

        ClipPlanePoints planePoints = _main.NearClipPlanePoints(current_cPos + (camDir * (distance)), clipPlaneMargin);
        ClipPlanePoints oldPoints = _main.NearClipPlanePoints(desired_cPos + (camDir * distance), clipPlaneMargin);

        //Check if Height is not blocked 
        if (Physics.SphereCast(targetPos, checkHeightRadius, Vector3.up, out hitInfo, cullingHeight + 0.2f, cullingLayer))
        {
            var t = hitInfo.distance - 0.2f;
            t -= height;
            t /= (cullingHeight - height);
            cullingHeight = Mathf.Lerp(height, cullingHeight, Mathf.Clamp(t, 0.0f, 1.0f));
        }

        //Check if desired target position is not blocked       
        if (CullingRayCast(desired_cPos, oldPoints, out hitInfo, distance + 0.2f, cullingLayer, Color.blue))
        {
            distance = hitInfo.distance - 0.2f;
            if (distance < defaultDistance)
            {
                var t = hitInfo.distance;
                t -= cullingMinDist;
                t /= cullingMinDist;
                currentHeight = Mathf.Lerp(cullingHeight, height, Mathf.Clamp(t, 0.0f, 1.0f));
                current_cPos = currentTargetPos + new Vector3(0, currentHeight, 0);
            }
        }
        else
        {
            currentHeight = height;
        }
        //Check if target position with culling height applied is not blocked
        if (CullingRayCast(current_cPos, planePoints, out hitInfo, distance, cullingLayer, Color.cyan)) distance = Mathf.Clamp(cullingDistance, 0.0f, defaultDistance);
        var lookPoint = current_cPos + targetLookAt.forward * 2f;
        lookPoint += (targetLookAt.right * Vector3.Dot(camDir * (distance), targetLookAt.right));
        targetLookAt.position = current_cPos;

        var localDodgeVel = transform.InverseTransformVector(Player.instance.currentDodgeVelocity);
        var verticalSway = localDodgeVel.z;
        var horizontalSway = localDodgeVel.x;

        verticalSway *= dodgeSwayVertical;
        horizontalSway *= dodgeSwayHorizontal;

        
        Quaternion newRot = Quaternion.Euler(mouseY + verticalSway, mouseX + horizontalSway, 0);
        targetLookAt.rotation = Quaternion.Slerp(targetLookAt.rotation, newRot, smoothCameraRotation * Time.deltaTime);
        transform.position = current_cPos + (camDir * (distance));
        var rotation = Quaternion.LookRotation((lookPoint) - transform.position);

        transform.rotation = rotation;
        movementSpeed = Vector2.zero;
    }

    /// <summary>
    /// Custom Raycast using NearClipPlanesPoints
    /// </summary>
    /// <param name="_to"></param>
    /// <param name="from"></param>
    /// <param name="hitInfo"></param>
    /// <param name="distance"></param>
    /// <param name="cullingLayer"></param>
    /// <returns></returns>
    bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask cullingLayer, Color color)
    {
        bool value = false;

        if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer))
        {
            value = true;
            cullingDistance = hitInfo.distance;
        }

        if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer))
        {
            value = true;
            if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
        }

        if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer))
        {
            value = true;
            if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
        }

        if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer))
        {
            value = true;
            if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
        }

        return hitInfo.collider && value;
    }
}
