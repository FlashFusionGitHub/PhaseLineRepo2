using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommentatorActor : MonoBehaviour {

    public enum ReactionCatagory
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

    public class Txt
    {
        [TextArea]
        public string subtitles = "-Insert-Subtitles-Here-";
        public float subtitleDisplayTime;
        public float subtitleTimer;
        public bool subtitlesFinished;
    }

    public class Animation
    {
        public Sprite[] sprites;
        public int index = 0;
        public float cycleSpeed = 0.01f;
        public float cycleTimer = 0f;
        public bool animationFinished = false;
    }

    public class Audio
    {
        public bool audioStarted = false;
        public AudioClip audioClip;
        public bool audioFinished = false;
    }

    public class VoiceLine
    {
        [HideInInspector]
        public bool tried;
        [HideInInspector]
        public string VoiceLineName;
        public ReactionCatagory reactionCatagory;
        public Txt text;
        public Audio audio;
        public Animation spriteSheet;
    }

    [Header("Reactions")]
    public SpeakingState speakingState;
    public VoiceLine currentReaction;
    public VoiceLine[] reactions;

    [Header("Boredum Level")]
    public ReactionCatagory idolState;
    public float timeUntilNextBoredumLevel;
    public float boredumLevelTimer;

    [Header("UI")]
    public Text subtitleField;
    public Image TextPanel;
    public Image portrait;
    AudioSource audioSource;

    TweenAnimator tweenAnimator;

    public UnityEvent displaySubtitle;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKey(KeyCode.A))
        {
            DisplaySubtitles();
        }
	}

    void PlayAudio()
    {
        //Play the voice line
    }

    void CycleSpriteSheet()
    {
        //Cycle through sprite sheet
    }

    public void DisplaySubtitles()
    {
        //Text change
        //Pop out text panel displaying text

        displaySubtitle.Invoke();
    }
}
