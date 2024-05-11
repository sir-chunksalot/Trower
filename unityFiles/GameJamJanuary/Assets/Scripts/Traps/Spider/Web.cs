using UnityEngine;

public class Web : MonoBehaviour
{

    [SerializeField] GameObject particles;
    [SerializeField] bool isRight;
    private void OnEnable()
    {
        if (!isRight)
        {
            Debug.Log("left" + isRight);
            Quaternion rot = Quaternion.Euler(particles.transform.rotation.x, particles.transform.rotation.x, 270);
            particles.transform.rotation = rot;

        }
        if (isRight)
        {
            Debug.Log("RIGHT");
            Quaternion rot = Quaternion.Euler(particles.transform.rotation.x, particles.transform.rotation.x, 0);
            particles.transform.rotation = rot;
        }

        particles.GetComponent<ParticleSystem>().Clear();
        particles.GetComponent<ParticleSystem>().Play();
    }
}
