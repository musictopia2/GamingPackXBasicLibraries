using BasicGameFramework.CommonInterfaces;
namespace BasicGameFramework.GameGraphicsCP.Interfaces
{
    public interface ICanvas
    {
        void SetLocation(ISelectableObject thisImage, double x, double y);
        void Clear();
        void AddChild(ISelectableObject thisImage);
    }
}