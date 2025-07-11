using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Camera fpsCamera;
    public WeaponData weaponData;
    public int currentAmmo;
    public int totalAmmo;

    private BoxCollider[] colliders;
    private float _range;
    private Rigidbody rb;
    private float nextTimeToFire = 0f;
    private bool isReloading;
    private bool isHeld;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<BoxCollider>();

        currentAmmo = weaponData.maxAmmo;
        _range = weaponData.range;

    }

    private void Update()
    {
        if (isReloading || !isHeld) return;

        if (Input.GetMouseButton(0) && currentAmmo >= 1 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + weaponData.fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }

        if ((Input.GetKeyDown(KeyCode.R) && totalAmmo >= 1) || (currentAmmo < 1 && totalAmmo >= 1))
        {
            StartCoroutine(Reload());
            return;
        }

    }

    private void Shoot()
    {
        currentAmmo--;
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, _range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(weaponData.damage);
                }
            }
            Debug.Log(hit.collider.name);
        }
    }
    
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponData.reloadDuration);

        if(weaponData.maxAmmo - currentAmmo <= totalAmmo)
        {
            totalAmmo -= weaponData.maxAmmo - currentAmmo;
        currentAmmo += weaponData.maxAmmo - currentAmmo;
        }
        else
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }


        isReloading = false;
    }


    private void Drop()
    {

        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        isHeld = false;
    }

    public void PickUp(Transform holder)
    {
        isHeld = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
    }
}
