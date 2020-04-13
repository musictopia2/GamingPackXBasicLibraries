using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Threading.Tasks;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.CommandClasses
{
    public class CommandContainer
    {
        private readonly CustomBasicList<IGameCommand> _commandList = new CustomBasicList<IGameCommand>();
        private readonly CustomBasicList<IGameCommand> _openList = new CustomBasicList<IGameCommand>();
        public event ExecutingChangedEventHandler? ExecutingChanged; //don't need anything else because can call a method to see where it stands.
        public delegate void ExecutingChangedEventHandler(); //i think
        private readonly CustomBasicList<IControlObservable> _controlList = new CustomBasicList<IControlObservable>();
        public CommandContainer()
        {
            IsExecuting = true; //i think it needs to start out as true.
        }

        public void StartExecuting()
        {
            IsExecuting = true;
            Processing = true;
        }

        internal async Task ProcessCustomCommandAsync<T>(Func<T, Task> action, T argument)
        {
            StartExecuting();
            await action.Invoke(argument);
            StopExecuting();
        }
        internal async Task ProcessCustomCommandAsync(Func<Task> action)
        {
            StartExecuting();
            await action.Invoke();
            StopExecuting();
        }


        public void StopExecuting()
        {
            if (ManuelFinish == false)
            {
                IsExecuting = false;
            }
            Processing = false;
        }
        public void ClearLists()
        {
            _commandList.Clear();
            _openList.Clear();
            _controlList.Clear();
        }

        private bool _executing;
        /// <summary>
        /// This is used when its not even your turn.
        /// Use Processing if you are able to do things out of turn as long
        /// as the other variable is false.
        /// </summary>
        /// <remarks>This is used when its not even your turn.
        /// Use Processing if you are able to do things out of turn as long
        /// as the other variable is false.</remarks>
        public bool IsExecuting
        {
            get
            {
                return _executing;
            }
            set
            {

                if (value == _executing)
                    return;
                _executing = value;
                ReportAll();
            }
        }
        private bool _openBusy;
        public bool OpenBusy
        {
            get
            {
                return _openBusy;
            }
            set
            {
                if (value == _openBusy)
                    return;
                _openBusy = value;
                ReportOpen();
            }
        }
        public void ReportOpen()
        {
            _openList.ForEach(Items => Items.ReportCanExecuteChange()); //i think this simple.
        }
        private bool _processing = true; //you need to start out that its processing.
        public bool Processing
        {
            get { return _processing; }
            set
            {
                if (value == _processing)
                    return;
                _processing = value;
                ReportLimited();
            }
        }
        public void ReportLimited()
        {
            ReportItems(EnumCommandBusyCategory.Limited);
        }
        public bool ManuelFinish { get; set; } = false;

        public void ManualReport()
        {
            _commandList.ForEach(x => x.ReportCanExecuteChange());
            _controlList.ForEach(x => x.ReportCanExecuteChange());
        }
        public void RemoveOldItems(object payLoad)
        {
            _commandList.RemoveAllOnly(x => x.Context == payLoad);
        }
        public void ReportAll() //when changing, will report to all no matter what.  decided it can be good to notify all that something has changed.
        {
            ReportItems(EnumCommandBusyCategory.None);
            if (ExecutingChanged != null)
                ExecutingChanged.Invoke();
        }
        private void ReportItems(EnumCommandBusyCategory thisBusy)
        {
            _commandList.ForConditionalItems(items => items.BusyCategory == thisBusy
            , items => items.ReportCanExecuteChange());
            _controlList.ForConditionalItems(items => items.BusyCategory == thisBusy
            , items => items.ReportCanExecuteChange());
        }
        public void AddOpen(IGameCommand thisOpen)
        {
            _openList.Add(thisOpen);
        }
        public void AddCommand(IGameCommand thisCommand)
        {
            _commandList.Add(thisCommand);
        }
        public void AddControl(IControlObservable thisControl)
        {
            _controlList.Add(thisControl);
        }
    }
}
