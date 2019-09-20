using BasicGameFramework.BasicDrawables.BasicClasses;
namespace BaseMahjongTilesCP
{
    public abstract class BasicMahjongTile : SimpleDeckObject, IMahjongTileInfo
    {
        public enum EnumDirectionType
        {
            IsNorth = 1,
            IsSouth = 2,
            IsWest = 3,
            IsEast = 4,
            IsNoDirection = 0
        }
        public enum EnumColorType
        {
            IsRed = 1,
            IsGreen = 2,
            IsWhite = 3,
            IsNoColor = 4
        }
        public enum EnumBonusType
        {
            IsNoBonus = 0,
            IsSeason = 1,
            IsFlower = 2
        }
        public enum EnumNumberType
        {
            IsCircle = 1,
            IsBamboo = 2,
            IsCharacter = 3,
            IsNoNumber = 0
        }
        private int _Index; // this is needed to get the proper image when it comes to drawing.
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                if (SetProperty(ref _Index, value) == true) { }
            }
        }
        public int NumberUsed { get; set; }
        public EnumColorType WhatColor { get; set; }
        public EnumBonusType WhatBonus { get; set; }
        public EnumNumberType WhatNumber { get; set; }
        public EnumDirectionType WhatDirection { get; set; }
        private float _Left;
        public float Left
        {
            get
            {
                return _Left;
            }
            set
            {
                if (SetProperty(ref _Left, value) == true) { }
            }
        }
        private float _Top;
        public float Top
        {
            get
            {
                return _Top;
            }
            set
            {
                if (SetProperty(ref _Top, value) == true) { }
            }
        }
        private bool _NeedsLeft;
        public bool NeedsLeft
        {
            get
            {
                return _NeedsLeft;
            }
            set
            {
                if (SetProperty(ref _NeedsLeft, value) == true) { }
            }
        }
        private bool _NeedsTop;
        public bool NeedsTop
        {
            get
            {
                return _NeedsTop;
            }
            set
            {
                if (SetProperty(ref _NeedsTop, value) == true) { }
            }
        }
        private bool _NeedsRight;
        public bool NeedsRight
        {
            get
            {
                return _NeedsRight;
            }
            set
            {
                if (SetProperty(ref _NeedsRight, value) == true) { }
            }
        }
        private bool _NeedsBottom;
        public bool NeedsBottom
        {
            get
            {
                return _NeedsBottom;
            }
            set
            {
                if (SetProperty(ref _NeedsBottom, value) == true) { }
            }
        }
    }
}