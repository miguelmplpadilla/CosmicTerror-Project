using System;
using UnityEngine;

public class FilePaperManager : MonoBehaviour
{
    public string alphabet = "a,b,c,ch,d,e,f,g,h,i,j,k,l,m,n,ñ,o,p,q,r,s,t,u,v,w,x,y,z";
    
    public GameObject prefabButtonFile;
    public GameObject container;

    private void Start()
    {
        foreach (var letter in alphabet.Split(","))
        {
            var letterObj = Instantiate(prefabButtonFile, container.transform);
            letterObj.
        }
    }
}
