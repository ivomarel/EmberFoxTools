namespace UIManager.Example.Scripts
{
    public class SampleMainScreen : UIScreen<SampleMainScreen>
    {
        private int nInfinitePopups;
        
        public void OnOptionsButton()
        {
            canvasManager.Show<SampleOptionsScreen>();
        }
        
        public void OnPopupButton()
        {
            canvasManager.Show<GenericPopupTwo>().Init(
                "Example", 
                "New screens automatically get pushed on top of old screens", 
                new ButtonData("Cancel", null, true),
                new ButtonData("Continue", OnContinueButton1, false)
            );
        }
        
        private void OnContinueButton1()
        {
            canvasManager.Show<GenericPopupTwo>().Init(
                "Pooling Example", 
                "By automatically pooling the screens, you can reuse the same screen with different content", 
                new ButtonData("Cancel"),
                new ButtonData("Continue", OnContinueButton2, false)
            );
        }

        private void OnContinueButton2()
        {
            canvasManager.Show<GenericPopupTwo>().Init(
                "Stack Example", 
                "Each screen is automatically added to a stack, so if you 'Pop' it, it will automatically show the previous screen.",
                new ButtonData("Cancel"),
                new ButtonData("Continue", OnContinueButton3, false)
            );
        }
        
        private void OnContinueButton3()
        {
            canvasManager.Show<GenericPopupTwo>().Init(
                "Infinity Example",
                $"You can do this forever! You have pushed {++nInfinitePopups} popups so far",
                new ButtonData("Cancel", null, true),
                new ButtonData("Continue", OnContinueButton3, false)
            );
        }
    }
}
