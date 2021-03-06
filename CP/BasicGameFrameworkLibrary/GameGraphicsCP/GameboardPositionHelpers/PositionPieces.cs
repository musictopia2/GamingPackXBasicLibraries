﻿using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
using System;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers
{
    public class PositionPieces
    {
        public SKPoint GetPosition(GameSpace thisSpace, float actualWidth, float actualHeight)
        {
            if ((thisSpace.PieceList.Count == 0) & (thisSpace.ObjectList.Count == 0))
                return new SKPoint(thisSpace.Area.Left, thisSpace.Area.Top);
            int stepX = (int)actualWidth;
            int stepY = (int)actualHeight;
            int lastX = -1;
            int firstY = 0;
            int breakY = 0;
            int breakX = 0;
            int numFound = 0;
            int y;
            int x = stepX - 1;
            bool isFound;
            y = 0;
            while (y <= (thisSpace.Area.Height - 1))
            {
                while (x <= (thisSpace.Area.Width - 1))
                {
                    if (thisSpace.NewArea![x, y] == 0)
                    {
                        isFound = true;
                        var loopTo = stepX - 1;
                        for (int d = 1; d <= loopTo; d++)
                        {
                            if (!(thisSpace.NewArea[x - d, y] == 0))
                            {
                                breakX = x - d;
                                breakY = y;
                                isFound = false;
                                break;
                            }
                        }
                        if (isFound)
                        {
                            if (lastX == ((x - stepX) + 1))
                            {
                                numFound += 1;
                            }
                            else
                            {
                                numFound = 1;
                                firstY = y;
                                lastX = (x - stepX) + 1;
                            }
                            break;
                        }
                        else
                        {
                            x = breakX + stepX;
                            y = firstY - 1;
                            break;
                        }
                    }
                    else
                    {
                        x += 1;
                    }
                }
                if (x > (thisSpace.Area.Width - 1))
                {
                    lastX = -1;
                    x = stepX - 1;
                    numFound = 0;
                    y = breakY;
                    breakY += 1;
                    firstY = y + 1;
                }
                if (numFound == stepY)
                    return new SKPoint(lastX + thisSpace.Area.Left, firstY + thisSpace.Area.Top);
                y += 1;
            }
            return new SKPoint(-1000, -1000);
        }
        public void AddPieceToArea(GameSpace thisSpace, BaseGraphicsCP obj)
        {
            var thisRect = obj.GetMainRect();
            AddRectToArea(thisSpace, thisRect);
        }
        public void AddRectToArea(GameSpace thisSpace, SKRect obj)
        {
            var loopTo = obj.Width + obj.Left - thisSpace.Area.Left;
            for (int x = (int)obj.Left - (int)thisSpace.Area.Left; x <= loopTo; x++)
            {
                var loopTo1 = obj.Height + obj.Top - thisSpace.Area.Top;
                for (int y = (int)obj.Top - (int)thisSpace.Area.Top; y <= loopTo1; y++)
                {
                    if (!((x < 0) | (y < 0)))
                        thisSpace.NewArea![x, y] = 1;
                }
            }
        }
        public void ClearArea(GameSpace thisSpace)
        {
            var thisRect = thisSpace.Area;
            thisSpace.Area = thisRect;
            foreach (var temps in thisSpace.ObjectList)
            {
                try
                {
                    AddRectToArea(thisSpace, temps);
                }
                catch (Exception)
                {
                }
            }
            thisSpace.PieceList = new CustomBasicList<BaseGraphicsCP>(); // can't use generics because we may have more than one like this like clue
        }
    }
}