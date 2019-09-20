using CommonBasicStandardLibraries.MVVMHelpers;
using System;
namespace BasicGameFramework.YahtzeeStyleHelpers
{
    public class RowInfo : ObservableObject
    {
        private string _Description = "";
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (SetProperty(ref _Description, value) == true) { }
            }
        }
        private int _RowNumber;
        public int RowNumber
        {
            get
            {
                return _RowNumber;
            }
            set
            {
                if (SetProperty(ref _RowNumber, value) == true) { }
            }
        }
        private EnumRowEnum _RowSection;
        public EnumRowEnum RowSection
        {
            get
            {
                return _RowSection;
            }
            set
            {
                if (SetProperty(ref _RowSection, value) == true) { }
            }
        }
        private bool _IsTop;
        public bool IsTop
        {
            get
            {
                return _IsTop;
            }
            set
            {
                if (SetProperty(ref _IsTop, value) == true) { }
            }
        }
        private bool _IsRecent;
        public bool IsRecent
        {
            get
            {
                return _IsRecent;
            }
            set
            {
                if (SetProperty(ref _IsRecent, value) == true) { }
            }
        }
        private int? _Possible;
        public int? Possible
        {
            get
            {
                return _Possible;
            }
            set
            {
                if (SetProperty(ref _Possible, value) == true) { }
            }
        }
        private int? _PointsObtained;
        public int? PointsObtained
        {
            get
            {
                return _PointsObtained;
            }
            set
            {
                if (SetProperty(ref _PointsObtained, value) == true) { }
            }
        }
        internal bool IsAllFive()
        {
            if (Description == "Kismet (5 Of A Kind)" || Description == "Yahtzee")
                return true;
            return false;
        }
        public bool HasFilledIn()
        {
            if (RowSection == EnumRowEnum.Header || RowSection == EnumRowEnum.Totals)
                throw new Exception("HasFilledIn can only be figured out for Bonus or Regular Rows");
            if (PointsObtained.HasValue == false)
                return false;
            return true;
        }
        public void ClearText()
        {
            Possible = default;
            PointsObtained = default;
            IsRecent = false;
        }
        public void ClearPossibleScores()
        {
            Possible = default;
        }
        public RowInfo(EnumRowEnum section, bool isTop)
        {
            RowSection = section;
            IsTop = isTop;
        }
        public RowInfo() { }
    }
}