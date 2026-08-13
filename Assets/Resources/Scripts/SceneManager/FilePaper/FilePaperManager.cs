using System;
using System.Collections.Generic;
using UnityEngine;

public class FilePaperManager : MonoBehaviour
{
    public static FilePaperManager instance;
    
    public string alphabet = "a,b,c,ch,d,e,f,g,h,i,j,k,l,m,n,ñ,o,p,q,r,s,t,u,v,w,x,y,z";
    
    public GameObject prefabButtonFile;
    public GameObject container;
    
    public List<FilePaperButton> filePaperButtons = new List<FilePaperButton>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        List<string> alphabetSplit = new List<string>(alphabet.Split(","));
        
        for (int i = 0; i < alphabetSplit.Count; i++)
        {
            var filePaperButton = Instantiate(prefabButtonFile, container.transform).GetComponent<FilePaperButton>();
            filePaperButton.textLetter.text = alphabetSplit[i].ToUpper();
            
            float pivotY = alphabetSplit.Count <= 1
                ? 1f
                : 1f - ((float)i / (alphabetSplit.Count - 1));

            filePaperButton.fileButtonRt.pivot = new Vector2(1, pivotY);
            
            filePaperButtons.Add(filePaperButton);
        }
    }

    public FilePaperButton GetFilePaperOpened()
    {
        foreach (var filePaperButton in filePaperButtons)
        {
            if (filePaperButton.panelOpened) return filePaperButton;
        }

        return null;
    }
}
