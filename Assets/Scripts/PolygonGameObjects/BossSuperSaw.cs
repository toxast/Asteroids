using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSuperSaw : SawEnemy {
    MSawBossData bossData;
    RotatingObjectsShield generatorsEffect;
    bool swapnedAnyGenerator = false;
    List<SpawnAxis> spawnAxises = new List<SpawnAxis>();
    public override void InitSawEnemy(MSawData data) {
        base.InitSawEnemy(data);
        bossData = data as MSawBossData;
        generatorsEffect = bossData.generatorsShieldData.Apply(this) as RotatingObjectsShield;
        this.SetShield(new ShieldData(99999, 9999, 0, 0));
    }

    public override void Tick(float delta) {
        base.Tick(delta);
        generatorsEffect.angleOffsetDeg = cacheTransform.eulerAngles.z;
        foreach (var s in spawnAxises) {
            s.Tick(delta);
        }
    }

    protected override bool DoCharge() {
        if (!swapnedAnyGenerator && generatorsEffect.AliveShieldObjects() > 0) {
            swapnedAnyGenerator = true;
        }
        if (!swapnedAnyGenerator) {
            return false;
        }
        bool anyAlive = generatorsEffect.AliveShieldObjects() > 0;

        if (!anyAlive && shield != null) {
            shield.DestroyShield();
            shield = null;
            StartChargeBeh();
        }

        return !anyAlive;
    }

    void StartChargeBeh() {
        spawnAxises.Add(new SpawnAxis(this, bossData.chargeSpawn, 0));
        spawnAxises.Add(new SpawnAxis(this, bossData.chargeSpawn, 120));
        spawnAxises.Add(new SpawnAxis(this, bossData.chargeSpawn, 240));
    }


    public class SpawnAxis {
        public PolygonGameObject holder;
        public MSpawnDataBase spawn;
        float angleOffset = 0; //deg angle
        float length;
        public float interval = 3;
        float timeUntilSpawn = 0;
        float rotationThreshold = 30f;

        public SpawnAxis(PolygonGameObject holder, MSpawnDataBase spawn, float angleOffset) {
            this.holder = holder;
            this.spawn = spawn;
            this.angleOffset = angleOffset;
            length = holder.polygon.R;
        }

        public void Tick(float delta) {
            timeUntilSpawn -= delta;
            if (timeUntilSpawn < 0 && Mathf.Abs(holder.rotation) > rotationThreshold && !Main.IsNull(holder.target)) {
                var basepos = holder.position;
                var edgePos = basepos + Math2d.RotateVertexDeg(new Vector2(length, 0), angleOffset + holder.cacheTransform.rotation.z); //fix
                var offset =  0.6f * (edgePos - basepos);
                var pivot = basepos + offset;
                var dirRotation = - Math2d.MakeRight(offset) * Mathf.Sign(holder.rotation);
                dirRotation = dirRotation.normalized;
                Debug.DrawRay(pivot, dirRotation);
                if (Vector2.Dot(dirRotation, (holder.target.position - pivot).normalized) > 0.9f) {
                    timeUntilSpawn = interval;
                    Spawn(0.8f, basepos, edgePos, dirRotation);
                    Spawn(0.6f, basepos, edgePos, dirRotation);
                    Spawn(0.4f, basepos, edgePos, dirRotation);
                }
            }
        }

        void Spawn(float a, Vector2 basepos, Vector2 edgePos, Vector2 dirNorm) { //a c [0, 1]
            var pos = basepos + a * (edgePos - basepos);
            float velocity = (a * length) * Mathf.Abs(holder.rotation) * (Mathf.PI / 180f);
            var shieldObj = spawn.Create(CollisionLayers.GetSpawnedLayer(holder.layerLogic));
            shieldObj.position = pos;
            shieldObj.cacheTransform.position = shieldObj.cacheTransform.position.SetZ(holder.cacheTransform.position.z + 1);
            shieldObj.velocity = dirNorm * velocity;
        }
    }
}
