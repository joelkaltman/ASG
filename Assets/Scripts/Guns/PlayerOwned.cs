using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerOwned : ServerOnlyMonobehavior
{
    [HideInInspector] public GameObject player;
}
