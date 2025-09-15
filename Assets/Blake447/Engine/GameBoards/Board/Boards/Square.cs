using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField]
    MeshRenderer meshRenderer;

    [SerializeField]
    MeshFilter meshFilter;

	[SerializeField]
	MaterialPropertyBlock white_block;

	[SerializeField]
	MaterialPropertyBlock black_block;
    //PieceMeshes piece_meshes = null;
    [SerializeField]
	PiecePallete pallete;

    public void InitializeSquare(PiecePallete pallete)
    {
        this.pallete = pallete;
        
    }
    public void SetMesh(int piece_index)
    {
        if (white_block == null)
        {
            white_block = new MaterialPropertyBlock();
            white_block.SetFloat("_Parity", 0);
        }
        if (black_block == null)
        {
            black_block = new MaterialPropertyBlock();
            black_block.SetFloat("_Parity", 1);
        }

        int array_index = piece_index % Overseer.PIECE_COUNT;

 

        meshFilter.mesh = pallete.GetMeshIndex(array_index);
        int mat_index = piece_index / Overseer.PIECE_COUNT;
        meshRenderer.SetPropertyBlock(mat_index == 0 ? white_block : black_block);
        meshRenderer.transform.rotation = Quaternion.LookRotation((-1 + 2 * mat_index)*meshRenderer.transform.parent.up, meshRenderer.transform.parent.forward);

    }
    public void SetMeshShogi(int piece_index)
    {



        if (white_block == null)
        {
            white_block = new MaterialPropertyBlock();
            white_block.SetFloat("_Parity", 0);
        }
        if (black_block == null)
        {
            black_block = new MaterialPropertyBlock();
            black_block.SetFloat("_Parity", 1);
        }

        int array_index = piece_index % Overseer.PIECE_COUNT;
        //Debug.Log(array_index);
        meshFilter.mesh = pallete.GetMeshIndex(array_index);

        int mat_index = piece_index / Overseer.PIECE_COUNT;
        if (meshRenderer.materials.Length < 2)
        {
            meshRenderer.materials = new Material[] { meshRenderer.material, meshRenderer.material };
        }
        meshRenderer.SetPropertyBlock(mat_index == 0 ? white_block : black_block, 0);
        meshRenderer.SetPropertyBlock(mat_index == 0 ? black_block : white_block, 1);
        if (piece_index != 0)
        {
            //this.transform.localRotation = Quaternion.identity;
            meshRenderer.transform.rotation = Quaternion.AngleAxis(180.0f*mat_index, Vector3.up);
        }
        else
        {
            meshRenderer.transform.rotation = Quaternion.identity;
        }
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
