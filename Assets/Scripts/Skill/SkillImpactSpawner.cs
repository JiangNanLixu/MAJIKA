﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ImpactSpawnType
{
    Default,
    OnGround,
    Barrage
}

public class SkillImpactSpawner : EntityBehaviour
{
    public GameObject Prefab;
    public ImpactSpawnType SpawnType = ImpactSpawnType.Default;
    public Vector2 SpawnOffset;
    public float TimeOffset = 0;
    public SkillImpact Instance;

    [Range(1, 16)]
    public int SpawnAmount;
    public Vector2 SpawnSectorDirection = Vector2.right;

    [Range(0, 360)]
    public float SpawnAngle = 60;

    [HideInInspector]
    [MAJIKA.Utils.StatusEffect]
    public List<EffectInstance> AdditionalEffect = new List<EffectInstance>();

    [HideInInspector]
    public Vector2[] SpawnDirections;

    public void SpawnSubImpact()
    {
        var impact = GetComponent<SkillImpact>();
        if (!impact)
            return;
        var dir = MathUtility.SignInt(impact.Direction.x);
        var target = impact.TargetEntity;
        var creator = impact.Creator;
        if (TimeOffset > 0)
        {
            StartCoroutine(SpawnCoroutine(AdditionalEffect.ToList(), dir, target, creator));
        }
        else
        {
            DoSpawn(AdditionalEffect.ToList(), dir, target, creator);
        }
    }

    IEnumerator SpawnCoroutine(List<EffectInstance> effects, float dir, GameEntity target, GameEntity creator)
    {
        yield return new WaitForSeconds(TimeOffset);
        DoSpawn(effects, dir, target, creator);
    }

    public void Spawn(List<EffectInstance> effects, float dir, GameEntity target)
    {
        dir = MathUtility.SignInt(dir);
        effects = effects.Union(AdditionalEffect).ToList();
        if(TimeOffset>0)
        {
            StartCoroutine(SpawnCoroutine(effects, dir, target, Entity));
        }
        else
        {
            DoSpawn(effects, dir, target, Entity);
        }
    }

    private SkillImpact SpawnOne(Vector3 pos, Vector3 dir, List<EffectInstance> effects, GameEntity target, GameEntity creator)
    {
        var impact = Utility.Instantiate(Prefab, gameObject.scene).GetComponent<SkillImpact>();
        impact.Creator = creator;
        impact.Effects = effects;
        if (SpawnType == ImpactSpawnType.OnGround)
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            var filter = new ContactFilter2D
            {
                layerMask = (1 << 8) | (1 << 9),
                useLayerMask = true
            };
            if (Physics2D.Raycast(pos.ToVector2(), Vector2.down, filter, hits) > 0)
            {
                pos = hits[0].point.ToVector3();
            }
        }
        impact.Activate(pos, dir, target);
        return impact;
    }

    private void DoSpawn(List<EffectInstance> effects, float dir, GameEntity target, GameEntity creator)
    {
        if(SpawnType == ImpactSpawnType.Barrage)
        {
            SpawnDirections.ForEach(spawnDir =>
            {
                var direction = spawnDir.ToVector3();
                direction.x *= dir;
                var offset = SpawnOffset;
                offset.x *= dir;
                var spawnPos = transform.position + SpawnOffset.ToVector3();
                SpawnOne(spawnPos, direction, effects, target, creator);
            });
        }
        else
        {
            Vector3 direction = Vector3.right * dir;
            var offset = SpawnOffset;
            offset.x *= dir;
            var pos = transform.position + offset.ToVector3();
            Instance = SpawnOne(pos, direction, effects, target, creator);
        }
    }

    public void Destory()
    {
        if (Instance)
            Destroy(Instance.gameObject);
    }

    public void Deactivate()
    {
        if (Instance)
            Instance.Deactivate();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + SpawnOffset.ToVector3(), 0.0625f);
    }
}
