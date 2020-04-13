using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using System;
namespace BasicGameFrameworkLibrary.BasicEventModels
{

    public class AnimateCardInPileEventModel<D> where D : class, IDeckObject, new()
    {
        public Action? FinalAction;
        public EnumAnimcationDirection Direction { get; set; }
        public D? ThisCard { get; set; }
        public MultiplePilesObservable.BasicPileInfo<D>? ThisPile { get; set; }
    }
}