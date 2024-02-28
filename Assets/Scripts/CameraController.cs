using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour {

    [Header("Speed Settings")]
    public float speedMove;
    public float speedRotate;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    private float currentMoveSpeed;
	
    [Header("Shake Settings")]
    public float shakeDuration;
    public float shakeAmount;

    private bool shaking;
    private float elapsedTime;

    Quaternion originalRotXY;
	
    void Start () {
        initialPosition = new Vector3 (0, 14, -8);
        initialRotation = new Vector3 (60, 0, 0);

        currentMoveSpeed = 0;
    }

    void Update () {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < shakeDuration && shaking) {
            Vector3 rotXY = Random.insideUnitCircle * shakeAmount;
            transform.Rotate (rotXY);
        } else if (shaking) {
            transform.rotation = originalRotXY;
            shaking = false;
        }
    }
	
    void LateUpdate ()
    {
        var player = MultiplayerManager.Instance.GetLocalPlayer();
        if (!player)
            return;

        var playerStats = player.GetComponent<PlayerStats>();
        
        if (currentMoveSpeed < speedMove) {
            currentMoveSpeed += 0.1f;
        }

        transform.position = Vector3.MoveTowards (transform.position, player.transform.position + initialPosition, Time.deltaTime * playerStats.Speed.Value * currentMoveSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(initialRotation), Time.deltaTime * speedRotate);
    }

    public void Shake(float duration, float amount)
    {
        originalRotXY = transform.rotation;
        shakeDuration = duration;
        shakeAmount = amount;
        shaking = true;
        elapsedTime = 0;
    }
}