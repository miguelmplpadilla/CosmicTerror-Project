using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.Scripts.NPCs
{
    [CreateAssetMenu(fileName = "SampledSpeechVoice", menuName = "Cosmic Terror/Sampled Speech Voice")]
    public class SampledSpeechVoice : ScriptableObject
    {
        public List<SpeechSample> samples = new List<SpeechSample>();
        public bool loadClipsFromResources = true;
        public string resourcesFolder = "Audio/Speech/Uncanny";

        [Header("Timing")]
        public float gapBetweenSamples = 0.035f;
        public float wordPause = 0.09f;
        public float punctuationPause = 0.18f;

        [Header("Uncanny Variation")]
        public Vector2 pitchRange = new Vector2(0.92f, 1.08f);
        public Vector2 volumeRange = new Vector2(0.85f, 1f);
        public bool repeatLastKnownTokenForMissingSamples = false;

        public static readonly string[] MinimalSpanishTokens =
        {
            "ma", "me", "mi", "mo", "mu",
            "na", "ne", "ni", "no", "nu",
            "la", "le", "li", "lo", "lu",
            "ra", "re", "ri", "ro", "ru",
            "sa", "se", "si", "so", "su",
            "ta", "te", "ti", "to", "tu",
            "ka", "ke", "ki", "ko", "ku",
            "pa", "pe", "pi", "po", "pu",
            "a", "e", "i", "o", "u"
        };

        private readonly Dictionary<string, List<AudioClip>> clipsByToken = new Dictionary<string, List<AudioClip>>();
        private List<string> tokensByLength = new List<string>();
        private string lastPlayableToken = "";

        private void OnValidate()
        {
            samples.RemoveAll(sample => sample == null);
            foreach (SpeechSample sample in samples)
                sample.NormalizeToken();
        }

        [ContextMenu("Populate Minimal Spanish Tokens")]
        public void PopulateMinimalSpanishTokens()
        {
            foreach (string token in MinimalSpanishTokens)
            {
                if (samples.Any(sample => sample != null && sample.token == token))
                    continue;

                samples.Add(new SpeechSample { token = token });
            }
        }

        public void RebuildLookup()
        {
            clipsByToken.Clear();

            foreach (SpeechSample sample in samples)
            {
                if (sample == null || string.IsNullOrWhiteSpace(sample.token))
                    continue;

                string token = NormalizeText(sample.token);
                List<AudioClip> clips = sample.clips.Where(clip => clip != null).ToList();
                if (clips.Count == 0)
                    continue;

                clipsByToken[token] = clips;
            }

            if (loadClipsFromResources)
                LoadResourceClips();

            tokensByLength = clipsByToken.Keys
                .Concat(MinimalSpanishTokens)
                .Distinct()
                .OrderByDescending(token => token.Length)
                .ThenBy(token => token)
                .ToList();
        }

        public IEnumerable<SpeechUnit> Tokenize(string text)
        {
            string normalized = NormalizeText(text);
            int index = 0;

            while (index < normalized.Length)
            {
                char current = normalized[index];

                if (char.IsWhiteSpace(current))
                {
                    index++;
                    yield return SpeechUnit.WordPause();
                    continue;
                }

                if (IsPunctuation(current))
                {
                    index++;
                    yield return SpeechUnit.PunctuationPause();
                    continue;
                }

                string token = FindLongestToken(normalized, index);
                if (!string.IsNullOrEmpty(token))
                {
                    index += token.Length;
                    yield return SpeechUnit.FromToken(token);
                    continue;
                }

                index++;
            }
        }

        public AudioClip GetClip(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            if (!clipsByToken.TryGetValue(token, out List<AudioClip> clips) || clips.Count == 0)
            {
                if (!repeatLastKnownTokenForMissingSamples || string.IsNullOrEmpty(lastPlayableToken))
                    return null;

                clips = clipsByToken[lastPlayableToken];
            }
            else
            {
                lastPlayableToken = token;
            }

            return clips[Random.Range(0, clips.Count)];
        }

        private string FindLongestToken(string text, int startIndex)
        {
            foreach (string token in tokensByLength)
            {
                if (startIndex + token.Length > text.Length)
                    continue;

                if (string.CompareOrdinal(text, startIndex, token, 0, token.Length) == 0)
                    return token;
            }

            char current = text[startIndex];
            if ("aeiou".IndexOf(current) >= 0)
                return current.ToString();

            return "";
        }

        private void LoadResourceClips()
        {
            if (string.IsNullOrWhiteSpace(resourcesFolder))
                return;

            string folder = resourcesFolder.Trim().Trim('/');
            IEnumerable<string> tokens = MinimalSpanishTokens.Concat(samples
                .Where(sample => sample != null && !string.IsNullOrWhiteSpace(sample.token))
                .Select(sample => NormalizeText(sample.token)));

            foreach (string token in tokens.Distinct())
            {
                AudioClip[] resourceClips = UnityEngine.Resources.LoadAll<AudioClip>(folder + "/" + token);
                if (resourceClips.Length == 0)
                    continue;

                if (!clipsByToken.TryGetValue(token, out List<AudioClip> clips))
                {
                    clips = new List<AudioClip>();
                    clipsByToken[token] = clips;
                }

                foreach (AudioClip clip in resourceClips)
                {
                    if (!clips.Contains(clip))
                        clips.Add(clip);
                }
            }
        }

        public static string NormalizeText(string text)
        {
            string normalized = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder(normalized.Length);

            foreach (char character in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);
                if (category == UnicodeCategory.NonSpacingMark)
                    continue;

                if (character == 'c' || character == 'q')
                {
                    builder.Append('k');
                    continue;
                }

                if (character == 'v')
                {
                    builder.Append('b');
                    continue;
                }

                builder.Append(character);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static bool IsPunctuation(char character)
        {
            return character == '.' || character == ',' || character == ';' ||
                   character == ':' || character == '?' || character == '!' ||
                   character == '\u00bf' || character == '\u00a1';
        }
    }

    public struct SpeechUnit
    {
        public readonly string Token;
        public readonly bool IsWordPause;
        public readonly bool IsPunctuationPause;

        private SpeechUnit(string token, bool isWordPause, bool isPunctuationPause)
        {
            Token = token;
            IsWordPause = isWordPause;
            IsPunctuationPause = isPunctuationPause;
        }

        public static SpeechUnit FromToken(string token)
        {
            return new SpeechUnit(token, false, false);
        }

        public static SpeechUnit WordPause()
        {
            return new SpeechUnit("", true, false);
        }

        public static SpeechUnit PunctuationPause()
        {
            return new SpeechUnit("", false, true);
        }
    }

    [Serializable]
    public class SpeechSample
    {
        public string token;
        public List<AudioClip> clips = new List<AudioClip>();

        public void NormalizeToken()
        {
            if (!string.IsNullOrWhiteSpace(token))
                token = token.Trim().ToLowerInvariant();
        }
    }
}
