using BasicGameFramework.MiscProcesses;
namespace BasicGameFramework.BasicEventModels
{
    public class AnimatePieceEventModel<T> where T : class //try this way.  if it causes problems, rethink.
    {
        public Vector PreviousSpace { get; set; }
        public Vector MoveToSpace { get; set; }
        public T? TemporaryObject { get; set; }
        public bool UseColumn { get; set; } // if using columns, needs to be a little different (like for games like connect four)
    }
}