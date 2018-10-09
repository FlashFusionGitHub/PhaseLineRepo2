using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommentatorActor : MonoBehaviour {

    public enum ReactionCategory
    {
        Victory,
        Positive,
        Nuetral,
        Negative,
        AFK,
        Defeat
    }

    public enum SpeakingState
    {
        Idol,
        IdolSpeaking,
        Speaking,
    }

    [System.Serializable]
    public class Txt
    {
        [TextArea]
        public string subtitles = "-Insert-Subtitles-Here-";
        public float subtitleDisplayTime;
        public float subtitleTimer;
        public bool subtitlesFinished;
    }

    [System.Serializable]
    public class Animation
    {
        public Sprite[] sprites;
        public int index = 0;
        public float cycleSpeed = 0.01f;
        public float cycleTimer = 0f;
        public bool animationFinished = false;
    }

    [System.Serializable]
    public class Audio
    {
        public bool audioStarted = false;
        public AudioClip audioClip;
        public bool audioFinished = false;
    }

    [System.Serializable]
    public class VoiceLine
    {
        [HideInInspector]
        public bool tried;
        [HideInInspector]
        public string VoiceLineName;
        public ReactionCategory reactionCatagory;
        public Txt text;
        public Audio audio;
        public Animation spriteSheet;
    }

    [Header("Reactions")]
    public SpeakingState speakingState;
    public VoiceLine currentReaction;
    public VoiceLine[] reactions;

    [Header("Boredum Level")]
    public ReactionCategory idleState;
    public float timeUntilNextBoredumLevel;
    public float boredumLevelTimer;

    [Header("UI")]
    public Text subtitleField;
    public Image TextPanel;
    public Image portrait;
    AudioSource audioSource;

    public TweenAnimator tweenAnimator;

    public UnityEvent displaySubtitle;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        if (currentReaction == null)
        {
            speakingState = SpeakingState.Idol;
        }

        if (speakingState != SpeakingState.Idol)
         {
             PlayAudio();
             CycleSpriteSheet();
             DisplaySubtitles();
             CheckFinished();
         }

        if (speakingState != SpeakingState.Speaking)
        {
            if (boredumLevelTimer < 0)
            {
                NextIdolState();
                attempts = reactions.Length;
                ChooseAReaction(idleState);
            }
            else
            {
                boredumLevelTimer -= Time.deltaTime;
            }
        }
    }

    void NextIdolState()
    {
        switch(idleState)
        {
            case ReactionCategory.Positive:
                idleState = ReactionCategory.Nuetral;
                break;
            case ReactionCategory.Negative:
                idleState = ReactionCategory.AFK;
                break;
            case ReactionCategory.Nuetral:
                idleState = ReactionCategory.Nuetral;
                break;
            case ReactionCategory.AFK:
                idleState = ReactionCategory.AFK;
                break;
            default:
                idleState = ReactionCategory.Nuetral;
                break;
        }

        boredumLevelTimer = timeUntilNextBoredumLevel;
    }

    void PlayAudio()
    {
        if (currentReaction.audio.audioClip != null)
        {
            if (!audioSource.isPlaying && !currentReaction.audio.audioStarted && !currentReaction.audio.audioFinished)
            {
                currentReaction.audio.audioStarted = true;
                audioSource.clip = currentReaction.audio.audioClip;
                audioSource.Play();
            }
            else if (!audioSource.isPlaying && currentReaction.audio.audioStarted)
            {
                currentReaction.audio.audioFinished = true;
                currentReaction.audio.audioStarted = false;
            }
        }
        else
        {
            currentReaction.audio.audioFinished = true;
        }
    }

    void CycleSpriteSheet()
    {
        if (currentReaction.spriteSheet.sprites.Length > 0)
        {
            portrait.sprite = currentReaction.spriteSheet.sprites[currentReaction.spriteSheet.index];
        }

        if (currentReaction.spriteSheet.cycleTimer <= 0 && !currentReaction.spriteSheet.animationFinished)
        {
            if (currentReaction.spriteSheet.index + 1 < currentReaction.spriteSheet.sprites.Length)
            {
                currentReaction.spriteSheet.index++;
            }
            else
            {
                currentReaction.spriteSheet.animationFinished = true;
            }

            currentReaction.spriteSheet.cycleTimer = currentReaction.spriteSheet.cycleSpeed;
        }
        else if (currentReaction.spriteSheet.cycleTimer > 0 && !currentReaction.spriteSheet.animationFinished)
        {
            currentReaction.spriteSheet.cycleTimer -= Time.deltaTime;
        }
    }

    public void DisplaySubtitles()
    {
        //is the current not subtitle finshed
        if (!currentReaction.text.subtitlesFinished)
        {
            //advance subtitle counter
            currentReaction.text.subtitleTimer += Time.deltaTime;
            //is the previous subtitle different to the current subtitle
            if (subtitleField.text != currentReaction.text.subtitles)
            {
                tweenAnimator.TweenToOutPos();
                //set the previous subtitle to the new subtitle
                subtitleField.text = currentReaction.text.subtitles;
            }
            // if the current has reached its allocated timer
            if (currentReaction.text.subtitleTimer > currentReaction.text.subtitleDisplayTime)
            {
                tweenAnimator.TweenToInPos();
                //set the current reaction state the finished
                currentReaction.text.subtitlesFinished = true;
            }
        }
    }

    int attempts;
    void ChooseAReaction(ReactionCategory reactionCategory)
    {
        currentReaction = reactions[Random.Range(0, reactions.Length)];



        speakingState = (speakingState == SpeakingState.IdolSpeaking) ? SpeakingState.Speaking : SpeakingState.IdolSpeaking;
    }

    void CheckFinished()
    {
        if (currentReaction.spriteSheet.animationFinished && currentReaction.audio.audioFinished && currentReaction.text.subtitlesFinished)
        {
            speakingState = SpeakingState.Idol;
        }
    }

    //Unity Event
    public void ReactPositively()
    {
        //if not currently speaking, enable a positive reaction
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionCategory.Positive);
            idleState = ReactionCategory.Positive;
        }
    }

    //Unity Event
    public void ReactNegatively()
    {
        //if not currently speaking, enable a Negative reaction
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionCategory.Negative);
            idleState = ReactionCategory.Negative;
        }
    }
}
