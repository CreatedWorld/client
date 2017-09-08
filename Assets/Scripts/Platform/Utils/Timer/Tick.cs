namespace Utils
{
    public delegate void TimerCallBack();

    public class Tick
    {
        public int tid;
        public float start;
        public int count;        
        public float interval;
        public TimerCallBack cbfunc;
        public bool pause;
        public bool fix;
    }
}
