using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MInvisibleBossData : MInvisibleSpaceshipData {
	[Header ("boss data")]
	public MMineData mineSpawn;
	public MSpaceshipData fighterSpawn;

	protected override PolygonGameObject CreateInternal (int layer) {
		return ObjectsCreator.CreateBossInvisibleSpaceship<SpaceShip>(this, layer);
	}
}