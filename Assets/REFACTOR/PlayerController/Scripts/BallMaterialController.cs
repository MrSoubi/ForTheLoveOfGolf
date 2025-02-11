using UnityEngine;

public class BallMaterialController : MonoBehaviour
{
    public Renderer ballRenderer; // R�f�rence au Renderer de la balle
    public float textureSpeed = 0.05f; // Vitesse de d�calage de la texture

    private Rigidbody rb;
    private Material ballMaterial;
    private Vector2 uvOffset = Vector2.zero; // Stocke le d�placement UV

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (ballRenderer == null)
        {
            ballRenderer = GetComponent<Renderer>();
        }

        if (ballRenderer != null)
        {
            ballMaterial = ballRenderer.material;
        }
        else
        {
            Debug.LogError("Aucun Renderer trouv� sur la balle !");
        }
    }

    void Update()
    {
        if (ballMaterial == null) return;

        // R�cup�rer la v�locit� de la balle
        Vector3 velocity = rb.linearVelocity;

        // Convertir la vitesse en d�placement UV
        float uvX = velocity.x * textureSpeed * Time.deltaTime;
        float uvY = velocity.z * textureSpeed * Time.deltaTime; // Utilisation de Z pour simuler le roulis

        // Appliquer le d�calage
        uvOffset += new Vector2(uvX, uvY);

        // Modifier la propri�t� de d�calage du mat�riau (_MainTex pour Standard Shader, _BaseMap pour HDRP/URP)
        ballMaterial.SetTextureOffset("_BaseMap", uvOffset);
    }
}
