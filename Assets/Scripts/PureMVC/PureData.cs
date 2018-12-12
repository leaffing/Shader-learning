
namespace MyPureMVC
{
    struct PureData
    {
        private int _num;

        public int Num
        {
            get { return _num; }
        }

        public PureData GetData()
        {
            return this;
        }

        public void UpdataData()
        {
            _num++;
        }
    }
}
