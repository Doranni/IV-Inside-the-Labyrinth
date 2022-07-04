using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavesHandler 
{
    public static SavesHandler instance = new SavesHandler();

    private SavesHandler() { }

    private Saves model;

    // load JSON
    public void load() {
       // model = loadJSON()
    }

    public void save() { }

    public void reset() {
        model = new Saves();
    }

}
