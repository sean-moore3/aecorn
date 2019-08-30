namespace NationalInstruments.Aecorn
{
    /// <summary>
    /// Creates a reference type around the specified generic type.
    /// </summary>
    public class Box<T>
    {
        public T item1;
        public Box(T item1)
        {
            this.item1 = item1;
        }
    }
}
