using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; } = null;

        [SerializeField]
        private AudioSource _audioSourceBGM = null;

        [SerializeField]
        private AudioSource _audioSourceSFX = null;

        [SerializeField]
        private AudioClip[] _BGMClips = null;

        [SerializeField]
        private AudioClip[] _sfxClips = null;

        private DataModel DataModel => JumpApp.Instance?.DataModel;

        public void Init()
        {
            Instance = this;

            SetBGMVolume(DataModel.MusicToggle);

            DataModel.onMusicToggleUpdated += OnMusicToggleUpdated;
        }

        void OnDestroy()
        {
            Instance = null;

            if (DataModel != null)
            {
                DataModel.onMusicToggleUpdated -= OnMusicToggleUpdated;
            }
        }

        private void OnMusicToggleUpdated(bool isOn)
        {
            SetBGMVolume(isOn);
        }

        private void SetBGMVolume(bool isOn)
        {
            SetBGMVolume(isOn ? 1f : 0f);
        }

        private void SetBGMVolume(float volume)
        {
            _audioSourceBGM.volume = volume;
        }

        public void PlayBGM(BGMEnum bgmEnum, bool isLoop)
        {
            AudioClip audioClip = _BGMClips[(uint)bgmEnum];
            _audioSourceBGM.clip = audioClip;
            _audioSourceBGM.loop = isLoop;
            _audioSourceBGM.Play();
        }

        public void StopBGM()
        {
            _audioSourceBGM.Stop();
            _audioSourceBGM.clip = null;
            _audioSourceBGM.loop = false;
        }

        public void PlaySFX(SFXEnum sfxEnum)
        {
            AudioClip audioClip = _sfxClips[(uint)sfxEnum];
            _audioSourceSFX.PlayOneShot(audioClip);
        }
    }
}