namespace MovieDbApi.Common.Domain.Utility
{
    public static class ObjectCopy
    {
        public static TValue ShallowCopy<TValue>(TValue obj)
        {
            if (obj == null)
            {
                return default(TValue);
            }

            TValue result = (TValue) Activator.CreateInstance(typeof(TValue));

            typeof(TValue).GetProperties().ForEach(x => x.SetValue(result, x.GetValue(obj)));

            return result;
        }
    }
}
