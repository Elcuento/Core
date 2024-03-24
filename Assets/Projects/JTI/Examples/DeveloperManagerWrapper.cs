using System.Collections.Generic;
using UnityEngine;

namespace JTI.Examples
{
    public class DeveloperManagerWrapper : DeveloperManager
    {
        protected override void OnAwaken()
        {
            base.OnAwaken();

        }

        protected override List<DeveloperItem> MainPage()
        {
            return new List<DeveloperItem>()
            {
                AddText("Just text", onUpdate: (b) => { b.Text.text = Random.Range(0, 99).ToString(); }),

                AddButton("Just button ", () => { Debug.Log("I am just a button after all.."); }),

                AddButton("Page 1", Page1),
                AddButton("Page 2", Page2),

                AddInputField("Page 2", a: (a) => { Debug.Log(a); }),

                AddTextWithInputField("Page 2", a: (a) => { Debug.Log(a); })
            };
        }

        private void Page1()
        {

            OpenPage(new List<DeveloperItem>()
            {
                AddInputField("Input some ", a: (str) => { Debug.Log("Some action with " + str); }),

                AddButton("Open page 11", () => { Page11(); })


            });
        }
        
        private void Page11()
        {

            OpenPage(new List<DeveloperItem>()
            {
                AddInputField("Input some ", a: (str) => { Debug.Log("Some action with " + str); }),

            });
        }

        private void Page2()
        {

            OpenPage(new List<DeveloperItem>()
            {
                AddInputField("Input some ", a: (str) => { Debug.Log("Some action with " + str); }),


            });
        }
    }
}