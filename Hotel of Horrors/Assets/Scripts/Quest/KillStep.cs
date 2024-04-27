using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class KillStep : QuestStep
{
    public string EnemyName { get; set; }

    public KillStep(string enemyName, string desc, bool completed, int currentAmo, int requiredAmo)
    {
        this.EnemyName = enemyName;
        this.Description = desc;
        this.Completed = completed;
        this.CurrentAmount = currentAmo;
        this.RequiredAmount = requiredAmo;
    }

    public override void Init()
    {
        base.Init();
        EnemyStateMachine.OnEnemyDeath += EnemyDied;
    }

    private void EnemyDied(object sender, EventArgs e)
    {
        EnemyStateMachine enemy = (EnemyStateMachine)sender;
        if (enemy.gameObject.name.Contains(EnemyName))
        {
            this.CurrentAmount++;
            Evaluate();
        }
    }
}
