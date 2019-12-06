using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    private LineRenderer _lineRenderer;
   // [Range(0,1)]
   // public float _lerpAmount;
    private Vector3[] _lerpPosition;
   // public float _generateMultiplier;
    private float[] _lerpAudio;

    [Header("Audio")] public AudioPeer _audioPeer;
    public int[] _audioBand;
   public Material _material;
    public Color _color;
    private Material _matInstance;
    public int _audioBandMaterial;
    public float _emissionMultiplier;

    void Start()
    {
        _lerpAudio = new float[_initiatorPointAmount];
       _lineRenderer = GetComponent<LineRenderer>();
       _lineRenderer.enabled = true;
       _lineRenderer.useWorldSpace = false;
       _lineRenderer.loop = true;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        _lerpPosition = new Vector3[_position.Length];

        _matInstance = new Material(_material);
        _lineRenderer.material = _matInstance;
    }

    private void Update()
    {
        _matInstance.SetColor("_Color", _color * _audioPeer._audioBandBuffer[_audioBandMaterial] *
                                                _emissionMultiplier);
        if (_generationCount != 0)
        {
            int count = 0;
            for (int i = 0; i < _initiatorPointAmount; i++)
            {
                _lerpAudio[i] = _audioPeer._audioBandBuffer[_audioBand[i]];
                for (int j = 0; j < (_position.Length - 1) / _initiatorPointAmount; j++)
                {
                    _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[i]);
                    count++;
                }
            }
            _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count],
                _lerpAudio[_initiatorPointAmount - 1]);
            
            if (_useBezierCurves)
            {
                _bezierPosition = BezierCurve(_lerpPosition, _bezierVertexCount);
                _lineRenderer.positionCount = _bezierPosition.Length;
                _lineRenderer.SetPositions(_bezierPosition);
            }
            else
            {
                _lineRenderer.positionCount = _lerpPosition.Length;
                _lineRenderer.SetPositions(_lerpPosition);
            }
        }
    }
}
 