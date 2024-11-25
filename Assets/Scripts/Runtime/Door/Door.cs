using UnityEngine;
using Cinemachine;
using DG.Tweening;
using TMPro;

public class Door : MonoBehaviour
{

    public RSO_CollectedCoins collectedCoins;
    public RSO_CompletedHoles completedHoles;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;
    [SerializeField] private TextMeshProUGUI textCoin;
    [SerializeField] private TextMeshProUGUI textHole;
    [SerializeField] private ParticleSystem particleLeft;
    [SerializeField] private ParticleSystem particleRight;
    [SerializeField] private AudioSource sfx;

    [Header("Settings")]
    [SerializeField] private int animeDuration;
    [SerializeField] private bool verticalMouvement;

    [Header("Cinematique Settings")]
    [SerializeField] private float delayBeforeActivation;
    [SerializeField] private float delayAfterActivation;

    [Header("Object Need")]
    public int coinTreshold;
    public int holeTreshold;

    [Header("__DEBUG__")]
    [SerializeField] private bool isOpen;

    private void OnEnable()
    {
        if (coinTreshold > 0)
        {
            collectedCoins.onValueChanged += UpdatePanelCoin;
        }

        if (holeTreshold > 0)
        {
            completedHoles.onValueChanged += UpdatePanelHole;
        }
    }

    private void Start()
    {
        UpdatePanelCoin(collectedCoins.Value);
        UpdatePanelHole(completedHoles.Value);
    }

    private void UpdatePanelCoin(int coinAmount)
    {
        textCoin.text = coinAmount.ToString() + "/" + coinTreshold.ToString() + " Coins";
    }

    private void UpdatePanelHole(int holeAmount)
    {
        textHole.text = holeAmount.ToString() + "/" + holeTreshold.ToString() + " Holes";
    }

    public void TriggerOpen(PlayerController pc)
    {
        if (CanBeOpened())
        {
            OpenDoor(pc);
            collectedCoins.onValueChanged -= UpdatePanelCoin;
            completedHoles.onValueChanged -= UpdatePanelHole;
        }
    }

    private bool CanBeOpened()
    {
        return !isOpen && completedHoles.Value >= holeTreshold && collectedCoins.Value >= coinTreshold;
    }

    private void OpenDoor(PlayerController pc)
    {
        pc.ToggleRoll(true);
        pc.SetDirection(Vector3.zero);
        pc.Block();

        CameraManager.Instance.ActivateCamera(cam);

        isOpen = true;

        StartCoroutine(Utils.Delay(() =>
        {
            if (verticalMouvement) OpenVerticaly();
            else OpenPivot();

            if (particleLeft != null) particleLeft?.Play();

            if (particleRight != null) particleRight?.Play();

            if (particleLeft != null)
            {
                if (verticalMouvement) particleLeft.transform.localScale = transform.localScale;
                else particleLeft.transform.localScale = doorLeft.transform.localScale;

                particleLeft?.Play();
            }

            if (particleRight != null)
            {
                particleLeft.transform.localScale = doorRight.transform.localScale;
                particleRight?.Play();
            }

        }, delayBeforeActivation));

        StartCoroutine(Utils.Delay(() =>
        {
            CameraManager.Instance.DeActivateCurrentCamera();
            pc.UnBlock(true);
        }, animeDuration + delayAfterActivation));
    }

    /// <summary>
    /// Ouvre la porte verticalement vers le bas
    /// </summary>
    private void OpenVerticaly() => doorLeft?.DOMove(new Vector3(transform.position.x, doorLeft.position.y - (doorLeft.localScale.y + 1), transform.position.z), animeDuration);

    /// <summary>
    /// Ouvre les 2 portes en les pivotants
    /// </summary>
    private void OpenPivot()
    {
        doorLeft?.DORotate(new Vector3(transform.rotation.x, doorLeft.eulerAngles.y - 90, transform.rotation.z), animeDuration);
        doorRight?.DORotate(new Vector3(transform.rotation.x, doorLeft.eulerAngles.y + 90, transform.rotation.z), animeDuration);
    }
}
