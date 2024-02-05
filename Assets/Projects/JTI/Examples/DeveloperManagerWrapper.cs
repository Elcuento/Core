using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeveloperManagerWrapper : DeveloperManager
{
    protected override void Main()
    {

        OpenPage(new List<DeveloperItem>()
        {
            AddText("Scene : "),

            AddButton("Just button ", () =>
            {
                Debug.Log("I am just a button after all..");
            }),

            AddButton("Page 1", Page1),
            AddButton("Page 2", Page1),

            AddInputField("Page 2", onUpdate: (b) =>
            {
                b.InputField.text = Random.Range(0, 99).ToString();
            })
        });
    }

    private void Page1()
    {

        OpenPage(new List<DeveloperItem>()
        {
            AddInputField("Input some ", a: (str) =>
            {
                Debug.Log("Some action with " + str);
            }), 

            AddText("Some text"),

            AddButton("Open page 11", () =>
            {
                Page11();
            })
            

        });
    }
    private void Page11()
    {

        OpenPage(new List<DeveloperItem>()
        {
            AddInputField("Input some ", a: (str) =>
            {
                Debug.Log("Some action with " + str);
            }),


        });
    }
    private void Page2()
    {

        OpenPage(new List<DeveloperItem>()
        {
            AddInputField("Input some ", a: (str) =>
            {
                Debug.Log("Some action with " + str);
            }),


        });
    }
}
