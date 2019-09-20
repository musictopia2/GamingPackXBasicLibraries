using BasicGameFramework.CommonInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System;
namespace BasicGameFramework.GameGraphicsCP.GamePieces
{
    public class CheckerChoiceCP<E> : CheckerPiecesCP, IEnumPiece<E> where E : Enum
    {
        private string _tempValue = "";
        E IEnumPiece<E>.EnumValue
        {
            get
            {
                return (E)Enum.Parse(typeof(E), _tempValue);
            }
            set
            {
                _tempValue = value.ToString();
                MainColor = value.ToColor();
            }
        }
    }
}