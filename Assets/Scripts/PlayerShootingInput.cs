using UnityEngine;

public class PlayerShootingInput : MonoBehaviour
{
    public Transform gunPivot;
    public Transform bodySprite;
    public GunController gun;

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)gunPivot.position).normalized;

        gunPivot.up = dir;

        if (dir.x != 0)
            bodySprite.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            gun.TryShoot(dir);
        }
    }
}
