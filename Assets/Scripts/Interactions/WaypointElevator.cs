using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(Rigidbody))]
public class WaypointElevator : MonoBehaviour
{
    [System.Serializable]
    public class MovingPart
    {
        public Transform partTransform;
        public Vector3 localMoveOffset;

        [HideInInspector] public Vector3 closedLocalPos;
        [HideInInspector] public Vector3 openLocalPos;

        public void Init()
        {
            if (partTransform == null) return;
            closedLocalPos = partTransform.localPosition;
            openLocalPos = closedLocalPos + localMoveOffset;
        }

        public bool MoveTowardsTarget(bool open, float speed, float deltaTime)
        {
            if (partTransform == null) return true;

            Vector3 target = open ? openLocalPos : closedLocalPos;
            partTransform.localPosition = Vector3.MoveTowards(
                partTransform.localPosition,
                target,
                speed * deltaTime);

            if (Vector3.Distance(partTransform.localPosition, target) < 0.001f)
            {
                partTransform.localPosition = target;
                return true;
            }
            return false;
        }
    }

    [Header("Path")]
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;

    [Header("Elevator Settings")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float startDelay = 5f;

    [Header("Doors (Child Objects)")]
    [SerializeField] private float doorDelay = 5f;
    [SerializeField] private float doorSpeed = 2.5f;
    [SerializeField] private MovingPart[] doors;

    [Header("Bridges (Child Objects)")]
    [SerializeField] private float bridgeSpeed = 5f;
    [SerializeField] private MovingPart[] bridges;

    [Header("Retractable Gate (External)")]
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private float gateDelay = 2.5f;
    [SerializeField] private string gateRetractBool = "IsRetracted";

    private Rigidbody _rb;
    private bool _isMoving = false;
    private bool _doorsMoving = false;
    private bool _doorsTargetOpen = false;
    private bool _bridgesMoving = false;
    private bool _bridgesTargetOpen = false;

    private Transform _currentTarget;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;

        foreach (var door in doors) door.Init();
        foreach (var bridge in bridges) bridge.Init();

        if (gateAnimator != null)
        {
            gateAnimator.keepAnimatorStateOnDisable = true;
            gateAnimator.enabled = false;
        }

        transform.position = bottomPoint.position;
        Physics.SyncTransforms();

        _currentTarget = topPoint;
        StartCoroutine(ElevatorSequence());
    }

    private IEnumerator ElevatorSequence()
    {
        yield return new WaitForSeconds(startDelay);
        _isMoving = true;

        yield return new WaitUntil(() => !_isMoving);

        yield return new WaitForSeconds(doorDelay);
        _doorsTargetOpen = true;
        _doorsMoving = true;
        _bridgesTargetOpen = true;
        _bridgesMoving = true;
        yield return new WaitUntil(() => !_doorsMoving && !_bridgesMoving);

        yield return new WaitForSeconds(gateDelay);
        if (gateAnimator != null)
        {
            gateAnimator.enabled = true;
            gateAnimator.SetBool(gateRetractBool, true);
        }
    }

    public void StartReturnSequence()
    {
        StartCoroutine(ReturnSequenceCoroutine());
    }

    private IEnumerator ReturnSequenceCoroutine()
    {
        if (gateAnimator != null)
        {
            gateAnimator.SetBool(gateRetractBool, false);
            StartCoroutine(DisableAnimatorAfterClose());
        }
        yield return new WaitForSeconds(gateDelay);

        _bridgesTargetOpen = false;
        _bridgesMoving = true;
        yield return new WaitUntil(() => !_bridgesMoving);

        yield return new WaitForSeconds(doorDelay * 0.2f);

        _doorsTargetOpen = false;
        _doorsMoving = true;
        yield return new WaitUntil(() => !_doorsMoving);

        yield return new WaitForSeconds(startDelay);

        _currentTarget = bottomPoint;
        _isMoving = true;
        yield return new WaitUntil(() => !_isMoving);

        this.enabled = false;
    }

    private IEnumerator DisableAnimatorAfterClose()
    {
        yield return null;

        yield return new WaitUntil(() =>
        !gateAnimator.IsInTransition(0) &&
        gateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        gateAnimator.enabled = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _currentTarget.position,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _currentTarget.position) < 0.001f)
            {
                transform.position = _currentTarget.position;
                _isMoving = false;
            }
        }

        if (_doorsMoving)
        {
            bool allDone = true;
            foreach (var door in doors)
            {
                if (!door.MoveTowardsTarget(_doorsTargetOpen, doorSpeed, Time.deltaTime))
                    allDone = false;
            }
            Physics.SyncTransforms();
            if (allDone) _doorsMoving = false;
        }

        if (_bridgesMoving)
        {
            bool allDone = true;
            foreach(var bridge in bridges)
            {
                if (!bridge.MoveTowardsTarget(_bridgesTargetOpen, bridgeSpeed, Time.deltaTime))
                    allDone = false;
            }
            Physics.SyncTransforms();
            if (allDone) _bridgesMoving = false;
        }
    }
}