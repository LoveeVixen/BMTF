// LOVEEVIXEN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [System.Serializable]
        public class Sound
        {
            [SerializeField] string name = "Sound Name";
            [SerializeField] AudioClip[] clips = new AudioClip[1];

            public string GetName() {  return name; }
            public AudioClip GetClip(int index) { return clips[index]; }
            public AudioClip GetRandomClip() { return clips[Random.Range(0, clips.Length)]; }
        }

        [System.Serializable]
        public class Music
        {
            [SerializeField] string name = "Music Name";
            [SerializeField] string composer = "Composer name";
            [SerializeField] AudioClip intro;
            [SerializeField] AudioClip loop;

            public string GetName() { return name; }
            public AudioClip GetIntro() { return intro; }
            public AudioClip GetLoop() { return loop; }
        }

        [SerializeField] AudioObject audioObjectPrefab;
        [SerializeField] AudioSource musicPlayerPrefab;
        private AudioSource musicPlayer;
        [SerializeField] List<Sound> sounds = new List<Sound>();
        [SerializeField] List<Music> music = new List<Music>();
        private Music playingMusic;
        private bool finishedMusicIntro;

        // Volume control.
        private float masterVolume = 1f;
        private float musicVolume = 1f;
        private float soundVolume = 1f;

        private void Awake()
        {
            // Makes sure only one game manager instance exists to track manager data.
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                // Setup music player.
                musicPlayer = Instantiate(musicPlayerPrefab);
                DontDestroyOnLoad(musicPlayer.gameObject);
            }
            else if (instance != this)
                Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            // Check that music is playing.
            if(playingMusic != null)
            {
                if(!musicPlayer.isPlaying && !finishedMusicIntro)
                {
                    // Play loop of current music.
                    finishedMusicIntro = true;
                    musicPlayer.clip = playingMusic.GetLoop();
                    musicPlayer.loop = true;
                    musicPlayer.Play();
                }
            }
        }

        // Play sound methods.
        #region
        public AudioObject PlaySound(string soundName, Vector3 pos)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab, pos, Quaternion.identity);
            AudioSource source = audioObj.GetSource();
            Sound sound = GetSound(soundName);
            if(sound != null) source.clip = sound.GetRandomClip();

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlaySound(string soundName, Vector3 pos, SoundProperties properties)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab, pos, Quaternion.identity);
            AudioSource source = audioObj.GetSource();
            Sound sound = GetSound(soundName);
            if(sound != null) source.clip = sound.GetRandomClip();

            // Apply sound properties.
            ApplySoundPropertiesToAudioObject(audioObj, properties);

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlaySound(AudioClip clip, Vector3 pos)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab, pos, Quaternion.identity);
            AudioSource source = audioObj.GetSource();
            source.clip = clip;

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlaySound(AudioClip clip, Vector3 pos, SoundProperties properties)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab, pos, Quaternion.identity);
            AudioSource source = audioObj.GetSource();
            source.clip = clip;

            // Apply sound properties.
            ApplySoundPropertiesToAudioObject(audioObj, properties);

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlayNonDiegeticSound(string soundName)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab);
            AudioSource source = audioObj.GetSource();
            Sound sound = GetSound(soundName);
            if (sound != null) source.clip = sound.GetRandomClip();
            source.spatialBlend = 0f;

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlayNonDiegeticSound(AudioClip clip)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab);
            AudioSource source = audioObj.GetSource();
            source.clip = clip;
            source.spatialBlend = 0f;

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlayNonDiegeticSound(string soundName, SoundProperties properties)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab);
            AudioSource source = audioObj.GetSource();
            Sound sound = GetSound(soundName);
            if (sound != null) source.clip = sound.GetRandomClip();
            source.spatialBlend = 0f;

            // Apply sound properties.
            ApplySoundPropertiesToAudioObject(audioObj, properties);

            // Play sound.
            source.Play();
            return audioObj;
        }

        public AudioObject PlayNonDiegeticSound(AudioClip clip, SoundProperties properties)
        {
            AudioObject audioObj = Instantiate(audioObjectPrefab);
            AudioSource source = audioObj.GetSource();
            source.clip = clip;
            source.spatialBlend = 0f;

            // Apply sound properties.
            ApplySoundPropertiesToAudioObject(audioObj, properties);

            // Play sound.
            source.Play();
            return audioObj;
        }

        private void ApplySoundPropertiesToAudioObject(AudioObject audioObj, SoundProperties properties)
        {
            audioObj.GetSource().volume = properties.volume;
            audioObj.GetSource().pitch = properties.pitch;
            audioObj.GetSource().loop = properties.loop;
            if (properties.follow != null) audioObj.FollowTransform(properties.follow);
            audioObj.GetSource().minDistance = properties.minDistance;
            audioObj.GetSource().maxDistance = properties.maxDistance;
        }

        public Sound GetSound(string soundName)
        {
            foreach (Sound sound in sounds)
            {
                if (sound.GetName() == soundName)
                    return sound;
            }

            Debug.Log("Couldn't find sound with name: " + soundName);
            return null;
        }
        #endregion

        public void PlayMusic(string musicName)
        {
            playingMusic = GetMusic(musicName);
            musicPlayer.clip = playingMusic.GetIntro();
            musicPlayer.loop = false;
            finishedMusicIntro = false;
            musicPlayer.Play();
        }

        public void StopMusic()
        {
            playingMusic = null;
            musicPlayer.Stop();
        }

        public Music GetMusic(string musicName)
        {
            foreach (Music soundtrack in music)
            {
                if (soundtrack.GetName() == musicName)
                    return soundtrack;
            }

            Debug.Log("Couldn't find sound with name: " + musicName);
            return null;
        }

        public float MasterVolume { get { return masterVolume; } set { masterVolume = value; } }
        public float MusicVolume { get { return musicVolume; } set { musicVolume = value; } }
        public float SoundVolume { get { return soundVolume; } set { soundVolume = value; } }
    }
}