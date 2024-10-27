namespace LuniLib.Helpers
{
    [System.Serializable]
    public struct SerializablePair<T1, T2>
    {
        public SerializablePair(T1 key, T2 value)
        {
            this.Key = key;
            this.Value = value;
        }

        public void Deconstruct(out T1 key, out T2 value)
        {
            key = this.Key;
            value = this.Value;
        }
        
        public T1 Key;
        public T2 Value;
    }

    [System.Serializable]
    public struct SerializableTuple<T1, T2>
    {
        public SerializableTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        
        public T1 Item1;
        public T2 Item2;
        
        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = this.Item1;
            item2 = this.Item2;
        }
    }
    
    [System.Serializable]
    public struct SerializableTuple<T1, T2, T3>
    {
        public SerializableTuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
        
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public void Deconstruct(out T1 item1, out T2 item2, out T3 item3)
        {
            item1 = this.Item1;
            item2 = this.Item2;
            item3 = this.Item3;
        }
    }
    
    [System.Serializable]
    public struct SerializableTuple<T1, T2, T3, T4>
    {
        public SerializableTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }
        
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public void Deconstruct(out T1 item1, out T2 item2, out T3 item3, out T4 item4)
        {
            item1 = this.Item1;
            item2 = this.Item2;
            item3 = this.Item3;
            item4 = this.Item4;
        }
    }
}