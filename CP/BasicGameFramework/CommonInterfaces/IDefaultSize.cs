using SkiaSharp;
namespace BasicGameFramework.CommonInterfaces
{
    public interface IDefaultSize
    {
        SKSize DefaultSize { get; set; } //this is the default size.  however each client has to calculate themselves
        SKSize SizeUsed { get; set; } //so an extension can work with this.
    }
}