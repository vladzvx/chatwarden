using ProGaudi.Tarantool.Client;

namespace ChatWarden.CoreLib.Bot.Queue
{
    public class QueueWorkerBase
    {
        protected readonly Box _box;
        public QueueWorkerBase(Box box)
        {
            _box = box;
        }
    }
}
