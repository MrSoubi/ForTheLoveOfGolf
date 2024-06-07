using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.ProBuilder.Shapes;

public class Door : MonoBehaviour
{
    [HideInInspector] private bool open;
    [HideInInspector] public int index;

    [Header("Config Door")]
    [SerializeField] private int animeDuration;
    [SerializeField] private bool verticalMouvement;

    [Header("Object Need")]
    public bool coinTreshold;
    public int coinQuantity;
    [Space]
    public bool holeTreshold;
    public int holeQuantity;

    [Header("Reference")]
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [SerializeField] private TextMeshProUGUI textCoin;
    [SerializeField] private TextMeshProUGUI textHole;
    [SerializeField] private ParticleSystem particleLeft;
    [SerializeField] private ParticleSystem particleRight;
    [SerializeField] private AudioSource sfx;

    private int holeCompleted;
    private int coinCollected;

    private void Start()
    {
        StartCoroutine(Utils.Delay(LinkEvent, 0.001f));
        UpdatePannelCoin();
        UpdatePannelHole();

        if (open)
        {
            OpenDoor();
        }
    }

    private void LinkEvent()
    {
        if (CoinManager.instance) CoinManager.instance.onCollectedCoin += UpdatePannelCoin;

        if (HoleManager.instance) HoleManager.instance.onCollectedHole += UpdatePannelHole;
    }

    private void UpdatePannelCoin()
    {
        if (coinTreshold)
        {
            if (CoinManager.instance) coinCollected = CoinManager.instance.coinCollected;
            textCoin.text = coinCollected.ToString() + "/" + coinQuantity.ToString() + " Coins";
        }
        else
        {
            textCoin.text = "";
        }
    }

    private void UpdatePannelHole()
    {
        if (holeTreshold)
        {
            if (HoleManager.instance) holeCompleted = HoleManager.instance.holeCollected;
            textHole.text = holeCompleted.ToString() + "/" + holeQuantity.ToString() + " Holes";
        }
        else
        {
            textHole.text = "";
        }
    }

    public void TriggerOpen()
    {
        if (!open)
        {
            if ((holeCompleted >= holeQuantity || !holeTreshold) && (coinCollected >= coinQuantity || !coinTreshold))
            {
                OpenDoor();
            }
        }
    }

    /// <summary>
    /// Ouvre la porte selon son type
    /// </summary>
    private void OpenDoor()
    {
        open = true;
        if(verticalMouvement)
        {
            OpenVerticaly();
        }
        else
        {
            OpenPivot();
        }

        if (particleLeft != null)
        {
            particleLeft?.Play();
        }
            
        if(particleRight != null)
        {
            particleRight?.Play();
        }

        if (sfx != null)
        {
            sfx.Play();
        }
    }

    /// <summary>
    /// Ouvre la porte verticalement vers le bas
    /// </summary>
    private void OpenVerticaly() => doorLeft?.DOMove(new Vector3(transform.position.x, transform.position.y - 4, transform.position.z), animeDuration);

    /// <summary>
    /// Ouvre les 2 portes en les pivotants
    /// </summary>
    private void OpenPivot()
    {
        doorLeft?.DORotate(new Vector3(transform.rotation.x, transform.rotation.y - 90, transform.rotation.z), animeDuration);
        doorRight?.DORotate(new Vector3(transform.rotation.x, transform.rotation.y + 90, transform.rotation.z), animeDuration);
    }
}
