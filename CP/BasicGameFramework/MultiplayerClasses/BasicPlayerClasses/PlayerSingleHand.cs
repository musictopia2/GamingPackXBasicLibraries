using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using Newtonsoft.Json;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public class PlayerSingleHand<D> : SimplePlayer, IPlayerSingleHand<D> where D : IDeckObject, new()
    {

        private int _ObjectCount;

        public int ObjectCount
        {
            get { return _ObjectCount; }
            set
            {
                if (SetProperty(ref _ObjectCount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public DeckObservableDict<D> MainHandList { get; set; } = new DeckObservableDict<D>();
        [JsonIgnore]
        public DeckRegularDict<D> StartUpList { get; set; } = new DeckRegularDict<D>();
        public PlayerSingleHand()
        {
            HookUpHand();
        }
        public virtual void HookUpHand() //because its possible that something else needs to be done for games like fluxx.
        {
            MainHandList.CollectionChanged += MainObjectList_CollectionChanged;
        }
        protected virtual int GetTotalObjectCount => MainHandList.Count;
        protected virtual void OnCollectionChange(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ObjectCount = GetTotalObjectCount; //games like racko needs something else to happen.
        }
        private void MainObjectList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChange(e);
        } //hopefully this will take care of a lot of the issues i had previously.
    }
}