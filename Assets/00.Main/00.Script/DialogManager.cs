using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    [SerializeField] private GameObject speechBubble;
    [SerializeField] private GameObject arrow;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private RectTransform speechBubbleRect;
    [SerializeField] private RectTransform nameBoxRect;

    [SerializeField] private DialogSO dialogSO;

    private Dictionary<int, DialogInfo> dialogDict;
    private string[] currentMessages;
    private int currentMessageIndex;
    public bool isDialogActive;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private string currentName;

    [SerializeField] private AudioClip tickSound;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    [SerializeField] Transform playerCameraPos;

    private bool canInteract = true;
    private bool canPress = true;

    [SerializeField] private PlayableDirector playableDirector; // 타임라인을 제어할 PlayableDirector

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        dialogDict = new Dictionary<int, DialogInfo>();
        foreach (var dialog in dialogSO.dialogInfo)
        {
            dialogDict[dialog.ID] = dialog;
        }
    }

    private void Update()
    {
        if (isDialogActive && canPress && Input.GetKeyDown(KeyCode.F))
        {
            if (isTyping)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }
                messageText.text = currentMessages[currentMessageIndex - 1];
                AdjustSpeechBubbleSize();
                isTyping = false;
                arrow.SetActive(true);
            }
            else
            {
                PlayClickSound();
                ShowNextMessage();
            }
        }
    }

    public void DialogStart(int id, Vector3 newPosition)
    {
        if (isDialogActive || !canInteract) return;

        if (dialogDict.TryGetValue(id, out var dialog))
        {
            currentMessages = dialog.message;
            currentName = dialog.name;
            currentMessageIndex = 0;
            isDialogActive = true;
            speechBubble.SetActive(true);
            nameText.text = currentName;
            AdjustNameBoxSize();

            speechBubble.transform.position = new Vector3(newPosition.x, newPosition.y + 1, newPosition.z);

            playerCameraPos.position += new Vector3(0, -1, 1);

            ShowNextMessage();

            StartCoroutine(PreventDoubleInput());
        }
    }

    IEnumerator PreventDoubleInput()
    {
        canPress = false;
        yield return new WaitForSeconds(0.2f); // 짧게 0.2초 정도 비활성화
        canPress = true;
    }

    void ShowNextMessage()
    {
        if (currentMessages != null && currentMessageIndex < currentMessages.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            arrow.SetActive(false);

            typingCoroutine = StartCoroutine(TypeMessage(currentMessages[currentMessageIndex]));
            currentMessageIndex++;
        }
        else
        {
            EndDialog();
        }
    }

    IEnumerator TypeMessage(string message)
    {
        isTyping = true;
        messageText.text = "";
        AdjustSpeechBubbleSize();

        foreach (char letter in message.ToCharArray())
        {
            messageText.text += letter;
            AdjustSpeechBubbleSize();
            PlayTickSound();
            yield return new WaitForSeconds(0.07f);
        }

        isTyping = false;
        typingCoroutine = null;

        arrow.SetActive(true);
    }

    void EndDialog()
    {
        if (TimeLineManager.instance.isCutScene)
        {
            ResumeTimeline();
        }
        isDialogActive = false;
        speechBubble.SetActive(false);
        arrow.SetActive(false);


        StartCoroutine(ResetInteractCooldown());
    }

    IEnumerator ResetInteractCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(0.5f);
        canInteract = true;
    }

    void PlayTickSound()
    {
        if (audioSource != null && tickSound != null)
        {
            audioSource.PlayOneShot(tickSound);
        }
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    void AdjustSpeechBubbleSize()
    {
        Vector2 textSize = messageText.GetPreferredValues(messageText.text);
        speechBubbleRect.sizeDelta = new Vector2(textSize.x + 100f, textSize.y + 135f);
    }

    void AdjustNameBoxSize()
    {
        Vector2 nameSize = nameText.GetPreferredValues(currentName);
        nameBoxRect.sizeDelta = new Vector2(nameSize.x + 100f, nameBoxRect.sizeDelta.y);
    }

    public void CutSceneDialogStart(int id)
    {
        if (isDialogActive) return;
        PauseTimeline();
        if (dialogDict.TryGetValue(id, out var dialog))
        {
            currentMessages = dialog.message;
            currentName = dialog.name;
            currentMessageIndex = 0;
            isDialogActive = true;
            speechBubble.SetActive(true);
            nameText.text = currentName;
            AdjustNameBoxSize();

            ShowNextMessage();
        }
    }

    public void CutSceneDialogStartPosition(Transform newPosition)
    {
        speechBubble.transform.position = new Vector3(newPosition.position.x, newPosition.position.y + 1, newPosition.position.z);
    }

    public void PauseTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause(); // 타임라인 멈추기
        }
    }

    public void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play(); // 타임라인 재생
        }
    }
}
