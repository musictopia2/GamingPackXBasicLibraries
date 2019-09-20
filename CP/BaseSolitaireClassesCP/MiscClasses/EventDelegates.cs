using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BaseSolitaireClassesCP.MiscClasses
{
    public delegate Task WastePileSelectedEventHandler(int index);
    public delegate Task WasteDoubleClickEventHandler(int index);
}