namespace Cloudsume.DataOperations
{
    public abstract class DataOperation
    {
        protected DataOperation(string key)
        {
            this.Key = key;
        }

        public string Key { get; }
    }
}
