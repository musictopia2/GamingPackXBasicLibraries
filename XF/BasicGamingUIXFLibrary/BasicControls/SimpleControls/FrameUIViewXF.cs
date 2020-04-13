using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGamingUIXFLibrary.BasicControls.SimpleControls
{
    public class FrameUIViewXF : BaseFrameXF, IUIView
    {
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

        Task IUIView.TryActivateAsync()
        {
            return TryActivateAsync();
        }

        Task IUIView.TryCloseAsync()
        {
            return TryCloseAsync();
        }
        protected virtual Task TryCloseAsync() => Task.CompletedTask;
        protected virtual Task TryActivateAsync() => Task.CompletedTask;
    }
}
