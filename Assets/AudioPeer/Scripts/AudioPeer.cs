using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    public static AudioSource _audioSource;
    
    //Microphone input
    public AudioClip _audioClip;

    //FFT values
    private static float[] _samplesLeft = new float[512];
    private static float[] _samplesRight = new float[512];

    private float[] _freqBand = new float[8];
    private float[] _bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];
    private float[] _freqBandHighest = new float[8];

    //audio band values
    [HideInInspector] public float[] _audioBand, _audioBandBuffer;

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
        _audioBand = new float[8];
        _audioBandBuffer = new float[8];
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);
        
        _audioSource.clip = _audioClip;

        //_audioSource.Play();
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < _freqBandHighest.Length; i++) _freqBandHighest[i] = audioProfile;
    }

    void GetAmplitude()
    {
        float _CurrentAmplitude = 0, _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }

        if (_CurrentAmplitude > _AmplitudeHighest) _AmplitudeHighest = _CurrentAmplitude;
        _Amplitude = _CurrentAmplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < _audioBand.Length; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i]) _freqBandHighest[i] = _freqBand[i];

            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
        
    }    
    
    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }

            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }

            if (_freqBand[g] < 0) _bandBuffer[g] = 0;
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * 22080 / 512 = 43hertz per sample
         *
         * 20 - 60 = hertz
            60 - 250 = hertz
            250 - 500 = hertz
            500 - 2000 = hertz
            2000 - 4000 = hertz
            4000 - 6000 = hertz
            6000 - 20000 = hertz
         * 
         *  0 - 2 = 86 hertz
         *  1 - 4 = 172 hertz - 87 - 258
         *  2 - 8 = 344 hertz - 259 - 602
         *  3 - 16 = 688 hertz - 603 - 1290
         *  4 - 32 = 1376 hertz - 1291 - 2666
         *  5 - 64 = 2752 hertz - 2667 - 5418
         *  6 - 128 = 5504 hertz - 5419 - 10922
         *  7 - 256 = 11008 hertz - 10923 - 21930
         *  510
         */

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int) Mathf.Pow(2, i) * 2;

            if (i == 7) sampleCount += 2;
            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == _channel.Stereo) average += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                else if (channel == _channel.Left) average += _samplesLeft[count] * (count + 1);
                else if (channel == _channel.Right) average += _samplesRight[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }

    }

}
