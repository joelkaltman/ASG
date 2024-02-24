using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour {

    public GameObject player;
	
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
	
    public float CurrentMoveSpeed{
        set{ currentMoveSpeed = value; }
    }

    // Use this for initialization
    void Start () {
        initialPosition = new Vector3 (0, 14, -8);
        initialRotation = new Vector3 (60, 0, 0);

        currentMoveSpeed = 0;

        PlayerSpawn.Instance.AddListener(OnPlayerSpawn);
    }

    private void OnDestroy()
    {
        PlayerSpawn.Instance.RemoveListener(OnPlayerSpawn);
    }

    private void OnPlayerSpawn(GameObject spawned)
    {
        if (!GameData.Instance.isOnline || spawned.GetComponent<NetworkObject>().IsOwner)
            player = spawned;
    }

    void Update () {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < shakeDuration && shaking) {
            Vector3 rotXY = Random.insideUnitCircle * shakeAmount;
            this.transform.Rotate (rotXY);
        } else if (shaking) {
            this.transform.rotation = originalRotXY;
            shaking = false;
        }
    }
	
    // Update is called once per frame
    void LateUpdate ()
    {
        if (player == null)
            return;
        
        if (currentMoveSpeed < speedMove) {
            currentMoveSpeed += 0.1f;
        }

        this.transform.position = Vector3.MoveTowards (this.transform.position, player.transform.position + initialPosition, Time.deltaTime * PlayerStats.Instance.speed * currentMoveSpeed);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(initialRotation), Time.deltaTime * speedRotate);
    }

    public void Shake(float duration, float amount)
    {
        this.originalRotXY = this.transform.rotation;
        this.shakeDuration = duration;
        this.shakeAmount = amount;
        this.shaking = true;
        this.elapsedTime = 0;
    }
}