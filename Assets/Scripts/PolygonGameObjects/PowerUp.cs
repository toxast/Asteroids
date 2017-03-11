using UnityEngine;
using System.Collections;

public class PowerUp : polygonGO.DropBase 
{
	public IApplyable effect;
	public void InitPowerUp(IApplyable effect)
	{
		this.effect = effect;
	}
    
	public override void OnInteracted(PolygonGameObject picker) {
		effect.Apply (picker);
    }
}

//public enum PowerUpEffect
//{
//	GravityShield,
//	GunsShow1,
//	PhysicalChanges1,
//	BackupTest,
//	HeavvyBulletTest,
//	ExtraGunTest,
//	TimeSlowTest,
//	ShieldObjsTest,
//	HealingTest,
//}