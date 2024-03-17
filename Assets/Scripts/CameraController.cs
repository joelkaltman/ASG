using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour {

    public Transform initialCameraTransform;
    
    [Header("Speed Settings")]
    public float speedMove;
    public float speedRotate;
	
    [Header("Shake Settings")]
    public float shakeDuration;
    public float shakeAmount;

    private Vector3 gameplayPosition = new (0, 14, -8);
    private Vector3 gameplayRotation = new (60, 0, 0);
    
    private float currentMoveSpeed;
    private bool shaking;
    private float elapsedTime;

    Quaternion originalRotXY;
	
    void Start () 
    {
        currentMoveSpeed = 0;
        MultiplayerManager.Instance.OnLocalPlayerReady += OnPlayerRead;
    }

    void OnPlayerRead(GameObject player)
    {
        transform.SetPositionAndRotation(initialCameraTransform.position, initialCameraTransform.rotation);
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

        transform.position = Vector3.MoveTowards (transform.position, player.transform.position + gameplayPosition, Time.deltaTime * playerStats.Speed.Value * currentMoveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(gameplayRotation), Time.deltaTime * speedRotate);
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