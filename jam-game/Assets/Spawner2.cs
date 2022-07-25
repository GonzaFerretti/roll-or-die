using UnityEngine;

public class Spawner2 : MonoBehaviour
{
    [SerializeField]
    private GameObject d8, d20;
    private float time = 0.0f;
    public float interpolationPeriod;

    private void Start()
    {
        int numberOfEnemies = Random.Range(0, 3);

        for (int i = 0; i <= numberOfEnemies; i++)
        {
            int enemy = Random.Range(0, 2);
            GameObject spawn = Instantiate(enemy == 0 ? d20 : d8, transform);
            spawn.transform.position = spawn.transform.position + new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), 1);
        }
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time >= interpolationPeriod)
        {
            time = 0.0f;

            int numberOfEnemies = Random.Range(0, 3);

            for(int i = 0; i <= numberOfEnemies; i++)
            {
                int enemy = Random.Range(0, 2);
                GameObject spawn = Instantiate(enemy == 0 ? d20 : d8, transform);
                spawn.transform.position = spawn.transform.position + new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), 1);
            }
        }
    }
}
