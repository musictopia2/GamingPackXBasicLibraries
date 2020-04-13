using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.Views
{
    public static class BeginningColorDimensions
    {
        public static float GraphicsHeight { get; set; }
        public static float GraphicsWidth { get; set; }
    }
    public class BeginningChooseColorView<GC, GW, E> : ContentView, IUIView, IHandleAsync<LoadEventModel>
        where GC : BaseGraphicsCP, IEnumPiece<E>, new()
                where GW : BaseGraphicsXF<GC>, new()
                where E : struct, Enum
    {
        private readonly IEventAggregator _aggregator;
        private readonly IBeginningColorModel<E, GC> _model;
        private readonly EnumPickerXF<GC, GW, E> _color;
        //private readonly StackPanel _stack;
        public BeginningChooseColorView(IEventAggregator aggregator, IBeginningColorModel<E, GC> model)
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();
            _color = new EnumPickerXF<GC, GW, E>();
            if (BeginningColorDimensions.GraphicsHeight > 0)
            {
                _color.GraphicsHeight = BeginningColorDimensions.GraphicsHeight;
            }
            if (BeginningColorDimensions.GraphicsWidth > 0)
            {
                _color.GraphicsWidth = BeginningColorDimensions.GraphicsWidth;
            }
            stack.Children.Add(_color);
            SimpleLabelGridXF simple = new SimpleLabelGridXF();
            simple.AddRow("Turn", nameof(IBeginningColorViewModel.Turn));
            simple.AddRow("Instructions", nameof(IBeginningColorViewModel.Instructions));
            stack.Children.Add(simple.GetContent);

            Content = stack;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _color.LoadLists(_model.ColorChooser);
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}