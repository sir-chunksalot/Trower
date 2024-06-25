using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class Wave : MonoBehaviour
{
	[SerializeField]
	public bool buildPhase;
	[MinMaxSlider(0.0f, 999)]
	public Vector2Int requiredKills; 


	public Hero GetHero(string name)
    {
		switch(name)
        {
			case "PrisonGuard":
				return prisonGuard;
			case "Knight":
				return knight;
			default:
				return null;
        }
    }

	public class Hero
    {
		[SerializeField]
		public bool canSpawn;

		[MinMaxSlider(0.0f, 1.0f)]
		public Vector2 spawnRate;

		[MinMaxSlider(0, 20)]
		public Vector2Int groupSize;

		[MinMaxSlider(0.0f, 40.0f)]
		public Vector2 cooldown;

		[MinMaxSlider(0.0f, 20.0f)]
		public Vector2 delay;
	}
	//---------------------------------------------------------------------------------------------//
	[PreviewSprite]
	public Sprite prisonGuardSprite;

	[System.Serializable]
	public class PrisonGuard : Hero
	{
	}
	public PrisonGuard prisonGuard;
	//---------------------------------------------------------------------------------------------//
	[PreviewSprite]
	public Sprite knightSprite;

	[System.Serializable]
	public class Knight : Hero
    {
    }
	public Knight knight;
	//---------------------------------------------------------------------------------------------//
	[PreviewSprite]
	public Sprite archerSprite;

	[System.Serializable]
	public class Ranger 
	{
		public bool canSpawn;

		[MinMaxSlider(0.0f, 1.0f)]
		public Vector2 spawnRate;

		[MinMaxSlider(0, 20)]
		public Vector2Int groupSize;

		[MinMaxSlider(0.0f, 40.0f)]
		public Vector2 cooldown;
	}
	public Ranger ranger;
	//---------------------------------------------------------------------------------------------//
}
