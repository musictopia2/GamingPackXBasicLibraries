using BasicGameFramework.BasicDrawables.Interfaces;
using System;
namespace BasicGameFramework.BasicEventModels
{

    public class AnimateCardInPileEventModel<D> where D : class, IDeckObject, new()
    {
        public Action? FinalAction;
        public EnumAnimcationDirection Direction { get; set; }
        public D? ThisCard { get; set; }
        public MultiplePilesViewModels.BasicPileInfo<D>? ThisPile { get; set; }
    }
}