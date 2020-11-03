using System.Threading.Tasks;

namespace CfmArt.Functional
{
    /// <summary></summary>
    public static class TaskToMonad
    {
        /// <summary></summary>
        public static MaybeTask<T> ToOptional<T>(this Task<Optional<T>> self)
            => MaybeTask.From(self);

        /// <summary></summary>
        public static StateTask<T> ToState<T>(this Task<T> self)
            => StateTask.From(self);
    }
}
