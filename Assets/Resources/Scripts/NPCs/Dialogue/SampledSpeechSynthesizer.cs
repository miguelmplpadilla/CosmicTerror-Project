using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.Scripts.NPCs
{
    public class SampledSpeechSynthesizer : MonoBehaviour
    {
        public AudioSource audioSource;
        public SampledSpeechVoice voice;

        private Coroutine speechCoroutine;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;

            if (voice == null)
                voice = UnityEngine.Resources.Load<SampledSpeechVoice>("Audio/Speech/Uncanny/SampledSpeechVoice");
        }

        public IEnumerator Speak(string text)
        {
            if (!isActiveAndEnabled || voice == null || string.IsNullOrWhiteSpace(text))
                yield break;

            if (speechCoroutine != null)
                StopCoroutine(speechCoroutine);

            voice.RebuildLookup();
            speechCoroutine = StartCoroutine(SpeakRoutine(text));
            yield return speechCoroutine;
        }

        public void StopSpeaking()
        {
            if (speechCoroutine != null)
                StopCoroutine(speechCoroutine);

            speechCoroutine = null;
            audioSource.Stop();
            audioSource.pitch = 1f;
        }

        private IEnumerator SpeakRoutine(string text)
        {
            foreach (SpeechUnit unit in voice.Tokenize(text))
            {
                if (unit.IsWordPause)
                {
                    yield return new WaitForSeconds(voice.wordPause);
                    continue;
                }

                if (unit.IsPunctuationPause)
                {
                    yield return new WaitForSeconds(voice.punctuationPause);
                    continue;
                }

                AudioClip clip = voice.GetClip(unit.Token);
                if (clip == null)
                    continue;

                float pitch = Random.Range(voice.pitchRange.x, voice.pitchRange.y);
                float volume = Random.Range(voice.volumeRange.x, voice.volumeRange.y);

                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip, volume);

                float clipDuration = clip.length / Mathf.Max(0.01f, Mathf.Abs(pitch));
                yield return new WaitForSeconds(clipDuration + voice.gapBetweenSamples);
            }

            audioSource.pitch = 1f;
            speechCoroutine = null;
        }
    }
}
