using UnityEngine;
using UnityEngine.UI;

namespace UIManager.Example.Scripts
{
    /// <summary>
    /// This OptionsScreen could be invoked from anywhere, and will simply return to the previous screen after the user is done
    /// </summary>
    public class SampleOptionsScreen : UIScreen<SampleOptionsScreen>
    {
        [SerializeField]
        private Slider randomSlider;
        
        private bool markedDirty;
        
        //Generally you would not store game-variables inside your UI. This is for testing purposes only
        private float randomValue;

        public override void OnShow()
        {
            base.OnShow();
            randomSlider.value = randomValue;
        }

        public void OnRandomSlider(float value)
        {
            Debug.Log($"I set some value to {value}");
            markedDirty = true;
        }

        public void OnBackButton()
        {
            if (markedDirty)
            {
                canvasManager.Show<GenericPopupTwo>().Init(
                    "Just making sure...",
                    $"Would you like to save your changes?",
                    new ButtonData("No", BackToPreviousMenu),
                    new ButtonData("Yes", SaveChanges)
                );
            }
            else
            {
                BackToPreviousMenu();
            }   
        }

        private void SaveChanges()
        {
            Debug.Log("This is where you would save your changes!");
            randomValue = randomSlider.value;
            BackToPreviousMenu();
        }

        private void BackToPreviousMenu()
        {
            canvasManager.Hide<SampleOptionsScreen>();
        }
        
        
    }
}
