using GD.MinMaxSlider;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField]
    public bool buildPhase;
    [SerializeField]
    public bool finalWave;
    [MinMaxSlider(0.0f, 999)]
    public Vector2Int requiredKills;
    [MinMaxSlider(0.0f, 999)]
    public Vector2Int waveLength;
    [SerializeField]
    public float earnedCoins;



    public Hero GetHero(string name)
    {
        switch (name)
        {
            case "PrisonGuard":
                return prisonGuard;
            case "DoorSmackGuard":
                return doorSmackGuard;
            case "Knight":
                return knight;
            default:
                return null;
        }
    }

    public void SetHeroSprite(string name, Sprite sprite)
    {
        switch (name)
        {
            case "PrisonGuard":
                prisonGuardSprite = sprite;
                break;
            case "DoorSmackGuard":
                doorSmackGuardSprite = sprite;
                break;
            case "Knight":
                knightSprite = sprite;
                break;
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

        public void SetCanSpawn(bool state)
        {
            canSpawn = state;
        }
        public bool GetCanSpawn()
        {
            return canSpawn;
        }
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
    public Sprite doorSmackGuardSprite;

    [System.Serializable]
    public class DoorsSmackGuard : Hero
    {
    }
    public DoorsSmackGuard doorSmackGuard;
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
    public class Ranger : Hero
    {
    }
    public Ranger ranger;
    //---------------------------------------------------------------------------------------------//
}
