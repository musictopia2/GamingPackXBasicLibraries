using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq; //sometimes i do use linq.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;

namespace BasicGameFrameworkLibrary.MiscProcesses
{

    //step 1:
    //copy my old code and see what changes are required.

    //i took a risk and used commandcontainer for parameter instead of interface for view model.
    //well see how this goes (?)
    //if i run into problems, make notes and rethink.

    public abstract class SimpleControlObservable : ObservableObject, IControlObservable
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (CanEnableFirst() == false)
                    value = false;// otherwise, value can always be false.
                if (SetProperty(ref _isEnabled, value) == true)
                    EnableChange();
            }
        }
        public EnumCommandBusyCategory BusyCategory { get; set; } = EnumCommandBusyCategory.None; //most of the time, none.
        public bool AlwaysDisabled { set; private get; }
        protected CommandContainer CommandContainer;
        //decided to let it invoke via di.  hopefully that works.
        //this should not care about the main view model anyways.
        //especially since its being splitted apart now.
        public SimpleControlObservable(CommandContainer container)
        {
            CommandContainer = container;
            CommandContainer.AddControl(this); //i think it should be here instead.
        }
        protected virtual bool CanEnableFirst()
        {
            return !AlwaysDisabled;
        }
        public void ManualChange()
        {
            EnableChange();
        }
        protected abstract void EnableChange();

        //hopefully no need for visible since its splitted out now.
        //however, if i need visible for other reasons, can put back in.
        //has to see what cases we still need the visible even though its splitted out.


        //private bool _visible;
        //public bool Visible
        //{
        //    get
        //    {
        //        return _visible;
        //    }
        //    set
        //    {
        //        if (SetProperty(ref _visible, value) == true)
        //            VisibleChange();
        //    }
        //}
        //protected abstract void VisibleChange();
        protected virtual bool SpecialEnable()
        {
            return true;
        }
        protected IBasicEnableProcess? _networkProcess;
        private IEnableAlways? _alwaysProcess;
        protected Func<bool>? _customFunction; // i think this will have no parameters for this one.
        protected bool _useSpecial;
        public virtual void SendEnableProcesses(IBasicEnableProcess nets, Func<bool> fun)
        {
            SendFunction(fun);
            _useSpecial = true;
            _networkProcess = nets;
        }
        //private void ShowSpecial()
        //{
        //    //if (_useSpecial == true)
        //    //    throw new BasicBlankException("Already Enabled");
        //    _useSpecial = true;
        //}
        public void SendFunction(Func<bool> fun)
        {
            //if (_useSpecial)
            //{
            //    return;
            //}
            //ShowSpecial();
            _useSpecial = true;
            _customFunction = fun;
        }
        public void SendAlwaysEnable(IEnableAlways always)
        {
            //ShowSpecial();
            _alwaysProcess = always;
            BusyCategory = EnumCommandBusyCategory.Limited;
            SetCommandsLimited();
        }
        protected virtual void SetCommandsLimited() { }
        protected abstract void PrivateEnableAlways(); //everything needs to decide what to do about it.
        public void ReportCanExecuteChange()
        {
            IsEnabled = true;
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
            {
                IsEnabled = false; //i think
                return;
            }
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
            {
                IsEnabled = false;
                return;
            }
            if (_useSpecial == false)
            {
                if (AlwaysDisabled == true)
                    IsEnabled = false;
                else
                    IsEnabled = true; //for now until i figure out further.
                return;
            }
            if (_alwaysProcess != null)
            {
                IsEnabled = _alwaysProcess.CanEnableAlways();
                return;
            }
            if (_networkProcess != null)
            {
                if (_networkProcess.CanEnableBasics() == false)
                {
                    IsEnabled = false;
                    return;
                }
            }
            IsEnabled = _customFunction!();
        }
        bool IControlObservable.CanExecute()
        {
            //if (Visible == false)
            //    return false;
            if (IsEnabled == false)
                return false;
            return SpecialEnable();
        }
    }
}
