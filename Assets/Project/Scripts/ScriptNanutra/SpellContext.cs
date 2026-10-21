using System.Collections.Generic;
using UnityEngine;

public class SpellContext
{
    public SpellData Data;
    public List<SpellModule> Modules;   // copie à chaque cast !
    public GameObject Caster;
    public Vector2 Direction;
    public List<Augmentation> Augmentations;
    public float DamageMultiplier = 1f;
    // ... état runtime (projectiles spawnés, timers, etc.)

    /*
    public void AddTimer(float delay, Action callback)
    {
        timers.Add(new Timer { timeLeft = delay, onDone = callback });
    }
    */
}