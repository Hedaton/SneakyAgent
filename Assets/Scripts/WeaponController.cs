using System.Collections;
using System.Linq.Expressions;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class WeaponController : MonoBehaviour, IEquippable
{
    [Header("General Settings")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AnimatorOverrideController overrideController;

    [Header("Controller Settings")]
    public bool isHeldByPlayer = true;

    [Header("Player Special References")]
    public Camera fpsCamera;
    public UIManager uiManager;


    public int currentAmmo;
    public int totalAmmo;

    private WeaponData weaponData;
    private Animator animator;
    private AudioSource audioSource;
    private Coroutine reloadCoroutine;
    private float nextTimeToFire = 0f;
    private bool isReloading;
    private bool isEquipped;

    public float FireRate => weaponData.fireRate;

    private void Awake()
    {
        Item item = GetComponent<Item>();
        weaponData = item.itemData as WeaponData;
        if (weaponData == null)
        {
            Debug.LogError("WeaponData is not set on the item.");
            enabled = false;
            return;
        }
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (overrideController != null)
            animator.runtimeAnimatorController = overrideController;

        currentAmmo = weaponData.maxAmmo;

        if (!isHeldByPlayer)
        {
            isEquipped = true;
        }

    }

    private void Update()
    {
        if (!isEquipped || isReloading || !isHeldByPlayer) return;

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }


    }

    public void Shoot()
    {
        if (!isHeldByPlayer) return;

        ShootInDirection(fpsCamera.transform.position, fpsCamera.transform.forward);
        
    }

    public void ShootInDirection(Vector3 position, Vector3 direction)
    {
        if (currentAmmo <= 0 && totalAmmo > 0)
        {
            Reload();
            return;
        }

        if (Time.time < nextTimeToFire || isReloading || currentAmmo <= 0) return;
        nextTimeToFire = Time.time + weaponData.fireRate;
        currentAmmo--;

        animator.SetTrigger("Shoot");
        audioSource.PlayOneShot(shootSound);

        RaycastHit hit;
        if (Physics.Raycast(position, direction, out hit, weaponData.range))
        {
            if (hit.collider.TryGetComponent<Health>(out Health health))
            {
                if (health.gameObject != this.transform.root.gameObject)
                {
                    health.TakeDamage(weaponData.damage);
                }
            }
        }
        UpdateAmmo();
    }

    public void Reload()
    {
        if (isReloading || totalAmmo <= 0 || currentAmmo == weaponData.maxAmmo) return;

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        animator.SetTrigger("Reload");
        audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(weaponData.reloadDuration);

        int ammoNeeded = weaponData.maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);

        currentAmmo += ammoToReload;
        totalAmmo -= ammoToReload;

        UpdateAmmo();
        isReloading = false;
        reloadCoroutine = null;
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;

            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            if (animator != null)
            {
                animator.Rebind();
            }
        }
    }

    public void OnEquip()
    {
        if (!isHeldByPlayer) return;
        isEquipped = true;
        uiManager.AmmoDisplay(true);
        UpdateAmmo();
    }

    public void OnUnequip()
    {
        if (!isHeldByPlayer) return;
        isEquipped = false;
        CancelReload();
        if (uiManager != null) uiManager.AmmoDisplay(false);
    }




    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        UpdateAmmo();
    }

    private void UpdateAmmo()
    {
        if (isHeldByPlayer && uiManager != null)
        {
            uiManager.UpdateAmmoDislay(currentAmmo, totalAmmo);
        }
    }

}
