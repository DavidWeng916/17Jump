using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Live17Game
{
    public enum SpawnDirection
    {
        Left,
        Right
    }

    public enum JumpResult
    {
        None,
        Success,
        Fail
    }

    public class PlatformPoint
    {
        public Vector3 PlayerPoint { get; private set; }
        public Vector3 ReferencePoint { get; private set; }
        public float FallDownAngle { get; private set; }

        public float Distance { get; private set; }

        public PlatformPoint(Vector3 playerPoint, Vector3 referencePoint, float fallDownAngle)
        {
            PlayerPoint = playerPoint;
            ReferencePoint = referencePoint;
            FallDownAngle = fallDownAngle;

            Distance = Vector3.Distance(PlayerPoint, ReferencePoint);
        }
    }

    public struct PlatformSizeRange
    {
        public uint UnitMin;
        public uint UnitMax;

        public PlatformSizeRange(uint unitMin, uint unitMax)
        {
            UnitMin = unitMin;
            UnitMax = unitMax;
        }
    }

    public struct PlatformData
    {
        public uint Size;
        public float AngleY;
        public Vector3 LocalPosition;

        public PlatformData(uint size, float angleY, Vector3 localPosition)
        {
            Size = size;
            AngleY = angleY;
            LocalPosition = localPosition;
        }
    }

    public struct LevelData
    {
        public PlatformSizeRange SizeRange { get; private set; }
        public uint DistanceUnit { get; private set; }

        public LevelData(PlatformSizeRange sizeRange, uint distanceUnit)
        {
            SizeRange = sizeRange;
            DistanceUnit = distanceUnit;
        }
    }

    [Serializable]
    public class GameData
    {
        public uint BestScore = 0;
        public bool MusicToggle = true;
    }

    public enum GameState
    {
        None,
        Start,
        End,
    }

    public struct CoordConvertData
    {
        public Camera Camera { get; private set; }
        public Canvas Canvas { get; private set; }
        public RectTransform Container { get; private set; }
        public Vector3 WorldPoint { get; private set; }

        public CoordConvertData(Camera camera, Canvas canvas, RectTransform container, Vector3 worldPoint)
        {
            Camera = camera;
            Canvas = canvas;
            Container = container;
            WorldPoint = worldPoint;
        }
    }

    public class IntervalSetting
    {
        public float IntervalMin { get; private set; }
        public float IntervalTimeMax { get; private set; }
        public float DecreaseTime { get; private set; }

        private float _intervalTime = 0f;
        private float _lastTime;

        private bool _isEnable = false;

        public Action _callback = null;

        public IntervalSetting(float intervalMin, float intervalMax, float decreaseTime, Action callback)
        {
            IntervalMin = intervalMin;
            IntervalTimeMax = intervalMax;
            DecreaseTime = decreaseTime;
            _callback = callback;
        }

        public void Reset()
        {
            _intervalTime = IntervalTimeMax;
        }

        public void Play()
        {
            Reset();
            Invoke();

            _isEnable = true;
        }

        public void Stop()
        {
            Reset();
            _isEnable = false;
        }

        public void Update()
        {
            if (!_isEnable)
            {
                return;
            }

            if (Time.time - _lastTime >= _intervalTime)
            {
                _intervalTime = Mathf.Max(IntervalMin, _intervalTime - DecreaseTime);
                // Debug.Log($"===== _intervalTime:{_intervalTime}");
                Invoke();
            }
        }

        private void Invoke()
        {
            _callback?.Invoke();
            _lastTime = Time.time;
        }
    }
}