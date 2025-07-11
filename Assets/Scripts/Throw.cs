using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Throw : MonoBehaviour
{
    public GameObject defaultPrefab;
    public float force = 10f;
    public float upwardForce = 5f;
    public float spawnDepth = 10f;
    private float tilt = 15f;
    private int throwCount= 1;

    private GameObject currentPrefabToThrow;

    private Camera cam;

    private void Start()
    {
        currentPrefabToThrow = defaultPrefab;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartCoroutine(Throw());
        }

    IEnumerator Throw()
    {
       
            for (int i = 0; i < throwCount; i++)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = spawnDepth;
                Vector3 targetPosition = cam.ScreenToWorldPoint(mousePosition);
                Vector3 launchDirection = (targetPosition - transform.position).normalized;

                float randomX = Random.Range(-tilt, tilt);
                float randomY = Random.Range(-tilt, tilt);
                float randomZ = Random.Range(-tilt, tilt);
                Quaternion randomTilt = Quaternion.Euler(randomX, randomY, randomZ);

                GameObject newObject = Instantiate(currentPrefabToThrow, transform.position, randomTilt);
                Rigidbody rb = newObject.GetComponent<Rigidbody>();
                rb.linearVelocity = (launchDirection * force) + (Vector3.up * upwardForce);
                yield return new WaitForSeconds(0.01f);
            }
        }

    }

    public void prefabToThrow(GameObject prefabToSet)
    {
        currentPrefabToThrow = prefabToSet;
    }

    public void howManyToThrow(int number)
    {
        throwCount = number;
    }


}
