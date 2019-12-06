using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer64 : MonoBehaviour
{
    private AudioSource _audioSource;

    //Microphone input
    public AudioClip _audioClip;
    public bool _useMicroPhone;
    public string _selectedDevices;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;

    //FFT values
    private static float[] _samplesLeft = new float[512];
    private static float[] _samplesRight = new float[512];

    private float[] _freqBand64 = new float[64];
    private float[] _bandBuffer64 = new float[64];
    private float[] _bufferDecrease64 = new float[64];
    private float[] _freqBandHighest64 = new float[64];

    //audio band values
    [HideInInspector] public float[] _audioBand64, _audioBandBuffer64;

    //Amplitude variables
    [HideInInspector] public float _Amplitude, _AmplitudeBuffer;
    private float _AmplitudeHighest;

    //audio profile
    public float _audioProfile;

    //stereo channels
    public enum _channel
    {
        Stereo,
        Left,
        Right
    };

    public _channel channel = new _channel();

    private void Start()
    {
        _audioBand64 = new float[64];
        _audioBandBuffer64 = new float[64];
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);

        //Microphone input
        if (_useMicroPhone)
        {
            if (Microphone.devices.Length > 0)
            {
                _selectedDevices = Microphone.devices[0].ToString();
                _audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;
                _audioSource.clip = Microphone.Start(_selectedDevices, true, 10, AudioSettings.outputSampleRate);
            }
            else _useMicroPhone = false;
        }
        else
        {
            _audioSource.outputAudioMixerGroup = _mixerGroupMaster;
            _audioSource.clip = _audioClip;
        }

        _audioSource.Play();
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands64();
        BandBuffer64();
        CreateAudioBands64();
        GetAmplitude();
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < _freqBandHighest64.Length; i++) _freqBandHighest64[i] = audioProfile;
    }

    void GetAmplitude()
    {
        float _CurrentAmplitude = 0, _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 64; i++)
        {
            _CurrentAmplitude += _audioBand64[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer64[i];
        }

        if (_CurrentAmplitude > _AmplitudeHighest) _AmplitudeHighest = _CurrentAmplitude;
        _Amplitude = _CurrentAmplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
    }


    void CreateAudioBands64()
    {
        for (int i = 0; i < _audioBand64.Length; i++)
        {
            if (_freqBand64[i] > _freqBandHighest64[i]) _freqBandHighest64[i] = _freqBand64[i];

            _audioBand64[i] = (_freqBand64[i] / _freqBandHighest64[i]);
            _audioBandBuffer64[i] = (_bandBuffer64[i] / _freqBandHighest64[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    void BandBuffer64()
    {
        for (int g = 0; g < _freqBand64.Length; g++)
        {
            if (_freqBand64[g] > _bandBuffer64[g])
            {
                _bandBuffer64[g] = _freqBand64[g];
                _bufferDecrease64[g] = 0.005f;
            }

            if (_freqBand64[g] < _bandBuffer64[g])
            {
                _bandBuffer64[g] -= _bufferDecrease64[g];
                _bufferDecrease64[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands64()
    {
        int count = 0, sampleCount = 1, power = 0;
        for (int i = 0; i < 64; i++)
        {
            float average = 0;

            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int) Mathf.Pow(2, power);
                if (power == 3) sampleCount -= 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == _channel.Stereo) average += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                else if (channel == _channel.Left) average += _samplesLeft[count] * (count + 1);
                else if (channel == _channel.Right) average += _samplesRight[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand64[i] = average * 80;
        }

    }

}
