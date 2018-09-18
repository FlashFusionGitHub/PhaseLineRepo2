using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class Commentator : MonoBehaviour {
    [System.Serializable]
    public enum ReactionType
    {
        Event,
        Idol
    }
    [System.Serializable]
    public enum ReactionCatagory
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
        Idol,
        IdolSpeaking,
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
        public ReactionCatagory reactionCatagory;
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
    public ReactionCatagory idolState;
    public float timeUntilNextBoredumLevel;
    public float boredumLevelTimer;

    [Header("UI")]
    public Text subtitleField;
    public Image portrait;
    AudioSource audioSource;

    private void Start()
    {
        attempts = reactions.Length;
        audioSource = GetComponent<AudioSource>();
    }
    public void ReactPositively()
    {
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionType.Event, ReactionCatagory.Positive);
            idolState = ReactionCatagory.Positive;
        }
    }

    public void ReactNegatively()
    {
        if (speakingState != SpeakingState.Speaking)
        {
            ChooseAReaction(ReactionType.Event, ReactionCatagory.Negative);
            idolState = ReactionCatagory.Negative;
        }
    }
    int attempts;
    void ChooseAReaction(ReactionType rt, ReactionCatagory rc)
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
        speakingState = (rt == ReactionType.Event) ? SpeakingState.Speaking : SpeakingState.IdolSpeaking;
    }

    private void Update()
    {
        if (currentReaction == null)
        {
            speakingState = SpeakingState.Idol;
        }
        if (speakingState == SpeakingState.Idol)
        {
            ChooseAReaction(ReactionType.Idol, idolState);
        }


        if ((speakingState != SpeakingState.Idol) && currentReaction != null)
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
                ChooseAReaction(ReactionType.Idol, idolState);
            }
            else
            {
                boredumLevelTimer -= Time.deltaTime;
            }
        }
    }
    void NextIdolState()
    {
        if (idolState == ReactionCatagory.Positive)
        {
            idolState = ReactionCatagory.Nuetral;
        }
        else if (idolState == ReactionCatagory.Nuetral)
        {
            idolState = ReactionCatagory.Negative;
        }
        else if (idolState == ReactionCatagory.Negative)
        {
            idolState = ReactionCatagory.AFK;
        }
        else if (idolState == ReactionCatagory.AFK)
        {
            idolState = ReactionCatagory.AFK;
        }
        else
        {
            idolState = ReactionCatagory.Nuetral;
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
        if (!currentReaction.text.subtitlesFinished)
        {
            currentReaction.text.subtitleTimer += Time.deltaTime;
            if (subtitleField.text != currentReaction.text.subtitles)
            {
                subtitleField.text = currentReaction.text.subtitles;
            }
            if (currentReaction.text.subtitleTimer > currentReaction.text.subtitleDisplayTime)
            {
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

            speakingState = SpeakingState.Idol;
        }
    }




    //private void OnDrawGizmos()
    //{
    //    foreach (VoiceLine vl in reactions)
    //    {
     //       string vlName = (vl.text.subtitles.Length < 20) ? (vl.reactionType.ToString() + " " + vl.reactionCatagory.ToString() + " (" + vl.text.subtitles + ")") : (vl.reactionType.ToString() + " " + vl.reactionCatagory.ToString() + " (" + vl.text.subtitles.Substring(0, 20) + "...)");
    //        if (vl.VoiceLineName != vlName)
     //           vl.VoiceLineName = vlName;
     //   }
   // }
}
