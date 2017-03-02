using UnityEngine;
using System.Collections;

public class PowerUp : polygonGO.DropBase 
{
	public PowerUpEffect effect;
	public void InitPowerUp(PowerUpEffect effect)
	{
		this.effect = effect;
	}
    
    public override void OnUserInteracted() {
        Singleton<Main>.inst.ApplyPowerUP(effect);
    }
}

public enum PowerUpEffect
{
	GravityShield,
	GunsShow1,
	PhysicalChanges1,
	BackupTest,
	HeavvyBulletTest,
	ExtraGunTest,
	TimeSlowTest,
	ShieldObjsTest,
}
