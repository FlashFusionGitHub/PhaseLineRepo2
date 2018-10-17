using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/* Commentator Class - Written By Michael Matthews */
/* Commented and Modified by Rowen Govender */

[RequireComponent(typeof(AudioSource))]
public class Commentator : MonoBehaviour {

    //Reaction Types used to distinguish the available reaction types
    [System.Serializable]
    public enum ReactionType
    {
        Event,
        Idle
    }

    //Reaction Category used in 'reaction creation' to give the created reaction a category
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

    //The current Speaking State of the commentator
    [System.Serializable]
    public enum SpeakingState
    {
        Idle,
        IdleSpeaking,
        Speaking,
    }

    //Used to set the parameters for a text output for the commentator
    [System.Serializable]
    public class Txt
    {
        [TextArea]
        public string subtitles = "-Insert-Subtitles-Here-";
        public float subtitleDisplayTime;
        public float subtitleTimer;
        public bool subtitlesFinished;
    }

    //Used to set the paramenters for an Animation event
    [System.Serializable]
    public class Animation
    {
        public Sprite[] sprites;
        public int index = 0;
        public float cycleSpeed = 0.01f;
        public float cycleTimer = 0f;
        public bool animationFinished = false;
    }

    //Used to set the parameters for an Audio event
    [System.Serializable]
    public class Audio
    {
        public bool audioStarted = false;
        public AudioClip audioClip;
        public bool audioFinished = false;
    }

    //Used in VoiceLine creation to specify all the parameter needed for a voiceline
    [System.Serializable]
    public class VoiceLine
    {
        [HideInInspector]
        public bool tried; /*Has this VoiceLine been tired before?*/
        [HideInInspector]
        public string VoiceLineName;
        public ReactionType reactionType;
        public ReactionCategory reactionCategory;
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
    public Image portrait;
    AudioSource audioSource;

    public TweenAnimator tweenAnimator;

    public UnityEvent displaySubtitle;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (currentReaction == null)
        {
            speakingState = SpeakingState.Idle;
        }
    }

    private void Update()
    {     
        /*If the speaking state is Idle, reset the boredum timer, and start a commentator Event*/
        /*if the commentator is then in an idle state begin boredum timer, cycling through boredum reactions until another Commentator event is triggered*/
        if ((speakingState != SpeakingState.Idle))
        {
            boredumLevelTimer = timeUntilNextBoredumLevel;
            PlayAudio();
            CycleSpriteSheet();
            DisplaySubtitles();
            CheckFinished();
        }
        else
        {
            if (boredumLevelTimer < 0)
            {
                NextIdleState();
                ChooseAReaction(ReactionType.Idle, idleState);
            }
            else
            {
                boredumLevelTimer -= Time.deltaTime;
            }
        }
    }

    void ChooseAReaction(ReactionType rt, ReactionCategory rc)
    {
        VoiceLine tempReaction = reactions[Random.Range(0, reactions.Length)];

        if(tempReaction == currentReaction)
        {
            ChooseAReaction(rt, rc);
        }
        else
        {
            currentReaction = tempReaction;
        }

        boredumLevelTimer = (rt == ReactionType.Event) ? timeUntilNextBoredumLevel : boredumLevelTimer;

        currentReaction.spriteSheet.animationFinished = false;
        currentReaction.spriteSheet.index = 0;
        currentReaction.audio.audioStarted = false;
        currentReaction.audio.audioFinished = false;
        currentReaction.text.subtitlesFinished = false;
        currentReaction.text.subtitleTimer = 0;

        speakingState = (rt == ReactionType.Event) ? SpeakingState.Speaking : SpeakingState.IdleSpeaking;

        /*foreach (VoiceLine vl in reactions)
        {
            vl.tried = false;
        }*/
    }

    //Used for Unity Event, attach to any Positive Tank Event
    public void ReactPositively()
    {
        // If a speaking event is not currently active, choose a random postive reaction
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionType.Event, ReactionCategory.Positive);
            idleState = ReactionCategory.Positive;
        }
    }

    //Used for Unity Event, attach to any Negative Tank Event
    public void ReactNegatively()
    {
        // If a speaking event is not currently active, choose a random negative reaction
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionType.Event, ReactionCategory.Negative);
            idleState = ReactionCategory.Negative;
        }
    }

    //Select an idle state
    //Cycle through all avaivable IdleStates until the default is hit
    void NextIdleState()
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

    //Check if the Voice Line is in the list
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

    //Play Audio
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

    //Cycle through a sprite sheet
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
        //If all the parametres that distinguish an 'Event' return a finished state, set speaking state to Idle
        if (currentReaction.spriteSheet.animationFinished && currentReaction.audio.audioFinished && currentReaction.text.subtitlesFinished)
        {
            speakingState = SpeakingState.Idle;
        }
    }


}
