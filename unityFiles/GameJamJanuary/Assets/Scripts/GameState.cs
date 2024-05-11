using MoreMountains.Feedbacks;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] GameObject BuildingPhaseUI;
    [SerializeField] float cameraSize;
    [SerializeField] float cameraHeight;
    [SerializeField] GameObject test;
    private bool isBuildingPhase; //whenever its not building phase its attack phase
    void Start() //there are two different game states. battling and building. building phase switches over when the player hits continue, attacking phase switches over when all enemies are dead.
    {
        Vector3 buildPhaseStartPos = new Vector3(0, cameraHeight, 0);
        Camera.main.orthographicSize = cameraSize;
        Camera.main.GetComponent<CameraController>().SetCameraPos(buildPhaseStartPos);
        Coins.SetCoin(1000);
        Coins.SetMaterial(10000);
        PhaseSwitch();//this switches it to build phase

        //test.GetComponent<MMFloatingTextSpawner>().spaw;
    }

    public bool IsBuildPhase()
    {
        return isBuildingPhase;
    }

    private void AttackPhase()
    {
        BuildingPhaseUI.SetActive(false);
    }

    private void BuildPhase()
    {
        BuildingPhaseUI.SetActive(true);
        //List<GameObject> nextWave = this.GetComponent<HeroSpawner>().WaveGenerate(0, 10, 1);
    }

    public void PhaseSwitch()
    {
        if (isBuildingPhase)
        {
            isBuildingPhase = false;
            AttackPhase();
        }
        else
        {
            isBuildingPhase = true;
            BuildPhase();
        }
    }



}
