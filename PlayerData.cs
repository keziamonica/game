using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int currentHealth;
    public float[] position;
    public int coins;

    public PlayerData (Player player)
    {
        level = player.level;
        currentHealth = player.currentHealth;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        coins = player.cm.coinCount;
    }
}
