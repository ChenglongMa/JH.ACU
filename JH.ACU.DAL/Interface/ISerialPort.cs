namespace JH.ACU.DAL.Interface
{
    public interface ISerialPort
    {
        void Open();
        void Read();
        void Write();
        void Close();
    }
}