using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UIManager
{
    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager main;

        private static Dictionary<CanvasTag, CanvasManager> tagToCanvas = new();

        public enum CanvasTag
        {
            Main,
            Quest
        }

        [SerializeField] private CanvasTag canvasTag;
        [SerializeField] private BaseUIScreen[] screenPrefabs;
        [SerializeField] private Transform screenParent;
        [SerializeField] private bool pushOpeningScreen = true;
        [SerializeField] private bool isLogging;

        /// <summary>
        /// This should not be edited directly. Instead, use Hide and Show to add/remove screens
        /// </summary>
        private List<BaseUIScreen> currentlyVisible = new List<BaseUIScreen>();

        private Dictionary<Type, Stack<BaseUIScreen>> typeToScreenPool;

        public static CanvasManager Get(CanvasTag tag)
        {
            CanvasManager mgr;
            if (tagToCanvas.TryGetValue(tag, out mgr))
                return mgr;
            Debug.LogError($"No canvas with tag {tag} found in the scene.");
            return null;
        }

        private void Awake()
        {
            tagToCanvas[canvasTag] = this;
            if (this.screenParent == null)
            {
                this.screenParent = this.transform;
            }
            if (canvasTag == CanvasTag.Main && main == null)
            {
                main = this;
            }

            typeToScreenPool = new Dictionary<Type, Stack<BaseUIScreen>>();

            foreach (BaseUIScreen screen in screenPrefabs)
            {
                var screenPool = new Stack<BaseUIScreen>() {};
                screenPool.Push(screen);
                typeToScreenPool.Add(screen.GetType(), screenPool );
            }

            if (pushOpeningScreen && screenPrefabs.Length > 0 && screenPrefabs[0] != null)
            {
                Show(screenPrefabs[0].GetType());
            }

        }

        private IEnumerator Start ()
        {
           
            //GetComponent<Canvas>().additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
            
            yield return null;
        }


        public BaseUIScreen GetScreenPrefab(Type screenType)
        {
            foreach (BaseUIScreen screen in screenPrefabs)
            {
                if (screen.GetType() == screenType)
                {
                    return screen;
                }
            }

            return null;
        }

        public T Show<T>() where T : BaseUIScreen
        {
            return Show(typeof(T)) as T;
        }

        public BaseUIScreen Show(Type screenType)
        {
            Log("Showing " + screenType);
            
            var screen = GetScreen(screenType);
            if (screen != null && !screen.duplicateOnReShow)
            {
                //We remove it from the list to avoid it being added twice 
                currentlyVisible.Remove(screen);
            }
            else
            {
                screen = GetOrCreateScreenInstance(screenType);
            }

            if (screen.hideCurrent)
            {
                for (int i = currentlyVisible.Count - 1; i >= 0; i--)
                {
                    currentlyVisible[i].OnHide();
                }
            }

            currentlyVisible.Add(screen);
            ApplyPriorityOrdering();
            screen.OnShow();
            return screen;
        }

        public bool IsShowing<T>()
        {
            return GetScreen(typeof(T)) != null;
        }
        
        public T GetScreen<T>() where T : BaseUIScreen
        {
            return GetScreen(typeof(T)) as T;
        }

        private BaseUIScreen GetScreen(Type screenType)
        {
            for (int i = currentlyVisible.Count - 1; i >= 0; i--)
            {
                var screen = currentlyVisible[i];
                if (screen.GetType() == screenType)
                {
                    return screen;
                }
            }
            return null;
        }


        public T Hide<T>() where T : BaseUIScreen
        {
            for (int i = currentlyVisible.Count - 1; i >= 0; i--)
            {
                var screen = currentlyVisible[i];
                
                var baseUIScreen = screen as T;
                if (baseUIScreen != null)
                {
                    Hide(screen);
                    return baseUIScreen;
                }
            }

            Log($"Unable to hide {typeof(T)}");
            return null;
        }

        public void HideAll()
        {
            for (int i = currentlyVisible.Count - 1; i >= 0; i--)
            {
                var screen = currentlyVisible[i];
                Hide(screen);
            }
        }

        public void Hide(BaseUIScreen screen)
        {
            screen.OnHide();
        }

        private BaseUIScreen GetOrCreateScreenInstance(Type screenType)
        {
            if (!typeToScreenPool.ContainsKey(screenType))
            {
                Debug.LogError(string.Format("CanvasManager does not contain ({0})", screenType));
                return null;
            }

            Stack<BaseUIScreen> screenPool = typeToScreenPool[screenType];
            
            if (screenPool.Count == 1)
            {
                //If there is only 1, this is a reference of the prefab. We create a copy.
                var newScreen = Instantiate(screenPool.Peek(), screenParent);
                screenPool.Push(newScreen);
            }
            
            return screenPool.Pop();
        }

        /// <summary>
        /// For INTERNAL use only
        /// </summary>
        /// <param name="screen"></param>
        public void RemoveVisible(BaseUIScreen screen)
        {
            currentlyVisible.Remove(screen);
            typeToScreenPool[screen.GetType()].Push(screen);
        }

        private void ApplyPriorityOrdering()
        {
            if (currentlyVisible.Count <= 1)
            {
                return;
            }

            var ordered = currentlyVisible
                .Select((screen, index) => new { screen, index })
                .OrderBy(entry => entry.screen.priority)
                .ThenBy(entry => entry.index)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].screen.transform.SetAsLastSibling();
            }
        }

        private void Log(object obj)
        {
            if (isLogging)
            {
                Debug.Log($"CanvasManager: {obj}");
            }
        }
    }
}
