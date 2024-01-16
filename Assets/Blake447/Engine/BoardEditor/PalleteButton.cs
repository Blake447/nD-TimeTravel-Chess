using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalleteButton : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public BoardEditor editor;
    int piece_index;
    public void SetChessPiece(nChessPiece piece)
    {
        if (piece != null)
        {
            text.text = piece.name;
            piece_index = piece.ID;
        }
    }
    public void OnClick()
    {
        editor.selectedPiece = piece_index;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
