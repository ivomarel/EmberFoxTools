namespace UIManager
{
    public class GenericPopupOne : GenericPopupBase
    {
        public void Init(string title, string description, ButtonData buttonData = null)
        {
            buttonData ??= new ButtonData();
            base.Init(title, description, buttonData);
        }
    }
}
