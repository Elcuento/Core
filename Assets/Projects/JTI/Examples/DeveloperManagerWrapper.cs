using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeveloperManagerWrapper : DeveloperManager
{
    protected override void Main()
    {
        var c = AddText("Scene : ");

        c.SetUpdate(() =>
        {
            c.Text.text = "Scene : " + SceneManager.GetActiveScene().name;
        });

        OpenPage(new List<DeveloperItem>()
        {
            c,
            AddButton("Just button ", () =>
            {
                Debug.Log("I am just a button after all..");
            }),
            AddButton("Page 1", Page1),
            AddButton("Page 2", Page2)
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
