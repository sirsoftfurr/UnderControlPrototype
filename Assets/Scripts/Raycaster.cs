using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public ParticleSystem splatParticles;
    public GameObject splatPrefab;
    public Transform splatHolder;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
    }

    private void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            bool isBG = hit.collider.CompareTag("BG");
            SpawnBlood(hit.point, isBG);
        }
    }
    
    public void SpawnBlood(Vector2 position, bool isBackground)
    {
        GameObject splat = Instantiate(splatPrefab, position, Quaternion.identity);
        splat.transform.SetParent(splatHolder, true);

        Splat splatScript = splat.GetComponent<Splat>();

        if (isBackground)
            splatScript.Initialize(Splat.SplatLocation.Background);
        else
            splatScript.Initialize(Splat.SplatLocation.Foreground);

        // 🔥 NEW: separate particle system per hit
        ParticleSystem ps = Instantiate(splatParticles, position, Quaternion.identity);
        ps.Play();

        Destroy(ps.gameObject, 2f); // cleanup
    }
}
