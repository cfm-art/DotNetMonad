using System.Threading.Tasks;

namespace CfmArt.Functional
{
    public static class TaskToMonad
    {
        public static MaybeTask<T> ToOptional<T>(this Task<Optional<T>> self)
            => MaybeTask.From(self);

        public static StateTask<T> ToState<T>(this Task<T> self)
            => StateTask.From(self);
    }
}
