using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Commentator : MonoBehaviour {

    [System.Serializable]
    public enum ReactionType
    {
        Event,
        Idle
    }

    [System.Serializable]
    public enum ReactionCategory
    {
        Victory,
        Positive,
        Nuetral,
        Negative,
        AFK,
        Defeat
    }

    [System.Serializable]
    public enum SpeakingState
    {
        Idle,
        IdleSpeaking,
        Speaking,
    }

    [System.Serializable]
    public class VoiceLine
    {
        [HideInInspector]
        public bool tried;
        [HideInInspector]
        public string VoiceLineName;
        public ReactionType reactionType;
        public ReactionCategory reactionCatagory;
        public Txt text;
        public Audio audio;
        public Animation spriteSheet;

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
    public Image portrait;
    AudioSource audioSource;

    public TweenAnimator tweenAnimator;

    public UnityEvent displaySubtitle;

    private void Start()
    {
        attempts = reactions.Length;
        audioSource = GetComponent<AudioSource>();
    }

    public void ReactPositively()
    {
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionType.Event, ReactionCategory.Positive);
            idleState = ReactionCategory.Positive;
        }
    }

    public void ReactNegatively()
    {
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionType.Event, ReactionCategory.Negative);
            idleState = ReactionCategory.Negative;
        }
    }

    int attempts;
    void ChooseAReaction(ReactionType rt, ReactionCategory rc)
    {
        currentReaction = reactions[Random.Range(0, reactions.Length)];
        if ((currentReaction.reactionType != rt || currentReaction.reactionCatagory != rc) && attempts >= 0)
        {
            if (!currentReaction.tried)
            {
                currentReaction.tried = true;
                attempts--;
            }
            ChooseAReaction(rt, rc);
            return;
        }
        else if (attempts <= 0)
        {
            if (reactions.Length > 0)
            currentReaction = reactions[0];
        }
        attempts = reactions.Length;
        foreach (VoiceLine vl in reactions)
        {
            vl.tried = false;
        }
        boredumLevelTimer = (rt == ReactionType.Event) ? timeUntilNextBoredumLevel: boredumLevelTimer;
        currentReaction.spriteSheet.animationFinished = false;
        currentReaction.spriteSheet.index = 0;
        currentReaction.audio.audioStarted = false;
        currentReaction.audio.audioFinished = false;
        currentReaction.text.subtitlesFinished = false;
        currentReaction.text.subtitleTimer = 0;
        speakingState = (rt == ReactionType.Event) ? SpeakingState.Speaking : SpeakingState.IdleSpeaking;
    }

    private void Update()
    {
        if (currentReaction == null)
        {
            speakingState = SpeakingState.Idle;
        }
        
        if (speakingState == SpeakingState.Idle)
        {
            ChooseAReaction(ReactionType.Idle, idleState);
        }
        
        
        if ((speakingState != SpeakingState.Idle) && currentReaction != null)
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
                ChooseAReaction(ReactionType.Idle, idleState);
            }
            else
            {
                boredumLevelTimer -= Time.deltaTime;
            }
        }
    }

    void NextIdolState()
    {
        if (idleState == ReactionCategory.Positive)
        {
            idleState = ReactionCategory.Nuetral;
        }
        else if (idleState == ReactionCategory.Nuetral)
        {
            idleState = ReactionCategory.Negative;
        }
        else if (idleState == ReactionCategory.Negative)
        {
            idleState = ReactionCategory.AFK;
        }
        else if (idleState == ReactionCategory.AFK)
        {
            idleState = ReactionCategory.AFK;
        }
        else
        {
            idleState = ReactionCategory.Nuetral;
        }

        boredumLevelTimer = timeUntilNextBoredumLevel;
    }

    bool isInList(VoiceLine[] list, VoiceLine checkThis)
    {
        foreach (VoiceLine vl in list)
        {
            if (checkThis == vl)
            {
                return true;
            }
        }
        return false;
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

    void DisplaySubtitles()
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
        else if (currentReaction.spriteSheet.cycleTimer >0 && !currentReaction.spriteSheet.animationFinished)
        {
            currentReaction.spriteSheet.cycleTimer -= Time.deltaTime;
        }
    }

    void CheckFinished()
    {
        if (currentReaction.spriteSheet.animationFinished && currentReaction.audio.audioFinished && currentReaction.text.subtitlesFinished)
        {
            speakingState = SpeakingState.Idle;
        }
    }


}
