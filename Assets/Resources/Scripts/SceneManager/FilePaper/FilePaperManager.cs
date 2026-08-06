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
        foreach (var letter in alphabet.Split(","))
        {
            var filePaperButton = Instantiate(prefabButtonFile, container.transform).GetComponent<FilePaperButton>();
            filePaperButton.textLetter.text = letter.ToUpper();
            
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
