using TMPro;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class PannelCollectible : MonoBehaviour
{
    [Header("Input events")]
    public RSO_CollectedCoins collectedCoins;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private TMP_Text textCollectibles;
    [SerializeField] private GameObject holeGo;
    [SerializeField] private VisualEffect particleEffect;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private GameObject pannelGo;

    [Header("Cinematique Settings")]
    [SerializeField] private float focusDuration;
    [SerializeField] private float delayBeforeActivation;

    [Header("Settings")]
    [SerializeField] private int nbCollectibles;

    private PC_MovingSphere players;

    private void Start()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].TryGetComponent(out PC_MovingSphere player))
            {
                players = player;

                break;
            }
        }

        textCollectibles.text = 0 + "/" + nbCollectibles;

        collectedCoins.onValueChanged += UpdatePannel;

        holeGo.SetActive(false);
    }

    private void UpdatePannel(int collectedCoinAmount)
    {
        textCollectibles.text = collectedCoinAmount.ToString() + "/" + nbCollectibles.ToString();

        if (collectedCoins.Value >= nbCollectibles)
        {
            players.ToggleRoll(true);
            players.SetDirection(Vector3.zero);
            players.Block();

            CameraManager.Instance.ActivateCamera(cam);

            StartCoroutine(Utils.Delay(() =>
            {
                if (sfx != null) sfx.Play();

                if (particleEffect != null)
                {
                    particleEffect.transform.localScale = holeGo.transform.localScale * 4;
                    particleEffect.Play();
                }

                Destroy(transform.GetChild(0).gameObject);
                holeGo.SetActive(true);
            }, delayBeforeActivation));

            StartCoroutine(Utils.Delay(() =>
            {
                CameraManager.Instance.DeActivateCurrentCamera();
                players.UnBlock(true);
            }, focusDuration));
        }
    }
}
