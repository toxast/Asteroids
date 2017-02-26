using UnityEngine;
using System.Collections;

public class PowerUp : polygonGO.DropBase 
{
	public EffectType effect;
	public void InitPowerUp(EffectType effect)
	{
		this.effect = effect;
	}
    
    public override void OnUserInteracted() {
        Singleton<Main>.inst.ApplyPowerUP(effect);
    }
}

public enum EffectType
{
	GravityShield,
	GunsShow1,
	PhysicalChanges1,
	BackupTest,
	HeavvyBulletTest,
	ExtraGunTest,
}
