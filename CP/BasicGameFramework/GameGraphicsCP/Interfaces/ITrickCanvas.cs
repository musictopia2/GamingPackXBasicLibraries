﻿using SkiaSharp;
namespace BasicGameFramework.GameGraphicsCP.Interfaces
{
    public interface ITrickCanvas
    {
        void SetLocation(int index, double x, double y);
        SKPoint GetStartingPoint(int index);
    }
}