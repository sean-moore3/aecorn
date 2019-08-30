namespace NationalInstruments.Aecorn
{
    /// <summary>
    /// Creates a reference type around the specified generic type.
    /// </summary>
    public class Box<T>
    {
        public T item;
        public Box(T item)
        {
            this.item = item;
        }
    }
}
